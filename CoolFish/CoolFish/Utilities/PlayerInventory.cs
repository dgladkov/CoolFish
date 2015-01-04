using System;
using CoolFishNS.Management.CoolManager.HookingLua;
using CoolFishNS.Properties;
using NLog;

namespace CoolFishNS.Utilities
{
    /// <summary>
    ///     All functions relating to the Player's in game inventory is stored here
    /// </summary>
    public static class PlayerInventory
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        ///     Gets the number of free inventory slots. (Note: only cares about absolute number. Doesn't matter what bag type it
        ///     is)
        /// </summary>
        /// <value>
        ///     The number of free slots.
        /// </value>
        public static uint FreeSlots
        {
            get
            {
                string slots =
                    DxHook.ExecuteScript(
                        "CFFreeslots = 0; for i=0,4 do local count = GetContainerNumFreeSlots(i); CFFreeslots = CFFreeslots + count; end ",
                        "CFFreeslots");

                if (string.IsNullOrEmpty(slots))
                {
                    Logger.Warn("Unable to determine free bag space.");
                    return 0;
                }

                return Convert.ToUInt32(slots);
            }
        }

        /// <summary>
        ///     Gets the name of a lure to use that e
        /// </summary>
        /// <value>
        ///     The lure name
        /// </value>
        public static string LureName
        {
            get
            {
                string name = DxHook.ExecuteScript(Resources.GetLureName, "LureName");

                return !string.IsNullOrWhiteSpace(name) ? name : null;
            }
        }

        /// <summary>
        ///     Returns a bool value indicating whether the local players bags are empty or not
        /// </summary>
        /// <returns> true if bags are empty or we failed to determine the bag slots; otherwise, false</returns>
        public static bool AreBagsEmpty()
        {
            return FreeSlots == 0;
        }

        public static bool HasLures()
        {
            return LureName != null;
        }
    }
}