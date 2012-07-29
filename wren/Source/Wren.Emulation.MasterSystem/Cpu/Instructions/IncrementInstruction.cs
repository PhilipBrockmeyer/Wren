using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wren.Emulation.MasterSystem.ExpressionLibraries;
using System.Linq.Expressions;

namespace Wren.Emulation.MasterSystem.Instructions
{
    public class IncrementInstruction
    {
        IDataAccessExpressionLibrary _cpuValueExpressions;
        IFlagLookupValuesExpressionLibrary _flagExpressions;
        ITemporaryExpressionLibrary _temporaryExpressions;
        IProgramControlExpressionLibrary _programExpressions;


        public IncrementInstruction(IDataAccessExpressionLibrary registerExpressions,
                              IFlagLookupValuesExpressionLibrary flagExpressions,
                              ITemporaryExpressionLibrary temporaryExpressions,
                              IProgramControlExpressionLibrary programExpressions)
        {
            _cpuValueExpressions = registerExpressions;
            _flagExpressions = flagExpressions;
            _temporaryExpressions = temporaryExpressions;
            _programExpressions = programExpressions;
        }


        private Expression Increment8(Expression value)
        {
            return Expression.Block(
                // Assign flags from precalculated flags array at [value] OR'd with existing carry flag.
                // flags = (flags & 0x01) | flags[value]
                        Expression.AndAssign(_cpuValueExpressions.FlagsRegister, Expression.Constant(0x01)),
                        Expression.OrAssign(_cpuValueExpressions.FlagsRegister,
                                Expression.ArrayIndex(_flagExpressions.IncrementFlagsCalcultorFlags, value)
                        ),

                        // Increment and truncate the result.
                        Expression.PreIncrementAssign(value),
                        Expression.AndAssign(value, Expression.Constant(0xFF, typeof(Int32)))
                    );
        }

