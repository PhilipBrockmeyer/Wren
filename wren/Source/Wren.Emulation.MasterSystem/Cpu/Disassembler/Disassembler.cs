using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wren.Emulation.MasterSystem.ExpressionLibraries;

namespace Wren.Emulation.MasterSystem.Disassembler
{
    public class Disassembler
    {
        Dictionary<String, List<String>> childInstructionSpaces;
        InstructionInfo[] _instructions;

        public Disassembler()
        {
            var libRegistry = BuildExpressionLibraryRegistry(new ArraySystemBus(1024));
            var instructionScanner = new InstructionScanner(libRegistry);
            _instructions = instructionScanner.BuildInstructionInfo(typeof(InstructionScanner).Assembly.GetTypes());

            childInstructionSpaces = new Dictionary<String, List<String>>();

            AddChildInstructionSpaces(String.Empty);
        }

        private void AddChildInstructionSpaces(String prefix)
        {
            childInstructionSpaces[prefix] = FindPrefixOpcodes(prefix, _instructions);

            foreach (var opcode in childInstructionSpaces[prefix])
            {
                AddChildInstructionSpaces(prefix + opcode);
            }
        }

        public String Disassemble(Byte[] bytes)
        {
            StringBuilder sb = new StringBuilder();
            String prefix = String.Empty;
            String instructionText = String.Empty;
            Int32 opcode = 0;
            Int32 val;
            Int32 val2;
            Int32 instructionAddress = 0;

            for (Int32 i = 0; i < 0x8000; i++)//bytes.Length; i++)
            {
                try
                {
                    instructionAddress = i;
                    prefix = String.Empty;

                    opcode = bytes[i];

                    while (childInstructionSpaces[prefix].Contains(opcode.ToString("X")))
                    {
                        prefix = prefix + opcode.ToString("X");
                        i = i + 1;
                        opcode = bytes[i];
                    }

                    if (prefix == "DDCB" || prefix == "FDCB")
                    {
                        var index = opcode;
                        opcode= bytes[++i];

                        var cbresult = _instructions.Where(
                            instr => instr.Opcode == opcode && instr.Prefix == prefix
                        );

                        if (cbresult.Count() == 0)
                        {
                            sb.AppendLine("[Data]" + prefix + opcode.ToString("X2"));
                            continue;
                        }
                        var cbinstruction = cbresult.First();

                        instructionText = cbinstruction.Mnemonic
                                .Replace("+d", "+" + index.ToString());

                        String cbtext = String.Format("{0}:\t {1}", instructionAddress.ToString("X4"), instructionText);
                        sb.AppendLine(cbtext);
                        continue;
                    }

                    var result = _instructions.Where(
                         instr => instr.Opcode == opcode
                                  && (
                                     instr.Prefix == prefix
                                     || (instr.Prefix == null && prefix == String.Empty)
                                  )
                             );

                    if (result.Count() == 0)
                    {
                        sb.AppendLine("[Data]" + prefix + opcode.ToString("X2"));
                        continue;
                    }

                    var instruction = result.First();

                    switch (instruction.ParameterMode)
                    {
                        case InstructionParameterMode.None:
                            instructionText = instruction.Mnemonic;
                            break;

                        case InstructionParameterMode.Byte:
                            val = bytes[++i];
                            instructionText = instruction.Mnemonic
                                .Replace(",n", "," + val.ToString("X2"))
                                .Replace("(n)", "(" + val.ToString("X2") + ")")
                                .Replace(" n", " " + val.ToString("X2"));
                            break;

                        case InstructionParameterMode.Word:
                            val = bytes[++i] | (bytes[++i] << 8);
                            instructionText = instruction.Mnemonic
                                .Replace(",nn", "," + val.ToString("X4"))
                                .Replace(" nn", " " + val.ToString("X4"))
                                .Replace("(nn)", "(" + val.ToString("X4") + ")");
                            break;

                        case InstructionParameterMode.Index:
                            val = bytes[++i];
                            var signedVal = val - ((val & 128) << 1);
                            
                            instructionText = instruction.Mnemonic
                                .Replace(",d", "," + signedVal.ToString())
                                .Replace("+d", "+" + signedVal.ToString())
                                .Replace(" d", " " + signedVal.ToString());
                            break;

                        case InstructionParameterMode.Address:
                            val = bytes[++i] | (bytes[++i] << 8);
                            instructionText = instruction.Mnemonic
                                .Replace(" nn", " " + val.ToString("X4"))
                                .Replace("(nn)", "(" + val.ToString("X4") + ")")
                                .Replace(",nn", "," + val.ToString("X4"));
                            break;

                        case InstructionParameterMode.IndexAndByte:
                            val = bytes[++i];
                            val2 = bytes[++i];
                            
                            var signedVal2 = val - ((val & 128) << 1);

                            instructionText = instruction.Mnemonic;
                            break;

                        default:
                            break;
                    }

                    String text = String.Format("{0}:\t {1}", instructionAddress.ToString("X4"), instructionText); 
                    sb.AppendLine(text);
                }

                catch (Exception ex)
                {
                    
                }

            }

            return sb.ToString();
        }

        /*
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
                            Expression.Assign(programControlExpressionLibrary.ParameterByte1, programControlExpressionLibrary.ParameterByte1)
                        ),

                        orderedSwitchCases.ToArray()
                    );
                }
                */


        public List<String> FindPrefixOpcodes(String prefix, IEnumerable<InstructionInfo> instructions)
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

            var strings = new List<String>();
            foreach (var i in prefixOpcodes)
            {
                strings.Add(i.ToString("X"));
            }

            return strings;
        }

        private IExpressionLibraryRegistry BuildExpressionLibraryRegistry(ISystemBus systemBus)
        {
            var libRegistry = new ExpressionLibraryRegistry();
            libRegistry.RegisterLibrary(new DataAccessExpressionLibrary(systemBus));
            libRegistry.RegisterLibrary(new FlagLookupValuesExpressionLibrary());
            libRegistry.RegisterLibrary(new TemporaryExpressionLibrary());
            libRegistry.RegisterLibrary(new ProgramControlExpressionLibrary(systemBus));
            libRegistry.RegisterLibrary(new InteruptExpressionLibrary());
            libRegistry.RegisterLibrary(new PrimeRegisterExpressionLibrary());

            return libRegistry;
        }
    }
}
