using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Xml;
using System.Xml.XPath;
using System.IO;

namespace Wren.Core.Statistics
{
    [Serializable]
    public class StatisticDefinitions : IXmlSerializable
    {
        IList<IStatistic> _statistics;

        public IStatistic[] Statistics 
        {
            get { return _statistics.ToArray(); }
            set { _statistics = new List<IStatistic>(value); }
        }

        public StatisticDefinitions()
        {
            _statistics = new List<IStatistic>();
        }

        public void AddStatistic(IStatistic statistic)
        {
            _statistics.Add(statistic);
        }

        public System.Xml.Schema.XmlSchema GetSchema()
        {
            throw new NotImplementedException();
        }
        public void ReadXml(XmlReader reader)
        {
            XPathDocument xPath = new XPathDocument(reader);
            XPathNavigator navigator = xPath.CreateNavigator();

            XPathNodeIterator iterator = navigator.Select("//StatisticDefinitions/Statistic");
            while (iterator.MoveNext())
            {
                String typeName = iterator.Current.GetAttribute("type", String.Empty);
                Type type = Type.GetType(typeName);

                XmlSerializer xs = new XmlSerializer(type);

                iterator.Current.MoveToFirstChild();
                IStatistic stat = (IStatistic)xs.Deserialize(iterator.Current.ReadSubtree());

                _statistics.Add(stat);
            }
        }

        public void WriteXml(XmlWriter writer)
        {
            XmlDocument xdoc = new XmlDocument();

            foreach (var stat in _statistics)
            {
                XmlNode element = xdoc.CreateElement("Statistic");

                var typeAttribute = xdoc.CreateAttribute("type", String.Empty);
                typeAttribute.Value = stat.GetType().FullName;
                element.Attributes.Append(typeAttribute);

                XmlElement content = xdoc.CreateElement("Content");

                StringBuilder sb = new StringBuilder();
                var sw = new StringWriter(sb);
                XmlSerializer xs = new XmlSerializer(stat.GetType());
                xs.Serialize(sw, stat);

                String inner = sb.ToString();
                String excess = "xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"";
                inner = inner.Remove(inner.IndexOf(excess), excess.Length);
                excess = "<?xml version=\"1.0\" encoding=\"utf-16\"?>";
                inner = inner.Remove(inner.IndexOf(excess), excess.Length);

                element.InnerXml = inner;

                element.WriteTo(writer);
            }
        }
    }
}
