using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wren.Emulation.MasterSystem.ExpressionLibraries;
using System.Linq.Expressions;

namespace Wren.Emulation.MasterSystem.Instructions
{
    public class LoadInstruction
    {
        IDataAccessExpressionLibrary _cpuValueExpressions;
        IProgramControlExpressionLibrary _programControlExpressions;

        public LoadInstruction(IDataAccessExpressionLibrary registerExpressions, IProgramControlExpressionLibrary programControlExpressions)
        {
            _cpuValueExpressions = registerExpressions;
            _programControlExpressions = programControlExpressions;
        }

        [Instruction("LD BC,nn", 0x01, Cycles = 10, ParameterMode = InstructionParameterMode.Word)]
        public Expression Ld_BC_nn
        {
            get 
            {
                return Expression.Block(
                    Expression.Assign(_cpuValueExpressions.RegisterB, _programControlExpressions.ParameterByte1),
                    Expression.Assign(_cpuValueExpressions.RegisterC, _programControlExpressions.ParameterByte2)
                );
            }
        }

        [Instruction("LD (BC),A", 0x02, Cycles = 7)]
        public Expression Ld_BCi_A
        {
            get
            {
                return _cpuValueExpressions.WriteByteBC(_cpuValueExpressions.RegisterA);
            }
        }        

        [Instruction("LD B,n", 0x06, Cycles = 7, ParameterMode = InstructionParameterMode.Byte)]
        public Expression Ld_B_n
        {
            get { return Expression.Assign(_cpuValueExpressions.RegisterB, _programControlExpressions.ParameterByte1); }
        }

        [Instruction("LD A,(BC)", 0x0A, Cycles = 7)]
        public Expression Ld_A_BCi
        {
            get { return Expression.Assign(_cpuValueExpressions.RegisterA, _cpuValueExpressions.ReadBC); }
        }

        [Instruction("LD C,n", 0x0E, Cycles = 7, ParameterMode=InstructionParameterMode.Byte)]
        public Expression Ld_C_n
        {
            get { return Expression.Assign(_cpuValueExpressions.RegisterC, _programControlExpressions.ParameterByte1); }
        }

        [Instruction("LD DE,nn", 0x11, Cycles = 10, ParameterMode = InstructionParameterMode.Word)]
        public Expression Ld_DE_nn
        {
            get
            {
                return Expression.Block(
                    Expression.Assign(_cpuValueExpressions.RegisterD, _programControlExpressions.ParameterByte1),
                    Expression.Assign(_cpuValueExpressions.RegisterE, _programControlExpressions.ParameterByte2)
                );
            }
        }

        [Instruction("LD (DE),A", 0x12, Cycles = 7)]
        public Expression Ld_DEi_A
        {
            get
            {
                return _cpuValueExpressions.WriteByteDE(_cpuValueExpressions.RegisterA);
            }
        }

        [Instruction("LD D,n", 0x16, Cycles = 7, ParameterMode = InstructionParameterMode.Byte)]
        public Expression Ld_D_n
        {
            get { return Expression.Assign(_cpuValueExpressions.RegisterD, _programControlExpressions.ParameterByte1); }
        }

        [Instruction("LD A,(DE)", 0x1A, Cycles = 7)]
        public Expression Ld_A_DEi
        {
            get { return Expression.Assign(_cpuValueExpressions.RegisterA, _cpuValueExpressions.ReadDE); }
        }

        [Instruction("LD E,n", 0x1E, Cycles = 7, ParameterMode = InstructionParameterMode.Byte)]
        public Expression Ld_E_n
        {
            get { return Expression.Assign(_cpuValueExpressions.RegisterE, _programControlExpressions.ParameterByte1); }
        }

        [Instruction("LD HL,nn", 0x21, Cycles = 10, ParameterMode = InstructionParameterMode.Word)]
        public Expression Ld_HL_nn
        {
            get
            {
                return Expression.Block(
                    Expression.Assign(_cpuValueExpressions.RegisterH, _programControlExpressions.ParameterByte1),
                    Expression.Assign(_cpuValueExpressions.RegisterL, _programControlExpressions.ParameterByte2)
                );
            }
        }

        [Instruction("LD (nn),HL", 0x22, Cycles = 16, ParameterMode=InstructionParameterMode.Word)]
        public Expression Ld_nni_HL
        {
            get
            {
                return _cpuValueExpressions.WriteWord(_programControlExpressions.ParameterByte1,
                                                      _programControlExpressions.ParameterByte2,
                                                      _cpuValueExpressions.RegisterH, 
                                                      _cpuValueExpressions.RegisterL);
            }
        }
        
        [Instruction("LD H,n", 0x26, Cycles = 7, ParameterMode = InstructionParameterMode.Byte)]
        public Expression Ld_H_n
        {
            get { return Expression.Assign(_cpuValueExpressions.RegisterH, _programControlExpressions.ParameterByte1); }
        }

        [Instruction("LD HL,(nn)", 0x2A, Cycles = 16, ParameterMode=InstructionParameterMode.Address)]
        public Expression Ld_HL_nni
        {
            get
            {
                return _cpuValueExpressions.ReadWord(_programControlExpressions.ParameterByte1,
                                                     _programControlExpressions.ParameterByte2,
                                                     _cpuValueExpressions.RegisterH,
                                                     _cpuValueExpressions.RegisterL);
            }
        }

        [Instruction("LD L,n", 0x2E, Cycles = 7, ParameterMode = InstructionParameterMode.Byte)]
        public Expression Ld_L_n
        {
            get { return Expression.Assign(_cpuValueExpressions.RegisterL, _programControlExpressions.ParameterByte1); }
        }

        [Instruction("LD SP,nn", 0x31, Cycles = 10, ParameterMode = InstructionParameterMode.Word)]
        public Expression Ld_SP_nn
        {
            get { return Expression.Assign(_cpuValueExpressions.StackPointerRegister, _programControlExpressions.ParameterWord); }
        }

        [Instruction("LD (nn),A", 0x32, Cycles = 13, ParameterMode = InstructionParameterMode.Word)]
        public Expression Ld_nni_A
        {
            get
            {
                return _cpuValueExpressions.WriteByte(_programControlExpressions.ParameterByte1,
                                                      _programControlExpressions.ParameterByte2,
                                                      _cpuValueExpressions.RegisterA);
            }
        }

        [Instruction("LD (HL),n", 0x36, Cycles = 10, ParameterMode=InstructionParameterMode.Byte)]
        public Expression Ld_HLi_n
        {
            get { return _cpuValueExpressions.WriteByteHL(_programControlExpressions.ParameterByte1); }
        }

        [Instruction("LD A,(nn)", 0x3A, Cycles = 13, ParameterMode = InstructionParameterMode.Word)]
        public Expression Ld_A_nni
        {
            get
            {
                return _cpuValueExpressions.ReadByte(_programControlExpressions.ParameterByte1,
                                                     _programControlExpressions.ParameterByte2,
                                                     _cpuValueExpressions.RegisterA);
            }
        }

        [Instruction("LD A,n", 0x3E, Cycles = 7, ParameterMode = InstructionParameterMode.Byte)]
        public Expression Ld_A_n
        {
            get { return Expression.Assign(_cpuValueExpressions.RegisterA, _programControlExpressions.ParameterByte1); }
        }

        #region LD B
        [Instruction("LD B,B", 0x40, Cycles = 4)]
        public Expression Ld_B_B
        {
            get { return Expression.Assign(_cpuValueExpressions.RegisterB, _cpuValueExpressions.RegisterB); }
        }

        [Instruction("LD B,C", 0x41, Cycles = 4)]
        public Expression Ld_B_C
        {
            get { return Expression.Assign(_cpuValueExpressions.RegisterB, _cpuValueExpressions.RegisterC); }
        }

