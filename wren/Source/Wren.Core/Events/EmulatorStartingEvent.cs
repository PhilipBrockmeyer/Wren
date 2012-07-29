using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wren.Core.Events
{
    public class EmulatorStartingEvent : IEvent
    {
        public Guid EmulationRunnerId { get; private set; }
        public IEmulationRunner EmulationRunner { get; private set; }
        public Game Game { get; private set; }
        public EmulationMode Mode { get; private set; }

        public EmulatorStartingEvent(Guid emulationRunnerId, IEmulationRunner emulationRunner, Game game, EmulationMode mode)
        {
            EmulationRunnerId = emulationRunnerId;
            EmulationRunner = emulationRunner;
            Game = game;
            Mode = mode;
        }
    }
}
