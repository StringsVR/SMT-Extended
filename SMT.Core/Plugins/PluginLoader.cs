using SMT.Core.Logging;
using SMT.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace SMT.Core.Plugins
{
    public class PluginLoader
    {
        public static List<IPlugin> LoadPlugins(IMain main, string pluginFolder)
        {
            var plugins = new List<IPlugin>();

            foreach (var dll in Directory.GetFiles(pluginFolder, "*.dll"))
            {
                try
                {
                    var assembly = Assembly.LoadFrom(dll);
                    var types = assembly.GetTypes()
                        .Where(t => typeof(IPlugin).IsAssignableFrom(t)
                                    && !t.IsInterface && !t.IsAbstract);

                    foreach (var type in types)
                    {
                        var plugin = Activator.CreateInstance(type) as IPlugin;
                        plugin.Initialize(main);
                        plugins.Add(plugin);
                    }
                }
                catch (Exception ex)
                {
                    CLogger.Warning($"Failed to load plugin {dll}: {ex.Message}");
                }
            }

            return plugins;
        }
    }
}
