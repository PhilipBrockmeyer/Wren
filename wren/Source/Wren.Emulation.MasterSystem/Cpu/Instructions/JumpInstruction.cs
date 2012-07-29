using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wren.Emulation.MasterSystem.ExpressionLibraries;
using System.Linq.Expressions;

namespace Wren.Emulation.MasterSystem.Instructions
{
    public class JumpInstruction
    {
        IDataAccessExpressionLibrary _cpuValueExpressions;
        ITemporaryExpressionLibrary _tempExpressions;
        IProgramControlExpressionLibrary _programControlExpressions;

        public JumpInstruction(IDataAccessExpressionLibrary registerExpressions,
                                 ITemporaryExpressionLibrary temporaryExpressions,
                                 IProgramControlExpressionLibrary programControlExpressions)
        {
            _cpuValueExpressions = registerExpressions;
            _tempExpressions = temporaryExpressions;
            _programControlExpressions = programControlExpressions;
        }

        private Expression RelativeJump(Expression test, Int32 cyclesWhenJumped, Int32 cyclesWhenNotJumped)
        {
            return Expression.Block(
                Expression.IfThen(
                    test,
                    
                    Expression.Block(
                        Expression.AddAssign(_programControlExpressions.ProgramCounterRegister, _programControlExpressions.ParameterByte1),
                        Expression.SubtractAssign(_programControlExpressions.CycleCounter, Expression.Constant(cyclesWhenJumped - cyclesWhenNotJumped))
                    )
                ),

                Expression.SubtractAssign(_programControlExpressions.CycleCounter, Expression.Constant(cyclesWhenNotJumped))

            );
        }

        private Expression AbsoluteJump(Expression test, Int32 cyclesWhenJumped, Int32 cyclesWhenNotJumped)
        {
            return Expression.Block(
                Expression.IfThen(
                    test,

                    Expression.Block(
                        Expression.Assign(_programControlExpressions.ProgramCounterRegister, _programControlExpressions.ParameterWord),
                        Expression.SubtractAssign(_programControlExpressions.CycleCounter, Expression.Constant(cyclesWhenJumped - cyclesWhenNotJumped))
                    )
                ),

                Expression.SubtractAssign(_programControlExpressions.CycleCounter, Expression.Constant(cyclesWhenNotJumped))
            );
        }

        [Instruction("DJNZ d", 0x10, ParameterMode=InstructionParameterMode.Index)]
        public Expression DJNZ_d
        {
            get
            {
                return Expression.Block(
                    // Decrement and truncate B.
                    Expression.Assign(_cpuValueExpressions.RegisterB,
                        Expression.And(
                            Expression.Decrement(_cpuValueExpressions.RegisterB),
                            Expression.Constant(0xFF)
                        )
                    ),

                    RelativeJump(Expression.NotEqual(_cpuValueExpressions.RegisterB, Expression.Constant(0x00)), 13, 8)
                );
            }
        }

        [Instruction("JR d", 0x18, ParameterMode = InstructionParameterMode.Index)]
        public Expression JR_d
        {
            get
            {
                return Expression.Block(
                    Expression.AddAssign(_programControlExpressions.ProgramCounterRegister, _programControlExpressions.ParameterByte1),
                    Expression.SubtractAssign(_programControlExpressions.CycleCounter, Expression.Constant(12))
                );
            }
        }

        [Instruction("JR NZ,d", 0x20, ParameterMode = InstructionParameterMode.Index)]
        public Expression JR_NZ_d
        {
            get
            {
                return RelativeJump(
                        Expression.Equal(
                            Expression.And(_cpuValueExpressions.FlagsRegister, Expression.Constant(Flags.ZeroFlag)), Expression.Constant(0x00)
                        ),
                        12, 
                        7
                    );
            }
        }

        [Instruction("JR Z,d", 0x28, ParameterMode = InstructionParameterMode.Index)]
        public Expression JR_Z_d
        {
            get
            {
                return RelativeJump(
                        Expression.NotEqual(
                            Expression.And(_cpuValueExpressions.FlagsRegister, Expression.Constant(Flags.ZeroFlag)), Expression.Constant(0x00)
                        ),
                        12,
                        7
                    );
            }
        }

        [Instruction("JR NC,d", 0x30, ParameterMode = InstructionParameterMode.Index)]
        public Expression JR_NC_d
        {
            get
            {
                return RelativeJump(
                        Expression.Equal(
                            Expression.And(_cpuValueExpressions.FlagsRegister, Expression.Constant(Flags.CarryFlag)), Expression.Constant(0x00)
                        ),
                        12,
                        7
                    );
            }
        }

        [Instruction("JR C,d", 0x38, ParameterMode = InstructionParameterMode.Index)]
        public Expression JR_C_d
        {
            get
            {
                return RelativeJump(
                        Expression.NotEqual(
                            Expression.And(_cpuValueExpressions.FlagsRegister, Expression.Constant(Flags.CarryFlag)), Expression.Constant(0x00)
                        ),
                        12,
                        7
                    );
            }
        }

