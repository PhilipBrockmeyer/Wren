using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wren.Core.Persistence;
using System.IO;
using System.Xml.Serialization;
using System.Xml;

namespace Wren.Core.Persistence
{
    public class ApplicationDataXmlPersistenceProvider : IPersistenceProvider
    {
        public T Load<T>(String itemName)
        {
            var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Wren");
            path = Path.Combine(path, itemName);

            if (!File.Exists(path))
                return default(T);

            using (var file = File.Open(path, FileMode.OpenOrCreate))
            {
                var xs = new XmlSerializer(typeof(T));
                 return (T)xs.Deserialize(file);
            }
        }

        public void Save(String itemName, Object obj)
        {
            var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Wren");

            System.IO.Directory.CreateDirectory(path);

            path = Path.Combine(path, itemName);            

            using (var file = File.Create(path))
            {
                var xs = new XmlSerializer(obj.GetType());
                xs.Serialize(file, obj);
            }
        }


        public IEnumerable<T> LoadMany<T>(string query)
        {
            throw new NotImplementedException();
        }
    }
}
