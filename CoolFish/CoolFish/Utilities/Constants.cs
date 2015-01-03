using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace CoolFishNS.Utilities
{
    /// <summary>
    ///     Defined constants that should remain the same always
    /// </summary>
    public static class Constants
    {
        /// <summary>
        ///     File name for where user preferences are saved
        /// </summary>
        public const string UserPreferencesFileName = "UserPreferences.dat";

        /// <summary>
        ///     Baits for use in WoD fishing areas
        /// </summary>
        public static readonly IList<NullableKeyValuePair<string, uint, uint>> Baits = new List<NullableKeyValuePair<string, uint, uint>>
        {
            new NullableKeyValuePair<string, uint, uint>("No Bait", 0, 0),
            new NullableKeyValuePair<string, uint, uint>("Abyssal Gulper Eel Bait", 110293, 158038),
            new NullableKeyValuePair<string, uint, uint>("Blackwater Whiptail Bait", 110294, 158039),
            new NullableKeyValuePair<string, uint, uint>("Blind Lake Sturgeon Bait", 110290, 158035),
            new NullableKeyValuePair<string, uint, uint>("Fat Sleeper Bait", 110289, 158034),
            new NullableKeyValuePair<string, uint, uint>("Fire Ammonite Bait", 110291, 158036),
            new NullableKeyValuePair<string, uint, uint>("Jawless Skulker Bait", 110274, 158031),
            new NullableKeyValuePair<string, uint, uint>("Sea Scorpion Bait", 110292, 158037),
        }.AsReadOnly();

        /// <summary>
        ///     The path to the currently executing process
        /// </summary>
        public static readonly Lazy<string> ApplicationPath = new Lazy<string>(() => Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));

        /// <summary>
        ///     The version of the currently executing process
        /// </summary>
        public static readonly Lazy<string> Version = new Lazy<string>(() => Assembly.GetExecutingAssembly().GetName().Version.ToString());
    }
}