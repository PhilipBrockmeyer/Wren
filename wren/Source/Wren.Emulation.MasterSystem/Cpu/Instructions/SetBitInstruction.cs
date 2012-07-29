using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wren.Emulation.MasterSystem.ExpressionLibraries;
using System.Linq.Expressions;

namespace Wren.Emulation.MasterSystem.Instructions
{
    public class SetBitInstruction
    {
        IDataAccessExpressionLibrary _cpuValueExpressions;
        IFlagLookupValuesExpressionLibrary _flagExpressions;
        ITemporaryExpressionLibrary _tempExpressions;
        IProgramControlExpressionLibrary _programControlExpressions;

        public SetBitInstruction(IDataAccessExpressionLibrary registerExpressions,
                                 IFlagLookupValuesExpressionLibrary flagExpressions,
                                 ITemporaryExpressionLibrary temporaryExpressions,
                                 IProgramControlExpressionLibrary programControlExpressions)
        {
            _cpuValueExpressions = registerExpressions;
            _flagExpressions = flagExpressions;
            _tempExpressions = temporaryExpressions;
            _programControlExpressions = programControlExpressions;
        }

        private Expression Set(Expression value, Int32 bitNumber)
        {
            // value = value | (0x01 << bitnumber)
            return Expression.Assign(value,
                    Expression.Or(value,
                        Expression.LeftShift(Expression.Constant(0x01), Expression.Constant(bitNumber))
                    )
                );
        }

        // Bit 0.
        [Instruction("SET 0,B", 0xC0, Cycles = 8, Prefix = "CB")]
        public Expression Set_0_B { get { return Set(_cpuValueExpressions.RegisterB, 0); } }

        [Instruction("SET 0,C", 0xC1, Cycles = 8, Prefix = "CB")]
        public Expression Set_0_C { get { return Set(_cpuValueExpressions.RegisterC, 0); } }

        [Instruction("SET 0,D", 0xC2, Cycles = 8, Prefix = "CB")]
        public Expression Set_0_D { get { return Set(_cpuValueExpressions.RegisterD, 0); } }

        [Instruction("SET 0,E", 0xC3, Cycles = 8, Prefix = "CB")]
        public Expression Set_0_E { get { return Set(_cpuValueExpressions.RegisterE, 0); } }

        [Instruction("SET 0,H", 0xC4, Cycles = 8, Prefix = "CB")]
        public Expression Set_0_H { get { return Set(_cpuValueExpressions.RegisterH, 0); } }

        [Instruction("SET 0,L", 0xC5, Cycles = 8, Prefix = "CB")]
        public Expression Set_0_L { get { return Set(_cpuValueExpressions.RegisterL, 0); } }

        [Instruction("SET 0,(HL)", 0xC6, Cycles = 12, Prefix = "CB")]
        public Expression Set_0_HLi
        {
            get
            {
                return Expression.Block(
                        Expression.Assign(_tempExpressions.Temp1, _cpuValueExpressions.ReadHL),
                        Set(_tempExpressions.Temp1, 0),
                        _cpuValueExpressions.WriteByteHL(_tempExpressions.Temp1)
                    );
            }
        }

        [Instruction("SET 0,A", 0xC7, Cycles = 8, Prefix = "CB")]
        public Expression Set_0_A { get { return Set(_cpuValueExpressions.RegisterA, 0); } }

        // Bit 1.
        [Instruction("SET 1,B", 0xC8, Cycles = 8, Prefix = "CB")]
        public Expression Set_1_B { get { return Set(_cpuValueExpressions.RegisterB, 1); } }

        [Instruction("SET 1,C", 0xC9, Cycles = 8, Prefix = "CB")]
        public Expression Set_1_C { get { return Set(_cpuValueExpressions.RegisterC, 1); } }

        [Instruction("SET 1,D", 0xCA, Cycles = 8, Prefix = "CB")]
        public Expression Set_1_D { get { return Set(_cpuValueExpressions.RegisterD, 1); } }

        [Instruction("SET 1,E", 0xCB, Cycles = 8, Prefix = "CB")]
        public Expression Set_1_E { get { return Set(_cpuValueExpressions.RegisterE, 1); } }

        [Instruction("SET 1,H", 0xCC, Cycles = 8, Prefix = "CB")]
        public Expression Set_1_H { get { return Set(_cpuValueExpressions.RegisterH, 1); } }

