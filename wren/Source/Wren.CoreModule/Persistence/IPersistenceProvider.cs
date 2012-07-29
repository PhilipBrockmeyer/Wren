using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Wren.Core.Persistence
{
    public interface IPersistenceProvider
    {
        T Load<T>(String itemName);
        IEnumerable<T> LoadMany<T>(String query);

        void Save(String itemName, Object obj);
    }
}
