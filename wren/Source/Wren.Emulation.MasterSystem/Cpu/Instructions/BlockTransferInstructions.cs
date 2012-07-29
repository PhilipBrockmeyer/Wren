using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wren.Emulation.MasterSystem.ExpressionLibraries;
using System.Linq.Expressions;

namespace Wren.Emulation.MasterSystem.Instructions
{
    public class BlockTransferInstructions
    {
        IDataAccessExpressionLibrary _cpuValueExpressions;
        IFlagLookupValuesExpressionLibrary _flagExpressions;
        ITemporaryExpressionLibrary _tempExpressions;
        IProgramControlExpressionLibrary _programControlExpressions;

        public BlockTransferInstructions(IDataAccessExpressionLibrary registerExpressions,
                                 IFlagLookupValuesExpressionLibrary flagExpressions,
                                 ITemporaryExpressionLibrary temporaryExpressions,
                                 IProgramControlExpressionLibrary programControlExpressions)
        {
            _cpuValueExpressions = registerExpressions;
            _flagExpressions = flagExpressions;
            _tempExpressions = temporaryExpressions;
            _programControlExpressions = programControlExpressions;
        }

        [Instruction("LDI", 0xA0, Cycles = 21, Prefix = "ED")]
        public Expression Ldi
        {
            get
            {
                return Expression.Block(
                    // (DE) = (HL)
                    _cpuValueExpressions.WriteByteDE(_cpuValueExpressions.ReadHL),
                                       
                    // DE++
                    Expression.Assign(_tempExpressions.Temp1, Expression.Or(Expression.LeftShift(_cpuValueExpressions.RegisterD, Expression.Constant(0x08)), _cpuValueExpressions.RegisterE)),
                    Expression.PostIncrementAssign(_tempExpressions.Temp1),
                    Expression.AndAssign(_tempExpressions.Temp1, Expression.Constant(0xFFFF)),
                    Expression.Assign(_cpuValueExpressions.RegisterD, Expression.RightShift(_tempExpressions.Temp1, Expression.Constant(0x08))),
                    Expression.Assign(_cpuValueExpressions.RegisterE, Expression.And(_tempExpressions.Temp1, Expression.Constant(0xFF))),

                    // HL++
                    Expression.Assign(_tempExpressions.Temp1, Expression.Or(Expression.LeftShift(_cpuValueExpressions.RegisterH, Expression.Constant(0x08)), _cpuValueExpressions.RegisterL)),
                    Expression.PostIncrementAssign(_tempExpressions.Temp1),
                    Expression.AndAssign(_tempExpressions.Temp1, Expression.Constant(0xFFFF)),
                    Expression.Assign(_cpuValueExpressions.RegisterH, Expression.RightShift(_tempExpressions.Temp1, Expression.Constant(0x08))),
                    Expression.Assign(_cpuValueExpressions.RegisterL, Expression.And(_tempExpressions.Temp1, Expression.Constant(0xFF))),
                    
                    // BC--
                    Expression.Assign(_tempExpressions.Temp1, Expression.Or(Expression.LeftShift(_cpuValueExpressions.RegisterB, Expression.Constant(0x08)), _cpuValueExpressions.RegisterC)),
                    Expression.PostDecrementAssign(_tempExpressions.Temp1),
                    Expression.AndAssign(_tempExpressions.Temp1, Expression.Constant(0xFFFF)),
                    Expression.Assign(_cpuValueExpressions.RegisterB, Expression.RightShift(_tempExpressions.Temp1, Expression.Constant(0x08))),
                    Expression.Assign(_cpuValueExpressions.RegisterC, Expression.And(_tempExpressions.Temp1, Expression.Constant(0xFF))),

                    // CZS unchanged, HN = 0, P = BC!=0
                    Expression.IfThenElse(Expression.NotEqual(_tempExpressions.Temp1, Expression.Constant(0)),
                        Expression.Assign(_cpuValueExpressions.FlagsRegister,
                            Expression.Or(
                                Expression.And(_cpuValueExpressions.FlagsRegister, Expression.Constant(0xE9)),
                                Expression.Constant(Flags.ParityFlag)
                            )
                        ),

                        Expression.Assign(_cpuValueExpressions.FlagsRegister,
                            Expression.And(_cpuValueExpressions.FlagsRegister, Expression.Constant(0xE9))                            
                        )
                    )
                    
                );
            }
        }

