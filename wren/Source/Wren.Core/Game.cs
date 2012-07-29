using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wren.Core
{
    public class Game
    {
        public String Id { get; private set; }
        public String RomPath { get; private set; }

        public static Game Empty
        {
            get { return new Game(String.Empty, String.Empty); }
        }

        public Game(String id, String romPath)
        {
            Id = id;
            RomPath = romPath;
        }
    }
}
