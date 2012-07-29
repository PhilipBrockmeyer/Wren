using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wren.Emulation.MasterSystem.ExpressionLibraries;
using System.Linq.Expressions;

namespace Wren.Emulation.MasterSystem.Instructions
{
    public class ResetBitInstruction
    {
        IDataAccessExpressionLibrary _cpuValueExpressions;
        IFlagLookupValuesExpressionLibrary _flagExpressions;
        ITemporaryExpressionLibrary _tempExpressions;
        IProgramControlExpressionLibrary _programControlExpressions;

        public ResetBitInstruction(IDataAccessExpressionLibrary registerExpressions,
                                 IFlagLookupValuesExpressionLibrary flagExpressions,
                                 ITemporaryExpressionLibrary temporaryExpressions,
                                 IProgramControlExpressionLibrary programControlExpressions)
        {
            _cpuValueExpressions = registerExpressions;
            _flagExpressions = flagExpressions;
            _tempExpressions = temporaryExpressions;
            _programControlExpressions = programControlExpressions;
        }

        private Expression Reset(Expression value, Int32 bitNumber)
        {
            return Expression.AndAssign(value, Expression.Constant(0xFF ^ (0x01 << bitNumber)));
        }

        // Bit 0.
        [Instruction("RES 0,B", 0x80, Cycles = 8, Prefix = "CB")]
        public Expression Res_0_B { get { return Reset(_cpuValueExpressions.RegisterB, 0); } }

        [Instruction("RES 0,C", 0x81, Cycles = 8, Prefix = "CB")]
        public Expression Res_0_C { get { return Reset(_cpuValueExpressions.RegisterC, 0); } }

        [Instruction("RES 0,D", 0x82, Cycles = 8, Prefix = "CB")]
        public Expression Res_0_D { get { return Reset(_cpuValueExpressions.RegisterD, 0); } }

        [Instruction("RES 0,E", 0x83, Cycles = 8, Prefix = "CB")]
        public Expression Res_0_E { get { return Reset(_cpuValueExpressions.RegisterE, 0); } }

        [Instruction("RES 0,H", 0x84, Cycles = 8, Prefix = "CB")]
        public Expression Res_0_H { get { return Reset(_cpuValueExpressions.RegisterH, 0); } }

        [Instruction("RES 0,L", 0x85, Cycles = 8, Prefix = "CB")]
        public Expression Res_0_L { get { return Reset(_cpuValueExpressions.RegisterL, 0); } }

        [Instruction("RES 0,(HL)", 0x86, Cycles = 12, Prefix = "CB")]
        public Expression Res_0_HLi
        {
            get
            {
                return Expression.Block(
                        Expression.Assign(_tempExpressions.Temp1, _cpuValueExpressions.ReadHL),
                        Reset(_tempExpressions.Temp1, 0),
                        _cpuValueExpressions.WriteByteHL(_tempExpressions.Temp1)
                    );
            }
        }

        [Instruction("RES 0,A", 0x87, Cycles = 8, Prefix = "CB")]
        public Expression Res_0_A { get { return Reset(_cpuValueExpressions.RegisterA, 0); } }

        // Bit 1.
        [Instruction("RES 1,B", 0x88, Cycles = 8, Prefix = "CB")]
        public Expression Res_1_B { get { return Reset(_cpuValueExpressions.RegisterB, 1); } }

        [Instruction("RES 1,C", 0x89, Cycles = 8, Prefix = "CB")]
        public Expression Res_1_C { get { return Reset(_cpuValueExpressions.RegisterC, 1); } }

        [Instruction("RES 1,D", 0x8A, Cycles = 8, Prefix = "CB")]
        public Expression Res_1_D { get { return Reset(_cpuValueExpressions.RegisterD, 1); } }

        [Instruction("RES 1,E", 0x8B, Cycles = 8, Prefix = "CB")]
        public Expression Res_1_E { get { return Reset(_cpuValueExpressions.RegisterE, 1); } }

        [Instruction("RES 1,H", 0x8C, Cycles = 8, Prefix = "CB")]
        public Expression Res_1_H { get { return Reset(_cpuValueExpressions.RegisterH, 1); } }

        [Instruction("RES 1,L", 0x8D, Cycles = 8, Prefix = "CB")]
        public Expression Res_1_L { get { return Reset(_cpuValueExpressions.RegisterL, 1); } }

