using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace Wren
{
    public class MemoryFilterViewModel : INotifyPropertyChanged
    {
        ICommand _overwriteAllCommand;
        ICommand _overwriteEqualToCommand;
        ICommand _filterEqualToCommand;
        ICommand _filterChangeCommand;
        ICommand _filterIncreaseCommand;
        ICommand _filterDecreaseCommand;
        ICommand _filterSameCommand;

        List<MemoryItem> _newMemory;

        public ICommand OverwriteAllCommand
        {
            get { return _overwriteAllCommand ?? (_overwriteAllCommand = new ActionCommand(ExecuteOverwriteAll, o => true)); }
        }

        public ICommand OverwriteEqualToCommand
        {
            get { return _overwriteEqualToCommand ?? (_overwriteEqualToCommand = new ActionCommand(ExecuteOverwriteEqualTo, o => true)); }
        }

        public ICommand FilterEqualToCommand
        {
            get { return _filterEqualToCommand ?? (_filterEqualToCommand = new ActionCommand(ExecuteFilterEqualTo, o => true)); }
        }

        public ICommand FilterChangeCommand
        {
            get { return _filterChangeCommand ?? (_filterChangeCommand = new ActionCommand(ExecuteFilterChange, o => true)); }
        }

        public ICommand FilterIncreaseCommand
        {
            get { return _filterIncreaseCommand ?? (_filterIncreaseCommand = new ActionCommand(ExecuteFilterIncrease, o => true)); }
        }

        public ICommand FilterDecreaseCommand
        {
            get { return _filterDecreaseCommand ?? (_filterDecreaseCommand = new ActionCommand(ExecuteFilterDecrease, o => true)); }
        }

        public ICommand FilterSameCommand
        {
            get { return _filterSameCommand ?? (_filterSameCommand = new ActionCommand(ExecuteFilterSame, o => true)); }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        ObservableCollection<MemoryItem> _memory;
        public ObservableCollection<MemoryItem> Memory
        {
            get { return _memory; }
            set
            {
                _memory = value;
                OnPropertyChanged("Memory");
            }
        }

        String _overwriteEqualToValue;
        public String OverwriteEqualToValue
        {
            get { return _overwriteEqualToValue; }
            set
            {
                _overwriteEqualToValue = value;
                OnPropertyChanged("OverwriteEqualToValue");
            }
        }

        String _filterEqualToValue;
        public String FilterEqualToValue
        {
            get { return _filterEqualToValue; }
            set
            {
                _filterEqualToValue = value;
                OnPropertyChanged("FilterEqualToValue");
            }
        }

        public MemoryFilterViewModel()
        {

        }
        
        public void SetMemory(Byte[] memory, Int32 baseAddress)
        {
            _newMemory = new List<MemoryItem>();

            for (int i = 0; i < memory.Length; i++)
            {
                _newMemory.Add(new MemoryItem() { Offset = i, Value = memory[i] , Address = i + baseAddress });
            }
        }

        protected void OnPropertyChanged(String propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        protected void ExecuteOverwriteAll(Object obj)
        {
            Memory = new ObservableCollection<MemoryItem>(_newMemory);
        }

        protected void ExecuteOverwriteEqualTo(Object obj)
        {
            if (String.IsNullOrEmpty(OverwriteEqualToValue))
                return;

            Int32 compareValue = -1;
            compareValue = Int32.Parse(OverwriteEqualToValue, System.Globalization.NumberStyles.HexNumber);

            Memory = new ObservableCollection<MemoryItem>(_newMemory.Where(m => m.Value == compareValue));
        }

        protected void ExecuteFilterEqualTo(Object obj)
        {
            if (String.IsNullOrEmpty(FilterEqualToValue))
                return;

            Int32 compareValue = -1;
            compareValue = Int32.Parse(OverwriteEqualToValue, System.Globalization.NumberStyles.HexNumber);

            var filteredMemory = new ObservableCollection<MemoryItem>();

            foreach (var item in Memory)
            {
                if (_newMemory.Count <= item.Offset)
                    continue;

                if (_newMemory[item.Offset].Value == compareValue)
                    filteredMemory.Add(_newMemory[item.Offset]);
            }

            Memory = filteredMemory;
        }

        protected void ExecuteFilterChange(Object obj)
        {
            var filteredMemory = new ObservableCollection<MemoryItem>();

            foreach (var item in Memory)
            {
                if (_newMemory.Count <= item.Offset)
                    continue;

                if (_newMemory[item.Offset].Value != item.Value)
                    filteredMemory.Add(_newMemory[item.Offset]);
            }

            Memory = filteredMemory;
        }

        protected void ExecuteFilterIncrease(Object obj)
        {
            var filteredMemory = new ObservableCollection<MemoryItem>();

            foreach (var item in Memory)
            {
                if (_newMemory.Count <= item.Offset)
                    continue;

                if (_newMemory[item.Offset].Value > item.Value)
                    filteredMemory.Add(_newMemory[item.Offset]);
            }

            Memory = filteredMemory;
        }

        protected void ExecuteFilterDecrease(Object obj)
        {
            var filteredMemory = new ObservableCollection<MemoryItem>();

            foreach (var item in Memory)
            {
                if (_newMemory.Count <= item.Offset)
                    continue;

                if (_newMemory[item.Offset].Value < item.Value)
                    filteredMemory.Add(_newMemory[item.Offset]);
            }

            Memory = filteredMemory;
        }

        protected void ExecuteFilterSame(Object obj)
        {
            var filteredMemory = new ObservableCollection<MemoryItem>();

            foreach (var item in Memory)
            {
                if (_newMemory.Count <= item.Offset)
                    continue;

                if (_newMemory[item.Offset].Value == item.Value)
                    filteredMemory.Add(_newMemory[item.Offset]);
            }

            Memory = filteredMemory;
        }
    }
}
