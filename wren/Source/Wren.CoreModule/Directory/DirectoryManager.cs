using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wren.Core.Settings;
using System.IO;

namespace Wren.Core.Directory
{
    public class DirectoryManager : IDirectoryManager
    {
        ISettingsManager _settings;
        IDictionary<String, IList<String>> _registeredExtensions;

        public DirectoryManager(ISettingsManager settings)
        {
            _settings = settings;
            _registeredExtensions = new Dictionary<String, IList<String>>();
        }

        public String GetPath(EmulationContext context, String pathKey)
        {
            var paths = _settings.LoadSettings<PathSettings>(context);

            var path = paths.Paths.Where(p => p.Key == pathKey).FirstOrDefault();

            return path.Path;
        }

        public String GetPath(EmulationContext context, String pathKey, String defaultPath)
        {
            var path = GetPath(context, pathKey);

            if (path.Length == null)
            {
                AddPath(EmulationContext.Empty, pathKey, defaultPath);
                return defaultPath;
            }

            return path;
        }

        public Wren.Core.Directory.PathSettings.PathSetting[] GetPaths(EmulationContext context, String pathKey)
        {
            var paths = _settings.LoadSettings<PathSettings>(context);

            var result = paths.Paths.Where(p => p.Key == pathKey);

            return result.ToArray();
        }

        public void ClearPaths(EmulationContext emulationContext, String pathKey)
        {
            var paths = _settings.LoadSettings<PathSettings>(emulationContext);
            paths.ClearPaths(pathKey);
        }

        public void AddPath(EmulationContext context, String pathKey, String path)
        {
            var paths = _settings.LoadSettings<PathSettings>(context);
            paths.AddPath(pathKey, path);
            _settings.ApplySettings(paths);
        }

        public IEnumerable<String> GetFilePaths(EmulationContext context, String pathKey, String extensionKey)
        {
            if (!_registeredExtensions.ContainsKey(extensionKey))
                return new List<String>();

            var files = new List<String>();

            foreach (var path in GetPaths(context, pathKey))
            {
                foreach (var extension in _registeredExtensions[extensionKey])
                {
                    if (System.IO.Directory.Exists(path.Path))
                    {
                        files.AddRange(System.IO.Directory.GetFiles(path.Path, "*." + extension));
                    }
                }
            }

            return files;
        }

        public void RegisterExtensionKey(String extensionKey, String extension)
        {
            if (!_registeredExtensions.ContainsKey(extensionKey))
                _registeredExtensions.Add(extensionKey, new List<String>());

            _registeredExtensions[extensionKey].Add(extension);
        }


        public Boolean FileExists(String path, String pathKey)
        {
            var paths = GetPaths(EmulationContext.Empty, pathKey);

            foreach (var p in paths)
            {
                if (Path.GetDirectoryName(path) == p.Path)
                    return true;
            }

            return false;
        }
    }
}
