using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Collections.ObjectModel;
using Wren.Core.Directory;
using Wren.Core;
using Wren.Core.GameLibrary;

namespace Wren.ViewModels
{
    public class RomPathsViewModel : INotifyPropertyChanged
    {
        IDirectoryManager _directoryManager;

        public event PropertyChangedEventHandler PropertyChanged;
        
        public RomPathsViewModel(IDirectoryManager directoryManager)
        {
            _directoryManager = directoryManager;
            Paths = new ObservableCollection<PathSettings.PathSetting>();

            Load();
        }

        public ObservableCollection<PathSettings.PathSetting> Paths { get; private set; }

        public void Load()
        {
            foreach (var p in _directoryManager.GetPaths(EmulationContext.Empty, GameLibraryModule.RomPathKey))
            {
                Paths.Add(p);
            }
        }

        public void AddPath()
        {
            Paths.Add(new PathSettings.PathSetting() { Key = GameLibraryModule.RomPathKey, Path = "New Folder" });
        }

        public void Save()
        {
            foreach (var p in Paths)
            {
                _directoryManager.ClearPaths(EmulationContext.Empty, GameLibraryModule.RomPathKey);
                _directoryManager.AddPath(EmulationContext.Empty, GameLibraryModule.RomPathKey, p.Path);
            }
        }

        protected void OnPropertyChanged(String propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public void DeletePath(Int32 index)
        {
            if (Paths.Count == 0)
                return;

            if (index < 0)
                return;

            Paths.RemoveAt(index);
        }
    }
}
