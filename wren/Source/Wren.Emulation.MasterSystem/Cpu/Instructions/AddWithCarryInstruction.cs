using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wren.Emulation.MasterSystem.ExpressionLibraries;
using System.Linq.Expressions;

namespace Wren.Emulation.MasterSystem.Instructions
{
    public class AddWithCarryInstruction
    {
        IDataAccessExpressionLibrary _cpuValueExpressions;
        IFlagLookupValuesExpressionLibrary _flagExpressions;
        ITemporaryExpressionLibrary _tempExpressions;
        IProgramControlExpressionLibrary _programControlExpressions;

        public AddWithCarryInstruction(IDataAccessExpressionLibrary registerExpressions,
                              IFlagLookupValuesExpressionLibrary flagExpressions,
                              ITemporaryExpressionLibrary temporaryExpressions,
                              IProgramControlExpressionLibrary programControlExpressions)
        {
            _cpuValueExpressions = registerExpressions;
            _flagExpressions = flagExpressions;
            _tempExpressions = temporaryExpressions;
            _programControlExpressions = programControlExpressions;
        }

        private Expression Adc(Expression addend)
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
                        Expression.ArrayIndex(_flagExpressions.AdditionWithCarryFlagsCalcultorFlags,
                            Expression.Or(
                                Expression.LeftShift(_cpuValueExpressions.RegisterA, Expression.Constant(8)),
                                addend
                            )
                        )
                    ),

