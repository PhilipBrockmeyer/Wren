using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wren.Core.Services;
using Wren.Core.Persistence;
using Wren.Core.Settings;
using Wren.Core.GameLibrary;
using Wren.Core.Events;
using System.Threading;
using Wren.Core.GameEvents;
using Wren.Transport.DataObjects;

namespace Wren.Core.Achievements
{
    public class AchievementsManager
    {
        IAchievementsService _achievementsService;
        ISettingsManager _settingsManager;
        IPersistenceManager _persistenceManager;
        IEventAggregator _eventAggregator;

        public AchievementsManager(IAchievementsService achievementsService, ISettingsManager settingsManager, IPersistenceManager persistenceManager, IEventAggregator eventAggregator)
        {
            _settingsManager = settingsManager;
            _persistenceManager = persistenceManager;
            _achievementsService = achievementsService;
            _eventAggregator = eventAggregator;

            eventAggregator.Subscribe<LoggedOnEvent>((ev) =>
                {
                    SynchronizeAchievementDefinitions();
                    SynchronizeAchievementState();
                });

            eventAggregator.Subscribe<EmulatorStartingEvent>(EmulatorStarting);
        }

        private void EmulatorStarting(IEvent e)
        {
            var ese = e as EmulatorStartingEvent;

            if (ese.Mode != EmulationMode.Playing)
                return;

            if (ese.EmulationRunner.Emulator is IDebuggingEmulator)
            {
                GameEventAggregator gea = new GameEventAggregator(_eventAggregator, ese.EmulationRunner);
                
                foreach (var a in GetAchievements(ese.Game))
                {
                    if (!a.IsUnlocked)
                    {
                        AchievementEvaluator evaluator = new AchievementEvaluator(a, gea);
                        evaluator.RegisterHandlers(new AchievementHelpers(a, this, ese.EmulationRunner.Emulator));
                    }
                }
            }
        }

        #region Server Synchronization
        public void SynchronizeAchievementDefinitions()
        {
            ThreadPool.QueueUserWorkItem((WaitCallback)delegate { DefinitionsThreadStart(); });
        }

        public void SynchronizeAchievementState()
        {
            ThreadPool.QueueUserWorkItem((WaitCallback)delegate { StateThreadStart(); });
        }

        private void DefinitionsThreadStart()
        {
            if (WrenCore.IsOffline)
                return;

            var settings = _settingsManager.LoadSettings<AchievementSettings>(EmulationContext.Empty);
            var newDefinitions = _achievementsService.GetAchievementDefinitions(settings.DefinitionsLastSynchronized);

            try
            {
                foreach (var dto in newDefinitions.Definitions)
                {
                    AchievementDefinition ad = new AchievementDefinition()
                    {
                        Code = dto.Code,
                        Id = dto.Id,
                        Description = dto.Description,
                        LockedImageUrl = dto.LockedImageUrl,
                        LongDisplayFormat = dto.LongDisplayFormat,
                        RomMd5 = dto.RomMd5,
                        ShortDisplayFormat = dto.ShortDisplayFormat,
                        UnlockedImageUrl = dto.UnlockedImageUrl,
                        Title = dto.Title
                    };

                    _persistenceManager.Save(ad.Id, AchievementsModule.AchievementsPersistenceProviderKey, ad);
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("There was an error updating local achievement definition database.", ex);
            }

            settings.DefinitionsLastSynchronized = newDefinitions.UpdateTimestamp;
            _settingsManager.ApplySettings(settings);
        }

        private void StateThreadStart()
        {
            if (WrenCore.IsOffline)
                return;

            var settings = _settingsManager.LoadSettings<AchievementSettings>(EmulationContext.Empty);

            if (String.IsNullOrEmpty(WrenCore.UserId))
                return;

            var localAchievements = _persistenceManager.LoadMany<AchievementState>(
                   String.Format("AchievementState/NotUploaded UserId:{0}", WrenCore.UserId),
                   AchievementsModule.AchievementsPersistenceProviderKey).ToList();

            var achievementDtos = new List<AchievementStateDto>();

            foreach (var a in localAchievements)
            {
                achievementDtos.Add(new AchievementStateDto()
                {
                    AchievementDefinitionId = a.AchievementDefinitionId,
                    Id = a.Id,
                    IsUnlocked = a.IsUnlocked,
                    RomMd5 = a.RomMd5,
                    ServerDate = a.ServerDate,
                    State = a.State,
                    UserId = a.UserId,
                    Version = a.Version++
                });
            }

            var dtoJson = Newtonsoft.Json.JsonConvert.SerializeObject(achievementDtos.ToArray());

            if (achievementDtos.Count() > 0)
                _achievementsService.UploadAchievementState(WrenCore.UserId, dtoJson);

            Thread.Sleep(200);

            var serverAchievements = _achievementsService.DownloadAchievementState(WrenCore.UserId, settings.StateLastDownloaded);

            settings.StateLastDownloaded = serverAchievements.UpdateTimestamp;

            foreach (var s in serverAchievements.State)
            {
                AchievementState local = new AchievementState()
                {
                    AchievementDefinitionId = s.AchievementDefinitionId,
                    Id = s.Id,
                    IsUnlocked = s.IsUnlocked,
                    IsUploaded = true,
                    ServerDate = s.ServerDate,
                    State = s.State,
                    UserId = s.UserId,
                    Version = s.Version
                };

                _persistenceManager.Save(s.Id, AchievementsModule.AchievementsPersistenceProviderKey, local);
            }

            _settingsManager.ApplySettings(settings);
        }
        #endregion

        #region LocalDataAccess

        public IEnumerable<Achievement> GetAchievements(Game game)
        {
            List<Achievement> achievements = new List<Achievement>();
            var definitions = GetAchievementDefinitions(game);
            var states = _persistenceManager.LoadMany<AchievementState>(
                   String.Format("AchievementState/GameAndUser UserId:{0} AND RomMd5:{1}", WrenCore.UserId, game.Id),
                   AchievementsModule.AchievementsPersistenceProviderKey);

            foreach (var def in definitions)
            {
                AchievementState state = states.Where(s => s.AchievementDefinitionId == def.Id).FirstOrDefault();
                
                if (state == null)
                {
                    state = new AchievementState()
                    {
                        AchievementDefinitionId = def.Id,
                        Id = Guid.NewGuid().ToString("N"),
                        IsUnlocked = false,
                        IsUploaded = false,
                        RomMd5 = def.RomMd5,
                        ServerDate = new DateTime(2000, 01, 01),
                        State= "{ }",
                        UserId = WrenCore.UserId,
                        Version = 0
                    };

                    _persistenceManager.Save(state.Id, AchievementsModule.AchievementsPersistenceProviderKey, state);
                }

                achievements.Add(new Achievement(def, state));
            }

            return achievements;
        }

        public IEnumerable<AchievementDefinition> GetAchievementDefinitions(Game game)
        {
            return _persistenceManager.LoadMany<AchievementDefinition>(
                    String.Format("AchievementDefinitions/RomMd5 RomMd5:{0}", game.Id),
                    AchievementsModule.AchievementsPersistenceProviderKey);
        }

        public void SaveState(AchievementState state)
        {
            _persistenceManager.Save(state.Id, AchievementsModule.AchievementsPersistenceProviderKey, state);
            SynchronizeAchievementState();
        }

        #endregion
    }
}
