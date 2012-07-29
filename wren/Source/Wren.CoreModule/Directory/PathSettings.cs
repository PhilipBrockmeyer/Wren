using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wren.Core.Settings;
using System.ComponentModel;

namespace Wren.Core.Directory
{
    [Serializable]
    [SettingsScope(SettingsScope.Global)]
    public class PathSettings : ISettings
    {
        [Serializable]
        public class PathSetting : INotifyPropertyChanged
        {
            private String _Key;

            public String Key
            {
                get { return _Key; }
                set
                {
                    _Key = value;
                    OnPropertyChanged("Key");
                }
            }
            
            private String _Path;

            public String Path
            {
                get { return _Path; }
                set
                {
                    _Path = value;
                    OnPropertyChanged("Path");
                }
            }
                        
            public event PropertyChangedEventHandler PropertyChanged;

            protected virtual void OnPropertyChanged(String propertyName)
            {
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        IList<PathSetting> _settings;

        public PathSetting[] Paths
        {
            get { return _settings.ToArray(); }
            set { _settings = new List<PathSetting>(value); }
        }

        public void AddPath(String pathKey, String path)
        {
            _settings.Add(new PathSetting() { Key = pathKey, Path = path });
        }

        public void ClearPaths(String pathKey)
        {
            for (Int32 index = _settings.Count - 1; index >= 0; index--)
            {
                if (_settings[index].Key == pathKey)
                    _settings.RemoveAt(index);
            }
        }

        public PathSettings()
        {
            _settings = new List<PathSetting>();
        }
    }
}
