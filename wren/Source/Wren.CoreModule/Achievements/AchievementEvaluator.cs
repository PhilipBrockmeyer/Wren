using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jint;
using Wren.Core.GameEvents;
using System.Globalization;

namespace Wren.Core.Achievements
{
    public class AchievementEvaluator
    {
        public const String ProgramTriggersLiteral = "ProgramTriggers:";
        public const String MemoryTriggersLiteral = "MemoryTriggers:";
        public const String CodeLiteral = "JS:";

        JintEngine _engine;
        GameEventAggregator _gameEventAggregator;
        Achievement _achievement;

        public AchievementEvaluator(Achievement achievement, GameEventAggregator gameEventAggregator)
        {
            _gameEventAggregator = gameEventAggregator;
            _achievement = achievement;
        }

        public void RegisterHandlers(AchievementHelpers helpers)
        {
            String code = _achievement.Code;

            var progTriggersIndex = code.IndexOf(ProgramTriggersLiteral);
            var memoryTriggersIndex = code.IndexOf(MemoryTriggersLiteral);
            var jsIndex = code.IndexOf(CodeLiteral);

            String programTriggersCode = code.Substring(
                progTriggersIndex + ProgramTriggersLiteral.Length,
                memoryTriggersIndex - (progTriggersIndex + ProgramTriggersLiteral.Length));

            String memoryTriggersCode = code.Substring(
                memoryTriggersIndex + MemoryTriggersLiteral.Length,
                jsIndex - (memoryTriggersIndex + MemoryTriggersLiteral.Length));

            String javascriptCode = code.Substring(
                jsIndex + CodeLiteral.Length,
                code.Length - (jsIndex + CodeLiteral.Length));

            LoadJavascriptCode(javascriptCode);
            InitializeHelperFunctions(helpers);
            LoadProgramTriggers(programTriggersCode);
            LoadMemoryTriggers(memoryTriggersCode);
            InitializeRunningContext();
            //_gameEventAggregator.AddGameEndedHandler(() => _achievement.State = (String)_engine.CallFunction("getState"));

            var result = _engine.CallFunction("initialize", _achievement.State);
        }

        private void InitializeRunningContext()
        {
            _engine.CallFunction("reset");
            _engine.CallFunction("initialize", _achievement.State);
        }

        private void InitializeHelperFunctions(AchievementHelpers helpers)
        {
            foreach (var function in helpers.GetBuiltInFunctions())
            {
                _engine.SetFunction(function.Name, function.Action);
            }
        }

        private void LoadMemoryTriggers(String memoryTriggersCode)
        {
            var declarations = memoryTriggersCode.Split(';');

            for (Int32 index = 0; index < declarations.Length - 1; index++)
            {
                var parts = declarations[index].Trim().Split('=');

                AddHandler(
                    Int32.Parse(parts[1].Trim().Substring(2, parts[1].Trim().Length - 2), NumberStyles.HexNumber),
                    parts[0].Trim()
                );              
            }
        }

        private void AddHandler(Int32 address, String functionName)
        {
            _gameEventAggregator.AddMemoryWatch(address,
                value => _engine.CallFunction(functionName, value.NewValue)
            );
        }

        private void LoadProgramTriggers(String programTriggersCode)
        {
            var declarations = programTriggersCode.Split(';');

            for (Int32 index = 0; index < declarations.Length - 1; index++)
            {
                var parts = declarations[index].Trim().Split('=');
            }
        }

        private void LoadJavascriptCode(String javascript)
        {
            _engine = new JintEngine();
            _engine.EnableSecurity();
            _engine.AllowClr = true;
            _engine.Run(javascript);
        }
    }
}
