using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using Wren.Emulation.MasterSystem.ExpressionLibraries;
using System.Reflection;
using Wren.Emulation.MasterSystem.Exceptions;

namespace Wren.Emulation.MasterSystem
{
    public class DefaultInstructionSpaceBuilder : IInstructionSpaceBuilder
    {
        ConstructorInfo _unknownInstruction;

        public DefaultInstructionSpaceBuilder()
        {
            _unknownInstruction = typeof(UnknownInstructionException).GetConstructor(new Type[] { typeof(Int32) });
        }

        public Expression BuildInstructionSpaceExpression(IEnumerable<InstructionInfo> instructions,
                                                          IProgramControlExpressionLibrary programControlExpressionLibrary,
                                                          IInstructionExpressionBuilder instructionBuilder,
                                                          InstructionSpace instructionSpace)
        {
            if (instructionSpace.Prefix == null)
                return BuildSwitch(instructions, programControlExpressionLibrary, instructionBuilder, String.Empty);
            else
                return BuildSwitch(instructions, programControlExpressionLibrary, instructionBuilder, instructionSpace.Prefix);
        }

        private SwitchExpression BuildSwitch(IEnumerable<InstructionInfo> instructions,
                                             IProgramControlExpressionLibrary programControlExpressionLibrary,
                                             IInstructionExpressionBuilder instructionBuilder,
                                             String prefix)
        {
            List<SwitchCase> switchCases = new List<SwitchCase>();

            // Build switch cases for instruction within this prefix space.
            foreach (var info in instructions)
            {
                if ((String.IsNullOrEmpty(prefix) && String.IsNullOrEmpty(info.Prefix)) || info.Prefix == prefix)
                {
                    try
                    {
                        switchCases.Add(
                            Expression.SwitchCase(
                                instructionBuilder.BuildExpression(info),
                                Expression.Constant(info.Opcode)
                            )
                        );
                    }
                    catch (Exception ex)
                    {
                        throw new ApplicationException(String.Format("There was an error trying to build the instruction expression for {0}", info.Mnemonic), ex);
                    }
                }
            }

            // Build switch cases for child instruction spaces.
            foreach (var prefixOpcode in FindPrefixOpcodes(prefix, instructions))
            {
                IInstructionSpaceBuilder instructionSpace;

                if ((prefix + prefixOpcode.ToString("X")) == "DDCB" || (prefix + prefixOpcode.ToString("X")) == "FDCB")
                {
                    instructionSpace = new CBInstructionSpaceBuilder();
                }
                else
                {
                    instructionSpace = new DefaultInstructionSpaceBuilder();
                }

                switchCases.Add(
                    Expression.SwitchCase(
                        instructionSpace.BuildInstructionSpaceExpression(instructions,
                                                programControlExpressionLibrary,
                                                instructionBuilder,
                                                new InstructionSpace(prefix + prefixOpcode.ToString("X"))),
                        Expression.Constant(prefixOpcode)
                    )
                );
            }

            var orderedSwitchCases = switchCases.OrderBy(sc => (Int32)((ConstantExpression)(sc.TestValues.First())).Value);

            return Expression.Switch(programControlExpressionLibrary.ReadAndIncrementProgramCounter,
                Expression.Block(
                    Expression.Throw(Expression.New(_unknownInstruction, Expression.Subtract(programControlExpressionLibrary.ProgramCounterRegister, Expression.Constant(1)))),
                    Expression.Assign(programControlExpressionLibrary.ParameterByte1, programControlExpressionLibrary.ParameterByte1)
                ),
                orderedSwitchCases.ToArray()
            );
        }

        public IEnumerable<Int32> FindPrefixOpcodes(String prefix, IEnumerable<InstructionInfo> instructions)
        {
            List<Int32> prefixOpcodes = new List<Int32>();

            // Build switch cases for prefixes.
            foreach (var info in instructions)
            {
                if (String.IsNullOrEmpty(info.Prefix))
                    continue;

                if (String.IsNullOrEmpty(prefix))
                {
                    var opCode = Int32.Parse(info.Prefix.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
                    if (prefixOpcodes.Contains(opCode))
                        continue;

                    prefixOpcodes.Add(opCode);
                    continue;
                }

                if (info.Prefix.StartsWith(prefix))
                {
                    var remainingPrefix = info.Prefix.Remove(0, prefix.Length);

                    if (!String.IsNullOrEmpty(remainingPrefix))
                    {
                        var opCode = Int32.Parse(remainingPrefix.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);

                        if (prefixOpcodes.Contains(opCode))
                            continue;

                        prefixOpcodes.Add(opCode);
                    }
                }
            }

            return prefixOpcodes;
        }
    }
}
