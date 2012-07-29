using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Wren.Core
{
    public class FileRomSource : IRomSource
    {
        String _fileName;

        public FileRomSource(String fileName)
        {
            _fileName = fileName;
        }

        public Stream GetRomData()
        {
            return File.OpenRead(_fileName);
        }
    }
}
