using System;
using CoolFishNS.Bots.FiniteStateMachine;
using CoolFishNS.Management;
using CoolFishNS.Properties;
using CoolFishNS.Utilities;
using NLog;

namespace CoolFishNS.Bots.CoolFishBot
{
    /// <summary>
    ///     Default CoolFish fishing bot that runs the provided IEngine.
    /// </summary>
    public sealed class CoolFishBot : IBot
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly CoolFishEngine _theEngine;

        private CoolFishBotSettings _window;

        /// <summary>
        ///     Constructor for default CoolFish bot. Assigns the passed Engine object.
        /// </summary>
        public CoolFishBot()
        {
            _theEngine = new CoolFishEngine();
        }

        /// <inheritdoc />
        public Version Version
        {
            get { return new Version(1, 0, 0, 0); }
        }

        /// <inheritdoc />
        public string Name
        {
            get { return "CoolFishBot"; }
        }

        /// <inheritdoc />
        public string Author
        {
            get { return Resources.BotAuthor; }
        }

        /// <inheritdoc />
        public bool IsRunning
        {
            get { return _theEngine.Running; }
        }

        /// <inheritdoc />
        /// <remarks>
        ///     The <see cref="CoolFishNS.Bots.CoolFishBot.CoolFishBot" /> implementation of this method
        ///     does some sanity checking and then starts the <see cref="CoolFishEngine" />
        /// </remarks>
        public void StartBot()
        {
            if (!BotManager.IsAttached)
            {
                Logger.Warn("Please attach to a WoW process.");
                return;
            }
            if (!BotManager.LoggedIn)
            {
                Logger.Warn("Please log into the game first.");
                return;
            }

            if (UserPreferences.Default.DoLoot && (UserPreferences.Default.LootOnlyItems &&
                                                   UserPreferences.Default.DontLootLeft))
            {
                Logger.Warn("You can't \"Loot only items on the left\" and \"Don't loot items on left\" at the same time");
                return;
            }

            if (UserPreferences.Default.DoLoot && UserPreferences.Default.LootQuality < 0)
            {
                Logger.Warn("Please select a minimum loot quality from the drop down.");
                return;
            }
            if (UserPreferences.Default.StopOnTime)
            {
                try
                {
                    var stoptime = DateTime.Now.AddMinutes(UserPreferences.Default.MinutesToStop);
                }
                catch (Exception ex)
                {
                    Logger.Warn("Invalid stop time. Please specify a valid number of minutes to stop after.", ex);
                    return;
                }
            }

            _theEngine.StartEngine();
        }

        /// <inheritdoc />
        /// <remarks>
        ///     The <see cref="CoolFishNS.Bots.CoolFishBot.CoolFishBot" /> implementation of this method
        ///     Stops the <see cref="CoolFishEngine" />
        /// </remarks>
        public void StopBot()
        {
            _theEngine.StopEngine();
        }

        /// <inheritdoc />
        /// <remarks>
        ///     The <see cref="CoolFishNS.Bots.CoolFishBot.CoolFishBot" /> implementation of this method
        ///     opens the <see cref="CoolFishNS.Bots.CoolFishBot.CoolFishBotSettings" /> window
        /// </remarks>
        public void Settings()
        {
            if (_window == null || (!_window.IsActive && !_window.IsVisible))
            {
                _window = new CoolFishBotSettings();
            }
        }
    }
}