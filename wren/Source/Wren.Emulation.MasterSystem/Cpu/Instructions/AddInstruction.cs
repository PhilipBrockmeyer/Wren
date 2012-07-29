using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using Wren.Emulation.MasterSystem.ExpressionLibraries;

namespace Wren.Emulation.MasterSystem.Instructions
{
    public class AddInstruction
    {
        IDataAccessExpressionLibrary _cpuValueExpressions;
        IFlagLookupValuesExpressionLibrary _flagExpressions;
        ITemporaryExpressionLibrary _tempExpressions;
        IProgramControlExpressionLibrary _programControlExpressions;

        public AddInstruction(IDataAccessExpressionLibrary registerExpressions,
                              IFlagLookupValuesExpressionLibrary flagExpressions,
                              ITemporaryExpressionLibrary temporaryExpressions,
                              IProgramControlExpressionLibrary programControlExpressions)
        {
            _cpuValueExpressions = registerExpressions;
            _flagExpressions = flagExpressions;
            _tempExpressions = temporaryExpressions;
            _programControlExpressions = programControlExpressions;
        }

        private Expression Add8(Expression addend)
        {
            return Expression.Block(
               // Assign flags from precalculated flags array at [(a << 8) | b].
                Expression.Assign(
                    _cpuValueExpressions.FlagsRegister, 
                    Expression.ArrayIndex(_flagExpressions.AdditionFlagsCalcultorFlags,
                        Expression.Or(
                            Expression.LeftShift(_cpuValueExpressions.RegisterA, Expression.Constant(8)),
                            addend
                        )
                    )
                ),
                        
                // Add and truncate result.
                // A = (A + B) & 0xFF 
                Expression.AddAssign(_cpuValueExpressions.RegisterA, addend),
                Expression.AndAssign(_cpuValueExpressions.RegisterA, Expression.Constant(0xFF, typeof(Int32)))
            );
        }

        private Expression Add16(Expression value1, Expression value2)
        {
            return Expression.Block(
                // Store Temporary result.
                Expression.Assign(_tempExpressions.Temp1, 
                    Expression.Add(value1, value2)
                ),

                // Set Flags
                // (F & 0xc4) | (((a ^ result ^ b) >> 8) & 0x10) | ((result >> 16) & 1)
                Expression.Assign(_cpuValueExpressions.FlagsRegister,
                    Expression.Or(
                        Expression.Or(
                            // F & C4 - leaves SZV flags unchanged.
                            Expression.And(_cpuValueExpressions.FlagsRegister, Expression.Constant(0xC4)),
                        
                            // ((a ^ result ^ b) >> 8) & 0x10 - sets half carry to half carry of high byte addition.
                            Expression.And(
                                Expression.RightShift(
                                    Expression.ExclusiveOr(Expression.ExclusiveOr(value1, _tempExpressions.Temp1), value2),
                                    Expression.Constant(0x08)
                                ),
                                Expression.Constant(0x10)
                            )

                        ),

                        // (result >> 16) & 0x01 - Set carry flag to 17th bit after addition.
                        Expression.And(
                            Expression.RightShift(_tempExpressions.Temp1, Expression.Constant(16)),
                            Expression.Constant(0x01)
                        )

                    )
                ),

                Expression.Assign(value1,
                    Expression.And(_tempExpressions.Temp1, Expression.Constant(0xFFFF))
                )
            );
        }

        [Instruction("ADD HL,BC", 0x09, Cycles = 11)]
        public Expression Add_HL_BC
        {
            get 
            { 
                // Temp2 = HL,
                // Temp3 = BC
                return Expression.Block(
                    Expression.Assign(_tempExpressions.Temp2, 
                        Expression.Or(
                            Expression.LeftShift(_cpuValueExpressions.RegisterH, Expression.Constant(0x08)),
                            _cpuValueExpressions.RegisterL
                        )
                    ),

                    Expression.Assign(_tempExpressions.Temp3, 
                        Expression.Or(
                            Expression.LeftShift(_cpuValueExpressions.RegisterB, Expression.Constant(0x08)),
                            _cpuValueExpressions.RegisterC
                        )
                    ),

                    Add16(_tempExpressions.Temp2, _tempExpressions.Temp3),

                    Expression.Assign(_cpuValueExpressions.RegisterH, 
                        Expression.RightShift(_tempExpressions.Temp2, Expression.Constant(0x08))
                    ),

                    Expression.Assign(_cpuValueExpressions.RegisterL, 
                        Expression.And(_tempExpressions.Temp2, Expression.Constant(0xFF))
                    )
                ); 
            }
        }

