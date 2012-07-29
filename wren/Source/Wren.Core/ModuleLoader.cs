using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;

namespace Wren.Core
{
    public class ModuleLoader
    {
        IList<IModule> _loadedModules;

        public ModuleLoader()
        {
            _loadedModules = new List<IModule>();
        }

        public void LoadModules(IModuleContext context)
        {
            var moduleTypes = new List<Type>();

            String path = Environment.CurrentDirectory;
            foreach (var file in Directory.GetFiles(path, "*.Module.*.dll"))
            {
                var assembly = Assembly.LoadFile(file);

                foreach (var type in assembly.GetExportedTypes())
                {
                    if (typeof(IModule).IsAssignableFrom(type))
                    {
                        moduleTypes.Add(type);
                    }
                }
            }

            List<Type> _dependencyGraph = new List<Type>();
            List<Type> _invalidModules = new List<Type>();

            while (_dependencyGraph.Count + _invalidModules.Count < moduleTypes.Count)
            {
                foreach (var type in moduleTypes)
                {
                    if (_dependencyGraph.Contains(type))
                        continue;

                    if (_invalidModules.Contains(type))
                        continue;

                    var dependencies = type.GetCustomAttributes(typeof(ModuleDependencyAttribute), false).Cast<ModuleDependencyAttribute>();

                    if (dependencies.Count() == 0)
                    {
                        _dependencyGraph.Add(type);
                        continue;
                    }

                    CheckDependencies(moduleTypes, _invalidModules, type, dependencies, _dependencyGraph);
                }
            }

            foreach (var m in _dependencyGraph)
            {
                var module = (IModule)Activator.CreateInstance(m);
                module.Load(context);
                _loadedModules.Add(module);
            }
        }

        public void UnloadModules(IModuleContext context)
        {
            foreach (var m in _loadedModules)
            {
                m.Unload(context);
            }
        }

        private void CheckDependencies(List<Type> moduleTypes, List<Type> _invalidModules, Type type, IEnumerable<ModuleDependencyAttribute> dependencies, List<Type> dependencyGraph)
        {
            foreach (var d in dependencies)
            {
                if (!moduleTypes.Contains(d.Module))
                {
                    _invalidModules.Add(type);
                    return;
                }

                if (!dependencyGraph.Contains(d.Module))
                    return;
            }

            dependencyGraph.Add(type);
        }
    }
}