        private Expression Increment16(Expression highByte, Expression lowByte)
        {
            return Expression.Block(
                // temp = ((highbyte << 8 | lowbyte) + 1) & 0xFFFF.
                  Expression.Assign(_temporaryExpressions.Temp1,
                      Expression.And(
                            Expression.Increment(
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

        [Instruction("INC BC", 0x03, Cycles = 6)]
        public Expression Inc_BC
        {
            get { return Increment16(_cpuValueExpressions.RegisterB, _cpuValueExpressions.RegisterC); }
        }

        [Instruction("INC B", 0x04, Cycles = 4)]
        public Expression Inc_B
        {
            get { return Increment8(_cpuValueExpressions.RegisterB); }
        }

        [Instruction("INC C", 0x0C, Cycles = 4)]
        public Expression Inc_C
        {
            get { return Increment8(_cpuValueExpressions.RegisterC); }
        }

        [Instruction("INC DE", 0x13, Cycles = 6)]
        public Expression Inc_DE
        {
            get { return Increment16(_cpuValueExpressions.RegisterD, _cpuValueExpressions.RegisterE); }
        }

        [Instruction("INC D", 0x14, Cycles = 4)]
        public Expression Inc_D
        {
            get { return Increment8(_cpuValueExpressions.RegisterD); }
        }

        [Instruction("INC E", 0x1C, Cycles = 4)]
        public Expression Inc_E
        {
            get { return Increment8(_cpuValueExpressions.RegisterE); }
        }

        [Instruction("INC HL", 0x23, Cycles = 6)]
        public Expression Inc_HL
        {
            get { return Increment16(_cpuValueExpressions.RegisterH, _cpuValueExpressions.RegisterL); }
        }

        [Instruction("INC H", 0x24, Cycles = 4)]
        public Expression Inc_H
        {
            get { return Increment8(_cpuValueExpressions.RegisterH); }
        }

        [Instruction("INC L", 0x2C, Cycles = 4)]
        public Expression Inc_L
        {
            get { return Increment8(_cpuValueExpressions.RegisterL); }
        }

        [Instruction("INC SP", 0x33, Cycles = 6)]
        public Expression Inc_SP
        {
            get
            {
                return Expression.Assign(_cpuValueExpressions.StackPointerRegister,
                        Expression.And(
                          Expression.Increment(
                              _cpuValueExpressions.StackPointerRegister
                          ),

                          Expression.Constant(0xFFFF)
                          )
                      );
            }
        }

        [Instruction("INC (HL)", 0x34, Cycles = 11)]
        public Expression Inc_HLi
        {
            get
            {
                return Expression.Block(
                    Expression.Assign(_temporaryExpressions.Temp1, _cpuValueExpressions.ReadHL),
                    Increment8(_temporaryExpressions.Temp1),
                    _cpuValueExpressions.WriteByteHL(_temporaryExpressions.Temp1)
                );
            }
        }

        [Instruction("INC A", 0x3C, Cycles = 4)]
        public Expression Inc_A
        {
            get { return Increment8(_cpuValueExpressions.RegisterA); }
        }

        [Instruction("INC IX", 0x23, Prefix = "DD", Cycles = 10)]
        public Expression Inc_IX
        {
            get
            {
                return Expression.Assign(_cpuValueExpressions.RegisterIX,
                        Expression.And(
                          Expression.Increment(
                              _cpuValueExpressions.RegisterIX
                          ),

                          Expression.Constant(0xFFFF)
                          )
                      );
            }
        }

        [Instruction("INC IY", 0x23, Prefix = "FD", Cycles = 10)]
        public Expression Inc_IY
        {
            get
            {
                return Expression.Assign(_cpuValueExpressions.RegisterIY,
                        Expression.And(
                          Expression.Increment(
                              _cpuValueExpressions.RegisterIY
                          ),

                          Expression.Constant(0xFFFF)
                          )
                      );
            }
        }

        /*[Instruction("INC IXH", 0x24, Prefix = "DD", Cycles = 9)]
        public Expression Inc_IXH
        {
            get
            {
                return Expression.Assign(_cpuValueExpressions.RegisterIX,
                    Expression.Or(
                        Expression.And(_cpuValueExpressions.RegisterIX, Expression.Constant(0x00FF)),
                        Expression.LeftShift(
                            Increment8(Expression.RightShift(_cpuValueExpressions.RegisterIX, Expression.Constant(0x08))),
                            Expression.Constant(0x08)
                        )
                    )
                );
            }
        }

        [Instruction("INC IYH", 0x24, Prefix = "FD", Cycles = 9)]
        public Expression Inc_IYH
        {
            get
            {
                return Expression.Assign(_cpuValueExpressions.RegisterIY,
                    Expression.Or(
                        Expression.And(_cpuValueExpressions.RegisterIY, Expression.Constant(0x00FF)),
                        Expression.LeftShift(
                            Increment8(Expression.RightShift(_cpuValueExpressions.RegisterIY, Expression.Constant(0x08))),
                            Expression.Constant(0x08)
                        )
                    )
                );
            }
        }

        [Instruction("INC IXL", 0x2C, Prefix = "DD", Cycles = 9)]
        public Expression Inc_IXL
        {
            get
            {
                return Expression.Assign(_cpuValueExpressions.RegisterIX,
                    Expression.Or(
                        Expression.And(_cpuValueExpressions.RegisterIX, Expression.Constant(0xFF00)),
                        Expression.And(
                            Increment8(Expression.And(_cpuValueExpressions.RegisterIX, Expression.Constant(0xFF))),
                            Expression.Constant(0xFF)
                        )

                    )
                );
            }
        }

        [Instruction("INC IYL", 0x2C, Prefix = "FD", Cycles = 9)]
        public Expression Inc_IYL
        {
            get
            {
                return Expression.Assign(_cpuValueExpressions.RegisterIY,
                    Expression.Or(
                        Expression.And(_cpuValueExpressions.RegisterIY, Expression.Constant(0xFF00)),
                        Expression.And(
                            Increment8(Expression.And(_cpuValueExpressions.RegisterIY, Expression.Constant(0xFF))),
                            Expression.Constant(0xFF)
                        )

                    )
                );
            }
        }*/


        [Instruction("INC (IX+d)", 0x34, Prefix = "DD", Cycles = 15, ParameterMode = InstructionParameterMode.Index)]
        public Expression Inc_IXdi
        {
            get
            {
                return Expression.Block(
                    _cpuValueExpressions.ReadByte(
                            Expression.Add(_cpuValueExpressions.RegisterIX, _programExpressions.ParameterByte1),
                            _temporaryExpressions.Temp1),

                    Increment8(_temporaryExpressions.Temp1),

                    _cpuValueExpressions.WriteByte(Expression.Add(_cpuValueExpressions.RegisterIX, _programExpressions.ParameterByte1),
                            _temporaryExpressions.Temp1)

                );
            }
        }

        [Instruction("INC (IY+d)", 0x34, Prefix = "FD", Cycles = 15, ParameterMode = InstructionParameterMode.Index)]
        public Expression Inc_IYdi
        {
            get
            {
                return Expression.Block(
                    _cpuValueExpressions.ReadByte(
                            Expression.Add(_cpuValueExpressions.RegisterIY, _programExpressions.ParameterByte1),
                            _temporaryExpressions.Temp1),

                    Increment8(_temporaryExpressions.Temp1),

                    _cpuValueExpressions.WriteByte(Expression.Add(_cpuValueExpressions.RegisterIY, _programExpressions.ParameterByte1),
                            _temporaryExpressions.Temp1)

                );
            }
        }
    }
}
