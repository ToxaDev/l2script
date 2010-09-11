using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using L2Script.Library;

namespace L2Script.Plugins
{
    public class Manager
    {
        public static Plugin[] GetPlugins(Assembly ass)
        {
            Stack<Plugin> PluginStack = new Stack<Plugin>();
            string[] plugins;
            string path;

            try
            {
                path = General.GetCurrentExecutingDirectory(ass);
                plugins = Directory.GetFiles(path + "\\plugins", "*.dll");
                Array.Sort(plugins);
            }
            catch (Exception ex)
            {
                Debug.Exception(" - Error Loading Plugin Directory", ex);
                return new Plugin[0];
            }

            for (int i = 0; i < plugins.Length; i++)
            {
                try
                {
                    string pluginName = plugins[i].Substring(plugins[i].LastIndexOf('\\') + 1);
                    pluginName = pluginName.Substring(0, pluginName.LastIndexOf('.'));

                    Assembly assembly = Assembly.LoadFrom(path + "\\plugins\\" + pluginName + ".dll");
                    Type[] t = assembly.GetTypes();

                    Plugin plugin = (Plugin)Activator.CreateInstance(t[0]);
                    PluginStack.Push(plugin);
                }
                catch (Exception ex)
                {
                    Debug.Exception(" - Error Loading Plugin", ex);
                }
            }
            return PluginStack.ToArray();
        }
    }
}