        [Instruction("SET 1,L", 0xCD, Cycles = 8, Prefix = "CB")]
        public Expression Set_1_L { get { return Set(_cpuValueExpressions.RegisterL, 1); } }

        [Instruction("SET 1,(HL)", 0xCE, Cycles = 12, Prefix = "CB")]
        public Expression Set_1_HLi
        {
            get
            {
                return Expression.Block(
                        Expression.Assign(_tempExpressions.Temp1, _cpuValueExpressions.ReadHL),
                        Set(_tempExpressions.Temp1, 1),
                        _cpuValueExpressions.WriteByteHL(_tempExpressions.Temp1)
                    );
            }
        }

        [Instruction("SET 1,A", 0xCF, Cycles = 8, Prefix = "CB")]
        public Expression Set_1_A { get { return Set(_cpuValueExpressions.RegisterA, 1); } }


        // Bit 2.
        [Instruction("SET 2,B", 0xD0, Cycles = 8, Prefix = "CB")]
        public Expression Set_2_B { get { return Set(_cpuValueExpressions.RegisterB, 2); } }

        [Instruction("SET 2,C", 0xD1, Cycles = 8, Prefix = "CB")]
        public Expression Set_2_C { get { return Set(_cpuValueExpressions.RegisterC, 2); } }

        [Instruction("SET 2,D", 0xD2, Cycles = 8, Prefix = "CB")]
        public Expression Set_2_D { get { return Set(_cpuValueExpressions.RegisterD, 2); } }

        [Instruction("SET 2,E", 0xD3, Cycles = 8, Prefix = "CB")]
        public Expression Set_2_E { get { return Set(_cpuValueExpressions.RegisterE, 2); } }

        [Instruction("SET 2,H", 0xD4, Cycles = 8, Prefix = "CB")]
        public Expression Set_2_H { get { return Set(_cpuValueExpressions.RegisterH, 2); } }

        [Instruction("SET 2,L", 0xD5, Cycles = 8, Prefix = "CB")]
        public Expression Set_2_L { get { return Set(_cpuValueExpressions.RegisterL, 2); } }

        [Instruction("SET 2,(HL)", 0xD6, Cycles = 12, Prefix = "CB")]
        public Expression Set_2_HLi
        {
            get
            {
                return Expression.Block(
                        Expression.Assign(_tempExpressions.Temp1, _cpuValueExpressions.ReadHL),
                        Set(_tempExpressions.Temp1, 2),
                        _cpuValueExpressions.WriteByteHL(_tempExpressions.Temp1)
                    );
            }
        }

        [Instruction("SET 2,A", 0xD7, Cycles = 8, Prefix = "CB")]
        public Expression Set_2_A { get { return Set(_cpuValueExpressions.RegisterA, 2); } }


        // Bit 3.
        [Instruction("SET 3,B", 0xD8, Cycles = 8, Prefix = "CB")]
        public Expression Set_3_B { get { return Set(_cpuValueExpressions.RegisterB, 3); } }

        [Instruction("SET 3,C", 0xD9, Cycles = 8, Prefix = "CB")]
        public Expression Set_3_C { get { return Set(_cpuValueExpressions.RegisterC, 3); } }

        [Instruction("SET 3,D", 0xDA, Cycles = 8, Prefix = "CB")]
        public Expression Set_3_D { get { return Set(_cpuValueExpressions.RegisterD, 3); } }

        [Instruction("SET 3,E", 0xDB, Cycles = 8, Prefix = "CB")]
        public Expression Set_3_E { get { return Set(_cpuValueExpressions.RegisterE, 3); } }

        [Instruction("SET 3,H", 0xDC, Cycles = 8, Prefix = "CB")]
        public Expression Set_3_H { get { return Set(_cpuValueExpressions.RegisterH, 3); } }

        [Instruction("SET 3,L", 0xDD, Cycles = 8, Prefix = "CB")]
        public Expression Set_3_L { get { return Set(_cpuValueExpressions.RegisterL, 3); } }

