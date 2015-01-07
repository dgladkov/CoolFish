using System;
using System.Collections.Generic;
using System.IO;
using NLog;

namespace CoolFishNS.Utilities
{
    /// <summary>
    ///     Settings class in order to save the user preferences for the applications
    /// </summary>
    [Serializable]
    public class UserPreferences
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public static UserPreferences Default { get; private set; }

        public int BaitIndex;
        public NullableKeyValuePair<string, uint, uint> BaitItem;
        public bool CloseWoWOnStop;
        public bool DoBobbing = true;
        public bool DoFishing = true;
        public bool DoLoot = true;
        public bool DontLootLeft;
        public List<SerializableItem> Items = new List<SerializableItem>();
        public int LogLevel = NLog.LogLevel.Info.Ordinal;
        public bool LogoutOnStop;
        public bool LootOnlyItems;
        public int LootQuality = 1;
        public double MinutesToStop;
        public bool NoLure;
        public Dictionary<string, SerializablePlugin> Plugins = new Dictionary<string, SerializablePlugin>();
        public bool ShutdownPcOnStop;
        public bool SoundOnWhisper;
        public bool StopOnBagsFull;
        public bool StopOnNoLures;
        public bool StopOnTime;
        public bool UseRaft;
        public bool UseRumsey;
        public bool UseSpear;

        private UserPreferences()
        {
        }

        public DateTime? StopTime { get; set; }

        /// <summary>
        ///     Loads default CoolFish settings
        /// </summary>
        public static void LoadDefaults()
        {
            Logger.Info("Loading Default Settings.");
            Default = new UserPreferences();
        }

        /// <summary>
        ///     Use seralization to save preferences
        /// </summary>
        public static void SaveSettings()
        {
            try
            {
                Serializer.Serialize(Constants.UserPreferencesFileName, Default);
            }
            catch (Exception ex)
            {
                Logger.Warn("Failed to save settings to disk. Settings may be lost upon reopening CoolFish", ex);
            }
        }

        /// <summary>
        ///     Use seralization to load settings
        /// </summary>
        public static void LoadSettings()
        {
            try
            {
                var result = Serializer.DeSerialize<UserPreferences>(Constants.UserPreferencesFileName);

                if (result == null)
                {
                    LoadDefaults();
                }
                else
                {
                    Default = result;
                }
            }
            catch (FileNotFoundException)
            {
                Logger.Warn("No settings files found");
                LoadDefaults();
            }
            catch (Exception ex)
            {
                Logger.Error("Error loading settings", ex);
                LoadDefaults();
            }
        }
    }
}