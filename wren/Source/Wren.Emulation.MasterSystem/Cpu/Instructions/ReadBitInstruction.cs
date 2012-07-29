using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wren.Emulation.MasterSystem.ExpressionLibraries;
using System.Linq.Expressions;

namespace Wren.Emulation.MasterSystem.Instructions
{
    public class ReadBitInstruction
    {
        IDataAccessExpressionLibrary _cpuValueExpressions;
        IFlagLookupValuesExpressionLibrary _flagExpressions;
        ITemporaryExpressionLibrary _tempExpressions;
        IProgramControlExpressionLibrary _programControlExpressions;

        public ReadBitInstruction(IDataAccessExpressionLibrary registerExpressions,
                                 IFlagLookupValuesExpressionLibrary flagExpressions,
                                 ITemporaryExpressionLibrary temporaryExpressions,
                                 IProgramControlExpressionLibrary programControlExpressions)
        {
            _cpuValueExpressions = registerExpressions;
            _flagExpressions = flagExpressions;
            _tempExpressions = temporaryExpressions;
            _programControlExpressions = programControlExpressions;
        }

        private Expression Bit(Expression value, Int32 bitNumber)
        {
            List<Expression> expressions = new List<Expression>();
            
                
            // Keep the Carry flag.
            expressions.Add(Expression.AndAssign(_cpuValueExpressions.FlagsRegister, Expression.Constant(Flags.CarryFlag)));
                
            // Set Half Carry flag
            expressions.Add(Expression.OrAssign(_cpuValueExpressions.FlagsRegister, Expression.Constant(Flags.HalfCarry)));

            // Set the 5 and 3 bits.
            expressions.Add(Expression.OrAssign(_cpuValueExpressions.FlagsRegister, 
                Expression.And(
                    Expression.And(value, Expression.Constant(0x01 << bitNumber)),
                    Expression.Constant(Flags.Flag5 | Flags.Flag3)
                )
            ));

            // if bit is set.
            expressions.Add(Expression.IfThen(
                Expression.Equal(
                    Expression.And(value, Expression.Constant(0x01 << bitNumber)),
                    Expression.Constant(0x00)
                ),

                Expression.OrAssign(_cpuValueExpressions.FlagsRegister, Expression.Constant(Flags.ZeroFlag | Flags.ParityFlag))
            ));

            if (bitNumber == 7)
            {
                expressions.Add(Expression.IfThen(
                    Expression.NotEqual(
                        Expression.And(value, Expression.Constant(0x01 << bitNumber)),
                        Expression.Constant(0x00)
                    ),

                    Expression.OrAssign(_cpuValueExpressions.FlagsRegister, Expression.Constant(Flags.SignFlag))
                ));
            }

            return Expression.Block(expressions);
        }

        // Bit 0.
        [Instruction("BIT 0,B", 0x40, Cycles = 8, Prefix="CB")]
        public Expression Bit_0_B { get { return Bit(_cpuValueExpressions.RegisterB, 0); } }

        [Instruction("BIT 0,C", 0x41, Cycles = 8, Prefix="CB")]
        public Expression Bit_0_C { get { return Bit(_cpuValueExpressions.RegisterC, 0); } }

        [Instruction("BIT 0,D", 0x42, Cycles = 8, Prefix="CB")]
        public Expression Bit_0_D { get { return Bit(_cpuValueExpressions.RegisterD, 0); } }

        [Instruction("BIT 0,E", 0x43, Cycles = 8, Prefix="CB")]
        public Expression Bit_0_E { get { return Bit(_cpuValueExpressions.RegisterE, 0); } }

        [Instruction("BIT 0,H", 0x44, Cycles = 8, Prefix="CB")]
        public Expression Bit_0_H { get { return Bit(_cpuValueExpressions.RegisterH, 0); } }

        [Instruction("BIT 0,L", 0x45, Cycles = 8, Prefix="CB")]
        public Expression Bit_0_L { get { return Bit(_cpuValueExpressions.RegisterL, 0); } }

        [Instruction("BIT 0,(HL)", 0x46, Cycles = 12, Prefix="CB")]
        public Expression Bit_0_HLi { get { return Bit(_cpuValueExpressions.ReadHL, 0); } }

