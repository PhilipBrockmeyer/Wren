using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wren.Emulation.MasterSystem.ExpressionLibraries;
using System.Linq.Expressions;

namespace Wren.Emulation.MasterSystem.Instructions
{
    public class SubtractWithCarryInstruction
    {
        IDataAccessExpressionLibrary _cpuValueExpressions;
        IFlagLookupValuesExpressionLibrary _flagExpressions;
        ITemporaryExpressionLibrary _tempExpressions;
        IProgramControlExpressionLibrary _programControlExpressions;

        public SubtractWithCarryInstruction(IDataAccessExpressionLibrary registerExpressions,
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
                Expression.Assign(_tempExpressions.Temp1, _cpuValueExpressions.FlagsRegister),

                 // Assign flags from precalculated flags array at [(a << 8) | b].
                Expression.IfThenElse(Expression.Equal(
                        Expression.And(_tempExpressions.Temp1, Expression.Constant(Flags.CarryFlag)),
                        Expression.Constant(Flags.CarryFlag)
                    ),

                    Expression.Assign(
                        _cpuValueExpressions.FlagsRegister,
                        Expression.ArrayIndex(_flagExpressions.SubtractionWithCarryFlagsCalcultorFlags,
                            Expression.Or(
                                Expression.LeftShift(_cpuValueExpressions.RegisterA, Expression.Constant(8)),
                                number
                            )
                        )
                    ),

                    Expression.Assign(
                        _cpuValueExpressions.FlagsRegister,
                        Expression.ArrayIndex(_flagExpressions.SubtractionFlagsCalcultorFlags,
                            Expression.Or(
                                Expression.LeftShift(_cpuValueExpressions.RegisterA, Expression.Constant(8)),
                                number
                            )
                        )
                    )
                ),

                // Subtract and truncate result.
                // A = (A - B - (F & 0x01)) & 0xFF 
                Expression.Assign(_cpuValueExpressions.RegisterA, 
                    Expression.And(
                        Expression.Subtract(
                            Expression.Subtract(_cpuValueExpressions.RegisterA, number),
                            Expression.And(_tempExpressions.Temp1, Expression.Constant(0x01))
                        ),
                        
                        Expression.Constant(0xFF, typeof(Int32))
                    )
                )
            );
        }

        private Expression Subtract16(Expression value2)
        {
            // value1 = HL
            return Expression.Block(
                // Temp1 = HL
                Expression.Assign(_tempExpressions.Temp1, Expression.Or(Expression.LeftShift(_cpuValueExpressions.RegisterH, Expression.Constant(0x08)), _cpuValueExpressions.RegisterL)),
                
                // Result = temp1 - (value2 + C).
                Expression.Assign(_tempExpressions.Temp2, 
                    Expression.Subtract(_tempExpressions.Temp1, 
                        Expression.Add(value2, Expression.And(_cpuValueExpressions.FlagsRegister, Expression.Constant(Flags.CarryFlag)))
                    )
                ),
                                
                // N = 1.
                Expression.Assign(_cpuValueExpressions.FlagsRegister, Expression.Constant(Flags.SubtractionFlag)),
                
                // ((H = HL ^ result ^ value2) >> 8) & 0x10
                Expression.OrAssign(_cpuValueExpressions.FlagsRegister, 
                    Expression.And(
                        Expression.RightShift(
                            Expression.ExclusiveOr(_tempExpressions.Temp1, 
                                Expression.ExclusiveOr(_tempExpressions.Temp2, value2)
                            ), Expression.Constant(0x08)
                        ), Expression.Constant(Flags.HalfCarry)
                    )
                ),

                // C = bit 17.
                Expression.OrAssign(_cpuValueExpressions.FlagsRegister, 
                    Expression.And(
                        Expression.RightShift(_tempExpressions.Temp2, Expression.Constant(16)),
                        Expression.Constant(Flags.CarryFlag)
                    )
                ),

                // S = bit 16.
                Expression.OrAssign(_cpuValueExpressions.FlagsRegister,
                    Expression.And(
                        Expression.RightShift(_tempExpressions.Temp2, Expression.Constant(0x08)),
                        Expression.Constant(Flags.SignFlag)
                    )
                ),

                // Z
                Expression.IfThen(
                    // result & 0xFFFF == 0
                    Expression.Equal(Expression.And(_tempExpressions.Temp2, Expression.Constant(0xFFFF)), Expression.Constant(0)),
                    Expression.OrAssign(_cpuValueExpressions.FlagsRegister, Expression.Constant(Flags.ZeroFlag))
                ),

                // V = ((value ^ HL) & (HL ^ result) & 0x8000) >> 13
                Expression.OrAssign(_cpuValueExpressions.FlagsRegister, 
                    Expression.RightShift(
                        Expression.And(
                            Expression.And(
                                Expression.ExclusiveOr(value2, _tempExpressions.Temp1),
                                Expression.ExclusiveOr(_tempExpressions.Temp2, _tempExpressions.Temp1)
                            ),
                            Expression.Constant(0x8000)
                        ), 
                        Expression.Constant(13)
                    )
                ),
                
                Expression.Assign(_cpuValueExpressions.RegisterH, Expression.And(Expression.RightShift(_tempExpressions.Temp2, Expression.Constant(8)), Expression.Constant(0xFF))),
                Expression.Assign(_cpuValueExpressions.RegisterL, Expression.And(_tempExpressions.Temp2, Expression.Constant(0xFF)))
            );
        }        

