using System;
using CoolFishNS.PluginSystem;

namespace TestPlugin
{
    public class TestPlugin : IPlugin
    {
        private ConfigWindow _window = new ConfigWindow();

        #region IPlugin Members

        /// <summary>
        ///     Show our config window and add handlers to buttons
        /// </summary>
        public void OnConfig()
        {
            if (!_window.IsVisible)
            {
                _window = new ConfigWindow();
                _window.Show();
            }
        }

        public void OnEnabled()
        {
        }

        public void OnDisabled()
        {
        }

        public void OnLoad()
        {
        }

        public void OnPulse()
        {
        }

        public void OnShutdown()
        {
        }

        public string Name
        {
            get { return "TestPlugin"; }
        }

        public Version Version
        {
            get { return new Version(1, 0, 0, 1); }
        }

        public string Author
        {
            get { return "~Unknown~"; }
        }

        public string Description
        {
            get { return "Example test plugin that does almost nothing"; }
        }

        #endregion
    }
}