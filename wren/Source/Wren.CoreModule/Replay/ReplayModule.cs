using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wren.Core;
using Wren.Core.Settings;
using Wren.Core.Input;
using System.IO;
using Wren.Core.Persistence;
using Wren.Core.Directory;

namespace Wren.Core.Replay
{
    [ModuleDependency(typeof(PersistenceModule))]
    [ModuleDependency(typeof(SettingsModule))]
    [ModuleDependency(typeof(DirectoryModule))]
    public class ReplayModule : IModule
    {
        public const String ReplayPathKey = "ReplayPathKey";
        public const String ReplayPersistenceProviderKey = "ReplayPersistenceProviderKey";

        public const String ReplayManagerPersistenceProviderKey = "ReplayManager";
        public const String ReplayManagerLibraryPersistenceName = "Replays.xml";

        public void Load(IModuleContext context)
        {
            var persistenceManager = context.ServiceLocator.GetInstance<IPersistenceManager>();
            persistenceManager.RegiserPersistenceProvider(ReplayPersistenceProviderKey, () => new BinaryFilePersistenceProvider());

            persistenceManager.RegiserPersistenceProvider(ReplayManagerPersistenceProviderKey, () => new ApplicationDataXmlPersistenceProvider());

            var settingsManager = context.ServiceLocator.GetInstance<ISettingsManager>();
            settingsManager.RegisterSettings<ReplaySettings>();

            context.ServiceLocator.RegisterSingleton<IReplayManager, ReplayManager>();

            var directoryManager = context.ServiceLocator.GetInstance<IDirectoryManager>();
            
            context.InputSourceAssembler.ConfigurePipeline((c, i) =>
                {
                    var settings = settingsManager.LoadSettings<ReplaySettings>(c);
                    if (settings.IsRecording)
                    {
                        return new RecordingInputSourceDecorator(i, Path.Combine(GetPath(c, directoryManager), settings.FileName), persistenceManager, 8);
                    }
                    else
                    {
                        return i;
                    }
                });

            context.InputSourceAssembler.ConfigureInputSource((c) =>
                {
                    var settings = settingsManager.LoadSettings<ReplaySettings>(c);
                    if (settings.IsPlayingBack)
                    {
                        var pp = new BinaryFilePersistenceProvider();
                        var replay = pp.Load<ReplayData>(System.IO.Path.Combine(GetPath(c, directoryManager), settings.FileName));
                        return new PlaybackInputSource(replay, 8);
                    }

                    return null;
                    
                });
        }

        public void Unload(IModuleContext context)
        {
            context.ServiceLocator.GetInstance<IReplayManager>().Save();
        }

        private String GetPath(EmulationContext context, IDirectoryManager directoryManager)
        {
            var path = directoryManager.GetPath(context, ReplayPathKey);

            if (String.IsNullOrEmpty(path))
            {
                path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Wren\\Replays");
                directoryManager.AddPath(context, ReplayPathKey, path);
                System.IO.Directory.CreateDirectory(path);
            }

            return path;
        }
    }
}