        [Instruction("LD B,D", 0x42, Cycles = 4)]
        public Expression Ld_B_D
        {
            get { return Expression.Assign(_cpuValueExpressions.RegisterB, _cpuValueExpressions.RegisterD); }
        }
        
        [Instruction("LD B,E", 0x43, Cycles = 4)]
        public Expression Ld_B_E
        {
            get { return Expression.Assign(_cpuValueExpressions.RegisterB, _cpuValueExpressions.RegisterE); }
        }

        [Instruction("LD B,H", 0x44, Cycles = 4)]
        public Expression Ld_B_H
        {
            get { return Expression.Assign(_cpuValueExpressions.RegisterB, _cpuValueExpressions.RegisterH); }
        }

        [Instruction("LD B,L", 0x45, Cycles = 4)]
        public Expression Ld_B_L
        {
            get { return Expression.Assign(_cpuValueExpressions.RegisterB, _cpuValueExpressions.RegisterL); }
        }
        
        [Instruction("LD B,(HL)", 0x46, Cycles = 7)]
        public Expression Ld_B_HLi
        {
            get { return Expression.Assign(_cpuValueExpressions.RegisterB, _cpuValueExpressions.ReadHL); }
        }
        
        [Instruction("LD B,A", 0x47, Cycles = 4)]
        public Expression Ld_B_A
        {
            get { return Expression.Assign(_cpuValueExpressions.RegisterB, _cpuValueExpressions.RegisterA); }
        }
        #endregion

        #region LD C
        [Instruction("LD C,B", 0x48, Cycles = 4)]
        public Expression Ld_C_B
        {
            get { return Expression.Assign(_cpuValueExpressions.RegisterC, _cpuValueExpressions.RegisterB); }
        }

        [Instruction("LD C,C", 0x49, Cycles = 4)]
        public Expression Ld_C_C
        {
            get { return Expression.Assign(_cpuValueExpressions.RegisterC, _cpuValueExpressions.RegisterC); }
        }

        [Instruction("LD C,D", 0x4A, Cycles = 4)]
        public Expression Ld_C_D
        {
            get { return Expression.Assign(_cpuValueExpressions.RegisterC, _cpuValueExpressions.RegisterD); }
        }

        [Instruction("LD C,E", 0x4B, Cycles = 4)]
        public Expression Ld_C_E
        {
            get { return Expression.Assign(_cpuValueExpressions.RegisterC, _cpuValueExpressions.RegisterE); }
        }

        [Instruction("LD C,H", 0x4C, Cycles = 4)]
        public Expression Ld_C_H
        {
            get { return Expression.Assign(_cpuValueExpressions.RegisterC, _cpuValueExpressions.RegisterH); }
        }

        [Instruction("LD C,L", 0x4D, Cycles = 4)]
        public Expression Ld_C_L
        {
            get { return Expression.Assign(_cpuValueExpressions.RegisterC, _cpuValueExpressions.RegisterL); }
        }

        [Instruction("LD C,(HL)", 0x4E, Cycles = 7)]
        public Expression Ld_C_HLi
        {
            get { return Expression.Assign(_cpuValueExpressions.RegisterC, _cpuValueExpressions.ReadHL); }
        }

        [Instruction("LD C,A", 0x4F, Cycles = 4)]
        public Expression Ld_C_A
        {
            get { return Expression.Assign(_cpuValueExpressions.RegisterC, _cpuValueExpressions.RegisterA); }
        }
        #endregion

        #region LD D
        [Instruction("LD D,B", 0x50, Cycles = 4)]
        public Expression Ld_D_B
        {
            get { return Expression.Assign(_cpuValueExpressions.RegisterD, _cpuValueExpressions.RegisterB); }
        }

        [Instruction("LD D,C", 0x51, Cycles = 4)]
        public Expression Ld_D_C
        {
            get { return Expression.Assign(_cpuValueExpressions.RegisterD, _cpuValueExpressions.RegisterC); }
        }

        [Instruction("LD D,D", 0x52, Cycles = 4)]
        public Expression Ld_D_D
        {
            get { return Expression.Assign(_cpuValueExpressions.RegisterD, _cpuValueExpressions.RegisterD); }
        }

        [Instruction("LD D,E", 0x53, Cycles = 4)]
        public Expression Ld_D_E
        {
            get { return Expression.Assign(_cpuValueExpressions.RegisterD, _cpuValueExpressions.RegisterE); }
        }

        [Instruction("LD D,H", 0x54, Cycles = 4)]
        public Expression Ld_D_H
        {
            get { return Expression.Assign(_cpuValueExpressions.RegisterD, _cpuValueExpressions.RegisterH); }
        }

        [Instruction("LD D,L", 0x55, Cycles = 4)]
        public Expression Ld_D_L
        {
            get { return Expression.Assign(_cpuValueExpressions.RegisterD, _cpuValueExpressions.RegisterL); }
        }

        [Instruction("LD D,(HL)", 0x56, Cycles = 7)]
        public Expression Ld_D_HLi
        {
            get { return Expression.Assign(_cpuValueExpressions.RegisterD, _cpuValueExpressions.ReadHL); }
        }

        [Instruction("LD D,A", 0x57, Cycles = 4)]
        public Expression Ld_D_A
        {
            get { return Expression.Assign(_cpuValueExpressions.RegisterD, _cpuValueExpressions.RegisterA); }
        }
        #endregion

        #region LD E
        [Instruction("LD E,B", 0x58, Cycles = 4)]
        public Expression Ld_E_B
        {
            get { return Expression.Assign(_cpuValueExpressions.RegisterE, _cpuValueExpressions.RegisterB); }
        }

        [Instruction("LD E,C", 0x59, Cycles = 4)]
        public Expression Ld_E_C
        {
            get { return Expression.Assign(_cpuValueExpressions.RegisterE, _cpuValueExpressions.RegisterC); }
        }

        [Instruction("LD E,D", 0x5A, Cycles = 4)]
        public Expression Ld_E_D
        {
            get { return Expression.Assign(_cpuValueExpressions.RegisterE, _cpuValueExpressions.RegisterD); }
        }

        [Instruction("LD E,E", 0x5B, Cycles = 4)]
        public Expression Ld_E_E
        {
            get { return Expression.Assign(_cpuValueExpressions.RegisterE, _cpuValueExpressions.RegisterE); }
        }

        [Instruction("LD E,H", 0x5C, Cycles = 4)]
        public Expression Ld_E_H
        {
            get { return Expression.Assign(_cpuValueExpressions.RegisterE, _cpuValueExpressions.RegisterH); }
        }

        [Instruction("LD E,L", 0x5D, Cycles = 4)]
        public Expression Ld_E_L
        {
            get { return Expression.Assign(_cpuValueExpressions.RegisterE, _cpuValueExpressions.RegisterL); }
        }

        [Instruction("LD E,(HL)", 0x5E, Cycles = 7)]
        public Expression Ld_E_HLi
        {
            get { return Expression.Assign(_cpuValueExpressions.RegisterE, _cpuValueExpressions.ReadHL); }
        }

        [Instruction("LD E,A", 0x5F, Cycles = 4)]
        public Expression Ld_E_A
        {
            get { return Expression.Assign(_cpuValueExpressions.RegisterE, _cpuValueExpressions.RegisterA); }
        }
        #endregion

        #region LD H
        [Instruction("LD H,B", 0x60, Cycles = 4)]
        public Expression Ld_H_B
        {
            get { return Expression.Assign(_cpuValueExpressions.RegisterH, _cpuValueExpressions.RegisterB); }
        }

        [Instruction("LD H,C", 0x61, Cycles = 4)]
        public Expression Ld_H_C
        {
            get { return Expression.Assign(_cpuValueExpressions.RegisterH, _cpuValueExpressions.RegisterC); }
        }

