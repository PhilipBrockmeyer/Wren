using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wren.Emulation.MasterSystem.Assembler
{
    public class Token
    {
        public Tokens TokenType { get; private set; }
        public Int32 StartIndex { get; private set; }
        public String SourceText { get; private set; }

        public Token(Tokens token, Int32 startIndex, String sourceText)
        {
            TokenType = token;
            StartIndex = startIndex;
            SourceText = sourceText;
        }

        public override string ToString()
        {
            return String.Format("{0} {1} {2}", TokenType.ToString(), SourceText, StartIndex);
        }
    }
}
