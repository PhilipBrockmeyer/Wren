using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wren.Core.Settings;

namespace Wren.Core.Replay
{
    [Serializable]
    [SettingsScope(SettingsScope.Override)]
    public class ReplaySettings : ISettings
    {
        public Boolean IsRecording { get; set; }
        public Boolean IsPlayingBack { get; set; }
        public String FileName { get; set; }
    }
}
