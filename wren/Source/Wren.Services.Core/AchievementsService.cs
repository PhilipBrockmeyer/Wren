using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wren.Transport.DataObjects;
using System.Text.RegularExpressions;
using Wren.Data;
using NHibernate.Linq;
using NHibernate.Criterion;
using Wren.Data.Entities;

namespace Wren.Services.Core
{
    public class AchievementsService
    {
        ISessionManager _sessionManager;

        public AchievementsService(ISessionManager sessionManager)
        {
            _sessionManager = sessionManager;
        }

        public AchievementDefinitionsDto GetAchievementDefinitions(DateTime lastUpdated)
        {
            AchievementDefinitionsDto definitions = new AchievementDefinitionsDto();
            definitions.UpdateTimestamp = DateTime.UtcNow;
            List<Wren.Transport.DataObjects.AchievementDefinitionDto> definitionList = new List<AchievementDefinitionDto>(); ;

            using (var session = _sessionManager.GetSession())
            {
                var results = session.Linq<AchievementDefinition>()
                    .Where(ad => ad.TimeStamp > lastUpdated);

                foreach (var definition in results)
                {
                    Wren.Transport.DataObjects.AchievementDefinitionDto dto = new AchievementDefinitionDto();
                    dto.Code = definition.Code;
                    dto.Description = definition.Description;
                    dto.Id = definition.Id;
                    dto.LockedImageUrl = definition.LockedImageUrl;
                    dto.LongDisplayFormat = definition.LongDisplayFormat;
                    dto.RomMd5 = definition.RomMd5;
                    dto.ShortDisplayFormat = definition.ShortDisplayFormat;
                    dto.UnlockedImageUrl = definition.UnlockedImageUrl;
                    dto.Title = definition.Title;
                    definitionList.Add(dto);
                }
            }

            definitions.Definitions = definitionList.ToArray();

            return definitions;
        }

        public AchievementStateDownloadDto GetAchievementState(String userId, DateTime lastUpdated)
        {
            AchievementStateDownloadDto download = new AchievementStateDownloadDto();
            download.UpdateTimestamp = DateTime.UtcNow;
            List<AchievementStateDto> stateList = new List<AchievementStateDto>(); ;

            using (var session = _sessionManager.GetSession())
            {
                var results = session.Linq<AchievementState>()
                    .Where(state => state.ServerDate > lastUpdated && state.UserId == userId);

                foreach (var state in results)
                {
                    AchievementStateDto dto = new AchievementStateDto();
                    dto.AchievementDefinitionId = state.AchievementDefinitionId;
                    dto.Id = state.Id;
                    dto.IsUnlocked = state.IsUnlocked;
                    dto.ServerDate = state.ServerDate;
                    dto.State = state.State;
                    dto.UserId = state.UserId;
                    dto.Version = state.Version;

                    stateList.Add(dto);
                }
            }

            download.State = stateList.ToArray();

            return download;
        }

        public Boolean UploadAchievementState(String userId, AchievementStateDto[] localAchievements)
        {
            if (localAchievements.Count() == 0)
                return true;

            using (var session = _sessionManager.GetSession())
            {
                using (var tx = session.BeginTransaction())
                {

                    List<String> ids = new List<String>();
                    foreach (var achievementState in localAchievements)
                    {
                        ids.Add(achievementState.Id);
                    }

                    var serverAchievements = session.CreateCriteria<AchievementState>()
                        .Add(Expression.In("Id", ids.ToArray()))
                        .List<AchievementState>();

                    foreach (var achievementState in localAchievements)
                    {
                        if (achievementState.UserId != userId)
                            continue;

                        var sa = serverAchievements.Where(a => a.Id == achievementState.Id).FirstOrDefault();

                        if (sa != null)
                        {
                            if (sa.Version < achievementState.Version)
                            {
                                sa.IsUnlocked = achievementState.IsUnlocked;
                                sa.ServerDate = DateTime.UtcNow;
                                sa.State = achievementState.State;
                                sa.Version = achievementState.Version;
                            }
                        }
                        else
                        {
                            AchievementState newState = new AchievementState()
                            {
                                AchievementDefinitionId = achievementState.AchievementDefinitionId,
                                Id = achievementState.Id,
                                RomMd5 = achievementState.RomMd5,
                                IsUnlocked = achievementState.IsUnlocked,
                                ServerDate = DateTime.UtcNow,
                                State = achievementState.State,
                                UserId = achievementState.UserId,
                                Version = achievementState.Version
                            };

                            session.SaveOrUpdate(newState);
                        }
                    }

                    tx.Commit();
                }
            }

            return true;
        }
    }
}
