using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wren.Emulation.MasterSystem.ExpressionLibraries;
using System.Linq.Expressions;

namespace Wren.Emulation.MasterSystem.Instructions
{
    public class RotateInstruction
    {
        IDataAccessExpressionLibrary _cpuValueExpressions;
        IFlagLookupValuesExpressionLibrary _flagExpressions;
        ITemporaryExpressionLibrary _tempExpressions;
        IProgramControlExpressionLibrary _programControlExpressions;

        public RotateInstruction(IDataAccessExpressionLibrary registerExpressions,
                                 IFlagLookupValuesExpressionLibrary flagExpressions,
                                 ITemporaryExpressionLibrary temporaryExpressions,
                                 IProgramControlExpressionLibrary programControlExpressions)
        {
            _cpuValueExpressions = registerExpressions;
            _flagExpressions = flagExpressions;
            _tempExpressions = temporaryExpressions;
            _programControlExpressions = programControlExpressions;
        }

        #region RLC
        private Expression Rlc(Expression value)
        {
            return Expression.Block(
                // Store carry in temp.
                Expression.Assign(_tempExpressions.Temp1,
                    // Carry flag is determined by MSB of register before rotate.
                    Expression.RightShift(value, Expression.Constant(0x07))
                ),

                // A = ((A << 1) | (Temp & 1)) & 0xFF
                Expression.Assign(value,
                    Expression.And(
                        Expression.Or(
                            Expression.LeftShift(value, Expression.Constant(0x01)),
                            _tempExpressions.Temp1
                        ),
                        Expression.Constant(0xFF)
                    )
                ),

                Expression.Assign(_cpuValueExpressions.FlagsRegister,
                    Expression.Or(
                        Expression.ArrayIndex(_flagExpressions.SignZeroParityCalcultorFlags, value),
                        _tempExpressions.Temp1
                    )
                )
            );
        }
        
        [Instruction("RLC B", 0x00, Prefix="CB", Cycles = 8)]
        public Expression Rlc_B
        {
            get { return Rlc(_cpuValueExpressions.RegisterB); }
        }

        [Instruction("RLC C", 0x01, Prefix = "CB", Cycles = 8)]
        public Expression Rlc_C
        {
            get { return Rlc(_cpuValueExpressions.RegisterC); }
        }

        [Instruction("RLC D", 0x02, Prefix = "CB", Cycles = 8)]
        public Expression Rlc_D
        {
            get { return Rlc(_cpuValueExpressions.RegisterD); }
        }

        [Instruction("RLC E", 0x03, Prefix = "CB", Cycles = 8)]
        public Expression Rlc_E
        {
            get { return Rlc(_cpuValueExpressions.RegisterE); }
        }

        [Instruction("RLC H", 0x04, Prefix = "CB", Cycles = 8)]
        public Expression Rlc_H
        {
            get { return Rlc(_cpuValueExpressions.RegisterH); }
        }

        [Instruction("RLC L", 0x05, Prefix = "CB", Cycles = 8)]
        public Expression Rlc_L
        {
            get { return Rlc(_cpuValueExpressions.RegisterL); }
        }

        [Instruction("RLC (HL)", 0x06, Prefix = "CB", Cycles = 15)]
        public Expression Rlc_HLi
        {
            get 
            {
                return Expression.Block(
                    Expression.Assign(_tempExpressions.Temp1, _cpuValueExpressions.ReadHL),
                    Rlc(_tempExpressions.Temp1),
                    _cpuValueExpressions.WriteByteHL(_tempExpressions.Temp1)
                );
            }
        }

        [Instruction("RLC A", 0x07, Prefix = "CB", Cycles = 8)]
        public Expression Rlc_A
        {
            get { return Rlc(_cpuValueExpressions.RegisterA); }
        }

        #endregion

