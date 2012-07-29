using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wren.Emulation.MasterSystem.ExpressionLibraries;
using System.Linq.Expressions;

namespace Wren.Emulation.MasterSystem.Instructions
{
    public class ReturnInstruction
    {
        IDataAccessExpressionLibrary _cpuValueExpressions;
        ITemporaryExpressionLibrary _tempExpressions;
        IProgramControlExpressionLibrary _programControlExpressions;

        public ReturnInstruction(IDataAccessExpressionLibrary registerExpressions,
                                 ITemporaryExpressionLibrary temporaryExpressions,
                                 IProgramControlExpressionLibrary programControlExpressions)
        {
            _cpuValueExpressions = registerExpressions;
            _tempExpressions = temporaryExpressions;
            _programControlExpressions = programControlExpressions;
        }

        private Expression ConditionalReturn(Expression test, Int32 cyclesWhenReturned, Int32 cyclesWhenNotReturned)
        {
            return Expression.Block(
                Expression.IfThen(
                    test,
                    
                    Expression.Block(
                        _cpuValueExpressions.Pop(_programControlExpressions.ProgramCounterRegister),
                        Expression.SubtractAssign(_programControlExpressions.CycleCounter, Expression.Constant(cyclesWhenReturned - cyclesWhenNotReturned))
                    )
                ),

                Expression.SubtractAssign(_programControlExpressions.CycleCounter, Expression.Constant(cyclesWhenNotReturned))

            );
        }

        [Instruction("RET NZ", 0xC0)]
        public Expression Ret_NZ
        {
            get
            {
                return ConditionalReturn(
                        Expression.Equal(
                            Expression.And(_cpuValueExpressions.FlagsRegister, Expression.Constant(Flags.ZeroFlag)), Expression.Constant(0x00)
                        ),
                        11, 
                        5
                    );
            }
        }

        [Instruction("RET Z", 0xC8)]
        public Expression Ret_Z
        {
            get
            {
                return ConditionalReturn(
                        Expression.NotEqual(
                            Expression.And(_cpuValueExpressions.FlagsRegister, Expression.Constant(Flags.ZeroFlag)), Expression.Constant(0x00)
                        ),
                        11,
                        5
                    );
            }
        }

        [Instruction("RET", 0xC9, Cycles=10)]
        public Expression Ret
        {
            get { return _cpuValueExpressions.Pop(_programControlExpressions.ProgramCounterRegister); }
        }

        [Instruction("RET NC", 0xD0)]
        public Expression Ret_NC
        {
            get
            {
                return ConditionalReturn(
                        Expression.Equal(
                            Expression.And(_cpuValueExpressions.FlagsRegister, Expression.Constant(Flags.CarryFlag)), Expression.Constant(0x00)
                        ),
                        11,
                        5
                    );
            }
        }

        [Instruction("RET C", 0xD8)]
        public Expression Ret_C
        {
            get
            {
                return ConditionalReturn(
                        Expression.NotEqual(
                            Expression.And(_cpuValueExpressions.FlagsRegister, Expression.Constant(Flags.CarryFlag)), Expression.Constant(0x00)
                        ),
                        11,
                        5
                    );
            }
        }

        [Instruction("RET PO", 0xE0)]
        public Expression Ret_PO
        {
            get
            {
                return ConditionalReturn(
                        Expression.Equal(
                            Expression.And(_cpuValueExpressions.FlagsRegister, Expression.Constant(Flags.ParityFlag)), Expression.Constant(0x00)
                        ),
                        11,
                        5
                    );
            }
        }

        [Instruction("RET PE", 0xE8)]
        public Expression Ret_PE
        {
            get
            {
                return ConditionalReturn(
                        Expression.NotEqual(
                            Expression.And(_cpuValueExpressions.FlagsRegister, Expression.Constant(Flags.ParityFlag)), Expression.Constant(0x00)
                        ),
                        11,
                        5
                    );
            }
        }

        // Positive
        [Instruction("RET P", 0xF0)]
        public Expression Ret_P
        {
            get
            {
                return ConditionalReturn(
                        Expression.Equal(
                            Expression.And(_cpuValueExpressions.FlagsRegister, Expression.Constant(Flags.SignFlag)), Expression.Constant(0x00)
                        ),
                        11,
                        5
                    );
            }
        }

        // Minus
        [Instruction("RET M", 0xF8)]
        public Expression Ret_M
        {
            get
            {
                return ConditionalReturn(
                        Expression.NotEqual(
                            Expression.And(_cpuValueExpressions.FlagsRegister, Expression.Constant(Flags.SignFlag)), Expression.Constant(0x00)
                        ),
                        11,
                        5
                    );
            }
        }
    }
}
