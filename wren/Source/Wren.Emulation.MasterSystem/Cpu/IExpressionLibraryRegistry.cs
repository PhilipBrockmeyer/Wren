using System;
using System.Collections.Generic;
namespace Wren.Emulation.MasterSystem
{
    public interface IExpressionLibraryRegistry
    {
        IEnumerable<IExpressionLibrary> GetLibraries();
        void RegisterLibrary(IExpressionLibrary library);

        TLibrary GetLibrary<TLibrary>()
            where TLibrary : class, IExpressionLibrary;

        IExpressionLibrary GetLibrary(Type interfaceType);
    }
}