        #region RRC
        private Expression Rrc(Expression value)
        {
            return Expression.Block(
                // Store carry in temp.
                Expression.Assign(_tempExpressions.Temp1,
                    Expression.And(value, Expression.Constant(0x01))
                ),

                // A = ((A >> 1) | (T << 7)
                Expression.Assign(value,
                    Expression.Or(
                        Expression.RightShift(value, Expression.Constant(0x01)),
                        Expression.LeftShift(_tempExpressions.Temp1, Expression.Constant(0x07))    
                    )
                ),               

                Expression.Assign(_cpuValueExpressions.FlagsRegister,
                    Expression.Or(
                        Expression.ArrayIndex(_flagExpressions.SignZeroParityCalcultorFlags, value),
                        _tempExpressions.Temp1
                    )
                )
            );
        }

        [Instruction("RRC B", 0x08, Prefix = "CB", Cycles = 8)]
        public Expression Rrc_B
        {
            get { return Rrc(_cpuValueExpressions.RegisterB); }
        }

        [Instruction("RRC C", 0x09, Prefix = "CB", Cycles = 8)]
        public Expression Rrc_C
        {
            get { return Rrc(_cpuValueExpressions.RegisterC); }
        }

        [Instruction("RRC D", 0x0A, Prefix = "CB", Cycles = 8)]
        public Expression Rrc_D
        {
            get { return Rrc(_cpuValueExpressions.RegisterD); }
        }

        [Instruction("RRC E", 0x0B, Prefix = "CB", Cycles = 8)]
        public Expression Rrc_E
        {
            get { return Rrc(_cpuValueExpressions.RegisterE); }
        }

        [Instruction("RRC H", 0x0C, Prefix = "CB", Cycles = 8)]
        public Expression Rrc_H
        {
            get { return Rrc(_cpuValueExpressions.RegisterH); }
        }

        [Instruction("RRC L", 0x0D, Prefix = "CB", Cycles = 8)]
        public Expression Rrc_L
        {
            get { return Rrc(_cpuValueExpressions.RegisterL); }
        }

        [Instruction("RRC (HL)", 0x0E, Prefix = "CB", Cycles = 15)]
        public Expression Rrc_HLi
        {
            get
            {
                return Expression.Block(
                    Expression.Assign(_tempExpressions.Temp1, _cpuValueExpressions.ReadHL),
                    Rrc(_tempExpressions.Temp1),
                    _cpuValueExpressions.WriteByteHL(_tempExpressions.Temp1)
                );
            }
        }

        [Instruction("RRC A", 0x0F, Prefix = "CB", Cycles = 8)]
        public Expression Rrc_A
        {
            get { return Rrc(_cpuValueExpressions.RegisterA); }
        }

        #endregion

        #region RL
        private Expression Rl(Expression value)
        {
            return Expression.Block(
                // Store carry in temp.
                Expression.Assign(_tempExpressions.Temp1,
                // Carry flag is determined by MSB of register before rotate.
                    Expression.RightShift(value, Expression.Constant(0x07))
                ),

                // A = ((A << 1) | (F & 1)) & 0xFF
                Expression.Assign(value,
                    Expression.And(
                        Expression.Or(
                            Expression.LeftShift(value, Expression.Constant(0x01)),
                            Expression.And(_cpuValueExpressions.FlagsRegister, Expression.Constant(0x01))
                        ),
                        Expression.Constant(0xFF)
                    )
                ),

                Expression.Assign(_cpuValueExpressions.FlagsRegister,
                    Expression.Or(
                        Expression.ArrayIndex(_flagExpressions.SignZeroParityCalcultorFlags, value),
                        _tempExpressions.Temp1
                    )
                )
            );
        }

        [Instruction("RL B", 0x10, Prefix = "CB", Cycles = 8)]
        public Expression Rl_B
        {
            get { return Rl(_cpuValueExpressions.RegisterB); }
        }

        [Instruction("RL C", 0x11, Prefix = "CB", Cycles = 8)]
        public Expression Rl_C
        {
            get { return Rl(_cpuValueExpressions.RegisterC); }
        }

        [Instruction("RL D", 0x12, Prefix = "CB", Cycles = 8)]
        public Expression Rl_D
        {
            get { return Rl(_cpuValueExpressions.RegisterD); }
        }

        [Instruction("RL E", 0x13, Prefix = "CB", Cycles = 8)]
        public Expression Rl_E
        {
            get { return Rl(_cpuValueExpressions.RegisterE); }
        }

