using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wren.Core.GameLibrary;
using Wren.Core.Settings;
using Wren.Core.Persistence;
using System.Collections.ObjectModel;
using System.Windows.Data;

namespace Wren.Core.Achievements
{
    public class AchievementGameInfoProvider : IGameInfoProvider
    {
        AchievementsManager _achievementsManager;

        public GameInfoProviderUpdateMode UpdateMode
        {
            get { return GameInfoProviderUpdateMode.GameSelected; }
        }

        public AchievementGameInfoProvider(AchievementsManager achievementsManager)
        {
            _achievementsManager = achievementsManager;
        }

        public void UpdateGameInfo(GameInfo gameInfo)
        {
            var achievements = new ObservableCollection<Achievement>();
            var collectionView = new ListCollectionView(achievements);
            collectionView.SortDescriptions.Add(new System.ComponentModel.SortDescription("IsUnlocked", System.ComponentModel.ListSortDirection.Ascending));
            collectionView.SortDescriptions.Add(new System.ComponentModel.SortDescription("Title", System.ComponentModel.ListSortDirection.Ascending));

            gameInfo.Data.Achievements = collectionView;
            
            foreach (var a in _achievementsManager.GetAchievements(gameInfo.Game))
            {
                achievements.Add(a);
            }
        }
    }
}