                    Expression.Assign(
                        _cpuValueExpressions.FlagsRegister, 
                        Expression.ArrayIndex(_flagExpressions.AdditionFlagsCalcultorFlags,
                            Expression.Or(
                                Expression.LeftShift(_cpuValueExpressions.RegisterA, Expression.Constant(8)),
                                addend
                            )
                        )
                    )
                ),
                        
                // Add and truncate result.
                // A = (A + B + (F & 0x01)) & 0xFF 
                Expression.Assign(_cpuValueExpressions.RegisterA, 
                    Expression.And(
                        Expression.Add(
                            Expression.And(_tempExpressions.Temp1, Expression.Constant(0x01)),
                            Expression.Add(_cpuValueExpressions.RegisterA, addend)
                        ),
                        
                        Expression.Constant(0xFF, typeof(Int32))
                    )
                )
            );
        }

        private Expression Adc16(Expression value2)
        {
            // value1 = HL
            return Expression.Block(
                // Temp1 = HL
                Expression.Assign(_tempExpressions.Temp1, Expression.Or(Expression.LeftShift(_cpuValueExpressions.RegisterH, Expression.Constant(0x08)), _cpuValueExpressions.RegisterL)),

                // Result = temp1 + value2 + C.
                Expression.Assign(_tempExpressions.Temp2,
                    Expression.Add(_tempExpressions.Temp1,
                        Expression.Add(value2, Expression.And(_cpuValueExpressions.FlagsRegister, Expression.Constant(Flags.CarryFlag)))
                    )
                ),               

                // ((H = HL ^ result ^ value2) >> 8) & 0x10
                Expression.Assign(_cpuValueExpressions.FlagsRegister,
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

                // V = ((value ^ _HLD ^ 0x8000) & (value ^ result) & 0x8000) >> 13
                Expression.OrAssign(_cpuValueExpressions.FlagsRegister,
                    Expression.RightShift(
                        Expression.And(
                            Expression.And(
                                Expression.ExclusiveOr(Expression.ExclusiveOr(value2, _tempExpressions.Temp1), Expression.Constant(0x8000)),
                                Expression.ExclusiveOr(value2, _tempExpressions.Temp2)
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

        [Instruction("ADC A,B", 0x88, Cycles = 4)]
        public Expression Adc_A_B
        {
            get { return Adc(_cpuValueExpressions.RegisterB); }
        }

        [Instruction("ADC A,C", 0x89, Cycles = 4)]
        public Expression Adc_A_C
        {
            get { return Adc(_cpuValueExpressions.RegisterC); }
        }

        [Instruction("ADC A,D", 0x8A, Cycles = 4)]
        public Expression Adc_A_D
        {
            get { return Adc(_cpuValueExpressions.RegisterD); }
        }

        [Instruction("ADC A,E", 0x8B, Cycles = 4)]
        public Expression Adc_A_E
        {
            get { return Adc(_cpuValueExpressions.RegisterE); }
        }

        [Instruction("ADC A,H", 0x8C, Cycles = 4)]
        public Expression Adc_A_H
        {
            get { return Adc(_cpuValueExpressions.RegisterH); }
        }

        [Instruction("ADC A,L", 0x8D, Cycles = 4)]
        public Expression Adc_A_L
        {
            get { return Adc(_cpuValueExpressions.RegisterL); }
        }

        [Instruction("ADC A,(HL)", 0x8E, Cycles = 7)]
        public Expression Adc_A_HLi
        {
            get { return Adc(_cpuValueExpressions.ReadHL); }
        }

        [Instruction("ADC A,A", 0x8F, Cycles = 4)]
        public Expression Adc_A_A
        {
            get { return Adc(_cpuValueExpressions.RegisterA); }
        }

        [Instruction("ADC A,n", 0xCE, Cycles = 7, ParameterMode=InstructionParameterMode.Byte)]
        public Expression Adc_A_n
        {
            get { return Adc(_programControlExpressions.ParameterByte1); }
        }

        [Instruction("ADC A,(IX+d)", 0x8E, Cycles = 11, Prefix = "DD", ParameterMode = InstructionParameterMode.Index)]
        public Expression Adc_A_IX_d
        {
            get
            {
                return Expression.Block(
                    Expression.Assign(_tempExpressions.Temp1, _cpuValueExpressions.ReadIXd(_programControlExpressions.ParameterByte1)),
                        Adc(_tempExpressions.Temp1)
                    );
            }
        }

        [Instruction("ADC A,(IY+d)", 0x8E, Cycles = 11, Prefix = "FD", ParameterMode = InstructionParameterMode.Index)]
        public Expression Adc_A_IY_d
        {
            get
            {
                return Expression.Block(
                    Expression.Assign(_tempExpressions.Temp1, _cpuValueExpressions.ReadIYd(_programControlExpressions.ParameterByte1)),
                        Adc(_tempExpressions.Temp1)
                    );
            }
        }

        [Instruction("ADC HL,BC", 0x4A, Cycles = 15, Prefix = "ED")]
        public Expression Adc_HL_BC
        {
            get
            {
                return Expression.Block(
                    Expression.Assign(_tempExpressions.Temp3, Expression.Or(Expression.LeftShift(_cpuValueExpressions.RegisterB, Expression.Constant(0x08)), _cpuValueExpressions.RegisterC)),
                    Adc16(_tempExpressions.Temp3)
                );
            }
        }

        [Instruction("ADC HL,DE", 0x5A, Cycles = 15, Prefix = "ED")]
        public Expression Adc_HL_DE
        {
            get
            {
                return Expression.Block(
                    Expression.Assign(_tempExpressions.Temp3, Expression.Or(Expression.LeftShift(_cpuValueExpressions.RegisterD, Expression.Constant(0x08)), _cpuValueExpressions.RegisterE)),
                    Adc16(_tempExpressions.Temp3)
                );
            }
        }

        [Instruction("ADC HL,HL", 0x6A, Cycles = 15, Prefix = "ED")]
        public Expression Adc_HL_HL
        {
            get
            {
                return Expression.Block(
                    Expression.Assign(_tempExpressions.Temp3, Expression.Or(Expression.LeftShift(_cpuValueExpressions.RegisterH, Expression.Constant(0x08)), _cpuValueExpressions.RegisterL)),
                    Adc16(_tempExpressions.Temp3)
                );
            }
        }


        [Instruction("ADC HL,SP", 0x7A, Cycles = 15, Prefix = "ED")]
        public Expression Sbc_HL_SP
        {
            get
            {
                return Adc16(_cpuValueExpressions.StackPointerRegister);
            }
        }

    }
}
