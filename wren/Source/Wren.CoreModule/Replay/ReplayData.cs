using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wren.Core.Replay
{
    [Serializable]
    public class ReplayData
    {
        public enum ButtonAction
        {
            Pressed = 0,
            Released = 1
        }

        [Serializable]
        public struct KeyData
        {
            public Int32 Frame { get; set; }
            public Byte ButtonId { get; set; }
            public Byte Action { get; set; }
        }

        IList<KeyData> _data;
        public KeyData[] Data
        {
            get { return _data.ToArray(); }
            set { _data = new List<KeyData>(value); }
        }

        public ReplayData()
        {
            _data = new List<KeyData>();
        }

        public void AddButtonAction(ButtonAction action, Int32 buttonId, Int32 frame)
        {
            _data.Add(new KeyData() { Action = (Byte)action, ButtonId = (Byte)buttonId, Frame = frame });
        }
    }
}
