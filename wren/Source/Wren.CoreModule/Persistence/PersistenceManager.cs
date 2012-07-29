using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wren.Core.Persistence
{
    public class PersistenceManager : IPersistenceManager
    {
        private struct ProviderInfo
        {
            public Func<IPersistenceProvider> ProviderFactory { get; set; }
            public Func<Boolean> ProviderPredicate { get; set; }
        }

        IDictionary<String, List<ProviderInfo>> _providers;

        public PersistenceManager()
        {
            _providers = new Dictionary<String, List<ProviderInfo>>();
        }

        public T Load<T>(String itemName, String providerKey)
        {
            return GetProvider(providerKey).Load<T>(itemName);
        }

        private IPersistenceProvider GetProvider(String providerKey)
        {
            if (!_providers.ContainsKey(providerKey))
                throw new ApplicationException(String.Format("Error loading record.  There was no persistence provider registered with the key '{0}'", providerKey));

            foreach (var p in _providers[providerKey])
            {
                if (p.ProviderPredicate.Invoke())
                {
                    return  p.ProviderFactory.Invoke();
                }
            }           

            throw new ApplicationException(String.Format("No applicaple persistence providers were set for {0}.", providerKey));
        }

        public void Save(String itemName, String providerKey, Object obj)
        {
            var provider = GetProvider(providerKey);
            provider.Save(itemName, obj);
        }

        public void RegiserPersistenceProvider(String providerKey, Func<IPersistenceProvider> providerFactory)
        {
            if (!_providers.ContainsKey(providerKey))
            {
                List<ProviderInfo> list = new List<ProviderInfo>();
                _providers.Add(providerKey, list);
            }

            _providers[providerKey].Add(new ProviderInfo() { ProviderFactory = providerFactory, ProviderPredicate = () => true });
        }

        public void RegiserPersistenceProvider(String providerKey, Func<IPersistenceProvider> providerFactory, Func<Boolean> isApplicable)
        {
            if (!_providers.ContainsKey(providerKey))
            {
                List<ProviderInfo> list = new List<ProviderInfo>();
                _providers.Add(providerKey, list);
            }

            _providers[providerKey].Add(new ProviderInfo() { ProviderFactory = providerFactory, ProviderPredicate = isApplicable });
        }


        public IEnumerable<T> LoadMany<T>(String query, String providerKey)
        {
            return GetProvider(providerKey).LoadMany<T>(query);
        }
    }
}
