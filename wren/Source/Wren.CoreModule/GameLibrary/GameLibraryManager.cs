using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wren.Core.Directory;
using System.IO;
using Wren.Core.Persistence;
using Wren.Core.Events;
using Ionic.Zip;

namespace Wren.Core.GameLibrary
{
    public class GameLibraryManager : IGameLibraryManager
    {
        IDirectoryManager _directoryManager;
        IList<GameInfo> _games;
        IList<IGameInfoProvider> _providers;
        IPersistenceManager _persistenceManager;
        GameInfoEntries _gameInfoEntries;

        public GameLibraryManager(IDirectoryManager directoryManager, IPersistenceManager persistenceManager, IEventAggregator eventAggregator)
        {
            _directoryManager = directoryManager;
            _persistenceManager = persistenceManager;
            _games = new List<GameInfo>();
            _providers = new List<IGameInfoProvider>();
            eventAggregator.Subscribe<GameSelectedEvent>(ev =>
                {
                    var gse = ev as GameSelectedEvent;
                    this.UpdateGameInfo(gse.Game);
                });

            _gameInfoEntries = persistenceManager.Load<GameInfoEntries>(GameLibraryModule.GameLibraryPersistenceName, GameLibraryModule.GameLibraryPersistenceProviderKey);

            if (_gameInfoEntries == null)
                _gameInfoEntries = new GameInfoEntries();
        }

        public void UpdateGameLibrary(ICollection<GameInfo> games)
        {
            RemoveMissingPaths();

            foreach (var game in _gameInfoEntries.Entries)
            {
                games.Add(game);
            }

            IEnumerable<GameInfo> newGames = FindNewGames();
            Initialize(newGames, games);
            _persistenceManager.Save(GameLibraryModule.GameLibraryPersistenceName, GameLibraryModule.GameLibraryPersistenceProviderKey, _gameInfoEntries);
        }

        public void UpdateGameInfo(GameInfo game)
        {
            foreach (var provider in _providers)
            {
                if (provider.UpdateMode == GameInfoProviderUpdateMode.GameSelected)
                    provider.UpdateGameInfo(game);
            }
        }

        private void Initialize(IEnumerable<GameInfo> newGames, ICollection<GameInfo> gamesList)
        {
            foreach (var game in newGames)
            {
                foreach (var provider in _providers)
                {
                    if (provider.UpdateMode == GameInfoProviderUpdateMode.InitializeOnly)
                        provider.UpdateGameInfo(game);
                }

                _gameInfoEntries.AddGame(game);
                gamesList.Add(game);
            }
        }

        private IEnumerable<GameInfo> FindNewGames()
        {
            List<GameInfo> newGames = new List<GameInfo>();

            foreach (var path in _directoryManager.GetFilePaths(EmulationContext.Empty, GameLibraryModule.RomPathKey, GameLibraryModule.RomExtensionKey))
            {
                if (_gameInfoEntries.ContainsGame(path))
                    continue;

                var game = new GameInfo() { RomPath = path };
                game.SetValue("Name", Path.GetFileName(path));

                var hashProvider = System.Security.Cryptography.MD5CryptoServiceProvider.Create();
                String md5 = String.Empty;

                if (game.RomPath.ToLower().EndsWith(".zip"))
                {
                    using (var zip = ZipFile.Read(game.RomPath))
                    {
                        if (zip.EntryFileNames.Count() > 1)
                            continue;

                        var file = zip.Entries.First();
                        md5 = ByteArrayToHexString(hashProvider.ComputeHash(file.OpenReader()));
                    }
                }
                else
                {
                    using (var fs = File.OpenRead(path))
                    {
                        md5 = ByteArrayToHexString(hashProvider.ComputeHash(fs));
                        game.Game = new Game(md5, path);
                    }
                }

                newGames.Add(game);
            }

            return newGames;
        }

        private void RemoveMissingPaths()
        {
            List<GameInfo> existingEntries = new List<GameInfo>();

            foreach (var entry in _gameInfoEntries.Entries)
            {
                if (_directoryManager.FileExists(entry.RomPath, GameLibraryModule.RomPathKey))
                    existingEntries.Add(entry);
            }

            _gameInfoEntries.Entries = existingEntries.ToArray();
        }

        private String ByteArrayToHexString(Byte[] bytes)
        {
            StringBuilder hex = new StringBuilder(bytes.Length * 2);
            foreach (byte b in bytes)
                hex.AppendFormat("{0:x2}", b);
            return hex.ToString();
        }

        public IEnumerable<GameInfo> GetGames()
        {
            return _games.AsEnumerable();
        }

        public void RegisterGameInfoProvider(IGameInfoProvider gameInfoProvider)
        {
            _providers.Add(gameInfoProvider);
        }
    }
}
