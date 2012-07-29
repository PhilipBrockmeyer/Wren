using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wren.Emulation.MasterSystem.ExpressionLibraries;
using System.Linq.Expressions;

namespace Wren.Emulation.MasterSystem.Instructions
{
    public class MiscInstructions
    {
        IDataAccessExpressionLibrary _cpuValueExpressions;
        IFlagLookupValuesExpressionLibrary _flagExpressions;
        ITemporaryExpressionLibrary _tempExpressions;
        IProgramControlExpressionLibrary _programControlExpressions;

        public MiscInstructions(IDataAccessExpressionLibrary registerExpressions,
                              IFlagLookupValuesExpressionLibrary flagExpressions,
                              ITemporaryExpressionLibrary temporaryExpressions,
                              IProgramControlExpressionLibrary programControlExpressions)
        {
            _cpuValueExpressions = registerExpressions;
            _flagExpressions = flagExpressions;
            _tempExpressions = temporaryExpressions;
            _programControlExpressions = programControlExpressions;
        }

        [Instruction("CPL", 0x2F, Cycles = 4)]
        public Expression Cpl
        {
            get 
            {
                return Expression.Block(
                    Expression.ExclusiveOrAssign(_cpuValueExpressions.RegisterA, Expression.Constant(0xFF)),
                    // F = (F & (0xc5)) | 0x12 | (A & (0x28));

                    Expression.Assign(_cpuValueExpressions.FlagsRegister,
                        Expression.Or(Expression.Constant(0x12),
                            Expression.Or(
                                Expression.And(_cpuValueExpressions.FlagsRegister, Expression.Constant(0xC5)),
                                Expression.And(_cpuValueExpressions.RegisterA, Expression.Constant(0x28))
                            )
                        )
                    )
                );
            }
        }

        [Instruction("SCF", 0x37, Cycles = 4)]
        public Expression Scf
        {
            get
            {
                return Expression.Block(
                    Expression.Assign(_cpuValueExpressions.FlagsRegister,
                        Expression.Or(Expression.Constant(0x01),
                            Expression.Or(
                                Expression.And(_cpuValueExpressions.FlagsRegister, Expression.Constant(0xC5)),
                                Expression.And(_cpuValueExpressions.RegisterA, Expression.Constant(0x28))
                            )
                        )
                    )
                );
            }
        }

        [Instruction("CCF", 0x3F, Cycles = 4)]
        public Expression Ccf
        {
            get
            {
                return Expression.Block(
                    Expression.Assign(_cpuValueExpressions.FlagsRegister,
                        Expression.ExclusiveOr(Expression.Constant(0x01),
                            Expression.Or(
                                Expression.And(_cpuValueExpressions.FlagsRegister, Expression.Constant(0xC5)),
                                Expression.And(_cpuValueExpressions.RegisterA, Expression.Constant(0x28))
                            )
                        )
                    )
                );
            }
        }
    }
}
