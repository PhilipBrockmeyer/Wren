using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using Wren.Emulation.MasterSystem.ExpressionLibraries;

namespace Wren.Emulation.MasterSystem.Instructions
{
    public class CallInstruction
    {
        IDataAccessExpressionLibrary _cpuValueExpressions;
        ITemporaryExpressionLibrary _tempExpressions;
        IProgramControlExpressionLibrary _programControlExpressions;

        public CallInstruction(IDataAccessExpressionLibrary registerExpressions,
                                 ITemporaryExpressionLibrary temporaryExpressions,
                                 IProgramControlExpressionLibrary programControlExpressions)
        {
            _cpuValueExpressions = registerExpressions;
            _tempExpressions = temporaryExpressions;
            _programControlExpressions = programControlExpressions;
        }

        private Expression ConditionalCall(Expression test, Int32 cyclesWhenReturned, Int32 cyclesWhenNotReturned)
        {
            return Expression.Block(
                Expression.IfThen(
                    test,
                    
                    Expression.Block(
                        _cpuValueExpressions.Push(_programControlExpressions.ProgramCounterRegister),
                        Expression.Assign(_programControlExpressions.ProgramCounterRegister, _programControlExpressions.ParameterWord),
                        Expression.SubtractAssign(_programControlExpressions.CycleCounter, Expression.Constant(cyclesWhenReturned - cyclesWhenNotReturned))
                    )
                ),

                Expression.SubtractAssign(_programControlExpressions.CycleCounter, Expression.Constant(cyclesWhenNotReturned))

            );
        }

        [Instruction("CALL NZ,nn", 0xC4, ParameterMode=InstructionParameterMode.Address)]
        public Expression Call_NZ_nn
        {
            get
            {
                return ConditionalCall(
                        Expression.Equal(
                            Expression.And(_cpuValueExpressions.FlagsRegister, Expression.Constant(Flags.ZeroFlag)), Expression.Constant(0x00)
                        ),
                        17, 
                        10
                    );
            }
        }

        [Instruction("RST 00", 0xC7, Cycles = 11)]
        public Expression Rst_00
        {
            get
            {
                return Expression.Block(
                        _cpuValueExpressions.Push(_programControlExpressions.ProgramCounterRegister),
                        Expression.Assign(_programControlExpressions.ProgramCounterRegister, Expression.Constant(0x00))
                    );
            }
        }

        [Instruction("CALL Z,nn", 0xCC, ParameterMode = InstructionParameterMode.Address)]
        public Expression Call_Z_nn
        {
            get
            {
                return ConditionalCall(
                        Expression.NotEqual(
                            Expression.And(_cpuValueExpressions.FlagsRegister, Expression.Constant(Flags.ZeroFlag)), Expression.Constant(0x00)
                        ),
                        17,
                        10
                    );
            }
        }

        [Instruction("CALL nn", 0xCD, Cycles = 17, ParameterMode = InstructionParameterMode.Address)]
        public Expression Call_nn
        {
            get
            {
                return Expression.Block(
                        _cpuValueExpressions.Push(_programControlExpressions.ProgramCounterRegister),
                        Expression.Assign(_programControlExpressions.ProgramCounterRegister, _programControlExpressions.ParameterWord)
                    );
            }
        }

        [Instruction("RST 08", 0xCF, Cycles = 11)]
        public Expression Rst_08
        {
            get
            {
                return Expression.Block(
                        _cpuValueExpressions.Push(_programControlExpressions.ProgramCounterRegister),
                        Expression.Assign(_programControlExpressions.ProgramCounterRegister, Expression.Constant(0x08))
                    );
            }
        }

        [Instruction("CALL NC,nn", 0xD4, ParameterMode = InstructionParameterMode.Address)]
        public Expression Call_NC_nn
        {
            get
            {
                return ConditionalCall(
                        Expression.Equal(
                            Expression.And(_cpuValueExpressions.FlagsRegister, Expression.Constant(Flags.CarryFlag)), Expression.Constant(0x00)
                        ),
                        17,
                        10
                    );
            }
        }

        [Instruction("RST 10", 0xD7, Cycles = 11)]
        public Expression Rst_10
        {
            get
            {
                return Expression.Block(
                        _cpuValueExpressions.Push(_programControlExpressions.ProgramCounterRegister),
                        Expression.Assign(_programControlExpressions.ProgramCounterRegister, Expression.Constant(0x10))
                    );
            }
        }

        [Instruction("CALL C,nn", 0xDC, ParameterMode = InstructionParameterMode.Address)]
        public Expression Call_C_nn
        {
            get
            {
                return ConditionalCall(
                        Expression.NotEqual(
                            Expression.And(_cpuValueExpressions.FlagsRegister, Expression.Constant(Flags.CarryFlag)), Expression.Constant(0x00)
                        ),
                        17,
                        10
                    );
            }
        }

