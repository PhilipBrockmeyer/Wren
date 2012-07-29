using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wren.Emulation.MasterSystem.ExpressionLibraries;
using System.Linq.Expressions;

namespace Wren.Emulation.MasterSystem.Instructions
{
    public class SearchBlockInstruction
    {
        IDataAccessExpressionLibrary _cpuValueExpressions;
        IFlagLookupValuesExpressionLibrary _flagExpressions;
        ITemporaryExpressionLibrary _tempExpressions;
        IProgramControlExpressionLibrary _programControlExpressions;

        public SearchBlockInstruction(IDataAccessExpressionLibrary registerExpressions,
                                 IFlagLookupValuesExpressionLibrary flagExpressions,
                                 ITemporaryExpressionLibrary temporaryExpressions,
                                 IProgramControlExpressionLibrary programControlExpressions)
        {
            _cpuValueExpressions = registerExpressions;
            _flagExpressions = flagExpressions;
            _tempExpressions = temporaryExpressions;
            _programControlExpressions = programControlExpressions;
        }

        //[Instruction("CPI", 0xA1, Cycles = 16, Prefix = "ED")]
        public Expression Cpi
        {
            get
            {
                return Expression.Block(
                    // Assign flags from precalculated flags array at [(a << 8) | (HL)].
                    Expression.Assign(
                        _cpuValueExpressions.FlagsRegister,
                        Expression.Or(Expression.And(_cpuValueExpressions.FlagsRegister, Expression.Constant(Flags.CarryFlag)),
                            Expression.And(
                                Expression.ArrayIndex(_flagExpressions.SubtractionFlagsCalcultorFlags,
                                    Expression.Or(
                                        Expression.LeftShift(_cpuValueExpressions.RegisterA, Expression.Constant(8)),
                                        _cpuValueExpressions.ReadHL
                                    )
                                ), Expression.Constant(0xFE)
                            )
                        )
                    ),

                    // HL++
                    Expression.Assign(_tempExpressions.Temp1, Expression.Or(Expression.LeftShift(_cpuValueExpressions.RegisterH, Expression.Constant(0x08)), _cpuValueExpressions.RegisterL)),
                    Expression.PostIncrementAssign(_tempExpressions.Temp1),
                    Expression.Assign(_cpuValueExpressions.RegisterH, Expression.RightShift(_tempExpressions.Temp1, Expression.Constant(0x08))),
                    Expression.Assign(_cpuValueExpressions.RegisterL, Expression.And(_tempExpressions.Temp1, Expression.Constant(0xFF))),

                    // BC--
                    Expression.Assign(_tempExpressions.Temp1, Expression.Or(Expression.LeftShift(_cpuValueExpressions.RegisterB, Expression.Constant(0x08)), _cpuValueExpressions.RegisterC)),
                    Expression.PostDecrementAssign(_tempExpressions.Temp1),
                    Expression.Assign(_cpuValueExpressions.RegisterB, Expression.RightShift(_tempExpressions.Temp1, Expression.Constant(0x08))),
                    Expression.Assign(_cpuValueExpressions.RegisterC, Expression.And(_tempExpressions.Temp1, Expression.Constant(0xFF))),

                    // Set p if BC == 0
                    Expression.IfThen(Expression.Equal(_tempExpressions.Temp1, Expression.Constant(0)),
                        Expression.OrAssign(_cpuValueExpressions.FlagsRegister,
                            Expression.Constant(Flags.ParityFlag)
                        )
                    )                    
                );
            }
        }