        [Instruction("LDD", 0xA8, Cycles = 21, Prefix = "ED")]
        public Expression Ldd
        {
            get
            {
                return Expression.Block(
                    // (DE) = (HL)
                    _cpuValueExpressions.WriteByteDE(_cpuValueExpressions.ReadHL),

                    // DE--
                    Expression.Assign(_tempExpressions.Temp1, Expression.Or(Expression.LeftShift(_cpuValueExpressions.RegisterD, Expression.Constant(0x08)), _cpuValueExpressions.RegisterE)),
                    Expression.PostDecrementAssign(_tempExpressions.Temp1),
                    Expression.AndAssign(_tempExpressions.Temp1, Expression.Constant(0xFFFF)),
                    Expression.Assign(_cpuValueExpressions.RegisterD, Expression.RightShift(_tempExpressions.Temp1, Expression.Constant(0x08))),
                    Expression.Assign(_cpuValueExpressions.RegisterE, Expression.And(_tempExpressions.Temp1, Expression.Constant(0xFF))),

                    // HL--
                    Expression.Assign(_tempExpressions.Temp1, Expression.Or(Expression.LeftShift(_cpuValueExpressions.RegisterH, Expression.Constant(0x08)), _cpuValueExpressions.RegisterL)),
                    Expression.PostDecrementAssign(_tempExpressions.Temp1),
                    Expression.AndAssign(_tempExpressions.Temp1, Expression.Constant(0xFFFF)),
                    Expression.Assign(_cpuValueExpressions.RegisterH, Expression.RightShift(_tempExpressions.Temp1, Expression.Constant(0x08))),
                    Expression.Assign(_cpuValueExpressions.RegisterL, Expression.And(_tempExpressions.Temp1, Expression.Constant(0xFF))),

                     // BC--
                    Expression.Assign(_tempExpressions.Temp1, Expression.Or(Expression.LeftShift(_cpuValueExpressions.RegisterB, Expression.Constant(0x08)), _cpuValueExpressions.RegisterC)),
                    Expression.PostDecrementAssign(_tempExpressions.Temp1),
                    Expression.AndAssign(_tempExpressions.Temp1, Expression.Constant(0xFFFF)),
                    Expression.Assign(_cpuValueExpressions.RegisterB, Expression.RightShift(_tempExpressions.Temp1, Expression.Constant(0x08))),
                    Expression.Assign(_cpuValueExpressions.RegisterC, Expression.And(_tempExpressions.Temp1, Expression.Constant(0xFF))),
                                        
                    // CZS unchanged, HN = 0, P = BC!=0
                    Expression.IfThenElse(Expression.NotEqual(_tempExpressions.Temp1, Expression.Constant(0)),
                        Expression.Assign(_cpuValueExpressions.FlagsRegister,
                            Expression.Or(
                                Expression.And(_cpuValueExpressions.FlagsRegister, Expression.Constant(0xE9)),
                                Expression.Constant(Flags.ParityFlag)
                            )
                        ),

                        Expression.Assign(_cpuValueExpressions.FlagsRegister,
                            Expression.And(_cpuValueExpressions.FlagsRegister, Expression.Constant(0xE9))
                        )
                    )
                );
            }
        }

        LabelTarget _endCopyLdir = Expression.Label("EndCopy");
        [Instruction("LDIR", 0xB0, Prefix = "ED")]
        public Expression Ldir
        {
            get
            {
                return Expression.Block(

                    // Temp1 = BC
                    Expression.Assign(_tempExpressions.Temp1, Expression.Or(Expression.LeftShift(_cpuValueExpressions.RegisterB, Expression.Constant(0x08)), _cpuValueExpressions.RegisterC)),
                    
                    // Cycles -= 5 + 16 * BC
                    Expression.SubtractAssign(_programControlExpressions.CycleCounter,
                            Expression.Multiply(Expression.Constant(16), _tempExpressions.Temp1)
                    ),

                    Expression.SubtractAssign(_programControlExpressions.CycleCounter, Expression.Constant(5)),
                    
                    // Temp2 = DE
                    Expression.Assign(_tempExpressions.Temp2, Expression.Or(Expression.LeftShift(_cpuValueExpressions.RegisterD, Expression.Constant(0x08)), _cpuValueExpressions.RegisterE)),
                    
                    // Temp3 = HL
                    Expression.Assign(_tempExpressions.Temp3, Expression.Or(Expression.LeftShift(_cpuValueExpressions.RegisterH, Expression.Constant(0x08)), _cpuValueExpressions.RegisterL)),

                    // CZS unchanged, HNP = 0
                    Expression.Assign(_cpuValueExpressions.FlagsRegister,
                        Expression.And(_cpuValueExpressions.FlagsRegister, Expression.Constant(0xE9))
                    ),
                    
                    Expression.Loop(
                        Expression.Block(
                            Expression.IfThen(Expression.Equal(_tempExpressions.Temp1, Expression.Constant(0)),
                                Expression.Goto(_endCopyLdir)
                            ),

                            // (DE) = (HL)
                            _cpuValueExpressions.ReadByte(_tempExpressions.Temp3, _tempExpressions.Temp4),
                            _cpuValueExpressions.WriteByte(_tempExpressions.Temp2, _tempExpressions.Temp4),

                            Expression.PreDecrementAssign(_tempExpressions.Temp1),
                            Expression.PostIncrementAssign(_tempExpressions.Temp2),
                            Expression.PostIncrementAssign(_tempExpressions.Temp3)
                        )
                    ),

                    Expression.Label(_endCopyLdir),
                    
                    // DE = Temp2
                    Expression.Assign(_cpuValueExpressions.RegisterD, Expression.RightShift(_tempExpressions.Temp2, Expression.Constant(0x08))),
                    Expression.Assign(_cpuValueExpressions.RegisterE, Expression.And(_tempExpressions.Temp2, Expression.Constant(0xFF))),

                    // HL = Temp3
                    Expression.Assign(_cpuValueExpressions.RegisterH, Expression.RightShift(_tempExpressions.Temp3, Expression.Constant(0x08))),
                    Expression.Assign(_cpuValueExpressions.RegisterL, Expression.And(_tempExpressions.Temp3, Expression.Constant(0xFF))),

                    // BC = 0
                    Expression.Assign(_cpuValueExpressions.RegisterB, Expression.Constant(0)),
                    Expression.Assign(_cpuValueExpressions.RegisterC, Expression.Constant(0)),

                    Expression.SubtractAssign(_programControlExpressions.CycleCounter, Expression.Constant(0))
                );
            }
        }

