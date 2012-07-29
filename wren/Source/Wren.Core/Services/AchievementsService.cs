using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wren.Transport.DataObjects;
using System.Globalization;
using System.Web;

namespace Wren.Core.Services
{
    public class AchievementsService : Service, IAchievementsService
    {
        public AchievementDefinitionsDto GetAchievementDefinitions(DateTime lastUpdated)
        {
            try
            {
                return base.WebGet<AchievementDefinitionsDto>("GameInfo/AchievementDefinitions", new KeyValuePair<String, String>("lastUpdated", lastUpdated.ToString(CultureInfo.InvariantCulture.DateTimeFormat)));
            }
            catch (Exception ex)
            {
                throw new ApplicationException("There was an error attempting to download the achievement definitions.", ex);
            }
        }

        private class SynchronizeAchievementStateParameters : IServiceParameters
        {
            public String UserId { get; set; }
            public String LocalAchievementState { get; set; }
        }

        public void UploadAchievementState(String userId, String localAchievementState)
        {
            var parameters = new SynchronizeAchievementStateParameters()
            {
                LocalAchievementState = localAchievementState,
                UserId = userId
            };

            base.WebPost<Boolean>(parameters, "GameInfo/AchievementState");
        }

        public AchievementStateDownloadDto DownloadAchievementState(String userId, DateTime lastUpdated)
        {
            return base.WebGet<AchievementStateDownloadDto>(
                "GameInfo/AchievementState/" + HttpUtility.UrlEncode(userId),
                new KeyValuePair<String, String>("lastUpdated", lastUpdated.ToString(CultureInfo.InvariantCulture.DateTimeFormat))
            );
        }
    }
}