        [Instruction("RES 1,(HL)", 0x8E, Cycles = 12, Prefix = "CB")]
        public Expression Res_1_HLi
        {
            get
            {
                return Expression.Block(
                        Expression.Assign(_tempExpressions.Temp1, _cpuValueExpressions.ReadHL),
                        Reset(_tempExpressions.Temp1, 1),
                        _cpuValueExpressions.WriteByteHL(_tempExpressions.Temp1)
                    );
            }
        }

        [Instruction("RES 1,A", 0x8F, Cycles = 8, Prefix = "CB")]
        public Expression Res_1_A { get { return Reset(_cpuValueExpressions.RegisterA, 1); } }


        // Bit 2.
        [Instruction("RES 2,B", 0x90, Cycles = 8, Prefix = "CB")]
        public Expression Res_2_B { get { return Reset(_cpuValueExpressions.RegisterB, 2); } }

        [Instruction("RES 2,C", 0x91, Cycles = 8, Prefix = "CB")]
        public Expression Res_2_C { get { return Reset(_cpuValueExpressions.RegisterC, 2); } }

        [Instruction("RES 2,D", 0x92, Cycles = 8, Prefix = "CB")]
        public Expression Res_2_D { get { return Reset(_cpuValueExpressions.RegisterD, 2); } }

        [Instruction("RES 2,E", 0x93, Cycles = 8, Prefix = "CB")]
        public Expression Res_2_E { get { return Reset(_cpuValueExpressions.RegisterE, 2); } }

        [Instruction("RES 2,H", 0x94, Cycles = 8, Prefix = "CB")]
        public Expression Res_2_H { get { return Reset(_cpuValueExpressions.RegisterH, 2); } }

        [Instruction("RES 2,L", 0x95, Cycles = 8, Prefix = "CB")]
        public Expression Res_2_L { get { return Reset(_cpuValueExpressions.RegisterL, 2); } }

        [Instruction("RES 2,(HL)", 0x96, Cycles = 12, Prefix = "CB")]
        public Expression Res_2_HLi
        {
            get
            {
                return Expression.Block(
                        Expression.Assign(_tempExpressions.Temp1, _cpuValueExpressions.ReadHL),
                        Reset(_tempExpressions.Temp1, 2),
                        _cpuValueExpressions.WriteByteHL(_tempExpressions.Temp1)
                    );
            }
        }

        [Instruction("RES 2,A", 0x97, Cycles = 8, Prefix = "CB")]
        public Expression Res_2_A { get { return Reset(_cpuValueExpressions.RegisterA, 2); } }


        // Bit 3.
        [Instruction("RES 3,B", 0x98, Cycles = 8, Prefix = "CB")]
        public Expression Res_3_B { get { return Reset(_cpuValueExpressions.RegisterB, 3); } }

        [Instruction("RES 3,C", 0x99, Cycles = 8, Prefix = "CB")]
        public Expression Res_3_C { get { return Reset(_cpuValueExpressions.RegisterC, 3); } }

        [Instruction("RES 3,D", 0x9A, Cycles = 8, Prefix = "CB")]
        public Expression Res_3_D { get { return Reset(_cpuValueExpressions.RegisterD, 3); } }

        [Instruction("RES 3,E", 0x9B, Cycles = 8, Prefix = "CB")]
        public Expression Res_3_E { get { return Reset(_cpuValueExpressions.RegisterE, 3); } }

        [Instruction("RES 3,H", 0x9C, Cycles = 8, Prefix = "CB")]
        public Expression Res_3_H { get { return Reset(_cpuValueExpressions.RegisterH, 3); } }

        [Instruction("RES 3,L", 0x9D, Cycles = 8, Prefix = "CB")]
        public Expression Res_3_L { get { return Reset(_cpuValueExpressions.RegisterL, 3); } }

        [Instruction("RES 3,(HL)", 0x9E, Cycles = 12, Prefix = "CB")]
        public Expression Res_3_HLi
        {
            get
            {
                return Expression.Block(
                        Expression.Assign(_tempExpressions.Temp1, _cpuValueExpressions.ReadHL),
                        Reset(_tempExpressions.Temp1, 3),
                        _cpuValueExpressions.WriteByteHL(_tempExpressions.Temp1)
                    );
            }
        }

