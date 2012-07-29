using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wren.Emulation.MasterSystem.ExpressionLibraries;
using System.Linq.Expressions;

namespace Wren.Emulation.MasterSystem.Instructions
{
    public class OrInstruction
    {
        IDataAccessExpressionLibrary _cpuValueExpressions;
        IFlagLookupValuesExpressionLibrary _flagExpressions;       
        IProgramControlExpressionLibrary _programControlExpressions;
        ITemporaryExpressionLibrary _tempExpressions;

        public OrInstruction(IDataAccessExpressionLibrary registerExpressions,
                              IFlagLookupValuesExpressionLibrary flagExpressions,
                              IProgramControlExpressionLibrary programControlExpressions,
                              ITemporaryExpressionLibrary tempExpressions)
        {
            _cpuValueExpressions = registerExpressions;
            _flagExpressions = flagExpressions;
            _programControlExpressions = programControlExpressions;
            _tempExpressions = tempExpressions;
        }

        private Expression Or(Expression value)
        {
            return Expression.Block(
                // Or the result.
                Expression.OrAssign(_cpuValueExpressions.RegisterA, value),

                // Assign flags from precalculated flags array.
                Expression.Assign(
                    _cpuValueExpressions.FlagsRegister,
                    Expression.ArrayIndex(_flagExpressions.SignZeroParityCalcultorFlags, _cpuValueExpressions.RegisterA)
                )
            );
        }

        [Instruction("OR B", 0xB0, Cycles = 4)]
        public Expression Or_B
        {
            get { return Or(_cpuValueExpressions.RegisterB); }
        }

        [Instruction("OR C", 0xB1, Cycles = 4)]
        public Expression Or_C
        {
            get { return Or(_cpuValueExpressions.RegisterC); }
        }

        [Instruction("OR D", 0xB2, Cycles = 4)]
        public Expression Or_D
        {
            get { return Or(_cpuValueExpressions.RegisterD); }
        }

        [Instruction("OR E", 0xB3, Cycles = 4)]
        public Expression Or_E
        {
            get { return Or(_cpuValueExpressions.RegisterE); }
        }

        [Instruction("OR H", 0xB4, Cycles = 4)]
        public Expression Or_H
        {
            get { return Or(_cpuValueExpressions.RegisterH); }
        }

        [Instruction("OR L", 0xB5, Cycles = 4)]
        public Expression Or_L
        {
            get { return Or(_cpuValueExpressions.RegisterL); }
        }

        [Instruction("OR (HL)", 0xB6, Cycles = 7)]
        public Expression Or_HLi
        {
            get { return Or(_cpuValueExpressions.ReadHL); }
        }

        [Instruction("OR A", 0xB7, Cycles = 4)]
        public Expression Or_A
        {
            get { return Or(_cpuValueExpressions.RegisterA); }
        }

        [Instruction("OR n", 0xF6, Cycles = 7, ParameterMode = InstructionParameterMode.Byte)]
        public Expression Or_n
        {
            get { return Or(_programControlExpressions.ParameterByte1); }
        }

        [Instruction("OR (IX+d)", 0xB6, Cycles = 11, Prefix = "DD", ParameterMode = InstructionParameterMode.Index)]
        public Expression Or_IXdi
        {
            get
            {
                return Expression.Block(
                    Expression.Assign(_tempExpressions.Temp1, _cpuValueExpressions.ReadIXd(_programControlExpressions.ParameterByte1)),
                        Or(_tempExpressions.Temp1)
                    );
            }
        }

        [Instruction("OR (IY+d)", 0xB6, Cycles = 11, Prefix = "FD", ParameterMode = InstructionParameterMode.Index)]
        public Expression Or_IYdi
        {
            get
            {
                return Expression.Block(
                    Expression.Assign(_tempExpressions.Temp1, _cpuValueExpressions.ReadIYd(_programControlExpressions.ParameterByte1)),
                        Or(_tempExpressions.Temp1)
                    );
            }
        }
    }
}
