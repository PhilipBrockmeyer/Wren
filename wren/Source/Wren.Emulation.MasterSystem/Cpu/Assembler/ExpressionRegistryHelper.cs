using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wren.Emulation.MasterSystem;
using Wren.Emulation.MasterSystem.ExpressionLibraries;

namespace Z80.Assembler
{
    public class ExpressionRegistryHelper
    {
        public static IExpressionLibraryRegistry Default
        {
            get
            {
                var lib = new ExpressionLibraryRegistry();
                lib.RegisterLibrary(new DataAccessExpressionLibrary(new ArraySystemBus(1024)));
                lib.RegisterLibrary(new FlagLookupValuesExpressionLibrary());
                lib.RegisterLibrary(new TemporaryExpressionLibrary());
                lib.RegisterLibrary(new ProgramControlExpressionLibrary(new ArraySystemBus(1024)));
                lib.RegisterLibrary(new InteruptExpressionLibrary());
                lib.RegisterLibrary(new PrimeRegisterExpressionLibrary());

                return lib;
            }
        }
    }
}
