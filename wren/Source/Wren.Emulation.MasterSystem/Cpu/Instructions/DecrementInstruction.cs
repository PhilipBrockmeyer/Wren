using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wren.Emulation.MasterSystem.ExpressionLibraries;
using System.Linq.Expressions;

namespace Wren.Emulation.MasterSystem.Instructions
{
    public class DecrementInstruction
    {
        IDataAccessExpressionLibrary _cpuValueExpressions;
        IFlagLookupValuesExpressionLibrary _flagExpressions;
        ITemporaryExpressionLibrary _temporaryExpressions;
        IProgramControlExpressionLibrary _programExpressions;


        public DecrementInstruction(IDataAccessExpressionLibrary registerExpressions,
                              IFlagLookupValuesExpressionLibrary flagExpressions,
                              ITemporaryExpressionLibrary temporaryExpressions,
                              IProgramControlExpressionLibrary programExpressions)
        {
            _cpuValueExpressions = registerExpressions;
            _flagExpressions = flagExpressions;
            _temporaryExpressions = temporaryExpressions;
            _programExpressions = programExpressions;
        }


        private Expression Decrement8(Expression value)
        {
            return Expression.Block(
                // Assign flags from precalculated flags array at [value] OR'd with existing carry flag.
                // flags = (flags & 0x01) | flags[value]
                        Expression.AndAssign(_cpuValueExpressions.FlagsRegister, Expression.Constant(0x01)),
                        Expression.OrAssign(_cpuValueExpressions.FlagsRegister,
                                Expression.ArrayIndex(_flagExpressions.DecrementFlagsCalcultorFlags, value)
                        ),

                        // Decrement and truncate the result.
                        Expression.PreDecrementAssign(value),
                        Expression.AndAssign(value, Expression.Constant(0xFF, typeof(Int32)))
                    );
        }

        private Expression Decrement16(Expression highByte, Expression lowByte)
        {
            return Expression.Block(
                // temp = ((highbyte << 8 | lowbyte) - 1) & 0xFFFF.
                  Expression.Assign(_temporaryExpressions.Temp1,
                      Expression.And(
                            Expression.Decrement(
                                Expression.Or(
                                    Expression.LeftShift(highByte, Expression.Constant(0x08)),
                                    lowByte
                                )
                            ),

                            Expression.Constant(0xFFFF)
                        )
                  ),

                  Expression.Assign(highByte,
                      Expression.RightShift(_temporaryExpressions.Temp1, Expression.Constant(0x08))
                  ),

                  Expression.Assign(lowByte,
                      Expression.And(_temporaryExpressions.Temp1, Expression.Constant(0xFF))
                  )
              );
        }

        [Instruction("DEC B", 0x05, Cycles = 4)]
        public Expression Dec_B
        {
            get { return Decrement8(_cpuValueExpressions.RegisterB); }
        }

        [Instruction("DEC BC", 0x0B, Cycles = 6)]
        public Expression Dec_BC
        {
            get { return Decrement16(_cpuValueExpressions.RegisterB, _cpuValueExpressions.RegisterC); }
        }
        
        [Instruction("DEC C", 0x0D, Cycles = 4)]
        public Expression Dec_C
        {
            get { return Decrement8(_cpuValueExpressions.RegisterC); }
        }

        [Instruction("DEC D", 0x15, Cycles = 4)]
        public Expression Dec_D
        {
            get { return Decrement8(_cpuValueExpressions.RegisterD); }
        }

        [Instruction("DEC DE", 0x1B, Cycles = 6)]
        public Expression Dec_DE
        {
            get { return Decrement16(_cpuValueExpressions.RegisterD, _cpuValueExpressions.RegisterE); }
        }
        
        [Instruction("DEC E", 0x1D, Cycles = 4)]
        public Expression Dec_E
        {
            get { return Decrement8(_cpuValueExpressions.RegisterE); }
        }

