using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using Wren.Emulation.MasterSystem.ExpressionLibraries;
using System.Reflection;

namespace Wren.Emulation.MasterSystem.InstructionAdvice
{
    public class InstructionCalls : IInstructionAdvice
    {
        public class InstructionCall
        {
            public String Mnemonic { get; private set; }
            public Int32 ProgramCounter { get; set; }
            public Int32 ParameterByte1 { get; private set; }
            public Int32 ParameterByte2 { get; private set; }
            public Int32 CallCount { get; set; }

            public InstructionCall(String mnemonic, Int32 programCounter, Int32 parameterByte1, Int32 parameterByte2)
            {
                Mnemonic = mnemonic;
                ProgramCounter = programCounter;
                ParameterByte1 = parameterByte1;
                ParameterByte2 = parameterByte2;
                CallCount = 0;
            }

            public override string ToString()
            {
                return String.Format("{0:X4}: {1}\t Call Count:{2}", ProgramCounter, Mnemonic.PadRight(14, ' '), CallCount);
            }
        }

        public IEnumerable<InstructionCall> Instructions
        {
            get
            {
                return _instructions.Values.OrderBy(ic => ic.ProgramCounter);
            }
        }

        public String InstructionText
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                foreach (var call in Instructions)
                {
                    sb.AppendLine(call.ToString());
                }

                return sb.ToString();
            }
        }

        Dictionary<Int32, InstructionCall> _instructions;
        MethodInfo _callMethod;
        ConstructorInfo _instructionCallConsturctor;

        public InstructionCalls()
        {
            _instructions = new Dictionary<Int32, InstructionCall>();
            _callMethod = this.GetType().GetMethod("AddInstructionCall");
            _instructionCallConsturctor = typeof(InstructionCall).GetConstructor(new Type[] {
                typeof(String),
                typeof(Int32),
                typeof(Int32),
                typeof(Int32)
            });
        }

        public Expression GetExpression(InstructionInfo info, IExpressionLibraryRegistry expressionLibraries)
        {
            var instructionSize = 0;
            switch (info.ParameterMode)
            {
                case InstructionParameterMode.None:
                    instructionSize = 1;
                    break;
                case InstructionParameterMode.Byte:
                    instructionSize = 2;
                    break;
                case InstructionParameterMode.Word:
                    instructionSize = 3;
                    break;
                case InstructionParameterMode.Index:
                    instructionSize = 2;
                    break;
                case InstructionParameterMode.Address:
                    instructionSize = 3;
                    break;
                case InstructionParameterMode.IndexAndByte:
                    instructionSize = 3;
                    break;
                default:
                    break;
            }

            if (!String.IsNullOrEmpty(info.Prefix))
                instructionSize += info.Prefix.Length / 2;

            var progExpr = expressionLibraries.GetLibrary<IProgramControlExpressionLibrary>();
            return Expression.Call(Expression.Constant(this), _callMethod,
                Expression.New(_instructionCallConsturctor,
                    Expression.Constant(info.Mnemonic),
                    Expression.Subtract(progExpr.ProgramCounterRegister,
                        Expression.Constant(instructionSize)),
                    progExpr.ParameterByte1,
                    progExpr.ParameterByte2)
                );
        }

        public void AddInstructionCall(InstructionCall item)
        {
            if (!_instructions.ContainsKey(item.ProgramCounter))
                _instructions[item.ProgramCounter] = item;

            _instructions[item.ProgramCounter].CallCount++;
            
        }
    }
}

