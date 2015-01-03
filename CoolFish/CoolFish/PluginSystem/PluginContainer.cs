using System;
using NLog;

namespace CoolFishNS.PluginSystem
{
    internal class PluginContainer
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        internal IPlugin Plugin;
        private bool _enabled;

        internal PluginContainer(IPlugin plugin)
        {
            Plugin = plugin;
            _enabled = false;
        }

        internal bool Enabled
        {
            get { return _enabled; }
            set
            {
                if (_enabled != value)
                {
                    _enabled = value;

                    if (_enabled)
                    {
                        try
                        {
                            Plugin.OnEnabled();
                        }
                        catch (Exception ex)
                        {
                            Logger.Error("Exception Enabling plugin: " + Plugin.Name, ex);
                        }
                    }
                    else
                    {
                        try
                        {
                            Plugin.OnDisabled();
                        }
                        catch (Exception ex)
                        {
                            Logger.Error("Exception Enabling plugin: " + Plugin.Name, ex);
                        }
                    }
                }
            }
        }
    }
}