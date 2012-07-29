using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wren.Emulation.MasterSystem
{
    public class FlagTables
    {
        List<FlagTable> _flagTables;

        public FlagTables()
        {
            _flagTables = new List<FlagTable>();
        }

        public void ScanForFlagTables(IEnumerable<Type> typesToScan)
        {
            foreach (var t in typesToScan)
            {
                if (typeof(IFlagCalculator).IsAssignableFrom(t) && t.IsClass && !t.IsAbstract)
                {
                    FlagTableLoader loader = new FlagTableLoader(t);
                    var flags = loader.LoadTable();
                    _flagTables.Add(new FlagTable() { FlagCalculatorType = t, FlagValues = flags });
                }
            }
        }

        public Int32[] GetFlags<TFlagCalculator>() where TFlagCalculator : IFlagCalculator
        {
            return _flagTables.Where(f => f.FlagCalculatorType == typeof(TFlagCalculator)).FirstOrDefault().FlagValues;
        }
    }
}
