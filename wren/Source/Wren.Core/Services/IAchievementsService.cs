using System;
using Wren.Transport.DataObjects;
namespace Wren.Core.Services
{
    public interface IAchievementsService
    {
        AchievementDefinitionsDto GetAchievementDefinitions(DateTime lastUpdated);
        void UploadAchievementState(String userId, String localAchievementState);
        AchievementStateDownloadDto DownloadAchievementState(String userId, DateTime lastUpdated);
    }
}
