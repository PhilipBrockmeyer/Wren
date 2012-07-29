using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Z80.Assembler;

namespace Wren.Emulation.MasterSystem.Assembler
{
    public class Compiler
    {
        ParseTree _tree;
        SymbolTable _symbols;
        InstructionInfo[] _instructions;
        List<String> _machineCode;

        public Compiler(ParseTree tree)
        {
            _tree = tree;
            _symbols = new SymbolTable();
            var instructionScanner = new InstructionScanner(ExpressionRegistryHelper.Default);
            _instructions = instructionScanner.BuildInstructionInfo(typeof(InstructionInfo).Assembly.GetTypes());
            _machineCode = new List<String>();
        }

        public Byte[] Compile()
        {
            BuildSymbolTable();
            BuildMachineCodeTable();
            ResolveSymbols();

            return new Byte[0];
        }

        private void BuildSymbolTable()
        {
            foreach (var statement in _tree.Statements)
            {
                if (!String.IsNullOrEmpty(statement.Label))
                {
                    _symbols.AddSymbol(statement.Label, Symbol.SymbolType.Word);
                }
            }
        }

        private void BuildMachineCodeTable()
        {
            foreach (var statement in _tree.Statements)
            {
                var instructionText = statement.Mnemonic;
                var literalParms = new List<String>();

                foreach (var parm in statement.Parameters)
                {
                    if (!String.IsNullOrEmpty(parm.Identifier))
                    {
                        String[] DefinedIdentifiers = { "A", "B", "C", "D", "E", "H", "L", "(HL)", "Z", "NZ", "PO", "PE", "(IX)", "(IY)", "NC", "C" };
                        if (DefinedIdentifiers.Contains(parm.Identifier))
                            instructionText += " " + parm.ToString();
                        else
                        {
                            var symbol = _symbols.GetSymbol(parm.Identifier);
                            if (symbol.Type == Symbol.SymbolType.Byte)
                            {
                                instructionText += "n";
                                literalParms.Add(symbol.Representation);
                            }
                            else
                            {
                                instructionText += "nn";
                                literalParms.Add(symbol.Representation);
                                literalParms.Add(symbol.Representation);
                            }
                        }
                    }
                    else
                    {
                        if (parm.NumberSize == 8)
                        {
                            instructionText += "n";
                            literalParms.Add(parm.Number.ToString());
                        }
                        else if (parm.NumberSize == 16)
                        {
                            instructionText += "nn";
                            literalParms.Add((parm.Number & 0xFF00).ToString());
                            literalParms.Add((parm.Number & 0x00FF).ToString());
                        }
                        else
                            throw new ApplicationException();
                    }

                    if (statement.Parameters.Last() != parm)
                        instructionText += ",";
                }

                var instruction = _instructions.Where(i => i.Mnemonic == instructionText).FirstOrDefault();

                if (instruction == null)
                    throw new ApplicationException(String.Format("Instruction {0} was not found.", instructionText));

                if (!String.IsNullOrEmpty(statement.Label))
                {
                    _symbols.SetSymbolValue(statement.Label, _machineCode.Count());
                }

                if (!String.IsNullOrEmpty(instruction.Prefix))
                    _machineCode.Add(instruction.Prefix);

                _machineCode.Add(instruction.Opcode.ToString());

                foreach (var parm in literalParms)
                {
                    _machineCode.Add(parm);
                }
            }
        }

        private void ResolveSymbols()
        {
            throw new NotImplementedException();
        }
    }
}