        [Instruction("LD H,D", 0x62, Cycles = 4)]
        public Expression Ld_H_D
        {
            get { return Expression.Assign(_cpuValueExpressions.RegisterH, _cpuValueExpressions.RegisterD); }
        }

        [Instruction("LD H,E", 0x63, Cycles = 4)]
        public Expression Ld_H_E
        {
            get { return Expression.Assign(_cpuValueExpressions.RegisterH, _cpuValueExpressions.RegisterE); }
        }

        [Instruction("LD H,H", 0x64, Cycles = 4)]
        public Expression Ld_H_H
        {
            get { return Expression.Assign(_cpuValueExpressions.RegisterH, _cpuValueExpressions.RegisterH); }
        }

        [Instruction("LD H,L", 0x65, Cycles = 4)]
        public Expression Ld_H_L
        {
            get { return Expression.Assign(_cpuValueExpressions.RegisterH, _cpuValueExpressions.RegisterL); }
        }

        [Instruction("LD H,(HL)", 0x66, Cycles = 7)]
        public Expression Ld_H_HLi
        {
            get { return Expression.Assign(_cpuValueExpressions.RegisterH, _cpuValueExpressions.ReadHL); }
        }

        [Instruction("LD H,A", 0x67, Cycles = 4)]
        public Expression Ld_H_A
        {
            get { return Expression.Assign(_cpuValueExpressions.RegisterH, _cpuValueExpressions.RegisterA); }
        }
        #endregion

        #region LD L
        [Instruction("LD L,B", 0x68, Cycles = 4)]
        public Expression Ld_L_B
        {
            get { return Expression.Assign(_cpuValueExpressions.RegisterL, _cpuValueExpressions.RegisterB); }
        }

        [Instruction("LD L,C", 0x69, Cycles = 4)]
        public Expression Ld_L_C
        {
            get { return Expression.Assign(_cpuValueExpressions.RegisterL, _cpuValueExpressions.RegisterC); }
        }

        [Instruction("LD L,D", 0x6A, Cycles = 4)]
        public Expression Ld_L_D
        {
            get { return Expression.Assign(_cpuValueExpressions.RegisterL, _cpuValueExpressions.RegisterD); }
        }

        [Instruction("LD L,E", 0x6B, Cycles = 4)]
        public Expression Ld_L_E
        {
            get { return Expression.Assign(_cpuValueExpressions.RegisterL, _cpuValueExpressions.RegisterE); }
        }

        [Instruction("LD L,H", 0x6C, Cycles = 4)]
        public Expression Ld_L_H
        {
            get { return Expression.Assign(_cpuValueExpressions.RegisterL, _cpuValueExpressions.RegisterH); }
        }

        [Instruction("LD L,L", 0x6D, Cycles = 4)]
        public Expression Ld_L_L
        {
            get { return Expression.Assign(_cpuValueExpressions.RegisterL, _cpuValueExpressions.RegisterL); }
        }

        [Instruction("LD L,(HL)", 0x6E, Cycles = 7)]
        public Expression Ld_L_HLi
        {
            get { return Expression.Assign(_cpuValueExpressions.RegisterL, _cpuValueExpressions.ReadHL); }
        }

        [Instruction("LD L,A", 0x6F, Cycles = 4)]
        public Expression Ld_L_A
        {
            get { return Expression.Assign(_cpuValueExpressions.RegisterL, _cpuValueExpressions.RegisterA); }
        }
        #endregion

        #region LD (HL)
        [Instruction("LD (HL),B", 0x70, Cycles = 7)]
        public Expression Ld_HLi_B
        {
            get { return _cpuValueExpressions.WriteByteHL(_cpuValueExpressions.RegisterB); }
        }

        [Instruction("LD (HL),C", 0x71, Cycles = 7)]
        public Expression Ld_HLi_C
        {
            get { return _cpuValueExpressions.WriteByteHL(_cpuValueExpressions.RegisterC); }
        }

        [Instruction("LD (HL),D", 0x72, Cycles = 7)]
        public Expression Ld_HLi_D
        {
            get { return _cpuValueExpressions.WriteByteHL(_cpuValueExpressions.RegisterD); }
        }

        [Instruction("LD (HL),E", 0x73, Cycles = 7)]
        public Expression Ld_HLi_E
        {
            get { return _cpuValueExpressions.WriteByteHL(_cpuValueExpressions.RegisterE); }
        }

        [Instruction("LD (HL),H", 0x74, Cycles = 7)]
        public Expression Ld_HLi_H
        {
            get { return _cpuValueExpressions.WriteByteHL(_cpuValueExpressions.RegisterH); }
        }

        [Instruction("LD (HL),L", 0x75, Cycles = 7)]
        public Expression Ld_HLi_L
        {
            get { return _cpuValueExpressions.WriteByteHL(_cpuValueExpressions.RegisterL); }
        }

        [Instruction("LD (HL),A", 0x77, Cycles = 7)]
        public Expression Ld_HLi_A
        {
            get { return _cpuValueExpressions.WriteByteHL(_cpuValueExpressions.RegisterA); }
        }


        #endregion

        #region LD A
        [Instruction("LD A,B", 0x78, Cycles = 4)]
        public Expression Ld_A_B
        {
            get { return Expression.Assign(_cpuValueExpressions.RegisterA, _cpuValueExpressions.RegisterB); }
        }

        [Instruction("LD A,C", 0x79, Cycles = 4)]
        public Expression Ld_A_C
        {
            get { return Expression.Assign(_cpuValueExpressions.RegisterA, _cpuValueExpressions.RegisterC); }
        }

        [Instruction("LD A,D", 0x7A, Cycles = 4)]
        public Expression Ld_A_D
        {
            get { return Expression.Assign(_cpuValueExpressions.RegisterA, _cpuValueExpressions.RegisterD); }
        }

        [Instruction("LD A,E", 0x7B, Cycles = 4)]
        public Expression Ld_A_E
        {
            get { return Expression.Assign(_cpuValueExpressions.RegisterA, _cpuValueExpressions.RegisterE); }
        }

        [Instruction("LD A,H", 0x7C, Cycles = 4)]
        public Expression Ld_A_H
        {
            get { return Expression.Assign(_cpuValueExpressions.RegisterA, _cpuValueExpressions.RegisterH); }
        }

        [Instruction("LD A,L", 0x7D, Cycles = 4)]
        public Expression Ld_A_L
        {
            get { return Expression.Assign(_cpuValueExpressions.RegisterA, _cpuValueExpressions.RegisterL); }
        }

        [Instruction("LD A,(HL)", 0x7E, Cycles=7)]
        public Expression Ld_A_HLi
        {
            get { return Expression.Assign(_cpuValueExpressions.RegisterA, _cpuValueExpressions.ReadHL); }
        }

        [Instruction("LD A,A", 0x7F, Cycles = 4)]
        public Expression Ld_A_A
        {
            get { return Expression.Assign(_cpuValueExpressions.RegisterA, _cpuValueExpressions.RegisterA); }
        }
        #endregion

        [Instruction("LD SP,HL", 0xF9, Cycles = 6)]
        public Expression Ld_BC_HL
        {
            get
            {
                return Expression.Assign(_cpuValueExpressions.StackPointerRegister, 
                           Expression.Or(
                                _cpuValueExpressions.RegisterL,
                                Expression.LeftShift(_cpuValueExpressions.RegisterH, Expression.Constant(0x08))
                           )                        
                       );
            }
        }

        [Instruction("LD IX,nn", 0x21, Prefix="DD", Cycles = 14, ParameterMode=InstructionParameterMode.Word)]
        public Expression Ld_IX_nn
        {
            get { return Expression.Assign(_cpuValueExpressions.RegisterIX, _programControlExpressions.ParameterWord); }
        }