        [Instruction("RES 3,A", 0x9F, Cycles = 8, Prefix = "CB")]
        public Expression Res_3_A { get { return Reset(_cpuValueExpressions.RegisterA, 3); } }



        // Bit 4.
        [Instruction("RES 4,B", 0xA0, Cycles = 8, Prefix = "CB")]
        public Expression Res_4_B { get { return Reset(_cpuValueExpressions.RegisterB, 4); } }

        [Instruction("RES 4,C", 0xA1, Cycles = 8, Prefix = "CB")]
        public Expression Res_4_C { get { return Reset(_cpuValueExpressions.RegisterC, 4); } }

        [Instruction("RES 4,D", 0xA2, Cycles = 8, Prefix = "CB")]
        public Expression Res_4_D { get { return Reset(_cpuValueExpressions.RegisterD, 4); } }

        [Instruction("RES 4,E", 0xA3, Cycles = 8, Prefix = "CB")]
        public Expression Res_4_E { get { return Reset(_cpuValueExpressions.RegisterE, 4); } }

        [Instruction("RES 4,H", 0xA4, Cycles = 8, Prefix = "CB")]
        public Expression Res_4_H { get { return Reset(_cpuValueExpressions.RegisterH, 4); } }

        [Instruction("RES 4,L", 0xA5, Cycles = 8, Prefix = "CB")]
        public Expression Res_4_L { get { return Reset(_cpuValueExpressions.RegisterL, 4); } }

        [Instruction("RES 4,(HL)", 0xA6, Cycles = 12, Prefix = "CB")]
        public Expression Res_4_HLi
        {
            get
            {
                return Expression.Block(
                        Expression.Assign(_tempExpressions.Temp1, _cpuValueExpressions.ReadHL),
                        Reset(_tempExpressions.Temp1, 4),
                        _cpuValueExpressions.WriteByteHL(_tempExpressions.Temp1)
                    );
            }
        }

        [Instruction("RES 4,A", 0xA7, Cycles = 8, Prefix = "CB")]
        public Expression Res_4_A { get { return Reset(_cpuValueExpressions.RegisterA, 4); } }


        // Bit 5.
        [Instruction("RES 5,B", 0xA8, Cycles = 8, Prefix = "CB")]
        public Expression Res_5_B { get { return Reset(_cpuValueExpressions.RegisterB, 5); } }

        [Instruction("RES 5,C", 0xA9, Cycles = 8, Prefix = "CB")]
        public Expression Res_5_C { get { return Reset(_cpuValueExpressions.RegisterC, 5); } }

        [Instruction("RES 5,D", 0xAA, Cycles = 8, Prefix = "CB")]
        public Expression Res_5_D { get { return Reset(_cpuValueExpressions.RegisterD, 5); } }

        [Instruction("RES 5,E", 0xAB, Cycles = 8, Prefix = "CB")]
        public Expression Res_5_E { get { return Reset(_cpuValueExpressions.RegisterE, 5); } }

        [Instruction("RES 5,H", 0xAC, Cycles = 8, Prefix = "CB")]
        public Expression Res_5_H { get { return Reset(_cpuValueExpressions.RegisterH, 5); } }

        [Instruction("RES 5,L", 0xAD, Cycles = 8, Prefix = "CB")]
        public Expression Res_5_L { get { return Reset(_cpuValueExpressions.RegisterL, 5); } }

        [Instruction("RES 5,(HL)", 0xAE, Cycles = 12, Prefix = "CB")]
        public Expression Res_5_HLi
        {
            get
            {
                return Expression.Block(
                        Expression.Assign(_tempExpressions.Temp1, _cpuValueExpressions.ReadHL),
                        Reset(_tempExpressions.Temp1, 5),
                        _cpuValueExpressions.WriteByteHL(_tempExpressions.Temp1)
                    );
            }
        }

        [Instruction("RES 5,A", 0xAF, Cycles = 8, Prefix = "CB")]
        public Expression Res_5_A { get { return Reset(_cpuValueExpressions.RegisterA, 5); } }


        // Bit 6.
        [Instruction("RES 6,B", 0xB0, Cycles = 8, Prefix = "CB")]
        public Expression Res_6_B { get { return Reset(_cpuValueExpressions.RegisterB, 6); } }

