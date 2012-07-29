using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wren.Emulation.MasterSystem.ExpressionLibraries;
using System.Linq.Expressions;
using Wren.Emulation.MasterSystem.InstructionAdvice;
using Wren.Emulation.MasterSystem.Exceptions;
using System.Reflection;
using System.IO;

namespace Wren.Emulation.MasterSystem
{
    public class Z80Cpu : IRunnableSystemComponent, IInteruptableSystemComponent, IStatefulSystemComponent
    {
        static Delegate _cpuCode;
        static FlagTables _flagTables;
        static ExpressionBuilder _expressionBuilder;

        static Z80Cpu()
        {
            _flagTables = new FlagTables();
            _flagTables.ScanForFlagTables(typeof(FlagTables).Assembly.GetTypes());

            var libRegistry = BuildExpressionLibraryRegistry(new ArraySystemBus(1024));
            var instructionScanner = new InstructionScanner(libRegistry);
            var instructions = instructionScanner.BuildInstructionInfo(typeof(InstructionScanner).Assembly.GetTypes());
            var instructionExpressionBuilder = new InstructionExpressionBuilder(libRegistry);

            /*if (IsDebugModeEnabled)
            {
                instructionExpressionBuilder.RegisterPreInstructionAdvice(new DisplayInstructionInDebugConsole());

                var broadCaster = new InstructionBroadcaster();
                broadCaster.InstructionRan += new EventHandler<CpuEventArgs>(broadCaster_InstructionRan);
                instructionExpressionBuilder.RegisterPostInstructionAdvice(broadCaster);
            }

            if (IsRecentHistoryEnabled)
            {
                _history = new RecentHistory();
                instructionExpressionBuilder.RegisterPreInstructionAdvice(_history);

                _instructions = new InstructionCalls();
                instructionExpressionBuilder.RegisterPreInstructionAdvice(_instructions);
            }*/

            var opcodeResolver = (new DefaultInstructionSpaceBuilder()).BuildInstructionSpaceExpression(instructions, libRegistry.GetLibrary<IProgramControlExpressionLibrary>(), instructionExpressionBuilder, new InstructionSpace());
            _expressionBuilder = new ExpressionBuilder(libRegistry);

            var methodBody = BuildLoopMethod(opcodeResolver, _expressionBuilder, libRegistry);
            var block = Expression.Block(_expressionBuilder.GetLocals(), methodBody);
            var lambda = Expression.Lambda(block, _expressionBuilder.GetParameterList());
            _cpuCode = lambda.Compile();
        }

        public event EventHandler<CpuEventArgs> InstructionRan;

        Int32 _cycleCounter;
        BreakpointHandler _breakpointHandler;
        CpuData _data = new CpuData();
        public CpuData Data { get { return _data; } }
        
        CpuContext _context = new CpuContext();

        RecentHistory _history;
        public IEnumerable<Wren.Emulation.MasterSystem.InstructionAdvice.RecentHistory.HistoryItem> InstructionHistory 
        { 
            get 
            {
                if (IsRecentHistoryEnabled)
                    return _history.Instructions.ToArray();

                return null;
            }
        }

        InstructionCalls _instructions;
        public IEnumerable<Wren.Emulation.MasterSystem.InstructionAdvice.InstructionCalls.InstructionCall> CallCounts
        {
            get
            {
                if (IsRecentHistoryEnabled)
                    return _instructions.Instructions.ToArray();

                return null;
            }
        }

        public Boolean IsDebugModeEnabled { get; set; }
        public Boolean IsRecentHistoryEnabled { get; set; }

        public Z80Cpu(BreakpointHandler breakpointHandler)
        {
            _breakpointHandler = breakpointHandler;
        }

        public RunDelegate GetRunMethod(ISystemBus systemBus)
        {
            _data.StackPointerRegister = 0;
            _data.InteruptMode = 1;

            return BuildRunMethod(systemBus);
        }

        private RunDelegate BuildRunMethod(ISystemBus systemBus)
        {
            _context.Data = _data;
            _context.Flags = _flagTables;
            _context.SystemBus = systemBus;
            _context.BreakpointHandler = _breakpointHandler;
            Object[] parameters = _expressionBuilder.GetObjects(_context);

            return ((cycles, bus) =>
                {
                    try
                    {
                        if (IsRecentHistoryEnabled)
                            _history.ClearHistory();

                        cycles = _cycleCounter + cycles;
                        _data.CycleCounter = cycles;

                        if (_data.CycleCounter > 0)
                            _cpuCode.DynamicInvoke(parameters);

                        _cycleCounter = _data.CycleCounter;
                        return 0;
                    }
                    catch (TargetInvocationException ex)
                    {
                        throw new InstructionExecutionException(InstructionHistory, Data, ex);
                    }
                }
            );
        }
               
