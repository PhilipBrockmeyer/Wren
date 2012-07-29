using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wren.Emulation.MasterSystem.ExpressionLibraries;
using System.Linq.Expressions;

namespace Wren.Emulation.MasterSystem.Instructions
{
    public class InteruptInstructions
    {
        IDataAccessExpressionLibrary _cpuValueExpressions;
        IInteruptExpressionLibrary _interuptExpressions;
        ITemporaryExpressionLibrary _tempExpressions;
        IProgramControlExpressionLibrary _programControlExpressions;
        IFlagLookupValuesExpressionLibrary _flagLookupExpressions;

        public InteruptInstructions(IDataAccessExpressionLibrary registerExpressions,
                              IInteruptExpressionLibrary interuptExpressions,
                              ITemporaryExpressionLibrary temporaryExpressions,
                              IProgramControlExpressionLibrary programControlExpressions,
                              IFlagLookupValuesExpressionLibrary flagLookupExpressions)
        {
            _cpuValueExpressions = registerExpressions;
            _interuptExpressions = interuptExpressions;
            _tempExpressions = temporaryExpressions;
            _programControlExpressions = programControlExpressions;
            _flagLookupExpressions = flagLookupExpressions;
        }

        [Instruction("HALT", 0x76, Cycles=4)]
        public Expression Halt
        {
            get
            {
                return Expression.Block(
                    Expression.Assign(_programControlExpressions.CycleCounter, Expression.Constant(0x04)),
                    Expression.Assign(_interuptExpressions.IsHalted, Expression.Constant(true))
                );
            }
        }

        [Instruction("DI", 0xF3, Cycles=4)]
        public Expression Di
        {
            get
            {
                return Expression.Block(
                    Expression.Assign(_interuptExpressions.IFF1, Expression.Constant(false)),
                    Expression.Assign(_interuptExpressions.IFF2, Expression.Constant(false))
                );
            }
        }

        [Instruction("EI", 0xFB, Cycles = 4)]
        public Expression Ei
        {
            get
            {
                return Expression.Block(
                    Expression.Assign(_interuptExpressions.IFF1, Expression.Constant(true)),
                    Expression.Assign(_interuptExpressions.IFF2, Expression.Constant(true)),

                    Expression.IfThen(Expression.Equal(_interuptExpressions.InteruptRequested, Expression.Constant(true)),
                            Expression.Block(
                                Expression.Assign(_interuptExpressions.InteruptRequested, Expression.Constant(false)),
                                Expression.Assign(_interuptExpressions.IFF1, Expression.Constant(false)),
                                _cpuValueExpressions.Push(_programControlExpressions.ProgramCounterRegister),
                                Expression.Assign(_programControlExpressions.ProgramCounterRegister, Expression.Constant(0x0038)),
                                Expression.SubtractAssign(_programControlExpressions.CycleCounter, Expression.Constant(13))
                            )
                        )
                );
            }
        }

        [Instruction("IM 0", 0x46, Prefix="ED", Cycles = 8)]
        public Expression IM_0
        {
            get { return Expression.Assign(_interuptExpressions.InteruptMode, Expression.Constant(0)); }
        }

        [Instruction("IM 1", 0x56, Prefix = "ED", Cycles = 8)]
        public Expression IM_1
        {
            get { return Expression.Assign(_interuptExpressions.InteruptMode, Expression.Constant(1)); }
        }

        [Instruction("IM 2", 0x5E, Prefix = "ED", Cycles = 8)]
        public Expression IM_2
        {
            get { return Expression.Assign(_interuptExpressions.InteruptMode, Expression.Constant(2)); }
        }

        [Instruction("LD I,A", 0x47, Prefix="ED", Cycles=9)]
        public Expression Ld_I_A
        {
            get { return Expression.Assign(_interuptExpressions.InteruptVectorRegister, _cpuValueExpressions.RegisterA); }
        }

        [Instruction("LD R,A", 0x4F, Prefix="ED", Cycles=9)]
        public Expression Ld_R_A
        {
            get { return Expression.Assign(_interuptExpressions.RefreshRegister, _cpuValueExpressions.RegisterA); }
        }

        [Instruction("LD A,I", 0x57, Prefix="ED", Cycles=9)]
        public Expression Ld_A_I
        {
            get 
            { 
                return Expression.Block(
                    Expression.Assign(_cpuValueExpressions.RegisterA, _interuptExpressions.InteruptVectorRegister),

                    Expression.Assign(_cpuValueExpressions.FlagsRegister,
                        Expression.Or(
                            Expression.And(Expression.ArrayIndex(_flagLookupExpressions.SignZeroParityCalcultorFlags, _cpuValueExpressions.RegisterA), Expression.Constant(0xFB)),
                            Expression.LeftShift(Expression.Convert(_interuptExpressions.IFF2, typeof(Int32)), Expression.Constant(0x02))
                        )
                    )
                ); 
            }
        }

        [Instruction("LD A,R", 0x5F, Prefix = "ED", Cycles = 9)]
        public Expression Ld_A_R
        {
            get 
            { 
                return  Expression.Assign(_cpuValueExpressions.RegisterA,
                            Expression.And(_interuptExpressions.RefreshRegister, Expression.Constant(0xFF))
                        ); 
            }
        }

        [Instruction("RETI", 0x4D, Prefix = "ED", Cycles = 14)]
        public Expression RetI
        {
            get { return _cpuValueExpressions.Pop(_programControlExpressions.ProgramCounterRegister); }
        }

        [Instruction("RETN", 0x45, Prefix = "ED", Cycles = 14)]
        public Expression RetN
        {
            get 
            {
                return Expression.Block(
                    _cpuValueExpressions.Pop(_programControlExpressions.ProgramCounterRegister),
                    Expression.Assign(_interuptExpressions.IFF1, _interuptExpressions.IFF2)
                );
            }
        }     
    }
}