        [Instruction("LD IY,nn", 0x21, Prefix="FD", Cycles = 14, ParameterMode = InstructionParameterMode.Word)]
        public Expression Ld_IY_nn
        {
            get { return Expression.Assign(_cpuValueExpressions.RegisterIY, _programControlExpressions.ParameterWord); }
        }

        [Instruction("LD (nn),IX", 0x22, Prefix = "DD", Cycles = 20, ParameterMode = InstructionParameterMode.Address)]
        public Expression Ld_nni_IX
        {
            get { return _cpuValueExpressions.WriteWord(_programControlExpressions.ParameterWord, _cpuValueExpressions.RegisterIX); }
        }

        [Instruction("LD (nn),IY", 0x22, Prefix = "FD", Cycles = 20, ParameterMode = InstructionParameterMode.Address)]
        public Expression Ld_nni_IY
        {
            get { return _cpuValueExpressions.WriteWord(_programControlExpressions.ParameterWord, _cpuValueExpressions.RegisterIY); }
        }

        [Instruction("LD IX,(nn)", 0x2A, Prefix = "DD", Cycles = 20, ParameterMode = InstructionParameterMode.Address)]
        public Expression Ld_IX_nni
        {
            get { return _cpuValueExpressions.ReadWord(_programControlExpressions.ParameterWord, _cpuValueExpressions.RegisterIX); }
        }

        [Instruction("LD IY,(nn)", 0x2A, Prefix = "FD", Cycles = 20, ParameterMode = InstructionParameterMode.Address)]
        public Expression Ld_IY_nni
        {
            get { return _cpuValueExpressions.ReadWord(_programControlExpressions.ParameterWord, _cpuValueExpressions.RegisterIY); }
        }

        [Instruction("LD (IX+d),n", 0x36, Prefix = "DD", Cycles = 19, ParameterMode = InstructionParameterMode.IndexAndByte)]
        public Expression Ld_IXdi_n
        {
            get { return _cpuValueExpressions.WriteByte(
                    Expression.Add(_cpuValueExpressions.RegisterIX, _programControlExpressions.ParameterByte1),
                    _programControlExpressions.ParameterByte2); }
        }

        [Instruction("LD (IY+d),n", 0x36, Prefix = "FD", Cycles = 19, ParameterMode = InstructionParameterMode.IndexAndByte)]
        public Expression Ld_IYdi_n
        {
            get
            {
                return _cpuValueExpressions.WriteByte(
                  Expression.Add(_cpuValueExpressions.RegisterIY, _programControlExpressions.ParameterByte1),
                  _programControlExpressions.ParameterByte2);
            }
        }

        #region LD B IXY
        [Instruction("LD B,IXH", 0x44, Prefix = "DD", Cycles = 9)]
        public Expression Ld_B_IXH
        {
            get { return Expression.Assign(_cpuValueExpressions.RegisterB,
                        Expression.RightShift(_cpuValueExpressions.RegisterIX, Expression.Constant(0x08))); }
        }

        [Instruction("LD B,IXL", 0x45, Prefix = "DD", Cycles = 9)]
        public Expression Ld_B_IXL
        {
            get
            {
                return Expression.Assign(_cpuValueExpressions.RegisterB,
                      Expression.And(_cpuValueExpressions.RegisterIX, Expression.Constant(0x08)));
            }
        }

        [Instruction("LD B,(IX+d)", 0x46, Prefix = "DD", Cycles = 11, ParameterMode = InstructionParameterMode.Index)]
        public Expression Ld_B_IXdi
        {
            get
            {
                return _cpuValueExpressions.ReadByte(Expression.Add(_cpuValueExpressions.RegisterIX, _programControlExpressions.ParameterByte1), _cpuValueExpressions.RegisterB);
            }
        }

        [Instruction("LD B,IYH", 0x44, Prefix = "FD", Cycles = 9)]
        public Expression Ld_B_IYH
        {
            get
            {
                return Expression.Assign(_cpuValueExpressions.RegisterB,
                      Expression.RightShift(_cpuValueExpressions.RegisterIY, Expression.Constant(0x08)));
            }
        }

        [Instruction("LD B,IYL", 0x45, Prefix = "FD", Cycles = 9)]
        public Expression Ld_B_IYL
        {
            get
            {
                return Expression.Assign(_cpuValueExpressions.RegisterB,
                      Expression.And(_cpuValueExpressions.RegisterIY, Expression.Constant(0x08)));
            }
        }

        [Instruction("LD B,(IY+d)", 0x46, Prefix = "FD", Cycles = 11, ParameterMode = InstructionParameterMode.Index)]
        public Expression Ld_B_IYdi
        {
            get
            {
                 return _cpuValueExpressions.ReadByte(Expression.Add(_cpuValueExpressions.RegisterIY, _programControlExpressions.ParameterByte1), _cpuValueExpressions.RegisterB);
            }
        }
        #endregion

        #region LD C IXY
        [Instruction("LD C,IXH", 0x4C, Prefix = "DD", Cycles = 9)]
        public Expression Ld_C_IXH
        {
            get
            {
                return Expression.Assign(_cpuValueExpressions.RegisterC,
                      Expression.RightShift(_cpuValueExpressions.RegisterIX, Expression.Constant(0x08)));
            }
        }

        [Instruction("LD C,IXL", 0x4D, Prefix = "DD", Cycles = 9)]
        public Expression Ld_C_IXL
        {
            get
            {
                return Expression.Assign(_cpuValueExpressions.RegisterC,
                      Expression.And(_cpuValueExpressions.RegisterIX, Expression.Constant(0x08)));
            }
        }

        [Instruction("LD C,(IX+d)", 0x4E, Prefix = "DD", Cycles = 11, ParameterMode = InstructionParameterMode.Index)]
        public Expression Ld_C_IXdi
        {
            get
            {
                return _cpuValueExpressions.ReadByte(Expression.Add(_cpuValueExpressions.RegisterIX, _programControlExpressions.ParameterByte1), _cpuValueExpressions.RegisterC);
            }
        }

        [Instruction("LD C,IYH", 0x4C, Prefix = "FD", Cycles = 9)]
        public Expression Ld_C_IYH
        {
            get
            {
                return Expression.Assign(_cpuValueExpressions.RegisterC,
                      Expression.RightShift(_cpuValueExpressions.RegisterIY, Expression.Constant(0x08)));
            }
        }

        [Instruction("LD C,IYL", 0x4D, Prefix = "FD", Cycles = 9)]
        public Expression Ld_C_IYL
        {
            get
            {
                return Expression.Assign(_cpuValueExpressions.RegisterC,
                      Expression.And(_cpuValueExpressions.RegisterIY, Expression.Constant(0x08)));
            }
        }

        [Instruction("LD C,(IY+d)", 0x4E, Prefix = "FD", Cycles = 11, ParameterMode = InstructionParameterMode.Index)]
        public Expression Ld_C_IYdi
        {
            get
            {
                return _cpuValueExpressions.ReadByte(Expression.Add(_cpuValueExpressions.RegisterIY, _programControlExpressions.ParameterByte1), _cpuValueExpressions.RegisterC);
            }
        }
        #endregion

        #region LD D IXY
        [Instruction("LD D,IXH", 0x54, Prefix = "DD", Cycles = 9)]
        public Expression Ld_D_IXH
        {
            get
            {
                return Expression.Assign(_cpuValueExpressions.RegisterD,
                      Expression.RightShift(_cpuValueExpressions.RegisterIX, Expression.Constant(0x08)));
            }
        }

