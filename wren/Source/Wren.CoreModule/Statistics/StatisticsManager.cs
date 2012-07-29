using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wren.Core.Persistence;
using Wren.Core.Directory;
using System.IO;
using Wren.Core.GameEvents;

namespace Wren.Core.Statistics
{
    public class StatisticsManager : IStatisticsManager
    {
        IPersistenceManager _persistenceManager;
        UserStatisticState _userState;

        public StatisticsManager(IPersistenceManager persistenceManager)
        {
            _persistenceManager = persistenceManager;

            _userState = persistenceManager.Load<UserStatisticState>("Statistics.state",
                StatisticsModule.StatisticsStatePersistenceProviderKey);

            if (_userState == null)
                _userState = new UserStatisticState();
        }

        public void InitializeStatistics(Game game, GameEventAggregator eventAggregator)
        {
            String statisticsDefinitionPath = 
                Path.Combine("Statistics", game.Id + ".statistics");

            var definitions = _persistenceManager.Load<StatisticDefinitions>(statisticsDefinitionPath, StatisticsModule.StatisticsPersistenceProviderKey);

            if (definitions == null)
                return;

            foreach (var stat in definitions.Statistics)
            {
                var uState = _userState.GetStatisticState(stat.Id);
                stat.State = uState;
                stat.Initialize(eventAggregator);

                if (uState == null)
                    _userState.AddState(stat.State);
            }
        }

        public void UpdateStatisticDefinitions(Game game)
        {
            /*var definitions = new StatisticDefinitions();

            definitions.AddStatistic(new GameplayTimerStatistic() { Description = "Time Played", Id = Guid.NewGuid() });

            String statisticsDefinitionPath =
                Path.Combine("Statistics", game.Id + ".statistics");

            _persistenceManager.Save(statisticsDefinitionPath, StatisticsModule.StatisticsPersistenceProviderKey,definitions);*/
        }

        public void SaveStatistics()
        {
            _persistenceManager.Save("Statistics.state",
                StatisticsModule.StatisticsStatePersistenceProviderKey,
                _userState);
        }
    }
}
