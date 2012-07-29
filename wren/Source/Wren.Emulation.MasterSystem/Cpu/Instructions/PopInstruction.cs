using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wren.Emulation.MasterSystem.ExpressionLibraries;
using System.Linq.Expressions;

namespace Wren.Emulation.MasterSystem.Instructions
{
    public class PopInstruction
    {
        IDataAccessExpressionLibrary _cpuValueExpressions;
        ITemporaryExpressionLibrary _tempExpressions;
        IProgramControlExpressionLibrary _programControlExpressions;

        public PopInstruction(IDataAccessExpressionLibrary registerExpressions,
                                 ITemporaryExpressionLibrary temporaryExpressions,
                                 IProgramControlExpressionLibrary programControlExpressions)
        {
            _cpuValueExpressions = registerExpressions;
            _tempExpressions = temporaryExpressions;
            _programControlExpressions = programControlExpressions;
        }

        [Instruction("POP BC", 0xC1, Cycles=10)]
        public Expression Pop_BC
        {
            get
            {
                return Expression.Block(
                        _cpuValueExpressions.Pop(_tempExpressions.Temp1),

                        Expression.Assign(_cpuValueExpressions.RegisterB, 
                            Expression.RightShift(_tempExpressions.Temp1, Expression.Constant(0x08))
                        ),

                        Expression.Assign(_cpuValueExpressions.RegisterC, 
                            Expression.And(_tempExpressions.Temp1, Expression.Constant(0xFF))
                        )
                    );
            }
        }

        [Instruction("POP DE", 0xD1, Cycles = 10)]
        public Expression Pop_DE
        {
            get
            {
                return Expression.Block(
                        _cpuValueExpressions.Pop(_tempExpressions.Temp1),

                        Expression.Assign(_cpuValueExpressions.RegisterD,
                            Expression.RightShift(_tempExpressions.Temp1, Expression.Constant(0x08))
                        ),

                        Expression.Assign(_cpuValueExpressions.RegisterE,
                            Expression.And(_tempExpressions.Temp1, Expression.Constant(0xFF))
                        )
                    );
            }
        }

        [Instruction("POP HL", 0xE1, Cycles = 10)]
        public Expression Pop_HL
        {
            get
            {
                return Expression.Block(
                        _cpuValueExpressions.Pop(_tempExpressions.Temp1),

                        Expression.Assign(_cpuValueExpressions.RegisterH,
                            Expression.RightShift(_tempExpressions.Temp1, Expression.Constant(0x08))
                        ),

                        Expression.Assign(_cpuValueExpressions.RegisterL,
                            Expression.And(_tempExpressions.Temp1, Expression.Constant(0xFF))
                        )
                    );
            }
        }

        [Instruction("POP AF", 0xF1, Cycles = 10)]
        public Expression Pop_AF
        {
            get
            {
                return Expression.Block(
                        _cpuValueExpressions.Pop(_tempExpressions.Temp1),

                        Expression.Assign(_cpuValueExpressions.RegisterA,
                            Expression.RightShift(_tempExpressions.Temp1, Expression.Constant(0x08))
                        ),

                        Expression.Assign(_cpuValueExpressions.FlagsRegister,
                            Expression.And(_tempExpressions.Temp1, Expression.Constant(0xFF))
                        )
                    );
            }
        }

        [Instruction("POP IX", 0xE1, Prefix="DD", Cycles=14)]
        public Expression Pop_IX
        {
            get { return _cpuValueExpressions.Pop(_cpuValueExpressions.RegisterIX); }
        }

        [Instruction("POP IY", 0xE1, Prefix = "FD", Cycles = 14)]
        public Expression Pop_IY
        {
            get { return _cpuValueExpressions.Pop(_cpuValueExpressions.RegisterIY); }
        }
    }
}
