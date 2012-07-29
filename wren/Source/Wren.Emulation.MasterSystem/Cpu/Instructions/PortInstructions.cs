using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wren.Emulation.MasterSystem.ExpressionLibraries;
using System.Linq.Expressions;

namespace Wren.Emulation.MasterSystem.Instructions
{
    public class PortInstructions
    {
        IDataAccessExpressionLibrary _cpuValueExpressions;
        IFlagLookupValuesExpressionLibrary _flagExpressions;
        ITemporaryExpressionLibrary _temporaryExpressions;
        IProgramControlExpressionLibrary _programExpressions;


        public PortInstructions(IDataAccessExpressionLibrary registerExpressions,
                              IFlagLookupValuesExpressionLibrary flagExpressions,
                              ITemporaryExpressionLibrary temporaryExpressions,
                              IProgramControlExpressionLibrary programExpressions)
        {
            _cpuValueExpressions = registerExpressions;
            _flagExpressions = flagExpressions;
            _temporaryExpressions = temporaryExpressions;
            _programExpressions = programExpressions;
        }

        private Expression In(Expression port, Expression readValue)
        {
            return Expression.Block(
                _cpuValueExpressions.ReadPort(port, readValue),

                Expression.Assign(_cpuValueExpressions.FlagsRegister,
                    Expression.Or(
                        Expression.ArrayIndex(_flagExpressions.SignZeroParityCalcultorFlags, _cpuValueExpressions.RegisterA),
                        Expression.And(_cpuValueExpressions.FlagsRegister, Expression.Constant(Flags.CarryFlag))
                    )
                )
            );
        }

        [Instruction("OUT (n),A", 0xD3, Cycles = 11, ParameterMode=InstructionParameterMode.Byte)]
        public Expression Out_ni_a { get { return _cpuValueExpressions.WritePort(_programExpressions.ParameterByte1, _cpuValueExpressions.RegisterA) ; } }

        [Instruction("IN A,(n)", 0xDB, Cycles = 11, ParameterMode = InstructionParameterMode.Byte)]
        public Expression Out_A_ni { get { return In(_programExpressions.ParameterByte1, _cpuValueExpressions.RegisterA); } }
        
        [Instruction("IN B,(C)", 0x40, Cycles = 12, Prefix="ED")]
        public Expression In_B_Ci { get { return In(_cpuValueExpressions.RegisterC, _cpuValueExpressions.RegisterB); } }

        [Instruction("IN C,(C)", 0x48, Cycles = 12, Prefix = "ED")]
        public Expression In_C_Ci { get { return In(_cpuValueExpressions.RegisterC, _cpuValueExpressions.RegisterC); } }

        [Instruction("IN D,(C)", 0x50, Cycles = 12, Prefix = "ED")]
        public Expression In_D_Ci { get { return In(_cpuValueExpressions.RegisterC, _cpuValueExpressions.RegisterD); } }

        [Instruction("IN E,(C)", 0x58, Cycles = 12, Prefix = "ED")]
        public Expression In_E_Ci { get { return In(_cpuValueExpressions.RegisterC, _cpuValueExpressions.RegisterE); } }

        [Instruction("IN H,(C)", 0x60, Cycles = 12, Prefix = "ED")]
        public Expression In_H_Ci { get { return In(_cpuValueExpressions.RegisterC, _cpuValueExpressions.RegisterH); } }

        [Instruction("IN L,(C)", 0x68, Cycles = 12, Prefix = "ED")]
        public Expression In_L_Ci { get { return In(_cpuValueExpressions.RegisterC, _cpuValueExpressions.RegisterL); } }

        [Instruction("IN A,(C)", 0x78, Cycles = 12, Prefix = "ED")]
        public Expression In_A_Ci { get { return In(_cpuValueExpressions.RegisterC, _cpuValueExpressions.RegisterA); } }
        
        [Instruction("OUT (C),B", 0x41, Cycles = 12, Prefix = "ED")]
        public Expression Out_B_Ci { get { return _cpuValueExpressions.WritePort(_cpuValueExpressions.RegisterC, _cpuValueExpressions.RegisterB); } }

        [Instruction("OUT (C),C", 0x49, Cycles = 12, Prefix = "ED")]
        public Expression Out_C_Ci { get { return _cpuValueExpressions.WritePort(_cpuValueExpressions.RegisterC, _cpuValueExpressions.RegisterC); } }

        [Instruction("OUT (C),D", 0x51, Cycles = 12, Prefix = "ED")]
        public Expression Out_D_Ci { get { return _cpuValueExpressions.WritePort(_cpuValueExpressions.RegisterC, _cpuValueExpressions.RegisterD); } }

        [Instruction("OUT (C),E", 0x59, Cycles = 12, Prefix = "ED")]
        public Expression Out_E_Ci { get { return _cpuValueExpressions.WritePort(_cpuValueExpressions.RegisterC, _cpuValueExpressions.RegisterE); } }

        [Instruction("OUT (C),H", 0x61, Cycles = 12, Prefix = "ED")]
        public Expression Out_H_Ci { get { return _cpuValueExpressions.WritePort(_cpuValueExpressions.RegisterC, _cpuValueExpressions.RegisterH); } }