        //[Instruction("CPD", 0xA9, Cycles = 16, Prefix = "ED")]
        public Expression Cpd
        {
            get
            {
                return Expression.Block(
                    // Assign flags from precalculated flags array at [(a << 8) | (HL)].
                    Expression.Assign(
                        _cpuValueExpressions.FlagsRegister,
                        Expression.Or(Expression.And(_cpuValueExpressions.FlagsRegister, Expression.Constant(Flags.CarryFlag)),
                            Expression.And(
                                Expression.ArrayIndex(_flagExpressions.SubtractionFlagsCalcultorFlags,
                                    Expression.Or(
                                        Expression.LeftShift(_cpuValueExpressions.RegisterA, Expression.Constant(8)),
                                        _cpuValueExpressions.ReadHL
                                    )
                                ), Expression.Constant(0xFE)
                            )
                        )
                    ),

                    // HL--
                    Expression.Assign(_tempExpressions.Temp1, Expression.Or(Expression.LeftShift(_cpuValueExpressions.RegisterH, Expression.Constant(0x08)), _cpuValueExpressions.RegisterL)),
                    Expression.PostDecrementAssign(_tempExpressions.Temp1),
                    Expression.Assign(_cpuValueExpressions.RegisterH, Expression.RightShift(_tempExpressions.Temp1, Expression.Constant(0x08))),
                    Expression.Assign(_cpuValueExpressions.RegisterL, Expression.And(_tempExpressions.Temp1, Expression.Constant(0xFF))),

                    // BC--
                    Expression.Assign(_tempExpressions.Temp1, Expression.Or(Expression.LeftShift(_cpuValueExpressions.RegisterB, Expression.Constant(0x08)), _cpuValueExpressions.RegisterC)),
                    Expression.PostDecrementAssign(_tempExpressions.Temp1),
                    Expression.Assign(_cpuValueExpressions.RegisterB, Expression.RightShift(_tempExpressions.Temp1, Expression.Constant(0x08))),
                    Expression.Assign(_cpuValueExpressions.RegisterC, Expression.And(_tempExpressions.Temp1, Expression.Constant(0xFF))),

                    // Set p if BC == 0
                    Expression.IfThen(Expression.Equal(_tempExpressions.Temp1, Expression.Constant(0)),
                        Expression.OrAssign(_cpuValueExpressions.FlagsRegister,
                            Expression.Constant(Flags.ParityFlag)
                        )
                    )
                );
            }
        }

        LabelTarget _endSearchCpir = Expression.Label("EndSearch");
        [Instruction("CPIR", 0xB1, Prefix = "ED")]
        public Expression Cpir
        {
            get
            {
                return Expression.Block(

                    // Temp1 = BC
                    Expression.Assign(_tempExpressions.Temp1, Expression.Or(Expression.LeftShift(_cpuValueExpressions.RegisterB, Expression.Constant(0x08)), _cpuValueExpressions.RegisterC)),
                   
                    // Temp3 = HL
                    Expression.Assign(_tempExpressions.Temp3, Expression.Or(Expression.LeftShift(_cpuValueExpressions.RegisterH, Expression.Constant(0x08)), _cpuValueExpressions.RegisterL)),

                    _cpuValueExpressions.ReadByte(_tempExpressions.Temp3, _tempExpressions.Temp4),

                    // Assign flags from precalculated flags array at [(a << 8) | (HL)].
                    Expression.Assign(
                        _cpuValueExpressions.FlagsRegister,
                        Expression.Or(Expression.And(_cpuValueExpressions.FlagsRegister, Expression.Constant(Flags.CarryFlag)),
                            Expression.And(
                                Expression.ArrayIndex(_flagExpressions.SubtractionFlagsCalcultorFlags,
                                    Expression.Or(
                                        Expression.LeftShift(_cpuValueExpressions.RegisterA, Expression.Constant(8)),
                                        _tempExpressions.Temp4
                                    )
                                ), Expression.Constant(0xFE)
                            )
                        )
                    ),

                    Expression.PostDecrementAssign(_tempExpressions.Temp1), // BC--
                    Expression.PostIncrementAssign(_tempExpressions.Temp3), // HL++
                    
                    // BC = Temp1
                    Expression.Assign(_cpuValueExpressions.RegisterB, Expression.RightShift(_tempExpressions.Temp1, Expression.Constant(0x08))),
                    Expression.Assign(_cpuValueExpressions.RegisterC, Expression.And(_tempExpressions.Temp1, Expression.Constant(0xFF))),

                    // HL = Temp3
                    Expression.Assign(_cpuValueExpressions.RegisterH, Expression.RightShift(_tempExpressions.Temp3, Expression.Constant(0x08))),
                    Expression.Assign(_cpuValueExpressions.RegisterL, Expression.And(_tempExpressions.Temp3, Expression.Constant(0xFF))),

                    // If BC != 0 && Z Not Set.
                    Expression.IfThenElse(
                        Expression.AndAlso(
                            Expression.NotEqual(_tempExpressions.Temp1, Expression.Constant(0)),
                            Expression.Equal(Expression.And(_cpuValueExpressions.FlagsRegister, Expression.Constant(Flags.ZeroFlag)), Expression.Constant(0))
                        ),
                        
                        Expression.Block(
                            Expression.SubtractAssign(_programControlExpressions.ProgramCounterRegister, Expression.Constant(2)),
                            
                            // Set P
                            Expression.OrAssign(_cpuValueExpressions.FlagsRegister, Expression.Constant(Flags.ParityFlag)),
                            Expression.SubtractAssign(_programControlExpressions.CycleCounter, Expression.Constant(21))
                        ),

                        Expression.Block(
                            // Reset P
                            Expression.AndAssign(_cpuValueExpressions.FlagsRegister, Expression.Constant(0xFB)),
                            Expression.SubtractAssign(_programControlExpressions.CycleCounter, Expression.Constant(16))                            
                        )
                    ),

                    Expression.SubtractAssign(_programControlExpressions.CycleCounter, Expression.Constant(4))
                );

            }
        }

