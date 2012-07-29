using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wren.Emulation.MasterSystem.Assembler
{
    public class SymbolTable
    {
        IList<Symbol> _symbols;

        public SymbolTable()
        {
            _symbols = new List<Symbol>();
        }

        public void AddSymbol(String representation, Symbol.SymbolType type)
        {
            if (_symbols.Where(s => s.Representation == representation).Count() > 0)
                throw new ApplicationException(String.Format("Symbol already defined: {0}", representation));

            _symbols.Add(new Symbol() { Representation = representation, Type= type });
        }

        public void SetSymbolValue(String representation, Int32 value)
        {
            var symbol = _symbols.Where(s => s.Representation == representation).FirstOrDefault();

            if (symbol == null)
                throw new ApplicationException(String.Format("Symbol not defined: {0}", representation));

            symbol.Value = value;
        }

        public Symbol GetSymbol(String representation)
        {
            var symbol = _symbols.Where(s => s.Representation == representation).FirstOrDefault();

            if (symbol == null)
                throw new ApplicationException(String.Format("Symbol not defined: {0}", representation));

            return symbol;
        }
    }
}
