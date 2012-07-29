using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Reflection;
using Wren.Emulation.MasterSystem.Properties;

namespace Wren.Emulation.MasterSystem.ExpressionLibraries
{
    public class ProgramControlExpressionLibrary : IProgramControlExpressionLibrary
    {
        private const Int32 MaximumRomSize = 512 * 1024;

        ISystemBus _systemBus;
        public Int32[] _memory = new Int32[0xFFFF];
        FieldInfo _memoryField = typeof(ProgramControlExpressionLibrary).GetField("_memory");
        MethodInfo _isBreakpoint = typeof(BreakpointHandler).GetMethod("IsBreakpoint");
        MethodInfo _handleBreakpoint = typeof(BreakpointHandler).GetMethod("HandleBreakpoint");
        MethodInfo _interuptAddress = typeof(BreakpointHandler).GetMethod("GetInteruptAddress");
        MethodInfo _instructionSize = typeof(BreakpointHandler).GetMethod("GetInstructionSize");

        public ParameterExpression CycleCounterParameter { get; private set; }
        public ParameterExpression ParameterByte1Parameter { get; private set; }
        public ParameterExpression ParameterByte2Parameter { get; private set; }
        public ParameterExpression ProgramCounterRegisterParameter { get; private set; }
        public ParameterExpression SystemBusParameter { get; private set; }
        public ParameterExpression CpuParameter { get; private set; }
        public ParameterExpression TempParameter { get; private set; }
        public ParameterExpression TempIParameter { get; private set; }
        public ParameterExpression CpuCacheParameter { get; private set; }
        public ParameterExpression BreakpointHandlerParameter { get; private set; }

        public ProgramControlExpressionLibrary(ISystemBus systemBus)
        {
            _systemBus = systemBus;

            CycleCounterParameter = Expression.Variable(typeof(Int32), "cycleCounter");
            ParameterByte1Parameter = Expression.Variable(typeof(Int32), "parameterByte1");
            ParameterByte2Parameter = Expression.Variable(typeof(Int32), "parameterByte2");
            ProgramCounterRegisterParameter = Expression.Variable(typeof(Int32), "programCounterRegister");
            CpuParameter = Expression.Variable(typeof(CpuData), "programControlCpu");
            SystemBusParameter = Expression.Variable(typeof(ISystemBus), "programControlSystemBus");
            TempParameter = Expression.Variable(typeof(Byte), "temp");
            TempIParameter = Expression.Variable(typeof(Int32), "tempI");
            CpuCacheParameter = Expression.Variable(typeof(Int32[]), "cpuCacheParameter");
            BreakpointHandlerParameter = Expression.Variable(typeof(BreakpointHandler), "breakpointHandler");

            readMethod = typeof(ISystemBus).GetMethod("ReadByte");
        }

        private MethodInfo readMethod;

        public Expression ParameterByte1
        {
            get { return ParameterByte1Parameter; }
        }

        public Expression ParameterByte2
        {
            get { return ParameterByte2Parameter; }
        }

        public Expression ParameterWord
        {
            get 
            {
                return Expression.Or(
                    Expression.LeftShift(ParameterByte1, Expression.Constant(0x08)),
                    ParameterByte2
                );
            }
        }

        public Expression CycleCounter
        {
            get { return CycleCounterParameter; }
        }

        public Expression ProgramCounterRegister
        {
            get { return ProgramCounterRegisterParameter; }
        }

        public Expression ReadAndIncrementProgramCounter
        {
            get
            {
                List<Expression> expressions = new List<Expression>();
                var systemBusExpressions = _systemBus as ISystemBusExpressionProvider;

                if (systemBusExpressions == null)
                {
                    // read from the cache.
                    expressions.Add(Expression.Assign(TempIParameter, Expression.ArrayIndex(CpuCacheParameter, ProgramCounterRegister)));

                    expressions.Add(Expression.IfThenElse(Expression.Equal(TempIParameter, Expression.Constant(-1)),
                        Expression.Block(
                            Expression.Assign(TempParameter, Expression.Call(SystemBusParameter, readMethod, ProgramCounterRegister)),
                            Expression.Assign(Expression.ArrayAccess(CpuCacheParameter, ProgramCounterRegister), Expression.Convert(TempParameter, typeof(Int32)))
                        ),

                        Expression.Assign(TempParameter, Expression.Convert(TempIParameter, typeof(Byte))                        
                    )));

                }
                else
                {
                    //expressions.Add(Expression.Assign(TempParameter, Expression.Convert(Expression.Constant(0), typeof(Byte))));
                    expressions.Add(systemBusExpressions.ReadByte(ProgramCounterRegister, TempParameter));
                }

                // PC++
                expressions.Add(Expression.PreIncrementAssign(ProgramCounterRegister));
                expressions.Add(Expression.Convert(TempParameter, typeof(Int32)));

                return Expression.Block(expressions);
            }
        }

        public Expression InitializeLocals
        {
            get
            {
                return Expression.Block(
                           Expression.Assign(CpuCacheParameter, Expression.Field(CpuParameter, "CpuCache")),
                           Expression.Assign(ProgramCounterRegisterParameter, Expression.Field(CpuParameter, FieldNames.CpuData_ProgramCounterRegister)),
                           Expression.Assign(CycleCounterParameter, Expression.Field(CpuParameter, FieldNames.CpuData_CycleCounter))
                       );
            }
        }

        public Expression FinalizeLocals
        {
            get
            {
                return Expression.Block(
                           Expression.Assign(Expression.Field(CpuParameter, FieldNames.CpuData_ProgramCounterRegister), ProgramCounterRegisterParameter),
                           Expression.Assign(Expression.Field(CpuParameter, FieldNames.CpuData_CycleCounter), CycleCounterParameter)
                       );
            }
        }

        public Expression BreakpointHandler
        {
            get { return Expression.Call(BreakpointHandlerParameter, _handleBreakpoint, ProgramCounterRegister); }
        }

        public Expression InstructionSize
        {
            get { return Expression.Call(BreakpointHandlerParameter, _instructionSize, ProgramCounterRegister); }
        }

        public Expression InteruptAddress
        {
            get { return Expression.Call(BreakpointHandlerParameter, _interuptAddress, ProgramCounterRegister); }
        }

        public Expression IsBreakpoint
        {
            get { return Expression.Call(BreakpointHandlerParameter, _isBreakpoint, ProgramCounterRegister); }
        }

        public IEnumerable<ParameterExpression> GetParameters()
        {         
            yield return SystemBusParameter;
            yield return CpuParameter;
            yield return BreakpointHandlerParameter;
        }

        public IEnumerable<Object> GetObjects(ICpuContext context)
        {            
            yield return context.SystemBus;
            yield return context.Data;
            yield return context.BreakpointHandler;
        }

        public IEnumerable<ParameterExpression> GetLocals()
        {
            yield return CycleCounterParameter;
            yield return ParameterByte1Parameter;
            yield return ParameterByte2Parameter;
            yield return ProgramCounterRegisterParameter;
            yield return CpuCacheParameter;
            yield return TempParameter;
            yield return TempIParameter;
        }
    }
}