        [Instruction("RES 6,C", 0xB1, Cycles = 8, Prefix = "CB")]
        public Expression Res_6_C { get { return Reset(_cpuValueExpressions.RegisterC, 6); } }

        [Instruction("RES 6,D", 0xB2, Cycles = 8, Prefix = "CB")]
        public Expression Res_6_D { get { return Reset(_cpuValueExpressions.RegisterD, 6); } }

        [Instruction("RES 6,E", 0xB3, Cycles = 8, Prefix = "CB")]
        public Expression Res_6_E { get { return Reset(_cpuValueExpressions.RegisterE, 6); } }

        [Instruction("RES 6,H", 0xB4, Cycles = 8, Prefix = "CB")]
        public Expression Res_6_H { get { return Reset(_cpuValueExpressions.RegisterH, 6); } }

        [Instruction("RES 6,L", 0xB5, Cycles = 8, Prefix = "CB")]
        public Expression Res_6_L { get { return Reset(_cpuValueExpressions.RegisterL, 6); } }

        [Instruction("RES 6,(HL)", 0xB6, Cycles = 12, Prefix = "CB")]
        public Expression Res_6_HLi
        {
            get
            {
                return Expression.Block(
                        Expression.Assign(_tempExpressions.Temp1, _cpuValueExpressions.ReadHL),
                        Reset(_tempExpressions.Temp1, 6),
                        _cpuValueExpressions.WriteByteHL(_tempExpressions.Temp1)
                    );
            }
        }

        [Instruction("RES 6,A", 0xB7, Cycles = 8, Prefix = "CB")]
        public Expression Res_6_A { get { return Reset(_cpuValueExpressions.RegisterA, 6); } }


        // Bit 7.
        [Instruction("RES 7,B", 0xB8, Cycles = 8, Prefix = "CB")]
        public Expression Res_7_B { get { return Reset(_cpuValueExpressions.RegisterB, 7); } }

        [Instruction("RES 7,C", 0xB9, Cycles = 8, Prefix = "CB")]
        public Expression Res_7_C { get { return Reset(_cpuValueExpressions.RegisterC, 7); } }

        [Instruction("RES 7,D", 0xBA, Cycles = 8, Prefix = "CB")]
        public Expression Res_7_D { get { return Reset(_cpuValueExpressions.RegisterD, 7); } }

        [Instruction("RES 7,E", 0xBB, Cycles = 8, Prefix = "CB")]
        public Expression Res_7_E { get { return Reset(_cpuValueExpressions.RegisterE, 7); } }

        [Instruction("RES 7,H", 0xBC, Cycles = 8, Prefix = "CB")]
        public Expression Res_7_H { get { return Reset(_cpuValueExpressions.RegisterH, 7); } }

        [Instruction("RES 7,L", 0xBD, Cycles = 8, Prefix = "CB")]
        public Expression Res_7_L { get { return Reset(_cpuValueExpressions.RegisterL, 7); } }

        [Instruction("RES 7,(HL)", 0xBE, Cycles = 12, Prefix = "CB")]
        public Expression Res_7_HLi
        {
            get
            {
                return Expression.Block(
                        Expression.Assign(_tempExpressions.Temp1, _cpuValueExpressions.ReadHL),
                        Reset(_tempExpressions.Temp1, 7),
                        _cpuValueExpressions.WriteByteHL(_tempExpressions.Temp1)
                    );
            }
        }

        [Instruction("RES 7,A", 0xBF, Cycles = 8, Prefix = "CB")]
        public Expression Res_7_A { get { return Reset(_cpuValueExpressions.RegisterA, 7); } }

        // IX+d
        [Instruction("RES 0,(IX+d)", 0x86, Cycles = 12, Prefix = "DDCB")]
        public Expression Res_0_IXdi
        {
            get
            {
                return Expression.Block(
                    Expression.Assign(_tempExpressions.Temp1, _cpuValueExpressions.ReadIXd(_programControlExpressions.ParameterByte1)),
                    Reset(_tempExpressions.Temp1, 0),
                    _cpuValueExpressions.WriteByteIXd(_programControlExpressions.ParameterByte1, _tempExpressions.Temp1)
                );
            }
        }

