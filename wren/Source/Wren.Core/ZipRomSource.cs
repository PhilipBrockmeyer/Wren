using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Ionic.Zip;

namespace Wren.Core
{
    public class ZipRomSource : IRomSource
    {
        String _filePath;

        public ZipRomSource(String filePath)
        {
            _filePath = filePath;
        }

        public Stream GetRomData()
        {
            var zip = ZipFile.Read(_filePath);
            if (zip.Entries.Count > 1)
                throw new ApplicationException("Attempted to read from a zip file with multiple rom entries.");

            return zip.Entries.Single().OpenReader();
        }
    }
}