        [Instruction("SET 3,(HL)", 0xDE, Cycles = 12, Prefix = "CB")]
        public Expression Set_3_HLi
        {
            get
            {
                return Expression.Block(
                        Expression.Assign(_tempExpressions.Temp1, _cpuValueExpressions.ReadHL),
                        Set(_tempExpressions.Temp1, 3),
                        _cpuValueExpressions.WriteByteHL(_tempExpressions.Temp1)
                    );
            }
        }

        [Instruction("SET 3,A", 0xDF, Cycles = 8, Prefix = "CB")]
        public Expression Set_3_A { get { return Set(_cpuValueExpressions.RegisterA, 3); } }



        // Bit 4.
        [Instruction("SET 4,B", 0xE0, Cycles = 8, Prefix = "CB")]
        public Expression Set_4_B { get { return Set(_cpuValueExpressions.RegisterB, 4); } }

        [Instruction("SET 4,C", 0xE1, Cycles = 8, Prefix = "CB")]
        public Expression Set_4_C { get { return Set(_cpuValueExpressions.RegisterC, 4); } }

        [Instruction("SET 4,D", 0xE2, Cycles = 8, Prefix = "CB")]
        public Expression Set_4_D { get { return Set(_cpuValueExpressions.RegisterD, 4); } }

        [Instruction("SET 4,E", 0xE3, Cycles = 8, Prefix = "CB")]
        public Expression Set_4_E { get { return Set(_cpuValueExpressions.RegisterE, 4); } }

        [Instruction("SET 4,H", 0xE4, Cycles = 8, Prefix = "CB")]
        public Expression Set_4_H { get { return Set(_cpuValueExpressions.RegisterH, 4); } }

        [Instruction("SET 4,L", 0xE5, Cycles = 8, Prefix = "CB")]
        public Expression Set_4_L { get { return Set(_cpuValueExpressions.RegisterL, 4); } }

        [Instruction("SET 4,(HL)", 0xE6, Cycles = 12, Prefix = "CB")]
        public Expression Set_4_HLi
        {
            get
            {
                return Expression.Block(
                        Expression.Assign(_tempExpressions.Temp1, _cpuValueExpressions.ReadHL),
                        Set(_tempExpressions.Temp1, 4),
                        _cpuValueExpressions.WriteByteHL(_tempExpressions.Temp1)
                    );
            }
        }

        [Instruction("SET 4,A", 0xE7, Cycles = 8, Prefix = "CB")]
        public Expression Set_4_A { get { return Set(_cpuValueExpressions.RegisterA, 4); } }


        // Bit 5.
        [Instruction("SET 5,B", 0xE8, Cycles = 8, Prefix = "CB")]
        public Expression Set_5_B { get { return Set(_cpuValueExpressions.RegisterB, 5); } }

        [Instruction("SET 5,C", 0xE9, Cycles = 8, Prefix = "CB")]
        public Expression Set_5_C { get { return Set(_cpuValueExpressions.RegisterC, 5); } }

        [Instruction("SET 5,D", 0xEA, Cycles = 8, Prefix = "CB")]
        public Expression Set_5_D { get { return Set(_cpuValueExpressions.RegisterD, 5); } }

        [Instruction("SET 5,E", 0xEB, Cycles = 8, Prefix = "CB")]
        public Expression Set_5_E { get { return Set(_cpuValueExpressions.RegisterE, 5); } }

        [Instruction("SET 5,H", 0xEC, Cycles = 8, Prefix = "CB")]
        public Expression Set_5_H { get { return Set(_cpuValueExpressions.RegisterH, 5); } }

        [Instruction("SET 5,L", 0xED, Cycles = 8, Prefix = "CB")]
        public Expression Set_5_L { get { return Set(_cpuValueExpressions.RegisterL, 5); } }

        [Instruction("SET 5,(HL)", 0xEE, Cycles = 12, Prefix = "CB")]
        public Expression Set_5_HLi
        {
            get
            {
                return Expression.Block(
                        Expression.Assign(_tempExpressions.Temp1, _cpuValueExpressions.ReadHL),
                        Set(_tempExpressions.Temp1, 5),
                        _cpuValueExpressions.WriteByteHL(_tempExpressions.Temp1)
                    );
            }
        }

        [Instruction("SET 5,A", 0xEF, Cycles = 8, Prefix = "CB")]
        public Expression Set_5_A { get { return Set(_cpuValueExpressions.RegisterA, 5); } }


