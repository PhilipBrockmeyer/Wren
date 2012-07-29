using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wren.Emulation.MasterSystem.ExpressionLibraries;
using System.Linq.Expressions;

namespace Wren.Emulation.MasterSystem.Instructions
{
    public class ExchangeInstruction
    {
        IDataAccessExpressionLibrary _cpuValueExpressions;
        ITemporaryExpressionLibrary _tempExpressions;
        IPrimeRegistersExpressionLibrary _primeExpressions;

        public ExchangeInstruction(IDataAccessExpressionLibrary registerExpressions,
                                   ITemporaryExpressionLibrary temporaryExpressions,
                                   IPrimeRegistersExpressionLibrary primeExpressions)
        {
            _cpuValueExpressions = registerExpressions;
            _tempExpressions = temporaryExpressions;
            _primeExpressions = primeExpressions;
        }

        [Instruction("EX AF,AF'", 0x08, Cycles = 4)]
        public Expression EX_AF_AF
        {
            get 
            { 
                return Expression.Block(
                    Expression.Assign(_tempExpressions.Temp1, _cpuValueExpressions.RegisterA),
                    Expression.Assign(_cpuValueExpressions.RegisterA, _primeExpressions.RegisterAPrime),
                    Expression.Assign(_primeExpressions.RegisterAPrime, _tempExpressions.Temp1),

                    Expression.Assign(_tempExpressions.Temp1, _cpuValueExpressions.FlagsRegister),
                    Expression.Assign(_cpuValueExpressions.FlagsRegister, _primeExpressions.RegisterFPrime),
                    Expression.Assign(_primeExpressions.RegisterFPrime, _tempExpressions.Temp1)
                );
            }
        }

        [Instruction("EXX", 0xD9, Cycles = 4)]
        public Expression EX
        {
            get
            {
                return Expression.Block(
                    Expression.Assign(_tempExpressions.Temp1, _cpuValueExpressions.RegisterB),
                    Expression.Assign(_cpuValueExpressions.RegisterB, _primeExpressions.RegisterBPrime),
                    Expression.Assign(_primeExpressions.RegisterBPrime, _tempExpressions.Temp1),

                    Expression.Assign(_tempExpressions.Temp1, _cpuValueExpressions.RegisterC),
                    Expression.Assign(_cpuValueExpressions.RegisterC, _primeExpressions.RegisterCPrime),
                    Expression.Assign(_primeExpressions.RegisterCPrime, _tempExpressions.Temp1),

                    Expression.Assign(_tempExpressions.Temp1, _cpuValueExpressions.RegisterD),
                    Expression.Assign(_cpuValueExpressions.RegisterD, _primeExpressions.RegisterDPrime),
                    Expression.Assign(_primeExpressions.RegisterDPrime, _tempExpressions.Temp1),

                    Expression.Assign(_tempExpressions.Temp1, _cpuValueExpressions.RegisterE),
                    Expression.Assign(_cpuValueExpressions.RegisterE, _primeExpressions.RegisterEPrime),
                    Expression.Assign(_primeExpressions.RegisterEPrime, _tempExpressions.Temp1),

                    Expression.Assign(_tempExpressions.Temp1, _cpuValueExpressions.RegisterH),
                    Expression.Assign(_cpuValueExpressions.RegisterH, _primeExpressions.RegisterHPrime),
                    Expression.Assign(_primeExpressions.RegisterHPrime, _tempExpressions.Temp1),

                    Expression.Assign(_tempExpressions.Temp1, _cpuValueExpressions.RegisterL),
                    Expression.Assign(_cpuValueExpressions.RegisterL, _primeExpressions.RegisterLPrime),
                    Expression.Assign(_primeExpressions.RegisterLPrime, _tempExpressions.Temp1)
                );
            }
        }

        [Instruction("EX (SP),HL", 0xE3, Cycles = 19)]
        public Expression EX_SPi_HL
        {
            get
            {
                return Expression.Block(
                        Expression.Assign(_tempExpressions.Temp1,
                            Expression.Or(
                                Expression.LeftShift(_cpuValueExpressions.RegisterH, Expression.Constant(0x08)),
                                _cpuValueExpressions.RegisterL
                            )
                        ),

                        _cpuValueExpressions.Pop(_tempExpressions.Temp2),

                        Expression.Assign(_cpuValueExpressions.RegisterH, 
                            Expression.RightShift(_tempExpressions.Temp2, Expression.Constant(0x08))
                        ),

                        Expression.Assign(_cpuValueExpressions.RegisterL,
                            Expression.And(_tempExpressions.Temp2, Expression.Constant(0xFF))
                        ),

                        _cpuValueExpressions.Push(_tempExpressions.Temp1)
                    );
            }
        }

        [Instruction("EX DE,HL", 0xEB, Cycles = 4)]
        public Expression EX_DE_HL
        {
            get
            {
                return Expression.Block(
                    Expression.Assign(_tempExpressions.Temp1, _cpuValueExpressions.RegisterD),
                    Expression.Assign(_cpuValueExpressions.RegisterD, _cpuValueExpressions.RegisterH),
                    Expression.Assign(_cpuValueExpressions.RegisterH, _tempExpressions.Temp1),

                    Expression.Assign(_tempExpressions.Temp1, _cpuValueExpressions.RegisterE),
                    Expression.Assign(_cpuValueExpressions.RegisterE, _cpuValueExpressions.RegisterL),
                    Expression.Assign(_cpuValueExpressions.RegisterL, _tempExpressions.Temp1)
                );
            }
        }

        [Instruction("EX (SP),IX", 0xE3, Prefix="DD", Cycles = 23)]
        public Expression EX_SPi_IX
        {
            get
            {
                return Expression.Block(
                        Expression.Assign(_tempExpressions.Temp1, _cpuValueExpressions.RegisterIX),
                        _cpuValueExpressions.Pop(_cpuValueExpressions.RegisterIX),
                        _cpuValueExpressions.Push(_tempExpressions.Temp1)
                    );
            }
        }

        [Instruction("EX (SP),IY", 0xE3, Prefix = "FD", Cycles = 23)]
        public Expression EX_SPi_IY
        {
            get
            {
                return Expression.Block(
                        Expression.Assign(_tempExpressions.Temp1, _cpuValueExpressions.RegisterIX),
                        _cpuValueExpressions.Pop(_cpuValueExpressions.RegisterIX),
                        _cpuValueExpressions.Push(_tempExpressions.Temp1)
                    );
            }
        }

    }
}
