using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Dynamic;

namespace Wren.Core
{
    public class GameInfo
    {
        public event PropertyChangedEventHandler PropertyChanged;

        IDictionary<String, Object> _values;

        public String RomPath { get; set; }
        public Game Game { get; set; }

        public dynamic Data { get; private set; }

        public GameInfo()
        {
            _values = new Dictionary<String, Object>();
            Data = new ExpandoObject();
        }

        public void SetValue(String key, Object value)
        {
            if (!_values.ContainsKey(key))
                _values.Add(key, value);

            _values[key] = value;

            ((IDictionary<String, Object>)Data)[key] = value;
        }

        public Object GetValue(String key)
        {
            if (!_values.ContainsKey(key))
                return String.Empty;

            return _values[key];
        }

        public IEnumerable<KeyValuePair<String, Object>> GetItems()
        {
            return _values;
        }        

        public override string ToString()
        {
            return (String)_values["Name"];
        }

        protected virtual void OnPropertyChanged(String propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
