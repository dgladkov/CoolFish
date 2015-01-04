using CoolFishNS.Management.CoolManager.HookingLua;
using CoolFishNS.Properties;
using CoolFishNS.Utilities;
using NLog;

namespace CoolFishNS.Bots.FiniteStateMachine.States
{
    /// <summary>
    ///     This state is run if we caught a fish and have items in the loot window
    /// </summary>
    public class StateDoLoot : State
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public override int Priority
        {
            get { return (int) CoolFishEngine.StatePriority.StateDoLoot; }
        }

        public override string Name
        {
            get { return "Looting items"; }
        }

        /// <summary>
        ///     If the loot window is open, then execute the lua to loot the items. See DoLoot.lua in the Resources for code
        /// </summary>
        public override bool Run()
        {
            if (!UserPreferences.Default.DoLoot)
            {
                return false;
            }
            if (DxHook.ExecuteScript(Resources.DoLoot, "StateResult") == "1")
            {
                Logger.Info(Name);
                return true;
            }
            return false;
        }
    }
}