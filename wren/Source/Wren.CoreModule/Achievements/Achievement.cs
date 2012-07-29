using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace Wren.Core.Achievements
{
    public class Achievement : INotifyPropertyChanged
    {
        AchievementDefinition _definition;
        AchievementState _state;

        public Boolean IsUnlocked
        {
            get { return _state.IsUnlocked; }
            set 
            { 
                _state.IsUnlocked = value;
                _state.IsUploaded = false;
                OnPropertyChanged("IsUnlocked");
            }
        }

        public String Code { get { return _definition.Code; } }
        public String State
        {
            get { return _state.State; }
            set 
            { 
                _state.State = value;
                _state.IsUploaded = false;
            }
        }

        public String Description { get { return _definition.Description; } }
        public String Title { get { return _definition.Title; } }
        public String UnlockedImage { get { return _definition.UnlockedImageUrl; } }
        public String LockedImage { get { return _definition.LockedImageUrl; } }

        public AchievementState AchievementState
        {
            get { return _state; }
        }

        public Achievement(AchievementDefinition definition, AchievementState state)
        {
            _definition = definition;
            _state = state;
        }

        protected virtual void OnPropertyChanged(String propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
