using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wren.Core.Persistence;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO.Compression;

namespace Wren.Core.Persistence
{
    public class BinaryFilePersistenceProvider : IPersistenceProvider
    {
        public T Load<T>(String itemName)
        {
            if (!File.Exists(itemName))
                return default(T);

            using (var fs = File.OpenRead(itemName))
            {
                using (var gs = new GZipStream(fs, CompressionMode.Decompress))
                {
                    var bf = new BinaryFormatter();
                    return (T)bf.Deserialize(gs);
                }
            }
        }

        public void Save(String itemName, Object obj)
        {
            System.IO.Directory.CreateDirectory(Path.GetDirectoryName(itemName));

            using (var fs = File.Create(itemName))
            {
                using (var gs = new GZipStream(fs, CompressionMode.Compress))
                {
                    var bf = new BinaryFormatter();
                    bf.Serialize(gs, obj);
                }
            }
        }


        public IEnumerable<T> LoadMany<T>(string query)
        {
            throw new NotImplementedException();
        }
    }
}