        [Instruction("SBC B", 0x98, Cycles = 4)]
        public Expression Sbc_B
        {
            get { return Subtract8(_cpuValueExpressions.RegisterB); }
        }

        [Instruction("SBC C", 0x99, Cycles = 4)]
        public Expression Sbc_C
        {
            get { return Subtract8(_cpuValueExpressions.RegisterC); }
        }

        [Instruction("SBC D", 0x9A, Cycles = 4)]
        public Expression Sbc_D
        {
            get { return Subtract8(_cpuValueExpressions.RegisterD); }
        }

        [Instruction("SBC E", 0x9B, Cycles = 4)]
        public Expression Sbc_E
        {
            get { return Subtract8(_cpuValueExpressions.RegisterE); }
        }

        [Instruction("SBC H", 0x9C, Cycles = 4)]
        public Expression Sbc_H
        {
            get { return Subtract8(_cpuValueExpressions.RegisterH); }
        }

        [Instruction("SBC L", 0x9D, Cycles = 4)]
        public Expression Sbc_L
        {
            get { return Subtract8(_cpuValueExpressions.RegisterL); }
        }

        [Instruction("SBC (HL)", 0x9E, Cycles = 7)]
        public Expression Sbc_HLi
        {
            get { return Subtract8(_cpuValueExpressions.ReadHL); }
        }
        [Instruction("SBC A", 0x9F, Cycles = 4)]
        public Expression Sbc_A
        {
            get { return Subtract8(_cpuValueExpressions.RegisterA); }
        }

        [Instruction("SBC n", 0xDE, Cycles = 7, ParameterMode = InstructionParameterMode.Byte)]
        public Expression Sbc_n
        {
            get { return Subtract8(_programControlExpressions.ParameterByte1); }
        }

        [Instruction("SBC A,(IX+d)", 0x9E, Cycles = 11, Prefix = "DD", ParameterMode = InstructionParameterMode.Index)]
        public Expression Sbc_IXdi
        {
            get
            {
                return Expression.Block(
                    Expression.Assign(_tempExpressions.Temp1, _cpuValueExpressions.ReadIXd(_programControlExpressions.ParameterByte1)),
                        Subtract8(_tempExpressions.Temp1)
                    );
            }
        }

        [Instruction("SBC A,(IY+d)", 0x9E, Cycles = 11, Prefix = "FD", ParameterMode = InstructionParameterMode.Index)]
        public Expression Sbc_IYdi
        {
            get
            {
                return Expression.Block(
                    Expression.Assign(_tempExpressions.Temp1, _cpuValueExpressions.ReadIYd(_programControlExpressions.ParameterByte1)),
                        Subtract8(_tempExpressions.Temp1)
                    );
            }
        }
        
        [Instruction("SBC HL,BC", 0x42, Cycles = 15, Prefix="ED")]
        public Expression Sbc_HL_BC
        {
            get 
            {
                return Expression.Block(
                    Expression.Assign(_tempExpressions.Temp3, Expression.Or(Expression.LeftShift(_cpuValueExpressions.RegisterB, Expression.Constant(0x08)), _cpuValueExpressions.RegisterC)),
                    Subtract16(_tempExpressions.Temp3)
                );
            }
        }

        [Instruction("SBC HL,DE", 0x52, Cycles = 15, Prefix = "ED")]
        public Expression Sbc_HL_DE
        {
            get
            {
                return Expression.Block(
                    Expression.Assign(_tempExpressions.Temp3, Expression.Or(Expression.LeftShift(_cpuValueExpressions.RegisterD, Expression.Constant(0x08)), _cpuValueExpressions.RegisterE)),
                    Subtract16(_tempExpressions.Temp3)
                );
            }
        }

        [Instruction("SBC HL,HL", 0x62, Cycles = 15, Prefix = "ED")]
        public Expression Sbc_HL_HL
        {
            get
            {
                return Expression.Block(
                    Expression.Assign(_tempExpressions.Temp3, Expression.Or(Expression.LeftShift(_cpuValueExpressions.RegisterH, Expression.Constant(0x08)), _cpuValueExpressions.RegisterL)),
                    Subtract16(_tempExpressions.Temp3)
                );
            }
        }


        [Instruction("SBC HL,SP", 0x72, Cycles = 15, Prefix = "ED")]
        public Expression Sbc_HL_SP
        {
            get
            {
                return Subtract16(_cpuValueExpressions.StackPointerRegister);
            }
        }
    }
}
