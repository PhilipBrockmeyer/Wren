using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wren.Emulation.MasterSystem.Assembler
{
    public class ParseTree
    {
        public IList<StatementNode> Statements { get; private set; }

        public ParseTree()
        {
            Statements = new List<StatementNode>();
        }
    }
}
