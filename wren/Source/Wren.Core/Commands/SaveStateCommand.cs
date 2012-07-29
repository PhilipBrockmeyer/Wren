using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Wren.Core.Commands
{
    public class SaveStateCommand : IEmulatorCommand
    {
        Game _game;
        Int32 _saveSlot;

        public SaveStateCommand(Game game, Int32 saveSlot)
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

            using (var file = File.Open(Path.Combine(stateFolder, _game.Id + _saveSlot + ".state"),
                    FileMode.Create, FileAccess.Write))
            {
                e.SaveState(new BinaryWriter(file));
            }
        }
    }
}