        [Instruction("LD D,IXL", 0x55, Prefix = "DD", Cycles = 9)]
        public Expression Ld_D_IXL
        {
            get
            {
                return Expression.Assign(_cpuValueExpressions.RegisterD,
                      Expression.And(_cpuValueExpressions.RegisterIX, Expression.Constant(0x08)));
            }
        }

        [Instruction("LD D,(IX+d)", 0x56, Prefix = "DD", Cycles = 11, ParameterMode = InstructionParameterMode.Index)]
        public Expression Ld_D_IXdi
        {
            get
            {
                return _cpuValueExpressions.ReadByte(Expression.Add(_cpuValueExpressions.RegisterIX, _programControlExpressions.ParameterByte1), _cpuValueExpressions.RegisterD);
            }
        }

        [Instruction("LD D,IYH", 0x54, Prefix = "FD", Cycles = 9)]
        public Expression Ld_D_IYH
        {
            get
            {
                return Expression.Assign(_cpuValueExpressions.RegisterD,
                      Expression.RightShift(_cpuValueExpressions.RegisterIY, Expression.Constant(0x08)));
            }
        }

        [Instruction("LD D,IYL", 0x55, Prefix = "FD", Cycles = 9)]
        public Expression Ld_D_IYL
        {
            get
            {
                return Expression.Assign(_cpuValueExpressions.RegisterD,
                      Expression.And(_cpuValueExpressions.RegisterIY, Expression.Constant(0x08)));
            }
        }

        [Instruction("LD D,(IY+d)", 0x56, Prefix = "FD", Cycles = 11, ParameterMode = InstructionParameterMode.Index)]
        public Expression Ld_D_IYdi
        {
            get
            {
                return _cpuValueExpressions.ReadByte(Expression.Add(_cpuValueExpressions.RegisterIY, _programControlExpressions.ParameterByte1), _cpuValueExpressions.RegisterD);
            }
        }
        #endregion

        #region LD E IXY
        [Instruction("LD E,IXH", 0x5C, Prefix = "DD", Cycles = 9)]
        public Expression Ld_E_IXH
        {
            get
            {
                return Expression.Assign(_cpuValueExpressions.RegisterE,
                      Expression.RightShift(_cpuValueExpressions.RegisterIX, Expression.Constant(0x08)));
            }
        }

        [Instruction("LD E,IXL", 0x5D, Prefix = "DD", Cycles = 9)]
        public Expression Ld_E_IXL
        {
            get
            {
                return Expression.Assign(_cpuValueExpressions.RegisterE,
                      Expression.And(_cpuValueExpressions.RegisterIX, Expression.Constant(0x08)));
            }
        }

        [Instruction("LD E,(IX+d)", 0x5E, Prefix = "DD", Cycles = 11, ParameterMode = InstructionParameterMode.Index)]
        public Expression Ld_E_IXdi
        {
            get
            {
                return _cpuValueExpressions.ReadByte(Expression.Add(_cpuValueExpressions.RegisterIX, _programControlExpressions.ParameterByte1), _cpuValueExpressions.RegisterE);
            }
        }

        [Instruction("LD E,IYH", 0x5C, Prefix = "FD", Cycles = 9)]
        public Expression Ld_E_IYH
        {
            get
            {
                return Expression.Assign(_cpuValueExpressions.RegisterE,
                      Expression.RightShift(_cpuValueExpressions.RegisterIY, Expression.Constant(0x08)));
            }
        }

        [Instruction("LD E,IYL", 0x5D, Prefix = "FD", Cycles = 9)]
        public Expression Ld_E_IYL
        {
            get
            {
                return Expression.Assign(_cpuValueExpressions.RegisterE,
                      Expression.And(_cpuValueExpressions.RegisterIY, Expression.Constant(0x08)));
            }
        }

        [Instruction("LD E,(IY+d)", 0x5E, Prefix = "FD", Cycles = 11, ParameterMode = InstructionParameterMode.Index)]
        public Expression Ld_E_IYdi
        {
            get
            {
                return _cpuValueExpressions.ReadByte(Expression.Add(_cpuValueExpressions.RegisterIY, _programControlExpressions.ParameterByte1), _cpuValueExpressions.RegisterE);
            }
        }
        #endregion

        #region LD IXYH
        [Instruction("LD IXH,B", 0x60, Prefix = "DD", Cycles = 9)]
        public Expression Ld_IXH_B
        {
            get
            {
                return Expression.Assign(
                        _cpuValueExpressions.RegisterIX,
                        Expression.Or(
                            Expression.And(_cpuValueExpressions.RegisterIX, Expression.Constant(0xFF)),
                            Expression.LeftShift(_cpuValueExpressions.RegisterB, Expression.Constant(0x08))
                        )
                    );
            }
        }

        [Instruction("LD IXH,C", 0x61, Prefix = "DD", Cycles = 9)]
        public Expression Ld_IXH_C
        {
            get
            {
                return Expression.Assign(
                        _cpuValueExpressions.RegisterIX,
                        Expression.Or(
                            Expression.And(_cpuValueExpressions.RegisterIX, Expression.Constant(0xFF)),
                            Expression.LeftShift(_cpuValueExpressions.RegisterC, Expression.Constant(0x08))
                        )
                    );
            }
        }

        [Instruction("LD IXH,D", 0x62, Prefix = "DD", Cycles = 9)]
        public Expression Ld_IXH_D
        {
            get
            {
                return Expression.Assign(
                        _cpuValueExpressions.RegisterIX,
                        Expression.Or(
                            Expression.And(_cpuValueExpressions.RegisterIX, Expression.Constant(0xFF)),
                            Expression.LeftShift(_cpuValueExpressions.RegisterD, Expression.Constant(0x08))
                        )
                    );
            }
        }

        [Instruction("LD IXH,E", 0x63, Prefix = "DD", Cycles = 9)]
        public Expression Ld_IXH_E
        {
            get
            {
                return Expression.Assign(
                        _cpuValueExpressions.RegisterIX,
                        Expression.Or(
                            Expression.And(_cpuValueExpressions.RegisterIX, Expression.Constant(0xFF)),
                            Expression.LeftShift(_cpuValueExpressions.RegisterE, Expression.Constant(0x08))
                        )
                    );
            }
        }

        [Instruction("LD IXH,IXH", 0x64, Prefix = "DD", Cycles = 9)]
        public Expression Ld_IXH_IXH
        {
            get
            {
                return Expression.Assign(
                        _cpuValueExpressions.RegisterIX,
                        Expression.Or(
                            Expression.And(_cpuValueExpressions.RegisterIX, Expression.Constant(0xFF)),
                            Expression.And(_cpuValueExpressions.RegisterIX, Expression.Constant(0xFF00))
                        )
                    );
            }
        }

        [Instruction("LD IXH,IXL", 0x65, Prefix = "DD", Cycles = 9)]
        public Expression Ld_IXH_IXL
        {
            get
            {
                return Expression.Assign(
                        _cpuValueExpressions.RegisterIX,
                        Expression.Or(
                            Expression.And(_cpuValueExpressions.RegisterIX, Expression.Constant(0xFF)),
                            Expression.And(
                                Expression.LeftShift(_cpuValueExpressions.RegisterIX, Expression.Constant(0x08)), 
                                Expression.Constant(0xFF00))
                        )
                    );
            }
        }

        [Instruction("LD H,(IX+d)", 0x66, Prefix = "DD", Cycles = 11, ParameterMode = InstructionParameterMode.Index)]
        public Expression Ld_H_IXdi
        {
            get
            {
                return _cpuValueExpressions.ReadByte(Expression.Add(_cpuValueExpressions.RegisterIX, _programControlExpressions.ParameterByte1), _cpuValueExpressions.RegisterH);
            }
        }