        [Instruction("BIT 0,A", 0x47, Cycles = 8, Prefix="CB")]
        public Expression Bit_0_A { get { return Bit(_cpuValueExpressions.RegisterA, 0); } }

        // Bit 1
        [Instruction("BIT 1,B", 0x48, Cycles = 8, Prefix="CB")]
        public Expression Bit_1_B { get { return Bit(_cpuValueExpressions.RegisterB, 1); } }

        [Instruction("BIT 1,C", 0x49, Cycles = 8, Prefix="CB")]
        public Expression Bit_1_C { get { return Bit(_cpuValueExpressions.RegisterC, 1); } }

        [Instruction("BIT 1,D", 0x4A, Cycles = 8, Prefix="CB")]
        public Expression Bit_1_D { get { return Bit(_cpuValueExpressions.RegisterD, 1); } }

        [Instruction("BIT 1,E", 0x4B, Cycles = 8, Prefix="CB")]
        public Expression Bit_1_E { get { return Bit(_cpuValueExpressions.RegisterE, 1); } }

        [Instruction("BIT 1,H", 0x4C, Cycles = 8, Prefix="CB")]
        public Expression Bit_1_H { get { return Bit(_cpuValueExpressions.RegisterH, 1); } }

        [Instruction("BIT 1,L", 0x4D, Cycles = 8, Prefix="CB")]
        public Expression Bit_1_L { get { return Bit(_cpuValueExpressions.RegisterL, 1); } }

        [Instruction("BIT 1,(HL)", 0x4E, Cycles = 12, Prefix="CB")]
        public Expression Bit_1_HLi { get { return Bit(_cpuValueExpressions.ReadHL, 1); } }

        [Instruction("BIT 1,A", 0x4F, Cycles = 8, Prefix="CB")]
        public Expression Bit_1_A { get { return Bit(_cpuValueExpressions.RegisterA, 1); } }

        // Bit 2.
        [Instruction("BIT 2,B", 0x50, Cycles = 8, Prefix="CB")]
        public Expression Bit_2_B { get { return Bit(_cpuValueExpressions.RegisterB, 2); } }

        [Instruction("BIT 2,C", 0x51, Cycles = 8, Prefix="CB")]
        public Expression Bit_2_C { get { return Bit(_cpuValueExpressions.RegisterC, 2); } }

        [Instruction("BIT 2,D", 0x52, Cycles = 8, Prefix="CB")]
        public Expression Bit_2_D { get { return Bit(_cpuValueExpressions.RegisterD, 2); } }

        [Instruction("BIT 2,E", 0x53, Cycles = 8, Prefix="CB")]
        public Expression Bit_2_E { get { return Bit(_cpuValueExpressions.RegisterE, 2); } }

        [Instruction("BIT 2,H", 0x54, Cycles = 8, Prefix="CB")]
        public Expression Bit_2_H { get { return Bit(_cpuValueExpressions.RegisterH, 2); } }

        [Instruction("BIT 2,L", 0x55, Cycles = 8, Prefix="CB")]
        public Expression Bit_2_L { get { return Bit(_cpuValueExpressions.RegisterL, 2); } }

        [Instruction("BIT 2,(HL)", 0x56, Cycles = 12, Prefix="CB")]
        public Expression Bit_2_HLi { get { return Bit(_cpuValueExpressions.ReadHL, 2); } }

        [Instruction("BIT 2,A", 0x57, Cycles = 8, Prefix="CB")]
        public Expression Bit_2_A { get { return Bit(_cpuValueExpressions.RegisterA, 2); } }

        // Bit 3
        [Instruction("BIT 3,B", 0x58, Cycles = 8, Prefix="CB")]
        public Expression Bit_3_B { get { return Bit(_cpuValueExpressions.RegisterB, 3); } }

        [Instruction("BIT 3,C", 0x59, Cycles = 8, Prefix="CB")]
        public Expression Bit_3_C { get { return Bit(_cpuValueExpressions.RegisterC, 3); } }