        [Instruction("RES 1,(IX+d)", 0x8E, Cycles = 12, Prefix = "DDCB")]
        public Expression Res_1_IXdi
        {
            get
            {
                return Expression.Block(
                    Expression.Assign(_tempExpressions.Temp1, _cpuValueExpressions.ReadIXd(_programControlExpressions.ParameterByte1)),
                    Reset(_tempExpressions.Temp1, 1),
                    _cpuValueExpressions.WriteByteIXd(_programControlExpressions.ParameterByte1, _tempExpressions.Temp1)
                );
            }
        }

        [Instruction("RES 2,(IX+d)", 0x96, Cycles = 12, Prefix = "DDCB")]
        public Expression Res_2_IXdi
        {
            get
            {
                return Expression.Block(
                    Expression.Assign(_tempExpressions.Temp1, _cpuValueExpressions.ReadIXd(_programControlExpressions.ParameterByte1)),
                    Reset(_tempExpressions.Temp1, 2),
                    _cpuValueExpressions.WriteByteIXd(_programControlExpressions.ParameterByte1, _tempExpressions.Temp1)
                );
            }
        }

        [Instruction("RES 3,(IX+d)", 0x9E, Cycles = 12, Prefix = "DDCB")]
        public Expression Res_3_IXdi
        {
            get
            {
                return Expression.Block(
                    Expression.Assign(_tempExpressions.Temp1, _cpuValueExpressions.ReadIXd(_programControlExpressions.ParameterByte1)),
                    Reset(_tempExpressions.Temp1, 3),
                    _cpuValueExpressions.WriteByteIXd(_programControlExpressions.ParameterByte1, _tempExpressions.Temp1)
                );
            }
        }

        [Instruction("RES 4,(IX+d)", 0xA6, Cycles = 12, Prefix = "DDCB")]
        public Expression Res_4_IXdi
        {
            get
            {
                return Expression.Block(
                    Expression.Assign(_tempExpressions.Temp1, _cpuValueExpressions.ReadIXd(_programControlExpressions.ParameterByte1)),
                    Reset(_tempExpressions.Temp1, 4),
                    _cpuValueExpressions.WriteByteIXd(_programControlExpressions.ParameterByte1, _tempExpressions.Temp1)
                );
            }
        }

        [Instruction("RES 5,(IX+d)", 0xAE, Cycles = 12, Prefix = "DDCB")]
        public Expression Res_5_IXdi
        {
            get
            {
                return Expression.Block(
                    Expression.Assign(_tempExpressions.Temp1, _cpuValueExpressions.ReadIXd(_programControlExpressions.ParameterByte1)),
                    Reset(_tempExpressions.Temp1, 5),
                    _cpuValueExpressions.WriteByteIXd(_programControlExpressions.ParameterByte1, _tempExpressions.Temp1)
                );
            }
        }

        [Instruction("RES 6,(IX+d)", 0xB6, Cycles = 12, Prefix = "DDCB")]
        public Expression Res_6_IXdi
        {
            get
            {
                return Expression.Block(
                    Expression.Assign(_tempExpressions.Temp1, _cpuValueExpressions.ReadIXd(_programControlExpressions.ParameterByte1)),
                    Reset(_tempExpressions.Temp1, 6),
                    _cpuValueExpressions.WriteByteIXd(_programControlExpressions.ParameterByte1, _tempExpressions.Temp1)
                );
            }
        }

        [Instruction("RES 7,(IX+d)", 0xBE, Cycles = 12, Prefix = "DDCB")]
        public Expression Res_7_IXdi
        {
            get
            {
                return Expression.Block(
                    Expression.Assign(_tempExpressions.Temp1, _cpuValueExpressions.ReadIXd(_programControlExpressions.ParameterByte1)),
                    Reset(_tempExpressions.Temp1, 7),
                    _cpuValueExpressions.WriteByteIXd(_programControlExpressions.ParameterByte1, _tempExpressions.Temp1)
                );
            }
        }

        // IY+d
        [Instruction("RES 0,(IY+d)", 0x86, Cycles = 12, Prefix = "FDCB")]
        public Expression Res_0_IYdi
        {
            get
            {
                return Expression.Block(
                    Expression.Assign(_tempExpressions.Temp1, _cpuValueExpressions.ReadIYd(_programControlExpressions.ParameterByte1)),
                    Reset(_tempExpressions.Temp1, 0),
                    _cpuValueExpressions.WriteByteIYd(_programControlExpressions.ParameterByte1, _tempExpressions.Temp1)
                );
            }
        }

