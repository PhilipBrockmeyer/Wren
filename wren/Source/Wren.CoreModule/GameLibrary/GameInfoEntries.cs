using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Xml.Schema;
using System.Xml.XPath;
using System.Xml;

namespace Wren.Core.GameLibrary
{
    [Serializable]
    public class GameInfoEntries : IXmlSerializable
    {
        IList<GameInfo> _entries;

        public GameInfo[] Entries
        {
            get { return _entries.ToArray(); }
            set { _entries = new List<GameInfo>(value); }
        }

        public GameInfoEntries()
        {
            _entries = new List<GameInfo>();
        }

        public bool ContainsGame(String path)
        {
            return _entries.Where(e => e.RomPath == path).Count() > 0;
        }

        public GameInfo GetGameInfo(String path)
        {
            return _entries.Where(e => e.RomPath == path).Single();
        }

        public void AddGame(GameInfo game)
        {
            _entries.Add(game);
        }

        public XmlSchema GetSchema()
        {
            throw new NotImplementedException();
        }

        public void ReadXml(XmlReader reader)
        {
            XPathDocument xPath = new XPathDocument(reader);
            XPathNavigator navigator = xPath.CreateNavigator();

            XPathNodeIterator iterator = navigator.Select("//GameInfoEntries/GameInfo");
            while (iterator.MoveNext())
            {
                GameInfo entry = new GameInfo();
                entry.RomPath = iterator.Current.GetAttribute("romPath", String.Empty);
                entry.Game = new Game(iterator.Current.GetAttribute("md5", String.Empty), entry.RomPath);
               
                if (iterator.Current.MoveToFirstChild())
                {
                    do
                    {
                        entry.SetValue(iterator.Current.Name, iterator.Current.InnerXml);
                    }
                    while (iterator.Current.MoveToNext());
                }                

                _entries.Add(entry);
            }
        }

        public void WriteXml(XmlWriter writer)
        {
            XmlDocument xdoc = new XmlDocument();

            foreach (var entry in Entries)
            {
                XmlNode element = xdoc.CreateElement("GameInfo");

                var pathAttribute = xdoc.CreateAttribute("romPath", String.Empty);
                pathAttribute.Value = entry.RomPath;
                element.Attributes.Append(pathAttribute);

                var md5Attribute = xdoc.CreateAttribute("md5", String.Empty);
                md5Attribute.Value = entry.Game.Id;
                element.Attributes.Append(md5Attribute);

                foreach (var value in entry.GetItems())
                {
                    if (value.Value is String)
                    {
                        XmlElement content = xdoc.CreateElement(value.Key);
                        content.InnerText = value.Value.ToString();
                        element.AppendChild(content);
                    }
                }

                element.WriteTo(writer);
            }
        }
    }
}
