using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using Wren.Emulation.MasterSystem.ExpressionLibraries;
using System.Reflection;

namespace Wren.Emulation.MasterSystem.InstructionAdvice
{
    public class InstructionBroadcaster : FilteredInstructionAdvice
    {
        public event EventHandler<CpuEventArgs> InstructionRan;

        ConstructorInfo _eventArgsConstructor;
        MethodInfo _instructionRanEvent;


        public InstructionBroadcaster()
        {
            _eventArgsConstructor = typeof(CpuEventArgs).GetConstructor(new Type[] 
                { 
                    typeof(Int32),
                    typeof(Int32),
                    typeof(Int32),
                    typeof(Int32),
                    typeof(Int32),
                    typeof(Int32),
                    typeof(Int32),
                    typeof(Int32),
                    typeof(Int32),
                    typeof(Int32),
                    typeof(Int32),
                    typeof(InstructionInfo),
                    typeof(ISystemBus)
                }
            );

            _instructionRanEvent = this.GetType().GetMethod("RaiseEvent");
        }

       
        public void RaiseEvent(CpuEventArgs e)
        {
            if (InstructionRan != null)
            {
                InstructionRan(this, e);
            }
        }

        protected override Expression GetExpressionImpl(InstructionInfo info, IExpressionLibraryRegistry expressionLibraries)
        {
            var dataExpLib = expressionLibraries.GetLibrary<IDataAccessExpressionLibrary>();
            var progLib = expressionLibraries.GetLibrary<IProgramControlExpressionLibrary>();

            return Expression.Call(Expression.Constant(this), _instructionRanEvent, Expression.New(_eventArgsConstructor,
                    dataExpLib.RegisterA,
                    dataExpLib.RegisterB,
                    dataExpLib.RegisterC,
                    dataExpLib.RegisterD,
                    dataExpLib.RegisterE,
                    dataExpLib.RegisterH,
                    dataExpLib.RegisterL,
                    dataExpLib.FlagsRegister,
                    progLib.ProgramCounterRegister,
                    dataExpLib.StackPointerRegister,
                    progLib.CycleCounter,
                    Expression.Constant(info),
                    ((DataAccessExpressionLibrary)dataExpLib).SystemBusParameter
                )
            );
        }
    }

    public class CpuEventArgs : EventArgs
    {
        public Int32 RegisterA { get; private set; }
        public Int32 RegisterB { get; private set; }
        public Int32 RegisterC { get; private set; }
        public Int32 RegisterD { get; private set; }
        public Int32 RegisterE { get; private set; }
        public Int32 RegisterH { get; private set; }
        public Int32 RegisterL { get; private set; }
        public Int32 FlagsRegister { get; private set; }
        public Int32 ProgramCounterRegister { get; private set; }
        public Int32 StackPointerRegister { get; private set; }
        public Int32 CycleCounter { get; private set; }
        public InstructionInfo Instruction { get; private set; }
        public ISystemBus SystemBus { get; private set; }

        public CpuEventArgs(Int32 registerA, 
                            Int32 registerB,
                            Int32 registerC,
                            Int32 registerD,
                            Int32 registerE,
                            Int32 registerH,
                            Int32 registerL,
                            Int32 flagsRegister,
                            Int32 programCounterRegsiter,
                            Int32 stackPointerRegsiter,
                            Int32 cycleCounter,
                            InstructionInfo info,
                            ISystemBus systemBus
            )
        {
            RegisterA = registerA;
            RegisterB = registerB;
            RegisterC = registerC;
            RegisterD = registerD;
            RegisterE = registerE;
            RegisterH = registerH;
            RegisterL = registerL;
            FlagsRegister = flagsRegister;
            ProgramCounterRegister = programCounterRegsiter;
            StackPointerRegister = stackPointerRegsiter;
            CycleCounter = cycleCounter;
            Instruction = info;
            SystemBus = systemBus;
        }
    }
}