        [Instruction("LD IXH,A", 0x67, Prefix = "DD", Cycles = 9)]
        public Expression Ld_IXH_A
        {
            get
            {
                return Expression.Assign(
                        _cpuValueExpressions.RegisterIX,
                        Expression.Or(
                            Expression.And(_cpuValueExpressions.RegisterIX, Expression.Constant(0xFF)),
                            Expression.LeftShift(_cpuValueExpressions.RegisterA, Expression.Constant(0x08))
                        )
                    );
            }
        }

        [Instruction("LD IYH,B", 0x60, Prefix = "FD", Cycles = 9)]
        public Expression Ld_IYH_B
        {
            get
            {
                return Expression.Assign(
                        _cpuValueExpressions.RegisterIY,
                        Expression.Or(
                            Expression.And(_cpuValueExpressions.RegisterIY, Expression.Constant(0xFF)),
                            Expression.LeftShift(_cpuValueExpressions.RegisterB, Expression.Constant(0x08))
                        )
                    );
            }
        }

        [Instruction("LD IYH,C", 0x61, Prefix = "FD", Cycles = 9)]
        public Expression Ld_IYH_C
        {
            get
            {
                return Expression.Assign(
                        _cpuValueExpressions.RegisterIY,
                        Expression.Or(
                            Expression.And(_cpuValueExpressions.RegisterIY, Expression.Constant(0xFF)),
                            Expression.LeftShift(_cpuValueExpressions.RegisterC, Expression.Constant(0x08))
                        )
                    );
            }
        }

        [Instruction("LD IYH,D", 0x62, Prefix = "FD", Cycles = 9)]
        public Expression Ld_IYH_D
        {
            get
            {
                return Expression.Assign(
                        _cpuValueExpressions.RegisterIY,
                        Expression.Or(
                            Expression.And(_cpuValueExpressions.RegisterIY, Expression.Constant(0xFF)),
                            Expression.LeftShift(_cpuValueExpressions.RegisterD, Expression.Constant(0x08))
                        )
                    );
            }
        }

        [Instruction("LD IYH,E", 0x63, Prefix = "FD", Cycles = 9)]
        public Expression Ld_IYH_E
        {
            get
            {
                return Expression.Assign(
                        _cpuValueExpressions.RegisterIY,
                        Expression.Or(
                            Expression.And(_cpuValueExpressions.RegisterIY, Expression.Constant(0xFF)),
                            Expression.LeftShift(_cpuValueExpressions.RegisterE, Expression.Constant(0x08))
                        )
                    );
            }
        }

        [Instruction("LD IYH,IYH", 0x64, Prefix = "FD", Cycles = 9)]
        public Expression Ld_IYH_IYH
        {
            get
            {
                return Expression.Assign(
                        _cpuValueExpressions.RegisterIY,
                        Expression.Or(
                            Expression.And(_cpuValueExpressions.RegisterIY, Expression.Constant(0xFF)),
                            Expression.And(_cpuValueExpressions.RegisterIY, Expression.Constant(0xFF00))
                        )
                    );
            }
        }

        [Instruction("LD IYH,IYL", 0x65, Prefix = "FD", Cycles = 9)]
        public Expression Ld_IYH_IYL
        {
            get
            {
                return Expression.Assign(
                        _cpuValueExpressions.RegisterIY,
                        Expression.Or(
                            Expression.And(_cpuValueExpressions.RegisterIY, Expression.Constant(0xFF)),
                            Expression.And(
                                Expression.LeftShift(_cpuValueExpressions.RegisterIY, Expression.Constant(0x08)),
                                Expression.Constant(0xFF00))
                        )
                    );
            }
        }

        [Instruction("LD H,(IY+d)", 0x66, Prefix = "FD", Cycles = 11, ParameterMode = InstructionParameterMode.Index)]
        public Expression Ld_H_IYdi
        {
            get
            {
                return _cpuValueExpressions.ReadByte(Expression.Add(_cpuValueExpressions.RegisterIY, _programControlExpressions.ParameterByte1), _cpuValueExpressions.RegisterH);
            }
        }

        [Instruction("LD IYH,A", 0x67, Prefix = "FD", Cycles = 9)]
        public Expression Ld_IYH_A
        {
            get
            {
                return Expression.Assign(
                        _cpuValueExpressions.RegisterIY,
                        Expression.Or(
                            Expression.And(_cpuValueExpressions.RegisterIY, Expression.Constant(0xFF)),
                            Expression.LeftShift(_cpuValueExpressions.RegisterA, Expression.Constant(0x08))
                        )
                    );
            }
        }


        #endregion

        #region LD IXYL
        [Instruction("LD IXL,B", 0x68, Prefix = "DD", Cycles = 9)]
        public Expression Ld_IXL_B
        {
            get
            {
                return Expression.Assign(
                        _cpuValueExpressions.RegisterIX,
                        Expression.Or(
                            Expression.And(_cpuValueExpressions.RegisterIX, Expression.Constant(0xFF00)),
                            _cpuValueExpressions.RegisterB
                        )
                    );
            }
        }

        [Instruction("LD IXL,C", 0x69, Prefix = "DD", Cycles = 9)]
        public Expression Ld_IXL_C
        {
            get
            {
                return Expression.Assign(
                        _cpuValueExpressions.RegisterIX,
                        Expression.Or(
                            Expression.And(_cpuValueExpressions.RegisterIX, Expression.Constant(0xFF00)),
                            _cpuValueExpressions.RegisterC
                        )
                    );
            }
        }

        [Instruction("LD IXL,D", 0x6A, Prefix = "DD", Cycles = 9)]
        public Expression Ld_IXL_D
        {
            get
            {
                return Expression.Assign(
                        _cpuValueExpressions.RegisterIX,
                        Expression.Or(
                            Expression.And(_cpuValueExpressions.RegisterIX, Expression.Constant(0xFF00)),
                            _cpuValueExpressions.RegisterD
                        )
                    );
            }
        }

        [Instruction("LD IXL,E", 0x6B, Prefix = "DD", Cycles = 9)]
        public Expression Ld_IXL_E
        {
            get
            {
                return Expression.Assign(
                        _cpuValueExpressions.RegisterIX,
                        Expression.Or(
                            Expression.And(_cpuValueExpressions.RegisterIX, Expression.Constant(0xFF00)),
                            _cpuValueExpressions.RegisterE
                        )
                    );
            }
        }

        [Instruction("LD IXL,IXH", 0x6C, Prefix = "DD", Cycles = 9)]
        public Expression Ld_IXL_IXH
        {
            get
            {
                return Expression.Assign(
                        _cpuValueExpressions.RegisterIX,
                        Expression.Or(
                            Expression.And(_cpuValueExpressions.RegisterIX, Expression.Constant(0xFF00)),
                            Expression.RightShift(_cpuValueExpressions.RegisterIX, Expression.Constant(0x08))
                        )
                    );
            }
        }

        [Instruction("LD IXL,IXL", 0x6D, Prefix = "DD", Cycles = 9)]
        public Expression Ld_IXL_IXL
        {
            get { return Expression.Assign(_cpuValueExpressions.RegisterIX,_cpuValueExpressions.RegisterIX); }
        }

        [Instruction("LD L,(IX+d)", 0x6E, Prefix = "DD", Cycles = 11, ParameterMode = InstructionParameterMode.Index)]
        public Expression Ld_L_IXdi
        {
            get
            {
                return _cpuValueExpressions.ReadByte(Expression.Add(_cpuValueExpressions.RegisterIX, _programControlExpressions.ParameterByte1), _cpuValueExpressions.RegisterL);
            }
        }

