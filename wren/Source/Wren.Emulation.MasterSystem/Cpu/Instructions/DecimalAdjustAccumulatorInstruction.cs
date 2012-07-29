using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wren.Emulation.MasterSystem.ExpressionLibraries;
using System.Linq.Expressions;

namespace Wren.Emulation.MasterSystem.Instructions
{
    public class DecimalAdjustAccumulatorInstruction
    {
        IDataAccessExpressionLibrary _cpuValueExpressions;
        IFlagLookupValuesExpressionLibrary _flagExpressions;
        ITemporaryExpressionLibrary _tempExpressions;
        IProgramControlExpressionLibrary _programControlExpressions;

        public DecimalAdjustAccumulatorInstruction(IDataAccessExpressionLibrary registerExpressions,
                              IFlagLookupValuesExpressionLibrary flagExpressions,
                              ITemporaryExpressionLibrary temporaryExpressions,
                              IProgramControlExpressionLibrary programControlExpressions)
        {
            _cpuValueExpressions = registerExpressions;
            _flagExpressions = flagExpressions;
            _tempExpressions = temporaryExpressions;
            _programControlExpressions = programControlExpressions;
        }

        /****************************************************************************
         *                                      Results Table                       *
         *    N   C   Value of     H  Value of     Hex no   C flag after            *
         *           high nibble     low nibble     added     execution             *                                        
         *    0   0      0-9       0     0-9         00           0                 *.
         *    0   0      A-F       0     0-9         60           1                 *.
         *    0   0      0-8       0     A-F         06           0                 *.
         *    0   0      9-F       0     A-F         66           1                 *.
         *    0   0      0-9       1     0-3         06           0                 *.
         *    0   0      A-F       1     0-3         66           1                 *.
         *    0   1      0-2       0     0-9         60           1                 *.
         *    0   1      0-2       0     A-F         66           1                 *.
         *    0   1      0-3       1     0-3         66           1                 *.
         *    1   0      0-9       0     0-9         00           0                 *.
         *    1   0      0-8       1     6-F         FA           0                 *
         *    1   1      7-F       0     0-9         A0           1                 *
         *    1   1      6-F       1     6-F         9A           1                 *
         ****************************************************************************/
        [Instruction("DAA", 0x27, Cycles = 4)]
        public Expression Daa
        {
            get
            {
                return Expression.Block(
                    Expression.Assign(_tempExpressions.Temp1, Expression.Constant(0x00)),

                    Expression.IfThenElse(
                        Expression.Equal(Expression.And(_cpuValueExpressions.FlagsRegister, Expression.Constant(Flags.SubtractionFlag)), Expression.Constant(0x00)),

                        // N not set
                        Expression.IfThenElse(
                            Expression.Equal(Expression.And(_cpuValueExpressions.FlagsRegister, Expression.Constant(Flags.CarryFlag)), Expression.Constant(0x00)),

                            // C not set
                            Expression.IfThenElse(
                                Expression.Equal(Expression.And(_cpuValueExpressions.FlagsRegister, Expression.Constant(Flags.HalfCarry)), Expression.Constant(0x00)),

                                // H not set
                                Expression.IfThenElse(
                                    Expression.LessThanOrEqual(Expression.And(_cpuValueExpressions.RegisterA, Expression.Constant(0x0F)), Expression.Constant(0x09)),
                                
                                    Expression.IfThenElse(
                                        Expression.LessThanOrEqual(_cpuValueExpressions.RegisterA, Expression.Constant(0x90)),

                                        // N 0 - C 0 - H 0 - LN 0..9 - HN 0..9
                                        Expression.Assign(_cpuValueExpressions.RegisterA, _cpuValueExpressions.RegisterA),

                                        // N 0 - C 0 - H 0 - LN 0..9 - HN A..F
                                        Expression.Block(
                                            Expression.Assign(_cpuValueExpressions.RegisterA,
                                                Expression.And(
                                                    Expression.Add(_cpuValueExpressions.RegisterA, Expression.Constant(0x60)),
                                                    Expression.Constant(0xFF)
                                                )
                                            ),
                                            Expression.Assign(_tempExpressions.Temp1, Expression.Constant(0x01))
                                        )
                                    ),

                                    Expression.IfThenElse(
                                        Expression.LessThanOrEqual(_cpuValueExpressions.RegisterA, Expression.Constant(0x80)),
                                    
                                        // N 0 - C 0 - H 0 - LN A..F - HN 0..8
                                        Expression.AddAssign(_cpuValueExpressions.RegisterA, Expression.Constant(0x06)),

                                        // N 0 - C 0 - H 0 - LN A..F - HN 9..F
                                        Expression.Block(
                                            Expression.Assign(_cpuValueExpressions.RegisterA,
                                                Expression.And(
                                                    Expression.Add(_cpuValueExpressions.RegisterA, Expression.Constant(0x66)),
                                                    Expression.Constant(0xFF)
                                                )
                                            ),
                                            Expression.Assign(_tempExpressions.Temp1, Expression.Constant(0x01))
                                        )
                                    )
                                ),

                                // H set
                                Expression.IfThenElse(
                                    Expression.LessThanOrEqual(_cpuValueExpressions.RegisterA, Expression.Constant(0x90)),

                                    // N 0 - C 0 - H 1 - LN 0..3 - HN 0..9   
                                    Expression.Block(
                                        Expression.Assign(_cpuValueExpressions.RegisterA,
                                            Expression.And(
                                                Expression.Add(_cpuValueExpressions.RegisterA, Expression.Constant(0x06)),
                                                Expression.Constant(0xFF)
                                            )
                                        ),
                                        Expression.ExclusiveOrAssign(_cpuValueExpressions.FlagsRegister, Expression.Constant(Flags.HalfCarry))
                                    ),

                                    // N 0 - C 0 - H 1 - LN 0..3 - HN A..F
                                    Expression.Block(
                                        Expression.Assign(_cpuValueExpressions.RegisterA,
                                            Expression.And(
                                                Expression.Add(_cpuValueExpressions.RegisterA, Expression.Constant(0x66)),
                                                Expression.Constant(0xFF)
                                            )
                                        ),
                                        Expression.ExclusiveOrAssign(_cpuValueExpressions.FlagsRegister, Expression.Constant(Flags.HalfCarry)),
                                        Expression.Assign(_tempExpressions.Temp1, Expression.Constant(0x01))

                                    )
                                )
                            ),

                            // C set
                            Expression.IfThenElse(
                                Expression.Equal(Expression.And(_cpuValueExpressions.FlagsRegister, Expression.Constant(Flags.HalfCarry)), Expression.Constant(0x00)),

                                // H not set
                                Expression.IfThenElse(
                                    Expression.LessThanOrEqual(Expression.And(_cpuValueExpressions.RegisterA, Expression.Constant(0x0F)), Expression.Constant(0x09)),

                                     // N 0 - C 1 - H 0 - LN 0..9 - HN 0..2
                                    Expression.Block(
                                        Expression.Assign(_cpuValueExpressions.RegisterA,
                                            Expression.And(
                                                Expression.Add(_cpuValueExpressions.RegisterA, Expression.Constant(0x60)),
                                                Expression.Constant(0xFF)
                                            )
                                        ),
                                        Expression.Assign(_tempExpressions.Temp1, Expression.Constant(0x01))
                                    ),

                                     // N 0 - C 1 - H 0 - LN A..F - HN 0..2
                                    Expression.Block(
                                        Expression.Assign(_cpuValueExpressions.RegisterA,
                                            Expression.And(
                                                Expression.Add(_cpuValueExpressions.RegisterA, Expression.Constant(0x66)),
                                                Expression.Constant(0xFF)
                                            )
                                        ),
                                        Expression.Assign(_tempExpressions.Temp1, Expression.Constant(0x01))
                                    )
                                ),

                                //H Set
                                // N 0 - C 1 - H 1 - LN 0..3 - HN 0..3
                                Expression.Block(
                                    Expression.Assign(_cpuValueExpressions.RegisterA,
                                        Expression.And(
                                            Expression.Add(_cpuValueExpressions.RegisterA, Expression.Constant(0x66)),
                                            Expression.Constant(0xFF)
                                        )
                                    ),
                                    Expression.ExclusiveOrAssign(_cpuValueExpressions.FlagsRegister, Expression.Constant(Flags.HalfCarry)),
                                    Expression.Assign(_tempExpressions.Temp1, Expression.Constant(0x01))

                                )
                            )     
                        ),

                        // N Set
                         Expression.IfThenElse(
                            Expression.Equal(Expression.And(_cpuValueExpressions.FlagsRegister, Expression.Constant(Flags.CarryFlag)), Expression.Constant(0x00)),

                            // C not set
                            Expression.IfThenElse(
                                Expression.Equal(Expression.And(_cpuValueExpressions.FlagsRegister, Expression.Constant(Flags.HalfCarry)), Expression.Constant(0x00)),

                                // H not set
                                Expression.Assign(_cpuValueExpressions.RegisterA, _cpuValueExpressions.RegisterA),

                                // H set
                                // N 1 - C 0 - H 1
                                Expression.Block(
                                    Expression.Assign(_cpuValueExpressions.RegisterA,
                                        Expression.And(
                                            Expression.Add(_cpuValueExpressions.RegisterA, Expression.Constant(0xFA)),
                                            Expression.Constant(0xFF)
                                        )
                                    ),
                                    Expression.ExclusiveOrAssign(_cpuValueExpressions.FlagsRegister, Expression.Constant(Flags.HalfCarry))
                                )
                            ),

                            // C set
                            Expression.IfThenElse(
                                Expression.Equal(Expression.And(_cpuValueExpressions.FlagsRegister, Expression.Constant(Flags.HalfCarry)), Expression.Constant(0x00)),

                                // N 1 - C 1 - H 0
                                Expression.Block(
                                    Expression.Assign(_cpuValueExpressions.RegisterA,
                                        Expression.And(
                                            Expression.Add(_cpuValueExpressions.RegisterA, Expression.Constant(0xA0)),
                                            Expression.Constant(0xFF)
                                        )
                                    ),
                                    Expression.Assign(_tempExpressions.Temp1, Expression.Constant(0x01))
                                ),

                                // H set
                                // N 1 - C 1 - H 1
                                Expression.Block(
                                    Expression.Assign(_cpuValueExpressions.RegisterA,
                                        Expression.And(
                                            Expression.Add(_cpuValueExpressions.RegisterA, Expression.Constant(0x9A)),
                                            Expression.Constant(0xFF)
                                        )
                                    ),
                                    Expression.ExclusiveOrAssign(_cpuValueExpressions.FlagsRegister, Expression.Constant(Flags.HalfCarry)),
                                    Expression.Assign(_tempExpressions.Temp1, Expression.Constant(0x01))
                                )
                            )
                        )
                    ),
                    
                    // Set the flags.
                    Expression.Assign(_cpuValueExpressions.FlagsRegister, 
                        Expression.Or(
                            Expression.ArrayIndex(_flagExpressions.SignZeroParityCalcultorFlags, _cpuValueExpressions.RegisterA),
                            _tempExpressions.Temp1
                        )
                    )
                );
            }
        }
    }
}