        private static List<Expression> BuildLoopMethod(Expression opcodeResolver, ExpressionBuilder expressionBuilder, IExpressionLibraryRegistry libRegistry)
        {
            LabelTarget beginMainLoop = Expression.Label("beginMainLoop");
            LabelTarget endMainLoop = Expression.Label("endMainLoop");

            IProgramControlExpressionLibrary pcel = libRegistry.GetLibrary<IProgramControlExpressionLibrary>();
            IInteruptExpressionLibrary iel = libRegistry.GetLibrary<IInteruptExpressionLibrary>();
            IDataAccessExpressionLibrary dael = libRegistry.GetLibrary<IDataAccessExpressionLibrary>();

            var methodBody = new List<Expression>();
            methodBody.Add(expressionBuilder.InitializeParameters());
            //methodBody.Add(Expression.Assign(pcel.CycleCounter, Expression.Constant(347)));

            methodBody.Add(Expression.Block(
                    // Handle IRQs if neccessary.
                    Expression.IfThen(Expression.Equal(iel.IFF1, Expression.Constant(true)),
                        Expression.IfThen(Expression.Equal(iel.InteruptRequested, Expression.Constant(true)),
                            Expression.Block(
                                Expression.Assign(iel.InteruptRequested, Expression.Constant(false)),
                                Expression.Assign(iel.IFF1, Expression.Constant(false)),
                                dael.Push(pcel.ProgramCounterRegister),
                                Expression.Assign(pcel.ProgramCounterRegister, Expression.Constant(0x0038)),
                                Expression.SubtractAssign(pcel.CycleCounter, Expression.Constant(13))
                            )
                        )
                    ),

                    Expression.Label(beginMainLoop),

                    Expression.IfThen(Expression.LessThanOrEqual(pcel.CycleCounter, Expression.Constant(0)),
                        Expression.Goto(endMainLoop)
                    ),

                    //Expression.SubtractAssign(pcel.CycleCounter, Expression.Constant(4)),

                    Expression.PostIncrementAssign(iel.RefreshRegister),
                    opcodeResolver,

                    Expression.Goto(beginMainLoop),
                    Expression.Label(endMainLoop)
                )
            );

            methodBody.Add(expressionBuilder.FinalizeParameters());
            return methodBody;
        }

        private static IExpressionLibraryRegistry BuildExpressionLibraryRegistry(ISystemBus systemBus)
        {
            var libRegistry = new ExpressionLibraryRegistry();
            libRegistry.RegisterLibrary(new DataAccessExpressionLibrary(systemBus));
            libRegistry.RegisterLibrary(new FlagLookupValuesExpressionLibrary());
            libRegistry.RegisterLibrary(new TemporaryExpressionLibrary());
            libRegistry.RegisterLibrary(new ProgramControlExpressionLibrary(systemBus));
            libRegistry.RegisterLibrary(new InteruptExpressionLibrary());
            libRegistry.RegisterLibrary(new PrimeRegisterExpressionLibrary());

            return libRegistry;
        }
        
        public void RegisterInteruptHandlers(IInteruptManager interuptManager)
        {
            interuptManager.RegisterInteruptHandler(Interupts.Scanline, HandleIrq);
        }

        public void HandleIrq()
        {
            _data.InteruptRequested = true;
        }

        void broadCaster_InstructionRan(object sender, CpuEventArgs e)
        {
            if (InstructionRan != null)
                InstructionRan(this, e);
        }

        public void SerializeComponentState(BinaryWriter writer)
        {
            writer.Write(_cycleCounter);
            writer.Write(_data.FlagsRegister);
            writer.Write(_data.IFF1);
            writer.Write(_data.IFF2);
            writer.Write(_data.InteruptMode);
            writer.Write(_data.InteruptRequested);
            writer.Write(_data.InteruptVectorRegister);
            writer.Write(_data.IsHalted);
            writer.Write(_data.ProgramCounterRegister);
            writer.Write(_data.RefreshRegister);
            writer.Write(_data.RegisterA);
            writer.Write(_data.RegisterAPrime);
            writer.Write(_data.RegisterB);
            writer.Write(_data.RegisterBPrime);
            writer.Write(_data.RegisterC);
            writer.Write(_data.RegisterCPrime);
            writer.Write(_data.RegisterD);
            writer.Write(_data.RegisterDPrime);
            writer.Write(_data.RegisterE);
            writer.Write(_data.RegisterEPrime);
            writer.Write(_data.RegisterFPrime);
            writer.Write(_data.RegisterH);
            writer.Write(_data.RegisterHPrime);
            writer.Write(_data.RegisterIX);
            writer.Write(_data.RegisterIY);
            writer.Write(_data.RegisterL);
            writer.Write(_data.RegisterLPrime);
            writer.Write(_data.StackPointerRegister);           
        }

        public void DeserializeComponentState(BinaryReader reader)
        {
            _cycleCounter = reader.ReadInt32();
            _data.FlagsRegister = reader.ReadInt32();
            _data.IFF1 = reader.ReadBoolean();
            _data.IFF2 = reader.ReadBoolean();
            _data.InteruptMode = reader.ReadInt32();
            _data.InteruptRequested = reader.ReadBoolean();
            _data.InteruptVectorRegister = reader.ReadInt32();
            _data.IsHalted = reader.ReadBoolean();
            _data.ProgramCounterRegister = reader.ReadInt32();
            _data.RefreshRegister = reader.ReadInt32();
            _data.RegisterA = reader.ReadInt32();
            _data.RegisterAPrime = reader.ReadInt32();
            _data.RegisterB = reader.ReadInt32();
            _data.RegisterBPrime = reader.ReadInt32();
            _data.RegisterC = reader.ReadInt32();
            _data.RegisterCPrime = reader.ReadInt32();
            _data.RegisterD = reader.ReadInt32();
            _data.RegisterDPrime = reader.ReadInt32();
            _data.RegisterE = reader.ReadInt32();
            _data.RegisterEPrime = reader.ReadInt32();
            _data.RegisterFPrime = reader.ReadInt32();
            _data.RegisterH = reader.ReadInt32();
            _data.RegisterHPrime = reader.ReadInt32();
            _data.RegisterIX = reader.ReadInt32();
            _data.RegisterIY = reader.ReadInt32();
            _data.RegisterL = reader.ReadInt32();
            _data.RegisterLPrime = reader.ReadInt32();
            _data.StackPointerRegister = reader.ReadInt32();
        }
    }
}
