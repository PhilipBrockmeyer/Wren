using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wren.Emulation.MasterSystem.ExpressionLibraries;
using System.Linq.Expressions;

namespace Wren.Emulation.MasterSystem.Instructions
{
    public class PushInstruction
    {
        IDataAccessExpressionLibrary _cpuValueExpressions;
        ITemporaryExpressionLibrary _tempExpressions;
        IProgramControlExpressionLibrary _programControlExpressions;

        public PushInstruction(IDataAccessExpressionLibrary registerExpressions,
                               ITemporaryExpressionLibrary temporaryExpressions,
                               IProgramControlExpressionLibrary programControlExpressions)
        {
            _cpuValueExpressions = registerExpressions;
            _tempExpressions = temporaryExpressions;
            _programControlExpressions = programControlExpressions;
        }

        [Instruction("PUSH BC", 0xC5, Cycles=11)]
        public Expression Push_BC
        {
            get
            {
                return _cpuValueExpressions.Push(
                            Expression.Or(
                                Expression.LeftShift(_cpuValueExpressions.RegisterB, Expression.Constant(0x08)),
                                _cpuValueExpressions.RegisterC
                            )
                        );
            }
        }

        [Instruction("PUSH DE", 0xD5, Cycles = 11)]
        public Expression Push_DE
        {
            get
            {
                return _cpuValueExpressions.Push(
                            Expression.Or(
                                Expression.LeftShift(_cpuValueExpressions.RegisterD, Expression.Constant(0x08)),
                                _cpuValueExpressions.RegisterE
                            )
                        );
            }
        }

        [Instruction("PUSH HL", 0xE5, Cycles = 11)]
        public Expression Push_HL
        {
            get
            {
                return _cpuValueExpressions.Push(
                            Expression.Or(
                                Expression.LeftShift(_cpuValueExpressions.RegisterH, Expression.Constant(0x08)),
                                _cpuValueExpressions.RegisterL
                            )
                        );
            }
        }

        [Instruction("PUSH AF", 0xF5, Cycles = 11)]
        public Expression Push_AF
        {
            get
            {
                return _cpuValueExpressions.Push(
                            Expression.Or(
                                Expression.LeftShift(_cpuValueExpressions.RegisterA, Expression.Constant(0x08)),
                                _cpuValueExpressions.FlagsRegister
                            )
                        );
            }
        }

        [Instruction("PUSH IX", 0xE5, Prefix = "DD", Cycles = 14)]
        public Expression Push_IX
        {
            get { return _cpuValueExpressions.Push(_cpuValueExpressions.RegisterIX); }
        }

        [Instruction("PUSH IY", 0xE5, Prefix = "FD", Cycles = 14)]
        public Expression Push_IY
        {
            get { return _cpuValueExpressions.Push(_cpuValueExpressions.RegisterIY); }
        }
    }
}
