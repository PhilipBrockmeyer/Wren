using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wren.Core
{
    public class EmulationContext
    {
        public Game Game { get; private set; }
        public EmulatedSystem EmulatedSystem { get; private set; }

        public static EmulationContext Empty 
        {
            get { return new EmulationContext(new Game(String.Empty, String.Empty), new EmulatedSystem(String.Empty)); }
        }

        public EmulationContext(Game gameIdentifier,
                               EmulatedSystem emulatedSystemIdentifier)
        {
            Game = gameIdentifier;
            EmulatedSystem = emulatedSystemIdentifier;
        }
    }
}