        [Instruction("LD IXL,A", 0x6F, Prefix = "DD", Cycles = 9)]
        public Expression Ld_IXL_A
        {
            get
            {
                return Expression.Assign(
                        _cpuValueExpressions.RegisterIX,
                        Expression.Or(
                            Expression.And(_cpuValueExpressions.RegisterIX, Expression.Constant(0xFF00)),
                            _cpuValueExpressions.RegisterA
                        )
                    );
            }
        }

        [Instruction("LD IYL,B", 0x68, Prefix = "FD", Cycles = 9)]
        public Expression Ld_IYL_B
        {
            get
            {
                return Expression.Assign(
                        _cpuValueExpressions.RegisterIY,
                        Expression.Or(
                            Expression.And(_cpuValueExpressions.RegisterIY, Expression.Constant(0xFF00)),
                            _cpuValueExpressions.RegisterB
                        )
                    );
            }
        }

        [Instruction("LD IYL,C", 0x69, Prefix = "FD", Cycles = 9)]
        public Expression Ld_IYL_C
        {
            get
            {
                return Expression.Assign(
                        _cpuValueExpressions.RegisterIY,
                        Expression.Or(
                            Expression.And(_cpuValueExpressions.RegisterIY, Expression.Constant(0xFF00)),
                            _cpuValueExpressions.RegisterC
                        )
                    );
            }
        }

        [Instruction("LD IYL,D", 0x6A, Prefix = "FD", Cycles = 9)]
        public Expression Ld_IYL_D
        {
            get
            {
                return Expression.Assign(
                        _cpuValueExpressions.RegisterIY,
                        Expression.Or(
                            Expression.And(_cpuValueExpressions.RegisterIY, Expression.Constant(0xFF00)),
                            _cpuValueExpressions.RegisterD
                        )
                    );
            }
        }

        [Instruction("LD IYL,E", 0x6B, Prefix = "FD", Cycles = 9)]
        public Expression Ld_IYL_E
        {
            get
            {
                return Expression.Assign(
                        _cpuValueExpressions.RegisterIY,
                        Expression.Or(
                            Expression.And(_cpuValueExpressions.RegisterIY, Expression.Constant(0xFF00)),
                            _cpuValueExpressions.RegisterE
                        )
                    );
            }
        }

        [Instruction("LD IYL,IYH", 0x6C, Prefix = "FD", Cycles = 9)]
        public Expression Ld_IYL_IYH
        {
            get
            {
                return Expression.Assign(
                        _cpuValueExpressions.RegisterIY,
                        Expression.Or(
                            Expression.And(_cpuValueExpressions.RegisterIY, Expression.Constant(0xFF00)),
                            Expression.RightShift(_cpuValueExpressions.RegisterIY, Expression.Constant(0x08))
                        )
                    );
            }
        }

        [Instruction("LD IYL,IYL", 0x6D, Prefix = "FD", Cycles = 9)]
        public Expression Ld_IYL_IYL
        {
            get { return Expression.Assign(_cpuValueExpressions.RegisterIY, _cpuValueExpressions.RegisterIY); }
        }

        [Instruction("LD L,(IY+d)", 0x6E, Prefix = "FD", Cycles = 11, ParameterMode = InstructionParameterMode.Index)]
        public Expression Ld_L_IYdi
        {
            get
            {
                return _cpuValueExpressions.ReadByte(Expression.Add(_cpuValueExpressions.RegisterIY, _programControlExpressions.ParameterByte1), _cpuValueExpressions.RegisterL);
            }
        }

        [Instruction("LD IYL,A", 0x6F, Prefix = "FD", Cycles = 9)]
        public Expression Ld_IYL_A
        {
            get
            {
                return Expression.Assign(
                        _cpuValueExpressions.RegisterIY,
                        Expression.Or(
                            Expression.And(_cpuValueExpressions.RegisterIY, Expression.Constant(0xFF00)),
                            _cpuValueExpressions.RegisterA
                        )
                    );
            }
        }
        #endregion

        #region LD A IXY
        [Instruction("LD A,IXH", 0x7C, Prefix = "DD", Cycles = 9)]
        public Expression Ld_A_IXH
        {
            get
            {
                return Expression.Assign(_cpuValueExpressions.RegisterA,
                      Expression.RightShift(_cpuValueExpressions.RegisterIX, Expression.Constant(0x08)));
            }
        }

        [Instruction("LD A,IXL", 0x7D, Prefix = "DD", Cycles = 9)]
        public Expression Ld_A_IXL
        {
            get
            {
                return Expression.Assign(_cpuValueExpressions.RegisterA,
                      Expression.And(_cpuValueExpressions.RegisterIX, Expression.Constant(0x08)));
            }
        }

        [Instruction("LD A,(IX+d)", 0x7E, Prefix = "DD", Cycles = 11, ParameterMode = InstructionParameterMode.Index)]
        public Expression Ld_A_IXdi
        {
            get
            {
                return _cpuValueExpressions.ReadByte(Expression.Add(_cpuValueExpressions.RegisterIX, _programControlExpressions.ParameterByte1), _cpuValueExpressions.RegisterA);
            }
        }

        [Instruction("LD A,IYH", 0x7C, Prefix = "FD", Cycles = 9)]
        public Expression Ld_A_IYH
        {
            get
            {
                return Expression.Assign(_cpuValueExpressions.RegisterA,
                      Expression.RightShift(_cpuValueExpressions.RegisterIY, Expression.Constant(0x08)));
            }
        }

        [Instruction("LD A,IYL", 0x7D, Prefix = "FD", Cycles = 9)]
        public Expression Ld_A_IYL
        {
            get
            {
                return Expression.Assign(_cpuValueExpressions.RegisterA,
                      Expression.And(_cpuValueExpressions.RegisterIY, Expression.Constant(0x08)));
            }
        }

        [Instruction("LD A,(IY+d)", 0x7E, Prefix = "FD", Cycles = 11, ParameterMode = InstructionParameterMode.Index)]
        public Expression Ld_A_IYdi
        {
            get
            {
                return _cpuValueExpressions.ReadByte(Expression.Add(_cpuValueExpressions.RegisterIY, _programControlExpressions.ParameterByte1), _cpuValueExpressions.RegisterA);
            }
        }
        #endregion

        [Instruction("LD (IX+d),B", 0x70, Prefix = "DD", Cycles = 11, ParameterMode = InstructionParameterMode.Index)]
        public Expression Ld_IXdi_B
        {
            get
            {
                return _cpuValueExpressions.WriteByte(
                  Expression.Add(_cpuValueExpressions.RegisterIX, _programControlExpressions.ParameterByte1),
                  _cpuValueExpressions.RegisterB);
            }
        }

        [Instruction("LD (IX+d),C", 0x71, Prefix = "DD", Cycles = 11, ParameterMode = InstructionParameterMode.Index)]
        public Expression Ld_IXdi_C
        {
            get
            {
                return _cpuValueExpressions.WriteByte(
                  Expression.Add(_cpuValueExpressions.RegisterIX, _programControlExpressions.ParameterByte1),
                  _cpuValueExpressions.RegisterC);
            }
        }

        [Instruction("LD (IX+d),D", 0x72, Prefix = "DD", Cycles = 11, ParameterMode = InstructionParameterMode.Index)]
        public Expression Ld_IXdi_D
        {
            get
            {
                return _cpuValueExpressions.WriteByte(
                  Expression.Add(_cpuValueExpressions.RegisterIX, _programControlExpressions.ParameterByte1),
                  _cpuValueExpressions.RegisterD);
            }
        }

        [Instruction("LD (IX+d),E", 0x73, Prefix = "DD", Cycles = 11, ParameterMode = InstructionParameterMode.Index)]
        public Expression Ld_IXdi_E
        {
            get
            {
                return _cpuValueExpressions.WriteByte(
                  Expression.Add(_cpuValueExpressions.RegisterIX, _programControlExpressions.ParameterByte1),
                  _cpuValueExpressions.RegisterE);
            }
        }

