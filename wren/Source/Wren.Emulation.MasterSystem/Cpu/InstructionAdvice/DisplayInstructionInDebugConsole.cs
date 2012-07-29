using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using Wren.Emulation.MasterSystem.ExpressionLibraries;
using System.Diagnostics;
using System.Reflection;

namespace Wren.Emulation.MasterSystem.InstructionAdvice
{
    public class DisplayInstructionInDebugConsole : IInstructionAdvice
    {
        MethodInfo debugWriteMethod;

        public DisplayInstructionInDebugConsole()
        {
            debugWriteMethod = typeof(Debug).GetMethod("Write", new Type[] { typeof(Object) });
        }

        public Expression GetExpression(InstructionInfo info, IExpressionLibraryRegistry expressionLibraries)
        {
            var programControl = expressionLibraries.GetLibrary<IProgramControlExpressionLibrary>();

            List<Expression> expressions = new List<Expression>();

            expressions.Add(Expression.Call(debugWriteMethod, Expression.Constant("PC: ")));

            switch (info.ParameterMode)
            {
                case InstructionParameterMode.None:
                    expressions.Add(Expression.Call(debugWriteMethod, Expression.TypeAs(Expression.Subtract(programControl.ProgramCounterRegister, Expression.Constant(1)), typeof(Object))));
                    break;
                case InstructionParameterMode.Byte:
                    expressions.Add(Expression.Call(debugWriteMethod, Expression.TypeAs(Expression.Subtract(programControl.ProgramCounterRegister, Expression.Constant(2)), typeof(Object))));
                    break;
                case InstructionParameterMode.Word:
                    expressions.Add(Expression.Call(debugWriteMethod, Expression.TypeAs(Expression.Subtract(programControl.ProgramCounterRegister, Expression.Constant(3)), typeof(Object))));
                    break;
                case InstructionParameterMode.Index:
                    expressions.Add(Expression.Call(debugWriteMethod, Expression.TypeAs(Expression.Subtract(programControl.ProgramCounterRegister, Expression.Constant(2)), typeof(Object))));
                    break;
                case InstructionParameterMode.Address:
                    expressions.Add(Expression.Call(debugWriteMethod, Expression.TypeAs(Expression.Subtract(programControl.ProgramCounterRegister, Expression.Constant(3)), typeof(Object))));
                    break;
                case InstructionParameterMode.IndexAndByte:
                    expressions.Add(Expression.Call(debugWriteMethod, Expression.TypeAs(Expression.Subtract(programControl.ProgramCounterRegister, Expression.Constant(3)), typeof(Object))));
                    break;
                default:
                    break;
            }
            expressions.Add(Expression.Call(debugWriteMethod, Expression.Constant("\t")));
            
            expressions.Add(Expression.Call(debugWriteMethod, Expression.Constant(String.Format("0x{0:X2}", info.Opcode))));
            expressions.Add(Expression.Call(debugWriteMethod, Expression.Constant("\t")));

            expressions.Add(Expression.Call(debugWriteMethod, Expression.Constant(info.Mnemonic.PadRight(15))));

            switch (info.ParameterMode)
            {
                case InstructionParameterMode.None:
                    break;
                case InstructionParameterMode.Byte:
                    expressions.Add(Expression.Call(debugWriteMethod, Expression.Constant("\tn = ")));
                    expressions.Add(Expression.Call(debugWriteMethod, Expression.TypeAs(programControl.ParameterByte1, typeof(Object))));
                    break;
                case InstructionParameterMode.Word:
                    expressions.Add(Expression.Call(debugWriteMethod, Expression.Constant("\tnn = ")));
                    expressions.Add(Expression.Call(debugWriteMethod, Expression.TypeAs(programControl.ParameterWord, typeof(Object))));
                    break;
                case InstructionParameterMode.Index:
                    expressions.Add(Expression.Call(debugWriteMethod, Expression.Constant("\td = ")));
                    expressions.Add(Expression.Call(debugWriteMethod, Expression.TypeAs(programControl.ParameterByte1, typeof(Object))));
                    break;
                case InstructionParameterMode.Address:
                    expressions.Add(Expression.Call(debugWriteMethod, Expression.Constant("\taddress = ")));
                    expressions.Add(Expression.Call(debugWriteMethod, Expression.TypeAs(programControl.ParameterWord, typeof(Object))));
                    break;
                case InstructionParameterMode.IndexAndByte:
                    expressions.Add(Expression.Call(debugWriteMethod, Expression.Constant("\tindex = ")));
                    expressions.Add(Expression.Call(debugWriteMethod, Expression.TypeAs(programControl.ParameterByte2, typeof(Object))));
                    expressions.Add(Expression.Call(debugWriteMethod, Expression.Constant("\tn = ")));
                    expressions.Add(Expression.Call(debugWriteMethod, Expression.TypeAs(programControl.ParameterByte1, typeof(Object))));
                    break;
                default:
                    break;
            }

            expressions.Add(Expression.Call(debugWriteMethod, Expression.Constant(Environment.NewLine)));

            return Expression.Block(expressions);

            //return Expression.PostIncrementAssign(programControl.ParameterByte1);
        }
    }
}
