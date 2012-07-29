using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wren.Emulation.MasterSystem.ExpressionLibraries;
using System.Linq.Expressions;

namespace Wren.Emulation.MasterSystem.Instructions
{
    public class ShiftInstruction
    {
        IDataAccessExpressionLibrary _cpuValueExpressions;
        IFlagLookupValuesExpressionLibrary _flagExpressions;
        ITemporaryExpressionLibrary _tempExpressions;
        IProgramControlExpressionLibrary _programControlExpressions;

        public ShiftInstruction(IDataAccessExpressionLibrary registerExpressions,
                                 IFlagLookupValuesExpressionLibrary flagExpressions,
                                 ITemporaryExpressionLibrary temporaryExpressions,
                                 IProgramControlExpressionLibrary programControlExpressions)
        {
            _cpuValueExpressions = registerExpressions;
            _flagExpressions = flagExpressions;
            _tempExpressions = temporaryExpressions;
            _programControlExpressions = programControlExpressions;
        }

        #region SLA
        private Expression SLA(Expression value)
        {
            return Expression.Block(
                // Store carry in temp.
                Expression.Assign(_tempExpressions.Temp1,
                // Carry flag is determined by MSB of register before rotate.
                    Expression.RightShift(value, Expression.Constant(0x07))
                ),

                // A = ((A << 1) & 0xFF
                Expression.Assign(value,
                    Expression.And(
                        Expression.LeftShift(value, Expression.Constant(0x01)),
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

        [Instruction("SLA B", 0x20, Prefix = "CB", Cycles = 8)]
        public Expression SLA_B
        {
            get { return SLA(_cpuValueExpressions.RegisterB); }
        }

        [Instruction("SLA C", 0x21, Prefix = "CB", Cycles = 8)]
        public Expression SLA_C
        {
            get { return SLA(_cpuValueExpressions.RegisterC); }
        }

        [Instruction("SLA D", 0x22, Prefix = "CB", Cycles = 8)]
        public Expression SLA_D
        {
            get { return SLA(_cpuValueExpressions.RegisterD); }
        }

        [Instruction("SLA E", 0x23, Prefix = "CB", Cycles = 8)]
        public Expression SLA_E
        {
            get { return SLA(_cpuValueExpressions.RegisterE); }
        }

        [Instruction("SLA H", 0x24, Prefix = "CB", Cycles = 8)]
        public Expression SLA_H
        {
            get { return SLA(_cpuValueExpressions.RegisterH); }
        }

        [Instruction("SLA L", 0x25, Prefix = "CB", Cycles = 8)]
        public Expression SLA_L
        {
            get { return SLA(_cpuValueExpressions.RegisterL); }
        }

        [Instruction("SLA (HL)", 0x26, Prefix = "CB", Cycles = 15)]
        public Expression SLA_HLi
        {
            get
            {
                return Expression.Block(
                    Expression.Assign(_tempExpressions.Temp1, _cpuValueExpressions.ReadHL),
                    SLA(_tempExpressions.Temp1),
                    _cpuValueExpressions.WriteByteHL(_tempExpressions.Temp1)
                );
            }
        }

        [Instruction("SLA A", 0x27, Prefix = "CB", Cycles = 8)]
        public Expression SLA_A
        {
            get { return SLA(_cpuValueExpressions.RegisterA); }
        }

        #endregion

        #region SRA
        private Expression Sra(Expression value)
        {
            return Expression.Block(
                // Store carry in temp.
                Expression.Assign(_tempExpressions.Temp1,
                    Expression.And(value, Expression.Constant(0x01))
                ),

                // A = ((A >> 1) | (A & 0x80)
                Expression.Assign(value,
                    Expression.Or(
                        Expression.RightShift(value, Expression.Constant(0x01)),
                        Expression.And(_tempExpressions.Temp1, Expression.Constant(0x80))
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

        [Instruction("SRA B", 0x28, Prefix = "CB", Cycles = 8)]
        public Expression SRA_B
        {
            get { return Sra(_cpuValueExpressions.RegisterB); }
        }

        [Instruction("SRA C", 0x29, Prefix = "CB", Cycles = 8)]
        public Expression SRA_C
        {
            get { return Sra(_cpuValueExpressions.RegisterC); }
        }

        [Instruction("SRA D", 0x2A, Prefix = "CB", Cycles = 8)]
        public Expression SRA_D
        {
            get { return Sra(_cpuValueExpressions.RegisterD); }
        }

        [Instruction("SRA E", 0x2B, Prefix = "CB", Cycles = 8)]
        public Expression SRA_E
        {
            get { return Sra(_cpuValueExpressions.RegisterE); }
        }

        [Instruction("SRA H", 0x2C, Prefix = "CB", Cycles = 8)]
        public Expression SRA_H
        {
            get { return Sra(_cpuValueExpressions.RegisterH); }
        }

        [Instruction("SRA L", 0x2D, Prefix = "CB", Cycles = 8)]
        public Expression SRA_L
        {
            get { return Sra(_cpuValueExpressions.RegisterL); }
        }

        [Instruction("SRA (HL)", 0x2E, Prefix = "CB", Cycles = 15)]
        public Expression SRA_HLi
        {
            get
            {
                return Expression.Block(
                    Expression.Assign(_tempExpressions.Temp1, _cpuValueExpressions.ReadHL),
                    Sra(_tempExpressions.Temp1),
                    _cpuValueExpressions.WriteByteHL(_tempExpressions.Temp1)
                );
            }
        }

        [Instruction("SRA A", 0x2F, Prefix = "CB", Cycles = 8)]
        public Expression SRA_A
        {
            get { return Sra(_cpuValueExpressions.RegisterA); }
        }

        #endregion

        #region SLL
        private Expression Sll(Expression value)
        {
            return Expression.Block(
                // Store carry in temp.
                Expression.Assign(_tempExpressions.Temp1,
                // Carry flag is determined by MSB of register before rotate.
                    Expression.RightShift(value, Expression.Constant(0x07))
                ),

                // A = ((A << 1) | (1)) & 0xFF
                Expression.Assign(value,
                    Expression.And(
                        Expression.Or(
                            Expression.LeftShift(value, Expression.Constant(0x01)),
                            Expression.Constant(0x01)
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

        [Instruction("SLL B", 0x30, Prefix = "CB", Cycles = 8)]
        public Expression SLL_B
        {
            get { return Sll(_cpuValueExpressions.RegisterB); }
        }

        [Instruction("SLL C", 0x31, Prefix = "CB", Cycles = 8)]
        public Expression SLL_C
        {
            get { return Sll(_cpuValueExpressions.RegisterC); }
        }

        [Instruction("SLL D", 0x32, Prefix = "CB", Cycles = 8)]
        public Expression SLL_D
        {
            get { return Sll(_cpuValueExpressions.RegisterD); }
        }

        [Instruction("SLL E", 0x33, Prefix = "CB", Cycles = 8)]
        public Expression SLL_E
        {
            get { return Sll(_cpuValueExpressions.RegisterE); }
        }

        [Instruction("SLL H", 0x34, Prefix = "CB", Cycles = 8)]
        public Expression SLL_H
        {
            get { return Sll(_cpuValueExpressions.RegisterH); }
        }

        [Instruction("SLL L", 0x35, Prefix = "CB", Cycles = 8)]
        public Expression SLL_L
        {
            get { return Sll(_cpuValueExpressions.RegisterL); }
        }

        [Instruction("SLL (HL)", 0x36, Prefix = "CB", Cycles = 15)]
        public Expression SLL_HLi
        {
            get
            {
                return Expression.Block(
                    Expression.Assign(_tempExpressions.Temp1, _cpuValueExpressions.ReadHL),
                    Sll(_tempExpressions.Temp1),
                    _cpuValueExpressions.WriteByteHL(_tempExpressions.Temp1)
                );
            }
        }

        [Instruction("SLL A", 0x37, Prefix = "CB", Cycles = 8)]
        public Expression SLL_A
        {
            get { return Sll(_cpuValueExpressions.RegisterA); }
        }

        #endregion

        #region SRL
        private Expression Srl(Expression value)
        {
            return Expression.Block(
                // Store carry in temp.
                Expression.Assign(_tempExpressions.Temp1,
                    Expression.And(value, Expression.Constant(0x01))
                ),

                // A = ((A >> 1)
                Expression.Assign(value, Expression.RightShift(value, Expression.Constant(0x01))),                        
                
                Expression.Assign(_cpuValueExpressions.FlagsRegister,
                    Expression.Or(
                        Expression.ArrayIndex(_flagExpressions.SignZeroParityCalcultorFlags, value),
                        _tempExpressions.Temp1
                    )
                )
            );
        }

        [Instruction("SRL B", 0x38, Prefix = "CB", Cycles = 8)]
        public Expression SRL_B
        {
            get { return Srl(_cpuValueExpressions.RegisterB); }
        }

        [Instruction("SRL C", 0x39, Prefix = "CB", Cycles = 8)]
        public Expression SRL_C
        {
            get { return Srl(_cpuValueExpressions.RegisterC); }
        }

        [Instruction("SRL D", 0x3A, Prefix = "CB", Cycles = 8)]
        public Expression SRL_D
        {
            get { return Srl(_cpuValueExpressions.RegisterD); }
        }

        [Instruction("SRL E", 0x3B, Prefix = "CB", Cycles = 8)]
        public Expression SRL_E
        {
            get { return Srl(_cpuValueExpressions.RegisterE); }
        }

        [Instruction("SRL H", 0x3C, Prefix = "CB", Cycles = 8)]
        public Expression SRL_H
        {
            get { return Srl(_cpuValueExpressions.RegisterH); }
        }

        [Instruction("SRL L", 0x3D, Prefix = "CB", Cycles = 8)]
        public Expression SRL_L
        {
            get { return Srl(_cpuValueExpressions.RegisterL); }
        }

        [Instruction("SRL (HL)", 0x3E, Prefix = "CB", Cycles = 15)]
        public Expression SRL_HLi
        {
            get
            {
                return Expression.Block(
                    Expression.Assign(_tempExpressions.Temp1, _cpuValueExpressions.ReadHL),
                    Sra(_tempExpressions.Temp1),
                    _cpuValueExpressions.WriteByteHL(_tempExpressions.Temp1)
                );
            }
        }

        [Instruction("SRL A", 0x3F, Prefix = "CB", Cycles = 8)]
        public Expression SRL_A
        {
            get { return Srl(_cpuValueExpressions.RegisterA); }
        }

        #endregion

        #region IX
        [Instruction("SLA (IX+d)", 0x26, Cycles = 23, Prefix = "DDCB")]
        public Expression Sla_IXdi
        {
            get
            {
                return Expression.Block(
                    Expression.Assign(_tempExpressions.Temp1, _cpuValueExpressions.ReadIXd(_programControlExpressions.ParameterByte1)),
                    SLA(_tempExpressions.Temp1),
                    _cpuValueExpressions.WriteByteIXd(_programControlExpressions.ParameterByte1, _tempExpressions.Temp1)
                );
            }
        }

        [Instruction("SRA (IX+d)", 0x2E, Cycles = 23, Prefix = "DDCB")]
        public Expression Sra_IXdi
        {
            get
            {
                return Expression.Block(
                    Expression.Assign(_tempExpressions.Temp1, _cpuValueExpressions.ReadIXd(_programControlExpressions.ParameterByte1)),
                    Sra(_tempExpressions.Temp1),
                    _cpuValueExpressions.WriteByteIXd(_programControlExpressions.ParameterByte1, _tempExpressions.Temp1)
                );
            }
        }

        [Instruction("SLL (IX+d)", 0x36, Cycles = 23, Prefix = "DDCB")]
        public Expression Sll_IXdi
        {
            get
            {
                return Expression.Block(
                    Expression.Assign(_tempExpressions.Temp1, _cpuValueExpressions.ReadIXd(_programControlExpressions.ParameterByte1)),
                    Sll(_tempExpressions.Temp1),
                    _cpuValueExpressions.WriteByteIXd(_programControlExpressions.ParameterByte1, _tempExpressions.Temp1)
                );
            }
        }

        [Instruction("SRL (IX+d)", 0x3E, Cycles = 23, Prefix = "DDCB")]
        public Expression Srl_IXdi
        {
            get
            {
                return Expression.Block(
                    Expression.Assign(_tempExpressions.Temp1, _cpuValueExpressions.ReadIXd(_programControlExpressions.ParameterByte1)),
                    Srl(_tempExpressions.Temp1),
                    _cpuValueExpressions.WriteByteIXd(_programControlExpressions.ParameterByte1, _tempExpressions.Temp1)
                );
            }
        }
        #endregion

        #region IY
        [Instruction("SLA (IY+d)", 0x26, Cycles = 23, Prefix = "FDCB")]
        public Expression Sla_IYdi
        {
            get
            {
                return Expression.Block(
                    Expression.Assign(_tempExpressions.Temp1, _cpuValueExpressions.ReadIYd(_programControlExpressions.ParameterByte1)),
                    SLA(_tempExpressions.Temp1),
                    _cpuValueExpressions.WriteByteIYd(_programControlExpressions.ParameterByte1, _tempExpressions.Temp1)
                );
            }
        }

        [Instruction("SRA (IY+d)", 0x2E, Cycles = 23, Prefix = "FDCB")]
        public Expression Sra_IYdi
        {
            get
            {
                return Expression.Block(
                    Expression.Assign(_tempExpressions.Temp1, _cpuValueExpressions.ReadIYd(_programControlExpressions.ParameterByte1)),
                    Sra(_tempExpressions.Temp1),
                    _cpuValueExpressions.WriteByteIYd(_programControlExpressions.ParameterByte1, _tempExpressions.Temp1)
                );
            }
        }

        [Instruction("SLL (IY+d)", 0x36, Cycles = 23, Prefix = "FDCB")]
        public Expression Sll_IYdi
        {
            get
            {
                return Expression.Block(
                    Expression.Assign(_tempExpressions.Temp1, _cpuValueExpressions.ReadIYd(_programControlExpressions.ParameterByte1)),
                    Sll(_tempExpressions.Temp1),
                    _cpuValueExpressions.WriteByteIYd(_programControlExpressions.ParameterByte1, _tempExpressions.Temp1)
                );
            }
        }

        [Instruction("SRL (IY+d)", 0x3E, Cycles = 23, Prefix = "FDCB")]
        public Expression Srl_IYdi
        {
            get
            {
                return Expression.Block(
                    Expression.Assign(_tempExpressions.Temp1, _cpuValueExpressions.ReadIYd(_programControlExpressions.ParameterByte1)),
                    Srl(_tempExpressions.Temp1),
                    _cpuValueExpressions.WriteByteIYd(_programControlExpressions.ParameterByte1, _tempExpressions.Temp1)
                );
            }
        }
        #endregion
    }
}