        [Instruction("LD (IX+d),H", 0x74, Prefix = "DD", Cycles = 11, ParameterMode = InstructionParameterMode.Index)]
        public Expression Ld_IXdi_H
        {
            get
            {
                return _cpuValueExpressions.WriteByte(
                  Expression.Add(_cpuValueExpressions.RegisterIX, _programControlExpressions.ParameterByte1),
                  _cpuValueExpressions.RegisterH);
            }
        }

        [Instruction("LD (IX+d),L", 0x75, Prefix = "DD", Cycles = 11, ParameterMode = InstructionParameterMode.Index)]
        public Expression Ld_IXdi_L
        {
            get
            {
                return _cpuValueExpressions.WriteByte(
                  Expression.Add(_cpuValueExpressions.RegisterIX, _programControlExpressions.ParameterByte1),
                  _cpuValueExpressions.RegisterL);
            }
        }

        [Instruction("LD (IX+d),A", 0x77, Prefix = "DD", Cycles = 11, ParameterMode = InstructionParameterMode.Index)]
        public Expression Ld_IXdi_A
        {
            get
            {
                return _cpuValueExpressions.WriteByte(
                  Expression.Add(_cpuValueExpressions.RegisterIX, _programControlExpressions.ParameterByte1),
                  _cpuValueExpressions.RegisterA);
            }
        }

        [Instruction("LD (IY+d),B", 0x70, Prefix = "FD", Cycles = 11, ParameterMode = InstructionParameterMode.Index)]
        public Expression Ld_IYdi_B
        {
            get
            {
                return _cpuValueExpressions.WriteByte(
                  Expression.Add(_cpuValueExpressions.RegisterIY, _programControlExpressions.ParameterByte1),
                  _cpuValueExpressions.RegisterB);
            }
        }

        [Instruction("LD (IY+d),C", 0x71, Prefix = "FD", Cycles = 11, ParameterMode = InstructionParameterMode.Index)]
        public Expression Ld_IYdi_C
        {
            get
            {
                return _cpuValueExpressions.WriteByte(
                  Expression.Add(_cpuValueExpressions.RegisterIY, _programControlExpressions.ParameterByte1),
                  _cpuValueExpressions.RegisterC);
            }
        }

        [Instruction("LD (IY+d),D", 0x72, Prefix = "FD", Cycles = 11, ParameterMode = InstructionParameterMode.Index)]
        public Expression Ld_IYdi_D
        {
            get
            {
                return _cpuValueExpressions.WriteByte(
                  Expression.Add(_cpuValueExpressions.RegisterIY, _programControlExpressions.ParameterByte1),
                  _cpuValueExpressions.RegisterD);
            }
        }

        [Instruction("LD (IY+d),E", 0x73, Prefix = "FD", Cycles = 11, ParameterMode = InstructionParameterMode.Index)]
        public Expression Ld_IYdi_E
        {
            get
            {
                return _cpuValueExpressions.WriteByte(
                  Expression.Add(_cpuValueExpressions.RegisterIY, _programControlExpressions.ParameterByte1),
                  _cpuValueExpressions.RegisterE);
            }
        }

        [Instruction("LD (IY+d),H", 0x74, Prefix = "FD", Cycles = 11, ParameterMode = InstructionParameterMode.Index)]
        public Expression Ld_IYdi_H
        {
            get
            {
                return _cpuValueExpressions.WriteByte(
                  Expression.Add(_cpuValueExpressions.RegisterIY, _programControlExpressions.ParameterByte1),
                  _cpuValueExpressions.RegisterH);
            }
        }

        [Instruction("LD (IY+d),L", 0x75, Prefix = "FD", Cycles = 11, ParameterMode = InstructionParameterMode.Index)]
        public Expression Ld_IYdi_L
        {
            get
            {
                return _cpuValueExpressions.WriteByte(
                  Expression.Add(_cpuValueExpressions.RegisterIY, _programControlExpressions.ParameterByte1),
                  _cpuValueExpressions.RegisterL);
            }
        }

        [Instruction("LD (IY+d),A", 0x77, Prefix = "FD", Cycles = 11, ParameterMode = InstructionParameterMode.Index)]
        public Expression Ld_IYdi_A
        {
            get
            {
                return _cpuValueExpressions.WriteByte(
                  Expression.Add(_cpuValueExpressions.RegisterIY, _programControlExpressions.ParameterByte1),
                  _cpuValueExpressions.RegisterA);
            }
        }

        [Instruction("LD SP,IX", 0xF9, Prefix = "DD", Cycles = 10)]
        public Expression Ld_SP_IX
        {
            get { return Expression.Assign(_cpuValueExpressions.StackPointerRegister, _cpuValueExpressions.RegisterIX); }
        }

        [Instruction("LD SP,IY", 0xF9, Prefix = "FD", Cycles = 10)]
        public Expression Ld_SP_IY
        {
            get { return Expression.Assign(_cpuValueExpressions.StackPointerRegister, _cpuValueExpressions.RegisterIY); }
        }

        [Instruction("LD (nn),BC", 0x43, Cycles = 20, Prefix="ED", ParameterMode = InstructionParameterMode.Address)]
        public Expression Ld_nni_BC
        {
            get
            {
                return _cpuValueExpressions.WriteWord(_programControlExpressions.ParameterByte1,
                                                      _programControlExpressions.ParameterByte2,
                                                      _cpuValueExpressions.RegisterB,
                                                      _cpuValueExpressions.RegisterC);
            }
        }

        [Instruction("LD BC,(nn)", 0x4B, Cycles = 20, Prefix = "ED", ParameterMode = InstructionParameterMode.Address)]
        public Expression Ld_BC_nni
        {
            get
            {
                return _cpuValueExpressions.ReadWord(_programControlExpressions.ParameterByte1,
                                                     _programControlExpressions.ParameterByte2,
                                                     _cpuValueExpressions.RegisterB,
                                                     _cpuValueExpressions.RegisterC);
            }
        }

        [Instruction("LD (nn),DE", 0x53, Cycles = 20, Prefix = "ED", ParameterMode = InstructionParameterMode.Address)]
        public Expression Ld_nni_DE
        {
            get
            {
                return _cpuValueExpressions.WriteWord(_programControlExpressions.ParameterByte1,
                                                      _programControlExpressions.ParameterByte2,
                                                      _cpuValueExpressions.RegisterD,
                                                      _cpuValueExpressions.RegisterE);
            }
        }

        [Instruction("LD DE,(nn)", 0x5B, Cycles = 20, Prefix = "ED", ParameterMode = InstructionParameterMode.Address)]
        public Expression Ld_DE_nni
        {
            get
            {
                return _cpuValueExpressions.ReadWord(_programControlExpressions.ParameterByte1,
                                                     _programControlExpressions.ParameterByte2,
                                                     _cpuValueExpressions.RegisterD,
                                                     _cpuValueExpressions.RegisterE);
            }
        }

        [Instruction("LD (nn),SP", 0x73, Cycles = 20, Prefix = "ED", ParameterMode = InstructionParameterMode.Address)]
        public Expression Ld_nni_SP
        {
            get
            {
                return _cpuValueExpressions.WriteWord(_programControlExpressions.ParameterWord,
                                                      _cpuValueExpressions.StackPointerRegister);
            }
        }

        [Instruction("LD SP,(nn)", 0x7B, Cycles = 20, Prefix = "ED", ParameterMode = InstructionParameterMode.Address)]
        public Expression Ld_SP_nni
        {
            get
            {
                return _cpuValueExpressions.ReadWord(_programControlExpressions.ParameterWord,
                                                     _cpuValueExpressions.StackPointerRegister);
            }
        }
    }
}
