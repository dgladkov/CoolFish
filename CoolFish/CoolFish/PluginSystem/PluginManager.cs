using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using CoolFishNS.Properties;
using CoolFishNS.Utilities;
using NLog;

namespace CoolFishNS.PluginSystem
{
    internal static class PluginManager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public static Dictionary<string, PluginContainer> Plugins = new Dictionary<string, PluginContainer>();

        internal static bool ShouldPulse = true;

        internal static Task PluginThread;

        internal static void StartPlugins()
        {
            ShouldPulse = true;

            PluginThread = Task.Factory.StartNew(PluginPulse, TaskCreationOptions.LongRunning);
        }

        internal static void StopPlugins()
        {
            ShouldPulse = false; // stop pulsing plugins
            if (PluginThread != null)
            {
                PluginThread.Wait(5000);
                PluginThread = null;
            }
        }

        internal static void ShutDownPlugins()
        {
            foreach (PluginContainer value in Plugins.Values)
            {
                try
                {
                    value.Plugin.OnShutdown(); // call all plugin shutdown procedures
                }
                catch (Exception ex)
                {
                    Logger.Error("Exception shutting down plugin: " + value.Plugin.Name, ex);
                }
            }
        }

        internal static IEnumerable<IPlugin> GetEnabledPlugins()
        {
            return
                Plugins.Where(plugin => plugin.Value != null && plugin.Value.Enabled).Select(
                    container => container.Value.Plugin);
        }

        internal static void PulseAllPlugins()
        {
            foreach (IPlugin enabledPlugin in GetEnabledPlugins())
            {
                try
                {
                    if (ShouldPulse)
                    {
                        enabledPlugin.OnPulse();
                    }
                    else
                    {
                        return;
                    }
                }
                catch (Exception ex)
                {
                    Logger.Warn(string.Format(Resources.PluginPulseException, enabledPlugin.Name), ex);
                    Plugins[enabledPlugin.Name].Enabled = false;
                }
            }
        }


        internal static void PluginPulse()
        {
            while (ShouldPulse)
            {
                PulseAllPlugins();

                Thread.Sleep(1000/60); // About 60 FPS pulses
            }
        }

        internal static void LoadPlugins()
        {
            if (!Directory.Exists(Constants.ApplicationPath.Value + "\\Plugins"))
            {
                Directory.CreateDirectory(Constants.ApplicationPath.Value + "\\Plugins");
            }
            Plugins.Clear();
            string[] plugins = Directory.GetFiles(Constants.ApplicationPath.Value + "\\Plugins", "*.dll");

            foreach (string plugin in plugins)
            {
                try
                {
                    Assembly asm = Assembly.LoadFrom(plugin);
                    foreach (Type bin in asm.GetTypes())
                    {
                        if (bin.IsClass && !bin.IsAbstract && typeof (IPlugin).IsAssignableFrom(bin))
                        {
                            var instance = Activator.CreateInstance(bin) as IPlugin;
                            if (instance != null)
                            {
                                instance.OnLoad();
                                Plugins[instance.Name] = new PluginContainer(instance);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Warn("Failed to load Plugin: " + plugin, ex);
                }
            }
        }
    }
}