        [Instruction("OUT (C),L", 0x69, Cycles = 12, Prefix = "ED")]
        public Expression Out_L_Ci { get { return _cpuValueExpressions.WritePort(_cpuValueExpressions.RegisterC, _cpuValueExpressions.RegisterL); } }

        [Instruction("OUT (C),A", 0x79, Cycles = 11, Prefix = "ED")]
        public Expression Out_A_Ci { get { return _cpuValueExpressions.WritePort(_cpuValueExpressions.RegisterC, _cpuValueExpressions.RegisterA); } }

        [Instruction("INI", 0xA2, Cycles = 16, Prefix = "ED")]
        public Expression Ini{ get { return _cpuValueExpressions.ReadPort(_cpuValueExpressions.RegisterC, _cpuValueExpressions.RegisterA); } }

        [Instruction("IND", 0xAA, Cycles = 16, Prefix = "ED")]
        public Expression Ind { get { return _cpuValueExpressions.ReadPort(_cpuValueExpressions.RegisterC, _cpuValueExpressions.RegisterA); } }

        /*[Instruction("INIR", 0xB2, Cycles = 16, Prefix = "ED")]
        public Expression Inir { get { return _cpuValueExpressions.ReadPort(_cpuValueExpressions.RegisterC, _cpuValueExpressions.RegisterA); } }
        
        [Instruction("INDR", 0xBA, Cycles = 16, Prefix = "ED")]
        public Expression Indr { get { return _cpuValueExpressions.ReadPort(_cpuValueExpressions.RegisterC, _cpuValueExpressions.RegisterA); } }*/

        [Instruction("OUTI", 0xA3, Cycles = 16, Prefix = "ED")]
        public Expression Outi
        {
            get
            {
                return Expression.Block(

                    Expression.Assign(_temporaryExpressions.Temp1, Expression.Or(Expression.LeftShift(_cpuValueExpressions.RegisterH, Expression.Constant(0x08)), _cpuValueExpressions.RegisterL)),
                    _cpuValueExpressions.ReadByte(_temporaryExpressions.Temp1, _temporaryExpressions.Temp2),
                    
                    _cpuValueExpressions.WritePort(_cpuValueExpressions.RegisterC, _temporaryExpressions.Temp2),

                    // HL++
                    Expression.PostIncrementAssign(_temporaryExpressions.Temp1),
                    Expression.AndAssign(_temporaryExpressions.Temp1, Expression.Constant(0xFFFF)),
                    Expression.Assign(_cpuValueExpressions.RegisterH, Expression.RightShift(_temporaryExpressions.Temp1, Expression.Constant(0x08))),
                    Expression.Assign(_cpuValueExpressions.RegisterL, Expression.And(_temporaryExpressions.Temp1, Expression.Constant(0xFF))),

                    // BC--
                    Expression.PostDecrementAssign(_cpuValueExpressions.RegisterB),
                    Expression.AndAssign(_cpuValueExpressions.RegisterB, Expression.Constant(0xFF)),

                    Expression.OrAssign(_cpuValueExpressions.FlagsRegister, Expression.Constant(Flags.SubtractionFlag)),

                    // Set Z flags
                    Expression.IfThenElse(Expression.Equal(_cpuValueExpressions.RegisterB, Expression.Constant(0)),
                        Expression.OrAssign(_cpuValueExpressions.FlagsRegister, Expression.Constant(Flags.ZeroFlag)),
                        Expression.AndAssign(_cpuValueExpressions.FlagsRegister, Expression.Constant(0xBF))
                    )

                );
            }
        }

        [Instruction("OUTD", 0xAB, Cycles = 16, Prefix = "ED")]
        public Expression Outd
        {
            get
            {
                return Expression.Block(

                    Expression.Assign(_temporaryExpressions.Temp1, Expression.Or(Expression.LeftShift(_cpuValueExpressions.RegisterH, Expression.Constant(0x08)), _cpuValueExpressions.RegisterL)),
                    _cpuValueExpressions.ReadByte(_temporaryExpressions.Temp1, _temporaryExpressions.Temp2),

                    _cpuValueExpressions.WritePort(_cpuValueExpressions.RegisterC, _temporaryExpressions.Temp2),

                    // HL--
                    Expression.PostDecrementAssign(_temporaryExpressions.Temp1),
                    Expression.AndAssign(_temporaryExpressions.Temp1, Expression.Constant(0xFFFF)),
                    Expression.Assign(_cpuValueExpressions.RegisterH, Expression.RightShift(_temporaryExpressions.Temp1, Expression.Constant(0x08))),
                    Expression.Assign(_cpuValueExpressions.RegisterL, Expression.And(_temporaryExpressions.Temp1, Expression.Constant(0xFF))),

                    // BC--
                    Expression.PostDecrementAssign(_cpuValueExpressions.RegisterB),
                    Expression.AndAssign(_cpuValueExpressions.RegisterB, Expression.Constant(0xFF)),

                    Expression.OrAssign(_cpuValueExpressions.FlagsRegister, Expression.Constant(Flags.SubtractionFlag)),

                    // Set Zero flag
                    Expression.IfThenElse(Expression.Equal(_cpuValueExpressions.RegisterB, Expression.Constant(0)),
                        Expression.OrAssign(_cpuValueExpressions.FlagsRegister, Expression.Constant(Flags.ZeroFlag)),
                        Expression.AndAssign(_cpuValueExpressions.FlagsRegister, Expression.Constant(0xBF))
                    )

                );
            }
        }