        [Instruction("ADD HL,DE", 0x19, Cycles = 11)]
        public Expression Add_HL_DE
        {
            get
            {
                // Temp2 = HL,
                // Temp3 = DE
                return Expression.Block(
                    Expression.Assign(_tempExpressions.Temp2,
                        Expression.Or(
                            Expression.LeftShift(_cpuValueExpressions.RegisterH, Expression.Constant(0x08)),
                            _cpuValueExpressions.RegisterL
                        )
                    ),

                    Expression.Assign(_tempExpressions.Temp3,
                        Expression.Or(
                            Expression.LeftShift(_cpuValueExpressions.RegisterD, Expression.Constant(0x08)),
                            _cpuValueExpressions.RegisterE
                        )
                    ),

                    Add16(_tempExpressions.Temp2, _tempExpressions.Temp3),

                    Expression.Assign(_cpuValueExpressions.RegisterH,
                        Expression.RightShift(_tempExpressions.Temp2, Expression.Constant(0x08))
                    ),

                    Expression.Assign(_cpuValueExpressions.RegisterL,
                        Expression.And(_tempExpressions.Temp2, Expression.Constant(0xFF))
                    )
                );
            }
        }

        [Instruction("ADD HL,HL", 0x29, Cycles = 11)]
        public Expression Add_HL_HL
        {
            get
            {
                // Temp2 = HL,
                // Temp3 = HL
                return Expression.Block(
                    Expression.Assign(_tempExpressions.Temp2,
                        Expression.Or(
                            Expression.LeftShift(_cpuValueExpressions.RegisterH, Expression.Constant(0x08)),
                            _cpuValueExpressions.RegisterL
                        )
                    ),

                    Expression.Assign(_tempExpressions.Temp3,
                        Expression.Or(
                            Expression.LeftShift(_cpuValueExpressions.RegisterH, Expression.Constant(0x08)),
                            _cpuValueExpressions.RegisterL
                        )
                    ),

                    Add16(_tempExpressions.Temp2, _tempExpressions.Temp3),

                    Expression.Assign(_cpuValueExpressions.RegisterH,
                        Expression.RightShift(_tempExpressions.Temp2, Expression.Constant(0x08))
                    ),

                    Expression.Assign(_cpuValueExpressions.RegisterL,
                        Expression.And(_tempExpressions.Temp2, Expression.Constant(0xFF))
                    )
                );
            }
        }

        [Instruction("ADD HL,SP", 0x39, Cycles = 11)]
        public Expression Add_HL_SP
        {
            get
            {
                // Temp2 = HL,
                // Temp3 = SP
                return Expression.Block(
                    Expression.Assign(_tempExpressions.Temp2,
                        Expression.Or(
                            Expression.LeftShift(_cpuValueExpressions.RegisterH, Expression.Constant(0x08)),
                            _cpuValueExpressions.RegisterL
                        )
                    ),

                    Expression.Assign(_tempExpressions.Temp3, _cpuValueExpressions.StackPointerRegister),

                    Add16(_tempExpressions.Temp2, _tempExpressions.Temp3),

                    Expression.Assign(_cpuValueExpressions.RegisterH,
                        Expression.RightShift(_tempExpressions.Temp2, Expression.Constant(0x08))
                    ),

                    Expression.Assign(_cpuValueExpressions.RegisterL,
                        Expression.And(_tempExpressions.Temp2, Expression.Constant(0xFF))
                    )
                );
            }
        }

        [Instruction("ADD A,B", 0x80, Cycles = 4)]
        public Expression Add_A_B
        {
            get { return Add8(_cpuValueExpressions.RegisterB); }
        }

        [Instruction("ADD A,C", 0x81, Cycles = 4)]
        public Expression Add_A_C
        {
            get { return Add8(_cpuValueExpressions.RegisterC); } 
        }

        [Instruction("ADD A,D", 0x82, Cycles = 4)]
        public Expression Add_A_D
        {
            get { return Add8(_cpuValueExpressions.RegisterD); }
        }

        [Instruction("ADD A,E", 0x83, Cycles = 4)]
        public Expression Add_A_E
        {
            get { return Add8(_cpuValueExpressions.RegisterE); }
        }

        [Instruction("ADD A,H", 0x84, Cycles = 4)]
        public Expression Add_A_H
        {
            get { return Add8(_cpuValueExpressions.RegisterH); }
        }

        [Instruction("ADD A,L", 0x85, Cycles = 4)]
        public Expression Add_A_L
        {
            get { return Add8(_cpuValueExpressions.RegisterL); }
        }

        [Instruction("ADD A,(HL)", 0x86, Cycles = 7)]
        public Expression Add_A_HLi
        {
            get
            {
                return Expression.Block(
                            Expression.Assign(_tempExpressions.Temp1,  _cpuValueExpressions.ReadHL),
                            Add8(_tempExpressions.Temp1)
                  );
            }
        }

        [Instruction("ADD A,A", 0x87, Cycles = 4)]
        public Expression Add_A_A
        {
            get { return Add8(_cpuValueExpressions.RegisterA); }
        }

        [Instruction("ADD A,n", 0xC6, Cycles = 7, ParameterMode=InstructionParameterMode.Byte)]
        public Expression Add_A_n
        {
            get { return Add8(_programControlExpressions.ParameterByte1); }
        }

        [Instruction("ADD IX,BC", 0x09, Prefix="DD", Cycles = 15)]
        public Expression Add_IX_BC
        {
            get
            {
                // Temp3 = BC
                return Expression.Block(
                    Expression.Assign(_tempExpressions.Temp3,
                        Expression.Or(
                            Expression.LeftShift(_cpuValueExpressions.RegisterB, Expression.Constant(0x08)),
                            _cpuValueExpressions.RegisterC
                        )
                    ),

                    Add16(_cpuValueExpressions.RegisterIX, _tempExpressions.Temp3)
                );
            }
        }