        // Bit 6.
        [Instruction("SET 6,B", 0xF0, Cycles = 8, Prefix = "CB")]
        public Expression Set_6_B { get { return Set(_cpuValueExpressions.RegisterB, 6); } }

        [Instruction("SET 6,C", 0xF1, Cycles = 8, Prefix = "CB")]
        public Expression Set_6_C { get { return Set(_cpuValueExpressions.RegisterC, 6); } }

        [Instruction("SET 6,D", 0xF2, Cycles = 8, Prefix = "CB")]
        public Expression Set_6_D { get { return Set(_cpuValueExpressions.RegisterD, 6); } }

        [Instruction("SET 6,E", 0xF3, Cycles = 8, Prefix = "CB")]
        public Expression Set_6_E { get { return Set(_cpuValueExpressions.RegisterE, 6); } }

        [Instruction("SET 6,H", 0xF4, Cycles = 8, Prefix = "CB")]
        public Expression Set_6_H { get { return Set(_cpuValueExpressions.RegisterH, 6); } }

        [Instruction("SET 6,L", 0xF5, Cycles = 8, Prefix = "CB")]
        public Expression Set_6_L { get { return Set(_cpuValueExpressions.RegisterL, 6); } }

        [Instruction("SET 6,(HL)", 0xF6, Cycles = 12, Prefix = "CB")]
        public Expression Set_6_HLi
        {
            get
            {
                return Expression.Block(
                        Expression.Assign(_tempExpressions.Temp1, _cpuValueExpressions.ReadHL),
                        Set(_tempExpressions.Temp1, 6),
                        _cpuValueExpressions.WriteByteHL(_tempExpressions.Temp1)
                    );
            }
        }

        [Instruction("SET 6,A", 0xF7, Cycles = 8, Prefix = "CB")]
        public Expression Set_6_A { get { return Set(_cpuValueExpressions.RegisterA, 6); } }


        // Bit 7.
        [Instruction("SET 7,B", 0xF8, Cycles = 8, Prefix = "CB")]
        public Expression Set_7_B { get { return Set(_cpuValueExpressions.RegisterB, 7); } }

        [Instruction("SET 7,C", 0xF9, Cycles = 8, Prefix = "CB")]
        public Expression Set_7_C { get { return Set(_cpuValueExpressions.RegisterC, 7); } }

        [Instruction("SET 7,D", 0xFA, Cycles = 8, Prefix = "CB")]
        public Expression Set_7_D { get { return Set(_cpuValueExpressions.RegisterD, 7); } }

        [Instruction("SET 7,E", 0xFB, Cycles = 8, Prefix = "CB")]
        public Expression Set_7_E { get { return Set(_cpuValueExpressions.RegisterE, 7); } }

        [Instruction("SET 7,H", 0xFC, Cycles = 8, Prefix = "CB")]
        public Expression Set_7_H { get { return Set(_cpuValueExpressions.RegisterH, 7); } }

        [Instruction("SET 7,L", 0xFD, Cycles = 8, Prefix = "CB")]
        public Expression Set_7_L { get { return Set(_cpuValueExpressions.RegisterL, 7); } }

        [Instruction("SET 7,(HL)", 0xFE, Cycles = 12, Prefix = "CB")]
        public Expression Set_7_HLi
        {
            get
            {
                return Expression.Block(
                        Expression.Assign(_tempExpressions.Temp1, _cpuValueExpressions.ReadHL),
                        Set(_tempExpressions.Temp1, 7),
                        _cpuValueExpressions.WriteByteHL(_tempExpressions.Temp1)
                    );
            }
        }

        [Instruction("SET 7,A", 0xFF, Cycles = 8, Prefix = "CB")]
        public Expression Set_7_A { get { return Set(_cpuValueExpressions.RegisterA, 7); } }

        // IX+d
        [Instruction("SET 0,(IX+d)", 0xC6, Cycles = 12, Prefix = "DDCB")]
        public Expression Set_0_IXdi 
        { 
            get 
            {
                return Expression.Block(
                    Expression.Assign(_tempExpressions.Temp1, _cpuValueExpressions.ReadIXd(_programControlExpressions.ParameterByte1)),
                    Set(_tempExpressions.Temp1, 0),
                    _cpuValueExpressions.WriteByteIXd(_programControlExpressions.ParameterByte1, _tempExpressions.Temp1)
                );
            } 
        }