        LabelTarget _endCopyOtir = Expression.Label("EndCopy");
        [Instruction("OTIR", 0xB3, Prefix = "ED")]
        public Expression Otir 
        { 
            get 
            { 
                 
                return Expression.Block(
                    
                    // Cycles -= 5 + 16 * B
                    Expression.SubtractAssign(_programExpressions.CycleCounter,
                        Expression.Add(Expression.Constant(5),
                            Expression.Multiply(Expression.Constant(16), _cpuValueExpressions.RegisterB)
                        )
                    ),
                                       
                    // Temp3 = HL
                    Expression.Assign(_temporaryExpressions.Temp1, Expression.Or(Expression.LeftShift(_cpuValueExpressions.RegisterH, Expression.Constant(0x08)), _cpuValueExpressions.RegisterL)),

                    // Z N set
                    Expression.OrAssign(_cpuValueExpressions.FlagsRegister, Expression.Constant(Flags.ZeroFlag | Flags.SubtractionFlag)),

                    // N reset
                    // Expression.AndAssign(_cpuValueExpressions.FlagsRegister, Expression.Constant(0xFD)),
                    
                    Expression.Loop(
                        Expression.Block(
                            Expression.IfThen(Expression.Equal(_cpuValueExpressions.RegisterB, Expression.Constant(0)),
                                Expression.Goto(_endCopyOtir)
                            ),

                            // Write (HL) to port (C)
                            _cpuValueExpressions.ReadByte(_temporaryExpressions.Temp1, _temporaryExpressions.Temp2),
                            _cpuValueExpressions.WritePort(_cpuValueExpressions.RegisterC,  _temporaryExpressions.Temp2),
                            
                            Expression.PreDecrementAssign(_cpuValueExpressions.RegisterB),
                            Expression.PostIncrementAssign(_temporaryExpressions.Temp1)
                        )
                    ),

                    Expression.Label(_endCopyOtir),
                    
                    // HL = Temp1
                    Expression.Assign(_cpuValueExpressions.RegisterH, Expression.RightShift(_temporaryExpressions.Temp1, Expression.Constant(0x08))),
                    Expression.Assign(_cpuValueExpressions.RegisterL, Expression.And(_temporaryExpressions.Temp1, Expression.Constant(0xFF))),

                    
                    Expression.SubtractAssign(_programExpressions.CycleCounter, Expression.Constant(0))
                );
            }
        }


        LabelTarget _endCopyOtdr = Expression.Label("EndCopy");
        [Instruction("OTDR", 0xBB, Prefix = "ED")]
        public Expression Otdr
        {
            get
            {

                return Expression.Block(

                    // Cycles -= 5 + 16 * B
                    Expression.SubtractAssign(_programExpressions.CycleCounter,
                        Expression.Add(Expression.Constant(5),
                            Expression.Multiply(Expression.Constant(16), _cpuValueExpressions.RegisterB)
                        )
                    ),

                    // Temp3 = HL
                    Expression.Assign(_temporaryExpressions.Temp1, Expression.Or(Expression.LeftShift(_cpuValueExpressions.RegisterH, Expression.Constant(0x08)), _cpuValueExpressions.RegisterL)),

                    // Z P set
                    Expression.OrAssign(_cpuValueExpressions.FlagsRegister, Expression.Constant(Flags.ZeroFlag | Flags.ParityFlag)),

                    // N reset
                    Expression.AndAssign(_cpuValueExpressions.FlagsRegister, Expression.Constant(0xFD)),

                    Expression.Loop(
                        Expression.Block(
                            Expression.IfThen(Expression.Equal(_cpuValueExpressions.RegisterB, Expression.Constant(0)),
                                Expression.Goto(_endCopyOtir)
                            ),

                            // Write (HL) to port (C)
                            _cpuValueExpressions.ReadByte(_temporaryExpressions.Temp1, _temporaryExpressions.Temp2),
                            _cpuValueExpressions.WritePort(_cpuValueExpressions.RegisterC, _temporaryExpressions.Temp2),

                            Expression.PreDecrementAssign(_cpuValueExpressions.RegisterB),
                            Expression.PostDecrementAssign(_temporaryExpressions.Temp1)
                        )
                    ),

                    Expression.Label(_endCopyOtir),

                    // HL = Temp1
                    Expression.Assign(_cpuValueExpressions.RegisterH, Expression.RightShift(_temporaryExpressions.Temp1, Expression.Constant(0x08))),
                    Expression.Assign(_cpuValueExpressions.RegisterL, Expression.And(_temporaryExpressions.Temp1, Expression.Constant(0xFF))),


                    Expression.SubtractAssign(_programExpressions.CycleCounter, Expression.Constant(0))
                );
            }
        }
    }
}