        [Instruction("JP NZ,nn", 0xC2, ParameterMode = InstructionParameterMode.Address)]
        public Expression JP_NZ_nn
        {
            get
            {
                return AbsoluteJump(
                        Expression.Equal(
                            Expression.And(_cpuValueExpressions.FlagsRegister, Expression.Constant(Flags.ZeroFlag)), Expression.Constant(0x00)
                        ),
                        10,
                        10
                    );
            }
        }

        [Instruction("JP nn", 0xC3, ParameterMode = InstructionParameterMode.Address)]
        public Expression JP_nn
        {
            get
            {
                return Expression.Block(
                    Expression.Assign(_programControlExpressions.ProgramCounterRegister, _programControlExpressions.ParameterWord),
                    Expression.SubtractAssign(_programControlExpressions.CycleCounter, Expression.Constant(10))
                );
            }
        }

        [Instruction("JP Z,nn", 0xCA, ParameterMode = InstructionParameterMode.Address)]
        public Expression JP_Z_nn
        {
            get
            {
                return AbsoluteJump(
                        Expression.NotEqual(
                            Expression.And(_cpuValueExpressions.FlagsRegister, Expression.Constant(Flags.ZeroFlag)), Expression.Constant(0x00)
                        ),
                        10,
                        10
                    );
            }
        }

        [Instruction("JP NC,nn", 0xD2, ParameterMode = InstructionParameterMode.Address)]
        public Expression JP_NC_nn
        {
            get
            {
                return AbsoluteJump(
                        Expression.Equal(
                            Expression.And(_cpuValueExpressions.FlagsRegister, Expression.Constant(Flags.CarryFlag)), Expression.Constant(0x00)
                        ),
                        10,
                        10
                    );
            }
        }

        [Instruction("JP C,nn", 0xDA, ParameterMode = InstructionParameterMode.Address)]
        public Expression JP_C_nn
        {
            get
            {
                return AbsoluteJump(
                        Expression.NotEqual(
                            Expression.And(_cpuValueExpressions.FlagsRegister, Expression.Constant(Flags.CarryFlag)), Expression.Constant(0x00)
                        ),
                        10,
                        10
                    );
            }
        }

        [Instruction("JP PO,nn", 0xE2, ParameterMode = InstructionParameterMode.Address)]
        public Expression JP_PO_nn
        {
            get
            {
                return AbsoluteJump(
                        Expression.Equal(
                            Expression.And(_cpuValueExpressions.FlagsRegister, Expression.Constant(Flags.ParityFlag)), Expression.Constant(0x00)
                        ),
                        10,
                        10
                    );
            }
        }

        [Instruction("JP (HL)", 0xE9)]
        public Expression JP_HL
        {
            get
            {
                return Expression.Block(
                    Expression.Assign(_programControlExpressions.ProgramCounterRegister,
                        Expression.Or(
                            Expression.LeftShift(_cpuValueExpressions.RegisterH, Expression.Constant(0x08)),
                            _cpuValueExpressions.RegisterL
                        )),
                    Expression.SubtractAssign(_programControlExpressions.CycleCounter, Expression.Constant(4))
                );
            }
        }

        [Instruction("JP PE,nn", 0xEA, ParameterMode = InstructionParameterMode.Address)]
        public Expression JP_PE_nn
        {
            get
            {
                return AbsoluteJump(
                        Expression.NotEqual(
                            Expression.And(_cpuValueExpressions.FlagsRegister, Expression.Constant(Flags.ParityFlag)), Expression.Constant(0x00)
                        ),
                        10,
                        10
                    );
            }
        }

        [Instruction("JP P,nn", 0xF2, ParameterMode = InstructionParameterMode.Address)]
        public Expression JP_P_nn
        {
            get
            {
                return AbsoluteJump(
                        Expression.Equal(
                            Expression.And(_cpuValueExpressions.FlagsRegister, Expression.Constant(Flags.SignFlag)), Expression.Constant(0x00)
                        ),
                        10,
                        10
                    );
            }
        }

        [Instruction("JP M,nn", 0xFA, ParameterMode = InstructionParameterMode.Address)]
        public Expression JP_M_nn
        {
            get
            {
                return AbsoluteJump(
                        Expression.NotEqual(
                            Expression.And(_cpuValueExpressions.FlagsRegister, Expression.Constant(Flags.SignFlag)), Expression.Constant(0x00)
                        ),
                        10,
                        10
                    );
            }
        }

        [Instruction("JP (IX)", 0xE9, Cycles=8, Prefix="DD")]
        public Expression JP_IX
        {
            get { return Expression.Assign(_programControlExpressions.ProgramCounterRegister, _cpuValueExpressions.RegisterIX); }
        }

        [Instruction("JP (IY)", 0xE9, Cycles=8, Prefix = "FD")]
        public Expression JP_IY
        {
            get { return Expression.Assign(_programControlExpressions.ProgramCounterRegister, _cpuValueExpressions.RegisterIY); }
        }

    }
}
