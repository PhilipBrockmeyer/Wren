using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wren.Core.Persistence
{
    public interface IPersistenceManager
    {
        T Load<T>(String itemName, String providerKey);
        IEnumerable<T> LoadMany<T>(String query, String providerKey);

        void Save(String itemName, String providerKey, Object obj);
        void RegiserPersistenceProvider(String providerKey, Func<IPersistenceProvider> providerFactory);
        void RegiserPersistenceProvider(String providerKey, Func<IPersistenceProvider> providerFactory, Func<Boolean> isApplicable);

        
    }
}