        [Instruction("DEC H", 0x25, Cycles = 4)]
        public Expression Dec_H
        {
            get { return Decrement8(_cpuValueExpressions.RegisterH); }
        }

        [Instruction("DEC HL", 0x2B, Cycles = 6)]
        public Expression Dec_HL
        {
            get { return Decrement16(_cpuValueExpressions.RegisterH, _cpuValueExpressions.RegisterL); }
        }

        [Instruction("DEC L", 0x2D, Cycles = 4)]
        public Expression Dec_L
        {
            get { return Decrement8(_cpuValueExpressions.RegisterL); }
        }

        [Instruction("DEC (HL)", 0x35, Cycles = 11)]
        public Expression Dec_HLi
        {
            get
            {
                return Expression.Block(
                    Expression.Assign(_temporaryExpressions.Temp1, _cpuValueExpressions.ReadHL),
                    Decrement8(_temporaryExpressions.Temp1),
                    _cpuValueExpressions.WriteByteHL(_temporaryExpressions.Temp1)
                );
            }
        }

        [Instruction("DEC SP", 0x3B, Cycles = 6)]
        public Expression Dec_SP
        {
            get
            {
                return Expression.Assign(_cpuValueExpressions.StackPointerRegister,
                        Expression.And(
                          Expression.Decrement(
                              _cpuValueExpressions.StackPointerRegister
                          ),

                          Expression.Constant(0xFFFF)
                          )
                      );
            }
        }

        [Instruction("DEC A", 0x3D, Cycles = 4)]
        public Expression Dec_A
        {
            get { return Decrement8(_cpuValueExpressions.RegisterA); }
        }

        [Instruction("DEC IX", 0x2B, Prefix = "DD", Cycles = 10)]
        public Expression Decc_IX
        {
            get
            {
                return Expression.Assign(_cpuValueExpressions.RegisterIX,
                        Expression.And(
                          Expression.Decrement(
                              _cpuValueExpressions.RegisterIX
                          ),

                          Expression.Constant(0xFFFF)
                          )
                      );
            }
        }

        [Instruction("DEC IY", 0x2B, Prefix = "FD", Cycles = 10)]
        public Expression Dec_IY
        {
            get
            {
                return Expression.Assign(_cpuValueExpressions.RegisterIY,
                        Expression.And(
                          Expression.Decrement(
                              _cpuValueExpressions.RegisterIY
                          ),

                          Expression.Constant(0xFFFF)
                          )
                      );
            }
        }

        [Instruction("DEC (IX+d)", 0x35, Prefix = "DD", Cycles = 15, ParameterMode = InstructionParameterMode.Index)]
        public Expression Inc_IXdi
        {
            get
            {
                return Expression.Block(
                    _cpuValueExpressions.ReadByte(
                            Expression.Add(_cpuValueExpressions.RegisterIX, _programExpressions.ParameterByte1),
                            _temporaryExpressions.Temp1),

                    Decrement8(_temporaryExpressions.Temp1),

                    _cpuValueExpressions.WriteByte(Expression.Add(_cpuValueExpressions.RegisterIX, _programExpressions.ParameterByte1),
                            _temporaryExpressions.Temp1)

                );
            }
        }

        [Instruction("DEC (IY+d)", 0x35, Prefix = "FD", Cycles = 15, ParameterMode = InstructionParameterMode.Index)]
        public Expression Inc_IYdi
        {
            get
            {
                return Expression.Block(
                    _cpuValueExpressions.ReadByte(
                            Expression.Add(_cpuValueExpressions.RegisterIY, _programExpressions.ParameterByte1),
                            _temporaryExpressions.Temp1),

                    Decrement8(_temporaryExpressions.Temp1),

                    _cpuValueExpressions.WriteByte(Expression.Add(_cpuValueExpressions.RegisterIY, _programExpressions.ParameterByte1),
                            _temporaryExpressions.Temp1)

                );
            }
        }

    }
}
