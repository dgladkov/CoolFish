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

        public static readonly UserPreferences Default = new UserPreferences();

        public int BaitIndex = -1;
        public NullableKeyValuePair<string, uint, uint> BaitItem = null;
        public bool CloseWoWOnStop = false;
        public bool DoBobbing = true;
        public bool DoFishing = true;
        public bool DoLoot = true;
        public bool DontLootLeft = false;
        public List<SerializableItem> Items = new List<SerializableItem>();
        public int LogLevel = NLog.LogLevel.Info.Ordinal;
        public bool LogoutOnStop = false;
        public bool LootOnlyItems = false;
        public int LootQuality = -1;
        public double MinutesToStop = 0;
        public bool NoLure = false;
        public Dictionary<string, SerializablePlugin> Plugins = new Dictionary<string, SerializablePlugin>();
        public bool ShutdownPcOnStop = false;
        public bool SoundOnWhisper = false;
        public bool StopOnBagsFull = false;
        public bool StopOnNoLures = false;
        public bool StopOnTime = false;
        public bool UseRaft = false;
        public bool UseRumsey = false;
        public bool UseSpear = false;

        private UserPreferences()
        {
        }

        public DateTime? StopTime { get; set; }

        /// <summary>
        ///     Loads default CoolFish settings
        /// </summary>
        public void LoadDefaults()
        {
            Logger.Info("Loading Default Settings.");
            CopySettings(new UserPreferences());
        }

        private void CopySettings(UserPreferences src)
        {
            if (src == null)
            {
                src = new UserPreferences();
            }

            Items = src.Items ?? new List<SerializableItem>();
            NoLure = src.NoLure;
            BaitItem = src.BaitItem;
            BaitIndex = src.BaitIndex;
            Plugins = src.Plugins ?? new Dictionary<string, SerializablePlugin>();
            LootOnlyItems = src.LootOnlyItems;
            StopOnNoLures = src.StopOnNoLures;
            StopOnBagsFull = src.StopOnBagsFull;
            LogoutOnStop = src.LogoutOnStop;
            UseRaft = src.UseRaft;
            ShutdownPcOnStop = src.ShutdownPcOnStop;
            DontLootLeft = src.DontLootLeft;
            MinutesToStop = src.MinutesToStop;
            LootQuality = src.LootQuality;
            SoundOnWhisper = src.SoundOnWhisper;
            UseRumsey = src.UseRumsey;
            UseSpear = src.UseSpear;
            LogLevel = src.LogLevel;
            CloseWoWOnStop = src.CloseWoWOnStop;
            StopOnTime = src.StopOnTime;
            DoFishing = src.DoFishing;
            DoBobbing = src.DoBobbing;
            DoLoot = src.DoLoot;
        }

        /// <summary>
        ///     Use seralization to save preferences
        /// </summary>
        public void SaveSettings()
        {
            try
            {
                Serializer.Serialize(Constants.UserPreferencesFileName, this);
            }
            catch (Exception ex)
            {
                Logger.Warn("Failed to save settings to disk. Settings may be lost upon reopening CoolFish", ex);
            }
        }

        /// <summary>
        ///     Use seralization to load settings
        /// </summary>
        public void LoadSettings()
        {
            try
            {
                CopySettings(Serializer.DeSerialize<UserPreferences>(Constants.UserPreferencesFileName));
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