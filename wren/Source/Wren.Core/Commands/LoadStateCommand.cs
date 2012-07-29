using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Wren.Core.Commands
{
    public class LoadStateCommand : IEmulatorCommand
    {
        Game _game;
        Int32 _saveSlot;

        public LoadStateCommand(Game game, Int32 saveSlot)
        {
            _game = game;
            _saveSlot = saveSlot;
        }

        public void Execute(IEmulator emulator)
        {
            IStatefulEmulator e = emulator as IStatefulEmulator;

            String stateFolder =
                Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "Wren",
                    "SaveState"
                );

            Directory.CreateDirectory(stateFolder);

            String filePath = Path.Combine(stateFolder, _game.Id + _saveSlot + ".state");

            if (!File.Exists(filePath))
                return;

            using (var file = File.Open(filePath, FileMode.Open, FileAccess.Read))
            {
                e.LoadState(new BinaryReader(file));
            }
        }
    }
}
