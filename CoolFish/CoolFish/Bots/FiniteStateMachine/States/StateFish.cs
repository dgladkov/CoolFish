using System;
using System.Threading;
using CoolFishNS.Management;
using CoolFishNS.Management.CoolManager;
using CoolFishNS.Management.CoolManager.HookingLua;
using CoolFishNS.Management.CoolManager.Objects;
using CoolFishNS.Utilities;
using NLog;

namespace CoolFishNS.Bots.FiniteStateMachine.States
{
    /// <summary>
    ///     This state is run if we have nothing else to do and we aren't casting the "fishing" spell already
    /// </summary>
    public class StateFish : State
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly Random _random = new Random();

        public override int Priority
        {
            get { return (int) CoolFishEngine.StatePriority.StateFish; }
        }


        /// <summary>
        ///     Gets a value indicating whether [need to run].
        /// </summary>
        /// <value>
        ///     <c>true</c> if [need to run]; otherwise, <c>false</c>.
        /// </value>
        private bool NeedToRun
        {
            get
            {
                if (!UserPreferences.Default.DoFishing)
                {
                    return false;
                }
                WoWPlayerMe me = ObjectManager.Me;
                if (me == null)
                {
                    return false;
                }

                return (me.Channeling == 0) && (DxHook.ExecuteScript("loot = IsFishingLoot();", "loot") != "1");
            }
        }

        public override string Name
        {
            get { return "Casting Fishing"; }
        }

        /// <summary>
        ///     Cast fishing
        /// </summary>
        public override bool Run()
        {
            if (!NeedToRun)
            {
                return false;
            }
            Thread.Sleep(_random.Next(1000));
            Logger.Info(Name);
            DxHook.ExecuteScript("local name = GetSpellInfo(131490);  CastSpellByName(name);");


            // This is the current system uptime as per GetTime() function in lua.
            // We write this value to LastHardwareAction so that our character isn't logged out due to inactivity
            var ticks = BotManager.Memory.Read<int>(Offsets.Addresses["Timestamp"]);

            BotManager.Memory.Write(Offsets.Addresses["LastHardwareAction"], ticks);
            Thread.Sleep(500);
            return true;
        }
    }
}