        LabelTarget _endCopyLddr = Expression.Label("EndCopy");
        [Instruction("LDDR", 0xB8, Prefix = "ED")]
        public Expression Lddr
        {
            get
            {
                return Expression.Block(

                    // Temp1 = BC
                    Expression.Assign(_tempExpressions.Temp1, Expression.Or(Expression.LeftShift(_cpuValueExpressions.RegisterB, Expression.Constant(0x08)), _cpuValueExpressions.RegisterC)),

                    // Cycles -= 5 + 16 * BC
                    Expression.SubtractAssign(_programControlExpressions.CycleCounter,
                        Expression.Add(Expression.Constant(5),
                            Expression.Multiply(Expression.Constant(16), _tempExpressions.Temp1)
                        )
                    ),

                    // Temp2 = DE
                    Expression.Assign(_tempExpressions.Temp2, Expression.Or(Expression.LeftShift(_cpuValueExpressions.RegisterD, Expression.Constant(0x08)), _cpuValueExpressions.RegisterE)),

                    // Temp3 = HL
                    Expression.Assign(_tempExpressions.Temp3, Expression.Or(Expression.LeftShift(_cpuValueExpressions.RegisterH, Expression.Constant(0x08)), _cpuValueExpressions.RegisterL)),

                    // CZS unchanged, HNP = 0
                    Expression.Assign(_cpuValueExpressions.FlagsRegister,
                        Expression.And(_cpuValueExpressions.FlagsRegister, Expression.Constant(0xE9))
                    ),

                    Expression.Loop(
                        Expression.Block(
                            Expression.IfThen(Expression.Equal(_tempExpressions.Temp1, Expression.Constant(0)),
                                Expression.Goto(_endCopyLddr)
                            ),

                             // (DE) = (HL)
                            _cpuValueExpressions.ReadByte(_tempExpressions.Temp3, _tempExpressions.Temp4),
                            _cpuValueExpressions.WriteByte(_tempExpressions.Temp2, _tempExpressions.Temp4),
                            Expression.PostDecrementAssign(_tempExpressions.Temp1),
                            Expression.PostDecrementAssign(_tempExpressions.Temp2),
                            Expression.PostDecrementAssign(_tempExpressions.Temp3)
                        )
                    ),

                    Expression.Label(_endCopyLddr),

                    // DE = Temp2
                    Expression.Assign(_cpuValueExpressions.RegisterD, Expression.RightShift(_tempExpressions.Temp2, Expression.Constant(0x08))),
                    Expression.Assign(_cpuValueExpressions.RegisterE, Expression.And(_tempExpressions.Temp2, Expression.Constant(0xFF))),

                    // HL = Temp3
                    Expression.Assign(_cpuValueExpressions.RegisterH, Expression.RightShift(_tempExpressions.Temp3, Expression.Constant(0x08))),
                    Expression.Assign(_cpuValueExpressions.RegisterL, Expression.And(_tempExpressions.Temp3, Expression.Constant(0xFF))),

                    // BC = 0
                    Expression.Assign(_cpuValueExpressions.RegisterB, Expression.Constant(0)),
                    Expression.Assign(_cpuValueExpressions.RegisterC, Expression.Constant(0)),

                    Expression.SubtractAssign(_programControlExpressions.CycleCounter, Expression.Constant(0))
                );
            }
        }
    }
}