        [Instruction("RST 18", 0xDF, Cycles = 11)]
        public Expression Rst_18
        {
            get
            {
                return Expression.Block(
                        _cpuValueExpressions.Push(_programControlExpressions.ProgramCounterRegister),
                        Expression.Assign(_programControlExpressions.ProgramCounterRegister, Expression.Constant(0x18))
                    );
            }
        }

        [Instruction("CALL PO,nn", 0xE4, ParameterMode = InstructionParameterMode.Address)]
        public Expression Call_PO_nn
        {
            get
            {
                return ConditionalCall(
                        Expression.Equal(
                            Expression.And(_cpuValueExpressions.FlagsRegister, Expression.Constant(Flags.ParityFlag)), Expression.Constant(0x00)
                        ),
                        17,
                        10
                    );
            }
        }

        [Instruction("RST 20", 0xE7, Cycles = 11)]
        public Expression Rst_20
        {
            get
            {
                return Expression.Block(
                        _cpuValueExpressions.Push(_programControlExpressions.ProgramCounterRegister),
                        Expression.Assign(_programControlExpressions.ProgramCounterRegister, Expression.Constant(0x20))
                    );
            }
        }
        
        [Instruction("CALL PE,nn", 0xEC, ParameterMode = InstructionParameterMode.Address)]
        public Expression Call_PE_nn
        {
            get
            {
                return ConditionalCall(
                        Expression.NotEqual(
                            Expression.And(_cpuValueExpressions.FlagsRegister, Expression.Constant(Flags.ParityFlag)), Expression.Constant(0x00)
                        ),
                        17,
                        10
                    );
            }
        }

        [Instruction("RST 28", 0xEF, Cycles = 11)]
        public Expression Rst_28
        {
            get
            {
                return Expression.Block(
                        _cpuValueExpressions.Push(_programControlExpressions.ProgramCounterRegister),
                        Expression.Assign(_programControlExpressions.ProgramCounterRegister, Expression.Constant(0x28))
                    );
            }
        }

        [Instruction("CALL P,nn", 0xF4, ParameterMode = InstructionParameterMode.Address)]
        public Expression Call_P_nn
        {
            get
            {
                return ConditionalCall(
                        Expression.Equal(
                            Expression.And(_cpuValueExpressions.FlagsRegister, Expression.Constant(Flags.SignFlag)), Expression.Constant(0x00)
                        ),
                        17,
                        10
                    );
            }
        }

        [Instruction("RST 30", 0xF7, Cycles = 11)]
        public Expression Rst_30
        {
            get
            {
                return Expression.Block(
                        _cpuValueExpressions.Push(_programControlExpressions.ProgramCounterRegister),
                        Expression.Assign(_programControlExpressions.ProgramCounterRegister, Expression.Constant(0x30))
                    );
            }
        }

        [Instruction("CALL M,nn", 0xFC, ParameterMode = InstructionParameterMode.Address)]
        public Expression Call_M_nn
        {
            get
            {
                return ConditionalCall(
                        Expression.NotEqual(
                            Expression.And(_cpuValueExpressions.FlagsRegister, Expression.Constant(Flags.SignFlag)), Expression.Constant(0x00)
                        ),
                        17,
                        10
                    );
            }
        }

        LabelTarget breakpointEnd = Expression.Label("breakpointEnd");

        [Instruction("RST 38", 0xFF)]
        public Expression Rst_38_BREAKPOINT
        {
            get
            {
                return Expression.Block(
                    Expression.PostDecrementAssign(_programControlExpressions.ProgramCounterRegister),
                    Expression.Assign(_tempExpressions.Temp1, _programControlExpressions.ProgramCounterRegister),
                    
                    // Handle RST 38.
                    Expression.IfThen(Expression.NotEqual(_programControlExpressions.IsBreakpoint, Expression.Constant(true)),
                        Expression.Block(
                            _cpuValueExpressions.Push(_programControlExpressions.ProgramCounterRegister),
                            Expression.Assign(_programControlExpressions.ProgramCounterRegister, Expression.Constant(0x38)),
                            Expression.SubtractAssign(_programControlExpressions.CycleCounter, Expression.Constant(11)),
                            Expression.Goto(breakpointEnd)
                        )
                    ),

                    _programControlExpressions.BreakpointHandler,

                    // Add cycles to offset the return call that was inserted in the breakpoint handler.
                    Expression.AddAssign(_programControlExpressions.CycleCounter, Expression.Constant(10)),

                    // Point the Program Counter to the start of the next instruction.
                    Expression.AddAssign(_programControlExpressions.ProgramCounterRegister, _programControlExpressions.InstructionSize),

                    // Push the ProgramCounter
                    _cpuValueExpressions.Push(_programControlExpressions.ProgramCounterRegister),

                    Expression.Assign(_programControlExpressions.ProgramCounterRegister, _tempExpressions.Temp1),

                    // Set the program counter to the breakpoint handler address.
                    Expression.Assign(_programControlExpressions.ProgramCounterRegister, _programControlExpressions.InteruptAddress),

                    Expression.Label(breakpointEnd),

                    Expression.SubtractAssign(_programControlExpressions.CycleCounter, Expression.Constant(0))
                );
            }
        }
    }
}
