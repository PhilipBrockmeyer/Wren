using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wren.Emulation.MasterSystem.ExpressionLibraries;
using System.Linq.Expressions;

namespace Wren.Emulation.MasterSystem.Instructions
{
    public class SubtractInstruction
    {
        IDataAccessExpressionLibrary _cpuValueExpressions;
        IFlagLookupValuesExpressionLibrary _flagExpressions;
        ITemporaryExpressionLibrary _tempExpressions;
        IProgramControlExpressionLibrary _programControlExpressions;

        public SubtractInstruction(IDataAccessExpressionLibrary registerExpressions,
                              IFlagLookupValuesExpressionLibrary flagExpressions,
                              ITemporaryExpressionLibrary temporaryExpressions,
                              IProgramControlExpressionLibrary programControlExpressions)
        {
            _cpuValueExpressions = registerExpressions;
            _flagExpressions = flagExpressions;
            _tempExpressions = temporaryExpressions;
            _programControlExpressions = programControlExpressions;
        }

        private Expression Subtract8(Expression number)
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
                ),

                // Subtract and truncate result.
                // A = (A - B) & 0xFF 
                Expression.SubtractAssign(_cpuValueExpressions.RegisterA, number),
                Expression.AndAssign(_cpuValueExpressions.RegisterA, Expression.Constant(0xFF, typeof(Int32)))
            );
        }

        [Instruction("SUB B", 0x90, Cycles = 4)]
        public Expression Sub_B
        {
            get { return Subtract8(_cpuValueExpressions.RegisterB); }
        }

        [Instruction("SUB C", 0x91, Cycles = 4)]
        public Expression Sub_C
        {
            get { return Subtract8(_cpuValueExpressions.RegisterC); }
        }

        [Instruction("SUB D", 0x92, Cycles = 4)]
        public Expression Sub_D
        {
            get { return Subtract8(_cpuValueExpressions.RegisterD); }
        }

        [Instruction("SUB E", 0x93, Cycles = 4)]
        public Expression Sub_E
        {
            get { return Subtract8(_cpuValueExpressions.RegisterE); }
        }

        [Instruction("SUB H", 0x94, Cycles = 4)]
        public Expression Sub_H
        {
            get { return Subtract8(_cpuValueExpressions.RegisterH); }
        }

        [Instruction("SUB L", 0x95, Cycles = 4)]
        public Expression Sub_L
        {
            get { return Subtract8(_cpuValueExpressions.RegisterL); }
        }

        [Instruction("SUB (HL)", 0x96, Cycles = 7)]
        public Expression Sub_HLi
        {
            get { return Subtract8(_cpuValueExpressions.ReadHL); }
        }

        [Instruction("SUB A", 0x97, Cycles = 4)]
        public Expression Sub_A
        {
            get { return Subtract8(_cpuValueExpressions.RegisterA); }
        }

        [Instruction("SUB n", 0xD6, Cycles = 7, ParameterMode=InstructionParameterMode.Byte)]
        public Expression Sub_n
        {
            get { return Subtract8(_programControlExpressions.ParameterByte1); }
        }

        [Instruction("SUB (IX+d)", 0x96, Cycles = 11, Prefix = "DD", ParameterMode = InstructionParameterMode.Index)]
        public Expression Sub_IXdi
        {
            get
            {
                return Expression.Block(
                    Expression.Assign(_tempExpressions.Temp1, _cpuValueExpressions.ReadIXd(_programControlExpressions.ParameterByte1)),
                        Subtract8(_tempExpressions.Temp1)
                    );
            }
        }

        [Instruction("SUB (IY+d)", 0x96, Cycles = 11, Prefix = "FD", ParameterMode = InstructionParameterMode.Index)]
        public Expression Sub_IYdi
        {
            get
            {
                return Expression.Block(
                    Expression.Assign(_tempExpressions.Temp1, _cpuValueExpressions.ReadIYd(_programControlExpressions.ParameterByte1)),
                        Subtract8(_tempExpressions.Temp1)
                    );
            }
        }

        [Instruction("NEG", 0x44, Cycles = 13, Prefix = "ED")]
        public Expression Neg
        {
            get
            {
                return Expression.Block(
                    Expression.Assign(_tempExpressions.Temp1, _cpuValueExpressions.RegisterA),
                    Expression.Assign(_cpuValueExpressions.RegisterA, Expression.Constant(0)),
                    Subtract8(_tempExpressions.Temp1)
                );
            }
        }
    }
}
