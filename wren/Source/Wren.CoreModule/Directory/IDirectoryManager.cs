using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wren.Core.Settings;

namespace Wren.Core.Directory
{
    public interface IDirectoryManager
    {
        String GetPath(EmulationContext context, String pathKey);
        String GetPath(EmulationContext context, String pathKey, String defaultPath);
        Wren.Core.Directory.PathSettings.PathSetting[] GetPaths(EmulationContext context, String pathKey);
        void AddPath(EmulationContext context, String pathKey, String path);
        IEnumerable<String> GetFilePaths(EmulationContext context, String pathKey, String extensionKey);
        void RegisterExtensionKey(String extensionKey, String extension);

        void ClearPaths(EmulationContext emulationContext, String pathKey);
        Boolean FileExists(String path, String pathKey);
    }
}
