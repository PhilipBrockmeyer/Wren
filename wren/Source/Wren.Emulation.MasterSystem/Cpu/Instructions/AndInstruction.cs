using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wren.Emulation.MasterSystem.ExpressionLibraries;
using System.Linq.Expressions;

namespace Wren.Emulation.MasterSystem.Instructions
{
    public class AndInstruction
    {
        IDataAccessExpressionLibrary _cpuValueExpressions;
        IFlagLookupValuesExpressionLibrary _flagExpressions;
        IProgramControlExpressionLibrary _programControlExpressions;
        ITemporaryExpressionLibrary _temporaryExpressions;

        public AndInstruction(IDataAccessExpressionLibrary registerExpressions,
                              IFlagLookupValuesExpressionLibrary flagExpressions,
                              IProgramControlExpressionLibrary programControlExpressions,
                              ITemporaryExpressionLibrary temporaryExpressions)
        {
            _cpuValueExpressions = registerExpressions;
            _flagExpressions = flagExpressions;
            _programControlExpressions = programControlExpressions;
            _temporaryExpressions = temporaryExpressions;
        }

        private Expression And(Expression value)
        {
            return Expression.Block(                                      
                // And the result.
                Expression.AndAssign(_cpuValueExpressions.RegisterA, value),

                // Assign flags from precalculated flags array + Half Carry.
                Expression.Assign(
                    _cpuValueExpressions.FlagsRegister, 
                    Expression.Or(
                        Expression.ArrayIndex(_flagExpressions.SignZeroParityCalcultorFlags,_cpuValueExpressions.RegisterA),
                        Expression.Constant(Flags.HalfCarry)
                    )
                )
            );
        }
        
        [Instruction("AND B", 0xA0, Cycles = 4)]
        public Expression And_B
        {
            get { return And(_cpuValueExpressions.RegisterB); }
        }

        [Instruction("AND C", 0xA1, Cycles = 4)]
        public Expression And_C
        {
            get { return And(_cpuValueExpressions.RegisterC); }
        }

        [Instruction("AND D", 0xA2, Cycles = 4)]
        public Expression And_D
        {
            get { return And(_cpuValueExpressions.RegisterD); }
        }

        [Instruction("AND E", 0xA3, Cycles = 4)]
        public Expression And_E
        {
            get { return And(_cpuValueExpressions.RegisterE); }
        }

        [Instruction("AND H", 0xA4, Cycles = 4)]
        public Expression And_H
        {
            get { return And(_cpuValueExpressions.RegisterH); }
        }

        [Instruction("AND L", 0xA5, Cycles = 4)]
        public Expression And_L
        {
            get { return And(_cpuValueExpressions.RegisterL); }
        }

        [Instruction("AND (HL)", 0xA6, Cycles = 7)]
        public Expression And_HLi
        {
            get { return And(_cpuValueExpressions.ReadHL); }
        }

        [Instruction("AND A", 0xA7, Cycles = 4)]
        public Expression And_A
        {
            get { return And(_cpuValueExpressions.RegisterA); }
        }

        [Instruction("AND n", 0xE6, Cycles = 7, ParameterMode=InstructionParameterMode.Byte)]
        public Expression And_n
        {
            get { return And(_programControlExpressions.ParameterByte1); }
        }

        [Instruction("AND (IX+d)", 0xA6, Cycles = 11, Prefix = "DD", ParameterMode = InstructionParameterMode.Index)]
        public Expression And_IXdi
        {
            get
            {
                return Expression.Block(
                    Expression.Assign(_temporaryExpressions.Temp1, _cpuValueExpressions.ReadIXd(_programControlExpressions.ParameterByte1)),
                        And(_temporaryExpressions.Temp1)
                    );
            }
        }

        [Instruction("AND (IY+d)", 0xA6, Cycles = 11, Prefix = "FD", ParameterMode = InstructionParameterMode.Index)]
        public Expression And_IYdi
        {
            get
            {
                return Expression.Block(
                    Expression.Assign(_temporaryExpressions.Temp1, _cpuValueExpressions.ReadIYd(_programControlExpressions.ParameterByte1)),
                        And(_temporaryExpressions.Temp1)
                    );
            }
        }
    }
}
