using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wren.Emulation.MasterSystem.ExpressionLibraries;
using System.Linq.Expressions;

namespace Wren.Emulation.MasterSystem.Instructions
{
    public class RotateAInstruction
    {
        IDataAccessExpressionLibrary _cpuValueExpressions;
        IFlagLookupValuesExpressionLibrary _flagExpressions;
        ITemporaryExpressionLibrary _tempExpressions;
        IProgramControlExpressionLibrary _programControlExpressions;

        public RotateAInstruction(IDataAccessExpressionLibrary registerExpressions,
                                 IFlagLookupValuesExpressionLibrary flagExpressions,
                                 ITemporaryExpressionLibrary temporaryExpressions,
                                 IProgramControlExpressionLibrary programControlExpressions)
        {
            _cpuValueExpressions = registerExpressions;
            _flagExpressions = flagExpressions;
            _tempExpressions = temporaryExpressions;
            _programControlExpressions = programControlExpressions;
        }

        [Instruction("RLCA", 0x07, Cycles = 4)]
        public Expression RLCA
        {
            get
            {
                return Expression.Block(
                        // Set Flags
                        Expression.Assign(_cpuValueExpressions.FlagsRegister,
                            // (F & 0xEC) | (A >> 7)
                            Expression.Or(
                                // Leave SZ5 3P unchanged, zero out HNC
                                Expression.And(_cpuValueExpressions.FlagsRegister, Expression.Constant(0xEC)),

                                // Carry flag is determined by MSB of register before rotate.
                                Expression.RightShift(_cpuValueExpressions.RegisterA, Expression.Constant(0x07))
                            )
                        ),

                        // A = ((A << 1) | (F & 1)) & 0xFF
                        Expression.Assign(_cpuValueExpressions.RegisterA,
                            Expression.And(
                                Expression.Or(
                                    Expression.LeftShift(_cpuValueExpressions.RegisterA, Expression.Constant(0x01)),
                                    Expression.And(_cpuValueExpressions.FlagsRegister, Expression.Constant(0x01))
                                ),
                                Expression.Constant(0xFF)
                            )
                        )
                    );
            }
        }

        [Instruction("RRCA", 0x0F, Cycles = 4)]
        public Expression RRCA
        {
            get
            {
                return Expression.Block(

                    // Set Flags
                    Expression.Assign(_cpuValueExpressions.FlagsRegister,
                    // (F & 0xEC) | (A & 0x29)
                        Expression.Or(
                    // Leave SZ5 3P unchanged, zero out HNC
                            Expression.And(_cpuValueExpressions.FlagsRegister, Expression.Constant(0xEC)),

                            // Carry 5 and 3 can be determined by the values at 5,3, and 0 before rotate.
                            Expression.And(_cpuValueExpressions.RegisterA, Expression.Constant(0x29))
                        )
                    ),

                    // A = ((A >> 1) | ((F & 1) << 7) & 0xFF
                    Expression.Assign(_cpuValueExpressions.RegisterA,
                        Expression.And(
                            Expression.Or(
                                Expression.RightShift(_cpuValueExpressions.RegisterA, Expression.Constant(0x01)),
                                Expression.LeftShift(
                                    Expression.And(_cpuValueExpressions.FlagsRegister, Expression.Constant(0x01)),
                                    Expression.Constant(0x07)
                                )
                            ),
                            Expression.Constant(0xFF)
                        )
                    )
                );
            }
        }

        [Instruction("RLA", 0x17, Cycles = 4)]
        public Expression RLA
        {
            get
            {
                return Expression.Block(

                    Expression.Assign(_tempExpressions.Temp1, _cpuValueExpressions.RegisterA),

                    // A = ((A << 1) | (F & 1)) & 0xFF
                    Expression.Assign(_cpuValueExpressions.RegisterA,
                        Expression.And(
                            Expression.Or(
                                Expression.LeftShift(_cpuValueExpressions.RegisterA, Expression.Constant(0x01)),
                                Expression.And(_cpuValueExpressions.FlagsRegister, Expression.Constant(0x01))
                            ),
                            Expression.Constant(0xFF)
                        )
                    ),

                    // Set Flags
                    Expression.Assign(_cpuValueExpressions.FlagsRegister,
                    // (F & 0xEC) | (A >> 7)
                        Expression.Or(
                    // Leave SZ5 3P unchanged, zero out HNC
                            Expression.And(_cpuValueExpressions.FlagsRegister, Expression.Constant(0xEC)),

                            // Carry flag is determined by MSB of register before rotate.
                            Expression.RightShift(_tempExpressions.Temp1, Expression.Constant(0x07))
                        )
                    )
                );
            }
        }

