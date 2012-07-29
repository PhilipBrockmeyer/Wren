using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using Wren.Emulation.MasterSystem.ExpressionLibraries;

namespace Wren.Emulation.MasterSystem
{
    public class CBInstructionSpaceBuilder : IInstructionSpaceBuilder
    {
        public Expression BuildInstructionSpaceExpression(IEnumerable<InstructionInfo> instructions, 
                                                          IProgramControlExpressionLibrary programControlExpressionLibrary, 
                                                          IInstructionExpressionBuilder instructionBuilder, 
                                                          InstructionSpace instructionSpace)
        {
            List<SwitchCase> switchCases = new List<SwitchCase>();

            foreach (var info in instructions)
            {
                if (String.IsNullOrEmpty(info.Prefix))
                    continue;

                if (info.Prefix.StartsWith(instructionSpace.Prefix))
                {
                    switchCases.Add(
                        Expression.SwitchCase(instructionBuilder.BuildExpression(info),
                            Expression.Constant(info.Opcode))
                    );
                }
            }

            return Expression.Block(
                    // Read index parameter.
                    // index - (index & 0x80) << 1     -  Handles negative indexes.
                    Expression.Assign(programControlExpressionLibrary.ParameterByte2, programControlExpressionLibrary.ReadAndIncrementProgramCounter),
                    Expression.Assign(programControlExpressionLibrary.ParameterByte1,
                            Expression.Subtract(
                                programControlExpressionLibrary.ParameterByte2,
                                Expression.LeftShift(
                                    Expression.And(programControlExpressionLibrary.ParameterByte2, Expression.Constant(0x80)), Expression.Constant(0x01)
                                )
                            )
                        ),

                    Expression.Switch(programControlExpressionLibrary.ReadAndIncrementProgramCounter,
                        Expression.Assign(programControlExpressionLibrary.ParameterByte1, programControlExpressionLibrary.ParameterByte1),
                        switchCases.ToArray())
                );                
        }
    }
}
