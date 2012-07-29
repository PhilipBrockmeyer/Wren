using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wren.Core.Events
{
    public class GameSelectedEvent : IEvent
    {
        public GameInfo Game { get; private set; }

        public GameSelectedEvent(GameInfo game)
        {
            Game = game;
        }
    }
}