        [Instruction("RL H", 0x14, Prefix = "CB", Cycles = 8)]
        public Expression Rl_H
        {
            get { return Rl(_cpuValueExpressions.RegisterH); }
        }

        [Instruction("RL L", 0x15, Prefix = "CB", Cycles = 8)]
        public Expression Rl_L
        {
            get { return Rl(_cpuValueExpressions.RegisterL); }
        }

        [Instruction("RL (HL)", 0x16, Prefix = "CB", Cycles = 15)]
        public Expression Rl_HLi
        {
            get
            {
                return Expression.Block(
                    Expression.Assign(_tempExpressions.Temp1, _cpuValueExpressions.ReadHL),
                    Rl(_tempExpressions.Temp1),
                    _cpuValueExpressions.WriteByteHL(_tempExpressions.Temp1)
                );
            }
        }

        [Instruction("RL A", 0x17, Prefix = "CB", Cycles = 8)]
        public Expression Rl_A
        {
            get { return Rl(_cpuValueExpressions.RegisterA); }
        }

        #endregion

        #region RR
        private Expression Rr(Expression value)
        {
            return Expression.Block(
                // Store carry in temp.
                Expression.Assign(_tempExpressions.Temp1,
                    Expression.And(value, Expression.Constant(0x01))
                ),

                // A = ((A >> 1)
                Expression.Assign(value,
                    Expression.Or(
                        Expression.RightShift(value, Expression.Constant(0x01)),
                        Expression.LeftShift(Expression.And(_cpuValueExpressions.FlagsRegister, Expression.Constant(0x01)), Expression.Constant(0x07))
                    )
                ),

                Expression.Assign(_cpuValueExpressions.FlagsRegister,
                    Expression.Or(
                        Expression.ArrayIndex(_flagExpressions.SignZeroParityCalcultorFlags, value),
                        _tempExpressions.Temp1
                    )
                )
            );
        }

        [Instruction("RR B", 0x18, Prefix = "CB", Cycles = 8)]
        public Expression Rr_B
        {
            get { return Rr(_cpuValueExpressions.RegisterB); }
        }

        [Instruction("RR C", 0x19, Prefix = "CB", Cycles = 8)]
        public Expression Rr_C
        {
            get { return Rr(_cpuValueExpressions.RegisterC); }
        }

        [Instruction("RR D", 0x1A, Prefix = "CB", Cycles = 8)]
        public Expression Rr_D
        {
            get { return Rr(_cpuValueExpressions.RegisterD); }
        }

        [Instruction("RR E", 0x1B, Prefix = "CB", Cycles = 8)]
        public Expression Rr_E
        {
            get { return Rr(_cpuValueExpressions.RegisterE); }
        }

        [Instruction("RR H", 0x1C, Prefix = "CB", Cycles = 8)]
        public Expression Rr_H
        {
            get { return Rr(_cpuValueExpressions.RegisterH); }
        }

        [Instruction("RR L", 0x1D, Prefix = "CB", Cycles = 8)]
        public Expression Rr_L
        {
            get { return Rr(_cpuValueExpressions.RegisterL); }
        }

        [Instruction("RR (HL)", 0x1E, Prefix = "CB", Cycles = 15)]
        public Expression Rr_HLi
        {
            get
            {
                return Expression.Block(
                    Expression.Assign(_tempExpressions.Temp1, _cpuValueExpressions.ReadHL),
                    Rrc(_tempExpressions.Temp1),
                    _cpuValueExpressions.WriteByteHL(_tempExpressions.Temp1)
                );
            }
        }

        [Instruction("RR A", 0x1F, Prefix = "CB", Cycles = 8)]
        public Expression Rr_A
        {
            get { return Rr(_cpuValueExpressions.RegisterA); }
        }

        #endregion

        #region IX
        [Instruction("RLC (IX+d)", 0x06, Cycles = 23, Prefix = "DDCB")]
        public Expression Rlc_IXdi
        {
            get
            {
                return Expression.Block(
                    Expression.Assign(_tempExpressions.Temp1, _cpuValueExpressions.ReadIXd(_programControlExpressions.ParameterByte1)),
                    Rlc(_tempExpressions.Temp1),
                    _cpuValueExpressions.WriteByteIXd(_programControlExpressions.ParameterByte1, _tempExpressions.Temp1)
                );
            }
        }

