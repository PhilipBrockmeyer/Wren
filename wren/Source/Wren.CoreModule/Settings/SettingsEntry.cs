using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Xml;
using System.IO;
using System.Xml.XPath;

namespace Wren.Core.Settings
{
    [Serializable]
    public class SettingsEntry
    {
        public String Key { get; set; }
        public ISettings Settings { get; set; }
    }
}
