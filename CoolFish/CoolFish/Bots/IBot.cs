using System;

namespace CoolFishNS.Bots
{
    /// <summary>
    ///     Abstract bot class that describes all Bots that can be run by the BotManager class.
    ///     Any bots that should run from CoolFish should inherit from this class and override its methods
    /// </summary>
    public interface IBot
    {
        /// <summary>
        ///     True if the bot is currently running
        /// </summary>
        /// <returns>true if running; otherwise false</returns>
        bool IsRunning { get; }

        /// <summary>
        ///     The name of the bot
        /// </summary>
        /// <returns>string name of the bot</returns>
        string Name { get; }

        /// <summary>
        ///     The author of the bot
        /// </summary>
        /// <returns>string author off the bot</returns>
        string Author { get; }

        /// <summary>
        ///     Current version of this bot in standard format
        /// </summary>
        /// <returns>System.Version type of the current version</returns>
        Version Version { get; }

        /// <summary>
        ///     Starts the bot
        /// </summary>
        void StartBot();

        /// <summary>
        ///     Stops the bot
        /// </summary>
        void StopBot();

        /// <summary>
        ///     Called whenever the user wishes to adjust the bots settings
        /// </summary>
        void Settings();
    }
}