        [Instruction("RES 1,(IY+d)", 0x8E, Cycles = 12, Prefix = "FDCB")]
        public Expression Res_1_IYdi
        {
            get
            {
                return Expression.Block(
                    Expression.Assign(_tempExpressions.Temp1, _cpuValueExpressions.ReadIYd(_programControlExpressions.ParameterByte1)),
                    Reset(_tempExpressions.Temp1, 1),
                    _cpuValueExpressions.WriteByteIYd(_programControlExpressions.ParameterByte1, _tempExpressions.Temp1)
                );
            }
        }

        [Instruction("RES 2,(IY+d)", 0x96, Cycles = 12, Prefix = "FDCB")]
        public Expression Res_2_IYdi
        {
            get
            {
                return Expression.Block(
                    Expression.Assign(_tempExpressions.Temp1, _cpuValueExpressions.ReadIYd(_programControlExpressions.ParameterByte1)),
                    Reset(_tempExpressions.Temp1, 2),
                    _cpuValueExpressions.WriteByteIYd(_programControlExpressions.ParameterByte1, _tempExpressions.Temp1)
                );
            }
        }

        [Instruction("RES 3,(IY+d)", 0x9E, Cycles = 12, Prefix = "FDCB")]
        public Expression Res_3_IYdi
        {
            get
            {
                return Expression.Block(
                    Expression.Assign(_tempExpressions.Temp1, _cpuValueExpressions.ReadIYd(_programControlExpressions.ParameterByte1)),
                    Reset(_tempExpressions.Temp1, 3),
                    _cpuValueExpressions.WriteByteIYd(_programControlExpressions.ParameterByte1, _tempExpressions.Temp1)
                );
            }
        }

        [Instruction("RES 4,(IY+d)", 0xA6, Cycles = 12, Prefix = "FDCB")]
        public Expression Res_4_IYdi
        {
            get
            {
                return Expression.Block(
                    Expression.Assign(_tempExpressions.Temp1, _cpuValueExpressions.ReadIYd(_programControlExpressions.ParameterByte1)),
                    Reset(_tempExpressions.Temp1, 4),
                    _cpuValueExpressions.WriteByteIYd(_programControlExpressions.ParameterByte1, _tempExpressions.Temp1)
                );
            }
        }

        [Instruction("RES 5,(IY+d)", 0xAE, Cycles = 12, Prefix = "FDCB")]
        public Expression Res_5_IYdi
        {
            get
            {
                return Expression.Block(
                    Expression.Assign(_tempExpressions.Temp1, _cpuValueExpressions.ReadIYd(_programControlExpressions.ParameterByte1)),
                    Reset(_tempExpressions.Temp1, 5),
                    _cpuValueExpressions.WriteByteIYd(_programControlExpressions.ParameterByte1, _tempExpressions.Temp1)
                );
            }
        }

        [Instruction("RES 6,(IY+d)", 0xB6, Cycles = 12, Prefix = "FDCB")]
        public Expression Res_6_IYdi
        {
            get
            {
                return Expression.Block(
                    Expression.Assign(_tempExpressions.Temp1, _cpuValueExpressions.ReadIYd(_programControlExpressions.ParameterByte1)),
                    Reset(_tempExpressions.Temp1, 6),
                    _cpuValueExpressions.WriteByteIYd(_programControlExpressions.ParameterByte1, _tempExpressions.Temp1)
                );
            }
        }

        [Instruction("RES 7,(IY+d)", 0xBE, Cycles = 12, Prefix = "FDCB")]
        public Expression Res_7_IYdi
        {
            get
            {
                return Expression.Block(
                    Expression.Assign(_tempExpressions.Temp1, _cpuValueExpressions.ReadIYd(_programControlExpressions.ParameterByte1)),
                    Reset(_tempExpressions.Temp1, 7),
                    _cpuValueExpressions.WriteByteIYd(_programControlExpressions.ParameterByte1, _tempExpressions.Temp1)
                );
            }
        }
    }
}
