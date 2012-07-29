using System;
using Wren.Core;
using Wren.Core.GameEvents;
namespace Wren.Core.Statistics
{
    public interface IStatisticsManager
    {
        void InitializeStatistics(Game game, GameEventAggregator eventAggregator);
        void UpdateStatisticDefinitions(Game game);
        void SaveStatistics();
    }
}