        [Instruction("RRC (IX+d)", 0x0E, Cycles = 23, Prefix = "DDCB")]
        public Expression Rrc_IXdi
        {
            get
            {
                return Expression.Block(
                    Expression.Assign(_tempExpressions.Temp1, _cpuValueExpressions.ReadIXd(_programControlExpressions.ParameterByte1)),
                    Rrc(_tempExpressions.Temp1),
                    _cpuValueExpressions.WriteByteIXd(_programControlExpressions.ParameterByte1, _tempExpressions.Temp1)
                );
            }
        }

        [Instruction("RL (IX+d)", 0x16, Cycles = 23, Prefix = "DDCB")]
        public Expression Rl_IXdi
        {
            get
            {
                return Expression.Block(
                    Expression.Assign(_tempExpressions.Temp1, _cpuValueExpressions.ReadIXd(_programControlExpressions.ParameterByte1)),
                    Rl(_tempExpressions.Temp1),
                    _cpuValueExpressions.WriteByteIXd(_programControlExpressions.ParameterByte1, _tempExpressions.Temp1)
                );
            }
        }

        [Instruction("RR (IX+d)", 0x1E, Cycles = 23, Prefix = "DDCB")]
        public Expression Rr_IXdi
        {
            get
            {
                return Expression.Block(
                    Expression.Assign(_tempExpressions.Temp1, _cpuValueExpressions.ReadIXd(_programControlExpressions.ParameterByte1)),
                    Rr(_tempExpressions.Temp1),
                    _cpuValueExpressions.WriteByteIXd(_programControlExpressions.ParameterByte1, _tempExpressions.Temp1)
                );
            }
        }
        #endregion

        #region IY
        [Instruction("RLC (IY+d)", 0x06, Cycles = 23, Prefix = "FDCB")]
        public Expression Rlc_IYdi
        {
            get
            {
                return Expression.Block(
                    Expression.Assign(_tempExpressions.Temp1, _cpuValueExpressions.ReadIYd(_programControlExpressions.ParameterByte1)),
                    Rlc(_tempExpressions.Temp1),
                    _cpuValueExpressions.WriteByteIYd(_programControlExpressions.ParameterByte1, _tempExpressions.Temp1)
                );
            }
        }

        [Instruction("RRC (IY+d)", 0x0E, Cycles = 23, Prefix = "FDCB")]
        public Expression Rrc_IYdi
        {
            get
            {
                return Expression.Block(
                    Expression.Assign(_tempExpressions.Temp1, _cpuValueExpressions.ReadIYd(_programControlExpressions.ParameterByte1)),
                    Rrc(_tempExpressions.Temp1),
                    _cpuValueExpressions.WriteByteIYd(_programControlExpressions.ParameterByte1, _tempExpressions.Temp1)
                );
            }
        }

        [Instruction("RL (IY+d)", 0x16, Cycles = 23, Prefix = "FDCB")]
        public Expression Rl_IYdi
        {
            get
            {
                return Expression.Block(
                    Expression.Assign(_tempExpressions.Temp1, _cpuValueExpressions.ReadIYd(_programControlExpressions.ParameterByte1)),
                    Rl(_tempExpressions.Temp1),
                    _cpuValueExpressions.WriteByteIYd(_programControlExpressions.ParameterByte1, _tempExpressions.Temp1)
                );
            }
        }

        [Instruction("RR (IY+d)", 0x1E, Cycles = 23, Prefix = "FDCB", ParameterMode = InstructionParameterMode.Index)]
        public Expression Rr_IYdi
        {
            get
            {
                return Expression.Block(
                    Expression.Assign(_tempExpressions.Temp1, _cpuValueExpressions.ReadIYd(_programControlExpressions.ParameterByte1)),
                    Rr(_tempExpressions.Temp1),
                    _cpuValueExpressions.WriteByteIYd(_programControlExpressions.ParameterByte1, _tempExpressions.Temp1)
                );
            }
        }
        #endregion
    }
}