        LabelTarget _endSearchCpdr= Expression.Label("EndSearch");
        //[Instruction("CPDR", 0xB9, Prefix = "ED")]
        public Expression Cpdr
        {
            get
            {
                return Expression.Block(

                    // Temp1 = BC
                    Expression.Assign(_tempExpressions.Temp1, Expression.Or(Expression.LeftShift(_cpuValueExpressions.RegisterB, Expression.Constant(0x08)), _cpuValueExpressions.RegisterC)),

                    // Temp3 = HL
                    Expression.Assign(_tempExpressions.Temp3, Expression.Or(Expression.LeftShift(_cpuValueExpressions.RegisterH, Expression.Constant(0x08)), _cpuValueExpressions.RegisterL)),

                    Expression.Loop(
                        Expression.Block(
                            _cpuValueExpressions.ReadByte(_tempExpressions.Temp3, _tempExpressions.Temp4),

                            // Assign flags from precalculated flags array at [(a << 8) | (HL)].
                            Expression.Assign(
                                _cpuValueExpressions.FlagsRegister,
                                Expression.Or(Expression.And(_cpuValueExpressions.FlagsRegister, Expression.Constant(Flags.CarryFlag)),
                                    Expression.And(
                                        Expression.ArrayIndex(_flagExpressions.SubtractionFlagsCalcultorFlags,
                                            Expression.Or(
                                                Expression.LeftShift(_cpuValueExpressions.RegisterA, Expression.Constant(8)),
                                                _cpuValueExpressions.ReadHL
                                            )
                                        ), Expression.Constant(0xFE)
                                    )
                                )
                            ),

                            Expression.PostDecrementAssign(_tempExpressions.Temp1), // BC--
                            Expression.PostDecrementAssign(_tempExpressions.Temp3), // HL--

                            // If BC == 0.
                            Expression.IfThen(Expression.Equal(_tempExpressions.Temp1, Expression.Constant(0)),
                                Expression.Block(
                                    Expression.OrAssign(_cpuValueExpressions.FlagsRegister,
                                        Expression.Constant(Flags.ParityFlag)
                                    ),
                                    Expression.Goto(_endSearchCpir)
                                )
                            ),

                            // If RegisterA == (HL)
                            Expression.IfThen(Expression.NotEqual(Expression.And(_cpuValueExpressions.FlagsRegister, Expression.Constant(Flags.ZeroFlag)), Expression.Constant(0)),
                                Expression.Goto(_endSearchCpir)
                            ),

                            // Cycles -= 21
                            Expression.SubtractAssign(_programControlExpressions.CycleCounter, Expression.Constant(21))
                        )
                    ),

                    Expression.Label(_endSearchCpir),

                    // BC = Temp1
                    Expression.Assign(_cpuValueExpressions.RegisterB, Expression.RightShift(_tempExpressions.Temp2, Expression.Constant(0x08))),
                    Expression.Assign(_cpuValueExpressions.RegisterC, Expression.And(_tempExpressions.Temp2, Expression.Constant(0xFF))),

                    // HL = Temp3
                    Expression.Assign(_cpuValueExpressions.RegisterH, Expression.RightShift(_tempExpressions.Temp3, Expression.Constant(0x08))),
                    Expression.Assign(_cpuValueExpressions.RegisterL, Expression.And(_tempExpressions.Temp3, Expression.Constant(0xFF))),

                    // Cycles -= 16
                    Expression.SubtractAssign(_programControlExpressions.CycleCounter, Expression.Constant(16))
                );
            }
        }
    }
}
