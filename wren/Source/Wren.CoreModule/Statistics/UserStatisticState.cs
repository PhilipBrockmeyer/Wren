using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Xml.XPath;
using System.Xml;
using System.IO;

namespace Wren.Core.Statistics
{
    [Serializable]
    public class UserStatisticState : IXmlSerializable
    {
        IList<IStatisticState> _state;

        public IStatisticState[] State
        {
            get { return _state.ToArray(); }
            set { _state = new List<IStatisticState>(value); }
        }

        public UserStatisticState()
        {
            _state = new List<IStatisticState>();
        }

        public IStatisticState GetStatisticState(Guid statisticDefinitionId)
        {
            return  _state.Where(s => s.StatisticDefinitionId == statisticDefinitionId).FirstOrDefault();
        }

        public void AddState(IStatisticState state)
        {
            _state.Add(state);
        }

        public System.Xml.Schema.XmlSchema GetSchema()
        {
            throw new NotImplementedException();
        }
        public void ReadXml(XmlReader reader)
        {
            XPathDocument xPath = new XPathDocument(reader);
            XPathNavigator navigator = xPath.CreateNavigator();

            XPathNodeIterator iterator = navigator.Select("//UserStatisticState/Statistic");
            while (iterator.MoveNext())
            {
                String typeName = iterator.Current.GetAttribute("type", String.Empty);
                Type type = Type.GetType(typeName);

                XmlSerializer xs = new XmlSerializer(type);

                iterator.Current.MoveToFirstChild();
                IStatisticState stat = (IStatisticState)xs.Deserialize(iterator.Current.ReadSubtree());

                _state.Add(stat);
            }
        }

        public void WriteXml(XmlWriter writer)
        {
            XmlDocument xdoc = new XmlDocument();

            foreach (var stat in _state)
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
