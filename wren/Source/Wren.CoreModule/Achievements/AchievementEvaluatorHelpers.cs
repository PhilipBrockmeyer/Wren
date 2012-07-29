using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wren.Module.Core.Achievements;

namespace Wren.Core.Achievements
{
    public class AchievementHelpers
    {
        AchievementsManager _achievementsManager;
        Achievement _achievement;
        IEmulator _emulator;

        public AchievementHelpers(Achievement achievement, AchievementsManager achievementsManager, IEmulator emulator)
        {
            _achievementsManager = achievementsManager;
            _achievement = achievement;
            _emulator = emulator;
        }

        public IEnumerable<BuiltInFunction> GetBuiltInFunctions()
        {
            yield return new BuiltInFunction() { Name = "complete", Action = (Action)Complete };
            yield return new BuiltInFunction() { Name = "saveState", Action = (Action<String>)SaveState };
            yield return new BuiltInFunction() { Name = "readMemory", Action = (Func<Int32, Int32>)ReadMemory };
        }

        public void Complete()
        {
            _achievement.IsUnlocked = true;
            _achievementsManager.SaveState(_achievement.AchievementState);
        }

        public Int32 ReadMemory(Int32 address)
        {
            if (_emulator is IDebuggingEmulator)
            {
                return ((IDebuggingEmulator)_emulator).ReadMemory(address);
            }
            else
            {
                return 0;
            }
        }

        public void SaveState(String state)
        {
            _achievement.State = state;
            _achievementsManager.SaveState(_achievement.AchievementState);
        }
    }
}
