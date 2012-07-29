using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wren.Emulation.MasterSystem.ExpressionLibraries;
using System.Linq.Expressions;

namespace Wren.Emulation.MasterSystem.Instructions
{
    public class XorInstruction
    {
        IDataAccessExpressionLibrary _cpuValueExpressions;
        IFlagLookupValuesExpressionLibrary _flagExpressions;
        IProgramControlExpressionLibrary _programControlExpressions;
        ITemporaryExpressionLibrary _tempExpressions;

        public XorInstruction(IDataAccessExpressionLibrary registerExpressions,
                              IFlagLookupValuesExpressionLibrary flagExpressions,
                              IProgramControlExpressionLibrary programControlExpressions,
                              ITemporaryExpressionLibrary tempExpressions)
        {
            _cpuValueExpressions = registerExpressions;
            _flagExpressions = flagExpressions;
            _programControlExpressions = programControlExpressions;
            _tempExpressions = tempExpressions;
        }

        private Expression Xor(Expression value)
        {
            return Expression.Block(
                // Xor the result.
                Expression.ExclusiveOrAssign(_cpuValueExpressions.RegisterA, value),

                // Assign flags from precalculated flags array.
                Expression.Assign(
                    _cpuValueExpressions.FlagsRegister,
                    Expression.ArrayIndex(_flagExpressions.SignZeroParityCalcultorFlags, _cpuValueExpressions.RegisterA)
                )
            );
        }

        [Instruction("XOR B", 0xA8, Cycles = 4)]
        public Expression Xor_B
        {
            get { return Xor(_cpuValueExpressions.RegisterB); }
        }

        [Instruction("XOR C", 0xA9, Cycles = 4)]
        public Expression Xor_C
        {
            get { return Xor(_cpuValueExpressions.RegisterC); }
        }

        [Instruction("XOR D", 0xAA, Cycles = 4)]
        public Expression Xor_D
        {
            get { return Xor(_cpuValueExpressions.RegisterD); }
        }

        [Instruction("XOR E", 0xAB, Cycles = 4)]
        public Expression Xor_E
        {
            get { return Xor(_cpuValueExpressions.RegisterE); }
        }

        [Instruction("XOR H", 0xAC, Cycles = 4)]
        public Expression Xor_H
        {
            get { return Xor(_cpuValueExpressions.RegisterH); }
        }

        [Instruction("XOR L", 0xAD, Cycles = 4)]
        public Expression Xor_L
        {
            get { return Xor(_cpuValueExpressions.RegisterL); }
        }

        [Instruction("XOR (HL)", 0xAE, Cycles = 7)]
        public Expression Xor_HLi
        {
            get { return Xor(_cpuValueExpressions.ReadHL); }
        }

        [Instruction("XOR A", 0xAF, Cycles = 4)]
        public Expression Xor_A
        {
            get { return Xor(_cpuValueExpressions.RegisterA); }
        }

        [Instruction("XOR n", 0xEE, Cycles = 7, ParameterMode = InstructionParameterMode.Byte)]
        public Expression And_n
        {
            get { return Xor(_programControlExpressions.ParameterByte1); }
        }

        [Instruction("XOR (IX+d)", 0xAE, Cycles = 11, Prefix = "DD", ParameterMode = InstructionParameterMode.Index)]
        public Expression Xor_IXdi
        {
            get
            {
                return Expression.Block(
                    Expression.Assign(_tempExpressions.Temp1, _cpuValueExpressions.ReadIXd(_programControlExpressions.ParameterByte1)),
                        Xor(_tempExpressions.Temp1)
                    );
            }
        }

        [Instruction("XOR (IY+d)", 0xAE, Cycles = 11, Prefix = "FD", ParameterMode = InstructionParameterMode.Index)]
        public Expression Xor_IYdi
        {
            get
            {
                return Expression.Block(
                    Expression.Assign(_tempExpressions.Temp1, _cpuValueExpressions.ReadIYd(_programControlExpressions.ParameterByte1)),
                        Xor(_tempExpressions.Temp1)
                    );
            }
        }
    }
}