        [Instruction("SET 1,(IX+d)", 0xCE, Cycles = 12, Prefix = "DDCB")]
        public Expression Set_1_IXdi
        {
            get
            {
                return Expression.Block(
                    Expression.Assign(_tempExpressions.Temp1, _cpuValueExpressions.ReadIXd(_programControlExpressions.ParameterByte1)),
                    Set(_tempExpressions.Temp1, 1),
                    _cpuValueExpressions.WriteByteIXd(_programControlExpressions.ParameterByte1, _tempExpressions.Temp1)
                );
            }
        }

        [Instruction("SET 2,(IX+d)", 0xD6, Cycles = 12, Prefix = "DDCB")]
        public Expression Set_2_IXdi
        {
            get
            {
                return Expression.Block(
                    Expression.Assign(_tempExpressions.Temp1, _cpuValueExpressions.ReadIXd(_programControlExpressions.ParameterByte1)),
                    Set(_tempExpressions.Temp1, 2),
                    _cpuValueExpressions.WriteByteIXd(_programControlExpressions.ParameterByte1, _tempExpressions.Temp1)
                );
            }
        }

        [Instruction("SET 3,(IX+d)", 0xDE, Cycles = 12, Prefix = "DDCB")]
        public Expression Set_3_IXdi
        {
            get
            {
                return Expression.Block(
                    Expression.Assign(_tempExpressions.Temp1, _cpuValueExpressions.ReadIXd(_programControlExpressions.ParameterByte1)),
                    Set(_tempExpressions.Temp1, 3),
                    _cpuValueExpressions.WriteByteIXd(_programControlExpressions.ParameterByte1, _tempExpressions.Temp1)
                );
            }
        }

        [Instruction("SET 4,(IX+d)", 0xE6, Cycles = 12, Prefix = "DDCB")]
        public Expression Set_4_IXdi
        {
            get
            {
                return Expression.Block(
                    Expression.Assign(_tempExpressions.Temp1, _cpuValueExpressions.ReadIXd(_programControlExpressions.ParameterByte1)),
                    Set(_tempExpressions.Temp1, 4),
                    _cpuValueExpressions.WriteByteIXd(_programControlExpressions.ParameterByte1, _tempExpressions.Temp1)
                );
            }
        }

        [Instruction("SET 5,(IX+d)", 0xEE, Cycles = 12, Prefix = "DDCB")]
        public Expression Set_5_IXdi
        {
            get
            {
                return Expression.Block(
                    Expression.Assign(_tempExpressions.Temp1, _cpuValueExpressions.ReadIXd(_programControlExpressions.ParameterByte1)),
                    Set(_tempExpressions.Temp1, 5),
                    _cpuValueExpressions.WriteByteIXd(_programControlExpressions.ParameterByte1, _tempExpressions.Temp1)
                );
            }
        }

        [Instruction("SET 6,(IX+d)", 0xF6, Cycles = 12, Prefix = "DDCB")]
        public Expression Set_6_IXdi
        {
            get
            {
                return Expression.Block(
                    Expression.Assign(_tempExpressions.Temp1, _cpuValueExpressions.ReadIXd(_programControlExpressions.ParameterByte1)),
                    Set(_tempExpressions.Temp1, 6),
                    _cpuValueExpressions.WriteByteIXd(_programControlExpressions.ParameterByte1, _tempExpressions.Temp1)
                );
            }
        }

        [Instruction("SET 7,(IX+d)", 0xFE, Cycles = 12, Prefix = "DDCB")]
        public Expression Set_7_IXdi
        {
            get
            {
                return Expression.Block(
                    Expression.Assign(_tempExpressions.Temp1, _cpuValueExpressions.ReadIXd(_programControlExpressions.ParameterByte1)),
                    Set(_tempExpressions.Temp1, 7),
                    _cpuValueExpressions.WriteByteIXd(_programControlExpressions.ParameterByte1, _tempExpressions.Temp1)
                );
            }
        }
        