        [Instruction("BIT 3,D", 0x5A, Cycles = 8, Prefix="CB")]
        public Expression Bit_3_D { get { return Bit(_cpuValueExpressions.RegisterD, 3); } }

        [Instruction("BIT 3,E", 0x5B, Cycles = 8, Prefix="CB")]
        public Expression Bit_3_E { get { return Bit(_cpuValueExpressions.RegisterE, 3); } }

        [Instruction("BIT 3,H", 0x5C, Cycles = 8, Prefix="CB")]
        public Expression Bit_3_H { get { return Bit(_cpuValueExpressions.RegisterH, 3); } }

        [Instruction("BIT 3,L", 0x5D, Cycles = 8, Prefix="CB")]
        public Expression Bit_3_L { get { return Bit(_cpuValueExpressions.RegisterL, 3); } }

        [Instruction("BIT 3,(HL)", 0x5E, Cycles = 12, Prefix="CB")]
        public Expression Bit_3_HLi { get { return Bit(_cpuValueExpressions.ReadHL, 3); } }

        [Instruction("BIT 3,A", 0x5F, Cycles = 8, Prefix="CB")]
        public Expression Bit_3_A { get { return Bit(_cpuValueExpressions.RegisterA, 3); } }

        // Bit 4.
        [Instruction("BIT 4,B", 0x60, Cycles = 8, Prefix="CB")]
        public Expression Bit_4_B { get { return Bit(_cpuValueExpressions.RegisterB, 4); } }

        [Instruction("BIT 4,C", 0x61, Cycles = 8, Prefix="CB")]
        public Expression Bit_4_C { get { return Bit(_cpuValueExpressions.RegisterC, 4); } }

        [Instruction("BIT 4,D", 0x62, Cycles = 8, Prefix="CB")]
        public Expression Bit_4_D { get { return Bit(_cpuValueExpressions.RegisterD, 4); } }

        [Instruction("BIT 4,E", 0x63, Cycles = 8, Prefix="CB")]
        public Expression Bit_4_E { get { return Bit(_cpuValueExpressions.RegisterE, 4); } }

        [Instruction("BIT 4,H", 0x64, Cycles = 8, Prefix="CB")]
        public Expression Bit_4_H { get { return Bit(_cpuValueExpressions.RegisterH, 4); } }

        [Instruction("BIT 4,L", 0x65, Cycles = 8, Prefix="CB")]
        public Expression Bit_4_L { get { return Bit(_cpuValueExpressions.RegisterL, 4); } }

        [Instruction("BIT 4,(HL)", 0x66, Cycles = 12, Prefix="CB")]
        public Expression Bit_4_HLi { get { return Bit(_cpuValueExpressions.ReadHL, 4); } }

        [Instruction("BIT 4,A", 0x67, Cycles = 8, Prefix="CB")]
        public Expression Bit_4_A { get { return Bit(_cpuValueExpressions.RegisterA, 4); } }

        // Bit 5
        [Instruction("BIT 5,B", 0x68, Cycles = 8, Prefix="CB")]
        public Expression Bit_5_B { get { return Bit(_cpuValueExpressions.RegisterB, 5); } }

        [Instruction("BIT 5,C", 0x69, Cycles = 8, Prefix="CB")]
        public Expression Bit_5_C { get { return Bit(_cpuValueExpressions.RegisterC, 5); } }

        [Instruction("BIT 5,D", 0x6A, Cycles = 8, Prefix="CB")]
        public Expression Bit_5_D { get { return Bit(_cpuValueExpressions.RegisterD, 5); } }

        [Instruction("BIT 5,E", 0x6B, Cycles = 8, Prefix="CB")]
        public Expression Bit_5_E { get { return Bit(_cpuValueExpressions.RegisterE, 5); } }

        [Instruction("BIT 5,H", 0x6C, Cycles = 8, Prefix="CB")]
        public Expression Bit_5_H { get { return Bit(_cpuValueExpressions.RegisterH, 5); } }

        [Instruction("BIT 5,L", 0x6D, Cycles = 8, Prefix="CB")]
        public Expression Bit_5_L { get { return Bit(_cpuValueExpressions.RegisterL, 5); } }