        [Instruction("RRA", 0x1F, Cycles = 4)]
        public Expression RRA
        {
            get
            {
                return Expression.Block(

                    Expression.Assign(_tempExpressions.Temp1, _cpuValueExpressions.RegisterA),

                    // A = ((A >> 1) | ((F & 1) << 7) & 0xFF
                    Expression.Assign(_cpuValueExpressions.RegisterA,
                        Expression.And(
                            Expression.Or(
                                Expression.RightShift(_cpuValueExpressions.RegisterA, Expression.Constant(0x01)),
                                Expression.LeftShift(
                                    Expression.And(_cpuValueExpressions.FlagsRegister, Expression.Constant(0x01)),
                                    Expression.Constant(0x07)
                                )
                            ),
                            Expression.Constant(0xFF)
                        )
                    ),

                    // Set Flags
                    Expression.Assign(_cpuValueExpressions.FlagsRegister,
                    // (F & 0xEC) | (A & 1)
                        Expression.Or(
                        // Leave SZ5 3P unchanged, zero out HNC
                            Expression.And(_cpuValueExpressions.FlagsRegister, Expression.Constant(0xEC)),

                            // Carry as bit 0 of A prior to rotate.
                            Expression.And(_tempExpressions.Temp1, Expression.Constant(Flags.CarryFlag))
                        )
                    )
                );
            }
        }

        [Instruction("RLD", 0x6F, Prefix = "ED", Cycles = 18)]
        public Expression RLD
        {
            get
            {
                return Expression.Block(
                    Expression.Assign(_tempExpressions.Temp1, _cpuValueExpressions.ReadHL),

                    _cpuValueExpressions.WriteByte(_cpuValueExpressions.RegisterH, _cpuValueExpressions.RegisterL,
                        Expression.Or(
                            Expression.And(_cpuValueExpressions.RegisterA, Expression.Constant(0x0F)),
                            Expression.LeftShift(Expression.And(_tempExpressions.Temp1, Expression.Constant(0x0F)), Expression.Constant(0x08))
                        )
                    ),

                    Expression.Assign(_cpuValueExpressions.RegisterA, 
                        Expression.Or(
                             Expression.And(_cpuValueExpressions.RegisterA, Expression.Constant(0xF0)),
                             Expression.RightShift(_tempExpressions.Temp1, Expression.Constant(0x08))
                        )
                    ),

                    Expression.Assign(_cpuValueExpressions.FlagsRegister, 
                        Expression.Or(
                            Expression.And(_cpuValueExpressions.FlagsRegister, Expression.Constant(0x01)),
                            Expression.ArrayIndex(_flagExpressions.SignZeroParityCalcultorFlags, _cpuValueExpressions.RegisterA)
                        )
                    )
                );
            }
        }

        [Instruction("RRD", 0x67, Prefix = "ED", Cycles = 18)]
        public Expression RRD
        {
            get
            {
                return Expression.Block(
                    Expression.Assign(_tempExpressions.Temp1, _cpuValueExpressions.ReadHL),

                    _cpuValueExpressions.WriteByte(_cpuValueExpressions.RegisterH, _cpuValueExpressions.RegisterL,
                        Expression.Or(
                            Expression.LeftShift(Expression.And(_cpuValueExpressions.RegisterA, Expression.Constant(0x0F)), Expression.Constant(0x08)),
                            Expression.RightShift(_tempExpressions.Temp1, Expression.Constant(0x08))
                        )
                    ),

                    Expression.Assign(_cpuValueExpressions.RegisterA,
                        Expression.Or(
                             Expression.And(_cpuValueExpressions.RegisterA, Expression.Constant(0xF0)),
                             Expression.And(_tempExpressions.Temp1, Expression.Constant(0x0F))
                        )
                    ),

                    Expression.Assign(_cpuValueExpressions.FlagsRegister,
                        Expression.Or(
                            Expression.And(_cpuValueExpressions.FlagsRegister, Expression.Constant(0x01)),
                            Expression.ArrayIndex(_flagExpressions.SignZeroParityCalcultorFlags, _cpuValueExpressions.RegisterA)
                        )
                    )
                );
            }
        }
    }
}