        // IY+d
        [Instruction("SET 0,(IY+d)", 0xC6, Cycles = 12, Prefix = "FDCB")]
        public Expression Set_0_IYdi
        {
            get
            {
                return Expression.Block(
                    Expression.Assign(_tempExpressions.Temp1, _cpuValueExpressions.ReadIYd(_programControlExpressions.ParameterByte1)),
                    Set(_tempExpressions.Temp1, 0),
                    _cpuValueExpressions.WriteByteIYd(_programControlExpressions.ParameterByte1, _tempExpressions.Temp1)
                );
            }
        }

        [Instruction("SET 1,(IY+d)", 0xCE, Cycles = 12, Prefix = "FDCB")]
        public Expression Set_1_IYdi
        {
            get
            {
                return Expression.Block(
                    Expression.Assign(_tempExpressions.Temp1, _cpuValueExpressions.ReadIYd(_programControlExpressions.ParameterByte1)),
                    Set(_tempExpressions.Temp1, 1),
                    _cpuValueExpressions.WriteByteIYd(_programControlExpressions.ParameterByte1, _tempExpressions.Temp1)
                );
            }
        }

        [Instruction("SET 2,(IY+d)", 0xD6, Cycles = 12, Prefix = "FDCB")]
        public Expression Set_2_IYdi
        {
            get
            {
                return Expression.Block(
                    Expression.Assign(_tempExpressions.Temp1, _cpuValueExpressions.ReadIYd(_programControlExpressions.ParameterByte1)),
                    Set(_tempExpressions.Temp1, 2),
                    _cpuValueExpressions.WriteByteIYd(_programControlExpressions.ParameterByte1, _tempExpressions.Temp1)
                );
            }
        }

        [Instruction("SET 3,(IY+d)", 0xDE, Cycles = 12, Prefix = "FDCB")]
        public Expression Set_3_IYdi
        {
            get
            {
                return Expression.Block(
                    Expression.Assign(_tempExpressions.Temp1, _cpuValueExpressions.ReadIYd(_programControlExpressions.ParameterByte1)),
                    Set(_tempExpressions.Temp1, 3),
                    _cpuValueExpressions.WriteByteIYd(_programControlExpressions.ParameterByte1, _tempExpressions.Temp1)
                );
            }
        }

        [Instruction("SET 4,(IY+d)", 0xE6, Cycles = 12, Prefix = "FDCB")]
        public Expression Set_4_IYdi
        {
            get
            {
                return Expression.Block(
                    Expression.Assign(_tempExpressions.Temp1, _cpuValueExpressions.ReadIYd(_programControlExpressions.ParameterByte1)),
                    Set(_tempExpressions.Temp1, 4),
                    _cpuValueExpressions.WriteByteIYd(_programControlExpressions.ParameterByte1, _tempExpressions.Temp1)
                );
            }
        }

        [Instruction("SET 5,(IY+d)", 0xEE, Cycles = 12, Prefix = "FDCB")]
        public Expression Set_5_IYdi
        {
            get
            {
                return Expression.Block(
                    Expression.Assign(_tempExpressions.Temp1, _cpuValueExpressions.ReadIYd(_programControlExpressions.ParameterByte1)),
                    Set(_tempExpressions.Temp1, 5),
                    _cpuValueExpressions.WriteByteIYd(_programControlExpressions.ParameterByte1, _tempExpressions.Temp1)
                );
            }
        }

        [Instruction("SET 6,(IY+d)", 0xF6, Cycles = 12, Prefix = "FDCB")]
        public Expression Set_6_IYdi
        {
            get
            {
                return Expression.Block(
                    Expression.Assign(_tempExpressions.Temp1, _cpuValueExpressions.ReadIYd(_programControlExpressions.ParameterByte1)),
                    Set(_tempExpressions.Temp1, 6),
                    _cpuValueExpressions.WriteByteIYd(_programControlExpressions.ParameterByte1, _tempExpressions.Temp1)
                );
            }
        }

        [Instruction("SET 7,(IY+d)", 0xFE, Cycles = 12, Prefix = "FDCB")]
        public Expression Set_7_IYdi
        {
            get
            {
                return Expression.Block(
                    Expression.Assign(_tempExpressions.Temp1, _cpuValueExpressions.ReadIYd(_programControlExpressions.ParameterByte1)),
                    Set(_tempExpressions.Temp1, 7),
                    _cpuValueExpressions.WriteByteIYd(_programControlExpressions.ParameterByte1, _tempExpressions.Temp1)
                );
            }
        }
    }
}
