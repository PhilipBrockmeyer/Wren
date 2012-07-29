using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Xml.XPath;
using System.Xml;
using System.IO;

namespace Wren.Core.Settings
{
    [Serializable]
    public class SettingsEntries : IXmlSerializable 
    {
        IList<SettingsEntry> _entries;

        public SettingsEntry[] Entries 
        { 
            get { return _entries.ToArray(); }
            set { _entries = new List<SettingsEntry>(value); }
        }

        public SettingsEntries()
        {
            _entries = new List<SettingsEntry>();
        }

        public void AddSettings(SettingsEntry settings)
        {
            if (_entries.Where(e => e.Key == settings.Key).Count() > 0)
                throw new ApplicationException("An attempt was made to add multiple settings with the same key.");

            _entries.Add(settings);
        }

        public System.Xml.Schema.XmlSchema GetSchema()
        {
            throw new NotImplementedException();
        }

        public void ReadXml(XmlReader reader)
        {
            XPathDocument xPath = new XPathDocument(reader);
            XPathNavigator navigator = xPath.CreateNavigator();

            XPathNodeIterator iterator = navigator.Select("//SettingsEntries/Setting");
            while (iterator.MoveNext())
            {
                SettingsEntry entry = new SettingsEntry();
                entry.Key = iterator.Current.GetAttribute("key", String.Empty);
                String typeName = iterator.Current.GetAttribute("type", String.Empty);
                Type type = Type.GetType(typeName);

                XmlSerializer xs = new XmlSerializer(type);

                iterator.Current.MoveToFirstChild();
                entry.Settings = (ISettings)xs.Deserialize(iterator.Current.ReadSubtree());

                _entries.Add(entry);
            }            
        }

        public void WriteXml(XmlWriter writer)
        {
            XmlDocument xdoc = new XmlDocument();

            foreach (var entry in Entries)
            {
                XmlNode element = xdoc.CreateElement("Setting");

                var keyAttribute = xdoc.CreateAttribute("key", String.Empty);
                keyAttribute.Value = entry.Key;
                element.Attributes.Append(keyAttribute);

                var typeAttribute = xdoc.CreateAttribute("type", String.Empty);
                typeAttribute.Value = entry.Settings.GetType().AssemblyQualifiedName;
                element.Attributes.Append(typeAttribute);

                XmlElement content = xdoc.CreateElement("Content");

                StringBuilder sb = new StringBuilder();
                var sw = new StringWriter(sb);
                XmlSerializer xs = new XmlSerializer(entry.Settings.GetType());
                xs.Serialize(sw, entry.Settings);

                String inner = sb.ToString();
                String excess = "xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"";
                
                if (inner.IndexOf(excess) != -1)
                    inner = inner.Remove(inner.IndexOf(excess), excess.Length);
                
                excess = "xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"";
                if (inner.IndexOf(excess) != -1)
                    inner = inner.Remove(inner.IndexOf(excess), excess.Length);
                
                excess = "<?xml version=\"1.0\" encoding=\"utf-16\"?>";
                inner = inner.Remove(inner.IndexOf(excess), excess.Length);

                element.InnerXml = inner;

                element.WriteTo(writer);
            }
        }
    }
}