        [Instruction("BIT 5,(HL)", 0x6E, Cycles = 12, Prefix="CB")]
        public Expression Bit_5_HLi { get { return Bit(_cpuValueExpressions.ReadHL, 5); } }

        [Instruction("BIT 5,A", 0x6F, Cycles = 8, Prefix="CB")]
        public Expression Bit_5_A { get { return Bit(_cpuValueExpressions.RegisterA, 5); } }

        // Bit 6.
        [Instruction("BIT 6,B", 0x70, Cycles = 8, Prefix="CB")]
        public Expression Bit_6_B { get { return Bit(_cpuValueExpressions.RegisterB, 6); } }

        [Instruction("BIT 6,C", 0x71, Cycles = 8, Prefix="CB")]
        public Expression Bit_6_C { get { return Bit(_cpuValueExpressions.RegisterC, 6); } }

        [Instruction("BIT 6,D", 0x72, Cycles = 8, Prefix="CB")]
        public Expression Bit_6_D { get { return Bit(_cpuValueExpressions.RegisterD, 6); } }

        [Instruction("BIT 6,E", 0x73, Cycles = 8, Prefix="CB")]
        public Expression Bit_6_E { get { return Bit(_cpuValueExpressions.RegisterE, 6); } }

        [Instruction("BIT 6,H", 0x74, Cycles = 8, Prefix="CB")]
        public Expression Bit_6_H { get { return Bit(_cpuValueExpressions.RegisterH, 6); } }

        [Instruction("BIT 6,L", 0x75, Cycles = 8, Prefix="CB")]
        public Expression Bit_6_L { get { return Bit(_cpuValueExpressions.RegisterL, 6); } }

        [Instruction("BIT 6,(HL)", 0x76, Cycles = 12, Prefix="CB")]
        public Expression Bit_6_HLi { get { return Bit(_cpuValueExpressions.ReadHL, 6); } }

        [Instruction("BIT 6,A", 0x77, Cycles = 8, Prefix="CB")]
        public Expression Bit_6_A { get { return Bit(_cpuValueExpressions.RegisterA, 6); } }

        // Bit 7
        [Instruction("BIT 7,B", 0x78, Cycles = 8, Prefix="CB")]
        public Expression Bit_7_B { get { return Bit(_cpuValueExpressions.RegisterB, 7); } }

        [Instruction("BIT 7,C", 0x79, Cycles = 8, Prefix="CB")]
        public Expression Bit_7_C { get { return Bit(_cpuValueExpressions.RegisterC, 7); } }

        [Instruction("BIT 7,D", 0x7A, Cycles = 8, Prefix="CB")]
        public Expression Bit_7_D { get { return Bit(_cpuValueExpressions.RegisterD, 7); } }

        [Instruction("BIT 7,E", 0x7B, Cycles = 8, Prefix="CB")]
        public Expression Bit_7_E { get { return Bit(_cpuValueExpressions.RegisterE, 7); } }

        [Instruction("BIT 7,H", 0x7C, Cycles = 8, Prefix="CB")]
        public Expression Bit_7_H { get { return Bit(_cpuValueExpressions.RegisterH, 7); } }

        [Instruction("BIT 7,L", 0x7D, Cycles = 8, Prefix="CB")]
        public Expression Bit_7_L { get { return Bit(_cpuValueExpressions.RegisterL, 7); } }

        [Instruction("BIT 7,(HL)", 0x7E, Cycles = 12, Prefix="CB")]
        public Expression Bit_7_HLi { get { return Bit(_cpuValueExpressions.ReadHL, 7); } }

        [Instruction("BIT 7,A", 0x7F, Cycles = 8, Prefix="CB")]
        public Expression Bit_7_A { get { return Bit(_cpuValueExpressions.RegisterA, 7); } }

        // IX+d instructions
        [Instruction("BIT 0,(IX+d)", 0x46, Cycles = 12, Prefix = "DDCB")]
        public Expression Bit_0_IXdi { get { return Bit(_cpuValueExpressions.ReadIXd(_programControlExpressions.ParameterByte1), 0); } }