        [Instruction("ADD IY,BC", 0x09, Prefix = "FD", Cycles = 15)]
        public Expression Add_IY_BC
        {
            get
            {
                // Temp3 = BC
                return Expression.Block(
                    Expression.Assign(_tempExpressions.Temp3,
                        Expression.Or(
                            Expression.LeftShift(_cpuValueExpressions.RegisterB, Expression.Constant(0x08)),
                            _cpuValueExpressions.RegisterC
                        )
                    ),

                    Add16(_cpuValueExpressions.RegisterIY, _tempExpressions.Temp3)
                );
            }
        }

        [Instruction("ADD IX,DE", 0x19, Prefix = "DD", Cycles = 15)]
        public Expression Add_IX_DE
        {
            get
            {
                // Temp3 = DE
                return Expression.Block(
                    Expression.Assign(_tempExpressions.Temp3,
                        Expression.Or(
                            Expression.LeftShift(_cpuValueExpressions.RegisterD, Expression.Constant(0x08)),
                            _cpuValueExpressions.RegisterE
                        )
                    ),

                    Add16(_cpuValueExpressions.RegisterIX, _tempExpressions.Temp3)
                );
            }
        }

        [Instruction("ADD IY,DE", 0x19, Prefix = "FD", Cycles = 15)]
        public Expression Add_IY_DE
        {
            get
            {
                // Temp3 = DE
                return Expression.Block(
                    Expression.Assign(_tempExpressions.Temp3,
                        Expression.Or(
                            Expression.LeftShift(_cpuValueExpressions.RegisterD, Expression.Constant(0x08)),
                            _cpuValueExpressions.RegisterE
                        )
                    ),

                    Add16(_cpuValueExpressions.RegisterIY, _tempExpressions.Temp3)
                );
            }
        }

        [Instruction("ADD IX,IX", 0x29, Prefix = "DD", Cycles = 14)]
        public Expression Add_IX_IX
        {
            get { return Add16(_cpuValueExpressions.RegisterIX, _cpuValueExpressions.RegisterIX); }
        }

        [Instruction("ADD IY,IY", 0x29, Prefix = "FD", Cycles = 14)]
        public Expression Add_IY_IY
        {
            get { return Add16(_cpuValueExpressions.RegisterIY, _cpuValueExpressions.RegisterIY); }
        }

        [Instruction("ADD IX,SP", 0x39, Prefix = "DD", Cycles = 15)]
        public Expression Add_IX_SP
        {
            get { return Add16(_cpuValueExpressions.RegisterIX, _cpuValueExpressions.StackPointerRegister); }
        }

        [Instruction("ADD IY,SP", 0x39, Prefix = "FD", Cycles = 15)]
        public Expression Add_IY_SP
        {
            get { return Add16(_cpuValueExpressions.RegisterIY, _cpuValueExpressions.StackPointerRegister); }
        }

        [Instruction("ADD A,IXH", 0x84, Prefix = "DD", Cycles = 9)]
        public Expression Add_A_IXH
        {
            get { return Add8(Expression.RightShift(_cpuValueExpressions.RegisterIX, Expression.Constant(0x08))); }
        }

        [Instruction("ADD A,IYH", 0x84, Prefix = "FD", Cycles = 9)]
        public Expression Add_A_IYH
        {
            get { return Add8(Expression.RightShift(_cpuValueExpressions.RegisterIY, Expression.Constant(0x08))); }
        }

        [Instruction("ADD A,IXL", 0x85, Prefix = "DD", Cycles = 9)]
        public Expression Add_A_IXL
        {
            get { return Add8(Expression.And(_cpuValueExpressions.RegisterIX, Expression.Constant(0xFF))); }
        }

        [Instruction("ADD A,IYL", 0x85, Prefix = "FD", Cycles = 9)]
        public Expression Add_A_IYL
        {
            get { return Add8(Expression.And(_cpuValueExpressions.RegisterIY, Expression.Constant(0xFF))); }
        }


        [Instruction("ADD A,(IX+d)", 0x86, Cycles = 11, Prefix = "DD", ParameterMode=InstructionParameterMode.Index)]
        public Expression Add_A_IX_d
        {
            get
            {
                return Expression.Block(
                    Expression.Assign(_tempExpressions.Temp1,  _cpuValueExpressions.ReadIXd(_programControlExpressions.ParameterByte1)),
                    Add8(_tempExpressions.Temp1)
                    );
            }
        }

        [Instruction("ADD A,(IY+d)", 0x86, Cycles = 11, Prefix = "FD", ParameterMode = InstructionParameterMode.Index)]
        public Expression Add_A_IY_d
        {
            get
            {
                return Expression.Block(
                    Expression.Assign(_tempExpressions.Temp1, _cpuValueExpressions.ReadIYd(_programControlExpressions.ParameterByte1)),
                    Add8(_tempExpressions.Temp1)
                    );
            }
        }
    }
}
