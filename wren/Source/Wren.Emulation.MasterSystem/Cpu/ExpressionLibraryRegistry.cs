using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wren.Emulation.MasterSystem.Exceptions;

namespace Wren.Emulation.MasterSystem
{
    public class ExpressionLibraryRegistry : IExpressionLibraryRegistry
    {
        IList<IExpressionLibrary> _libraries;

        public ExpressionLibraryRegistry()
        {
            _libraries = new List<IExpressionLibrary>();
        }

        public IEnumerable<IExpressionLibrary> GetLibraries()
        {
            return _libraries.AsEnumerable();
        }

        public void RegisterLibrary(IExpressionLibrary library)
        {
            if (library == null)
                throw new ArgumentNullException("library");

            _libraries.Add(library);
        }

        public TLibrary GetLibrary<TLibrary>()
            where TLibrary : class, IExpressionLibrary
        {
            return (TLibrary)GetLibrary(typeof(TLibrary));
        }

        public IExpressionLibrary GetLibrary(Type type)
        {
            foreach (var lib in _libraries)
            {
                if (type.IsAssignableFrom(lib.GetType()))
                    return lib;
            }

            throw new LibraryNotRegisteredException(String.Format("No library implementing {0} was found.", type));
        }
    }
}
