using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wren.Emulation.MasterSystem.Assembler
{
    public class StatementNode
    {
        public String Label { get; set; }
        public String Mnemonic { get; set; }
        public IList<ParameterNode> Parameters { get; private set; }

        public StatementNode()
        {
            Parameters = new List<ParameterNode>();
        }

        public override string ToString()
        {
            String s = String.Empty;
            if (!String.IsNullOrEmpty(Label))
                s += Label + " : ";

            s += Mnemonic;

            foreach (var parm in Parameters)
            {
                s += " " + parm.ToString();

                if (Parameters.Last() != parm)
                    s += ",";
            }

            return s;
        }
    }
}