        [Instruction("BIT 1,(IX+d)", 0x4E, Cycles = 12, Prefix = "DDCB")]
        public Expression Bit_1_IXdi { get { return Bit(_cpuValueExpressions.ReadIXd(_programControlExpressions.ParameterByte1), 1); } }

        [Instruction("BIT 2,(IX+d)", 0x56, Cycles = 12, Prefix = "DDCB")]
        public Expression Bit_2_IXdi { get { return Bit(_cpuValueExpressions.ReadIXd(_programControlExpressions.ParameterByte1), 2); } }

        [Instruction("BIT 3,(IX+d)", 0x5E, Cycles = 12, Prefix = "DDCB")]
        public Expression Bit_3_IXdi { get { return Bit(_cpuValueExpressions.ReadIXd(_programControlExpressions.ParameterByte1), 3); } }

        [Instruction("BIT 4,(IX+d)", 0x66, Cycles = 12, Prefix = "DDCB")]
        public Expression Bit_4_IXdi { get { return Bit(_cpuValueExpressions.ReadIXd(_programControlExpressions.ParameterByte1), 4); } }

        [Instruction("BIT 5,(IX+d)", 0x6E, Cycles = 12, Prefix = "DDCB")]
        public Expression Bit_5_IXdi { get { return Bit(_cpuValueExpressions.ReadIXd(_programControlExpressions.ParameterByte1), 5); } }

        [Instruction("BIT 6,(IX+d)", 0x76, Cycles = 12, Prefix = "DDCB")]
        public Expression Bit_6_IXdi { get { return Bit(_cpuValueExpressions.ReadIXd(_programControlExpressions.ParameterByte1), 6); } }

        [Instruction("BIT 7,(IX+d)", 0x7E, Cycles = 12, Prefix = "DDCB")]
        public Expression Bit_7_IXdi { get { return Bit(_cpuValueExpressions.ReadIXd(_programControlExpressions.ParameterByte1), 7); } }

        // IY+d instructions
        [Instruction("BIT 0,(IY+d)", 0x46, Cycles = 12, Prefix = "FDCB")]
        public Expression Bit_0_IYdi { get { return Bit(_cpuValueExpressions.ReadIYd(_programControlExpressions.ParameterByte1), 0); } }

        [Instruction("BIT 1,(IY+d)", 0x4E, Cycles = 12, Prefix = "FDCB")]
        public Expression Bit_1_IYdi { get { return Bit(_cpuValueExpressions.ReadIYd(_programControlExpressions.ParameterByte1), 1); } }

        [Instruction("BIT 2,(IY+d)", 0x56, Cycles = 12, Prefix = "FDCB")]
        public Expression Bit_2_IYdi { get { return Bit(_cpuValueExpressions.ReadIYd(_programControlExpressions.ParameterByte1), 2); } }

        [Instruction("BIT 3,(IY+d)", 0x5E, Cycles = 12, Prefix = "FDCB")]
        public Expression Bit_3_IYdi { get { return Bit(_cpuValueExpressions.ReadIYd(_programControlExpressions.ParameterByte1), 3); } }

        [Instruction("BIT 4,(IY+d)", 0x66, Cycles = 12, Prefix = "FDCB")]
        public Expression Bit_4_IYdi { get { return Bit(_cpuValueExpressions.ReadIYd(_programControlExpressions.ParameterByte1), 4); } }

        [Instruction("BIT 5,(IY+d)", 0x6E, Cycles = 12, Prefix = "FDCB")]
        public Expression Bit_5_IYdi { get { return Bit(_cpuValueExpressions.ReadIYd(_programControlExpressions.ParameterByte1), 5); } }

        [Instruction("BIT 6,(IY+d)", 0x76, Cycles = 12, Prefix = "FDCB")]
        public Expression Bit_6_IYdi { get { return Bit(_cpuValueExpressions.ReadIYd(_programControlExpressions.ParameterByte1), 6); } }

        [Instruction("BIT 7,(IY+d)", 0x7E, Cycles = 12, Prefix = "FDCB")]
        public Expression Bit_7_IYdi { get { return Bit(_cpuValueExpressions.ReadIYd(_programControlExpressions.ParameterByte1), 7); } }

    }
}
