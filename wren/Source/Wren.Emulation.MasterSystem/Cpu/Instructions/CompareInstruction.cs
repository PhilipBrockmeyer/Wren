using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wren.Emulation.MasterSystem.ExpressionLibraries;
using System.Linq.Expressions;

namespace Wren.Emulation.MasterSystem.Instructions
{
    public class CompareInstruction
    {
        IDataAccessExpressionLibrary _cpuValueExpressions;
        IFlagLookupValuesExpressionLibrary _flagExpressions;
        ITemporaryExpressionLibrary _tempExpressions;
        IProgramControlExpressionLibrary _programControlExpressions;

        public CompareInstruction(IDataAccessExpressionLibrary registerExpressions,
                              IFlagLookupValuesExpressionLibrary flagExpressions,
                              ITemporaryExpressionLibrary temporaryExpressions,
                              IProgramControlExpressionLibrary programControlExpressions)
        {
            _cpuValueExpressions = registerExpressions;
            _flagExpressions = flagExpressions;
            _tempExpressions = temporaryExpressions;
            _programControlExpressions = programControlExpressions;
        }

        private Expression Compare(Expression number)
        {
            return Expression.Block(
                // Assign flags from precalculated flags array at [(a << 8) | b].
                Expression.Assign(
                    _cpuValueExpressions.FlagsRegister,
                    Expression.ArrayIndex(_flagExpressions.SubtractionFlagsCalcultorFlags,
                        Expression.Or(
                            Expression.LeftShift(_cpuValueExpressions.RegisterA, Expression.Constant(8)),
                            number
                        )
                    )
                )
            );
        }

        [Instruction("CP B", 0xB8, Cycles = 4)]
        public Expression Cp_B
        {
            get { return Compare(_cpuValueExpressions.RegisterB); }
        }

        [Instruction("CP C", 0xB9, Cycles = 4)]
        public Expression Cp_C
        {
            get { return Compare(_cpuValueExpressions.RegisterC); }
        }

        [Instruction("CP D", 0xBA, Cycles = 4)]
        public Expression Cp_D
        {
            get { return Compare(_cpuValueExpressions.RegisterD); }
        }

        [Instruction("CP E", 0xBB, Cycles = 4)]
        public Expression Cp_E
        {
            get { return Compare(_cpuValueExpressions.RegisterE); }
        }

        [Instruction("CP H", 0xBC, Cycles = 4)]
        public Expression CP_H
        {
            get { return Compare(_cpuValueExpressions.RegisterH); }
        }

        [Instruction("CP L", 0xBD, Cycles = 4)]
        public Expression CP_L
        {
            get { return Compare(_cpuValueExpressions.RegisterL); }
        }

        [Instruction("CP (HL)", 0xBE, Cycles = 7)]
        public Expression Cp_HLi
        {
            get { return Compare(_cpuValueExpressions.ReadHL); }
        }

        [Instruction("CP A", 0xBF, Cycles = 4)]
        public Expression Cp_A
        {
            get { return Compare(_cpuValueExpressions.RegisterA); }
        }

        [Instruction("CP n", 0xFE, Cycles = 7, ParameterMode=InstructionParameterMode.Byte)]
        public Expression Cp_n
        {
            get { return Compare(_programControlExpressions.ParameterByte1); }
        }


        [Instruction("CP (IX+d)", 0xBE, Cycles = 11, Prefix = "DD", ParameterMode = InstructionParameterMode.Index)]
        public Expression Cp_IXdi
        {
            get
            {
                return Expression.Block(
                    Expression.Assign(_tempExpressions.Temp1, _cpuValueExpressions.ReadIXd(_programControlExpressions.ParameterByte1)),
                        Compare(_tempExpressions.Temp1)
                    );
            }
        }

        [Instruction("CP (IY+d)", 0xBE, Cycles = 11, Prefix = "FD", ParameterMode = InstructionParameterMode.Index)]
        public Expression Cp_IYdi
        {
            get
            {
                return Expression.Block(
                    Expression.Assign(_tempExpressions.Temp1, _cpuValueExpressions.ReadIYd(_programControlExpressions.ParameterByte1)),
                        Compare(_tempExpressions.Temp1)
                    );
            }
        }
    }
}
