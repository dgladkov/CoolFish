using System.Threading;
using CoolFishNS.Management.CoolManager.HookingLua;
using CoolFishNS.Properties;
using CoolFishNS.Utilities;
using NLog;

namespace CoolFishNS.Bots.FiniteStateMachine.States
{
    /// <summary>
    ///     State which handles applying a fishing lure if we need one
    /// </summary>
    public class StateApplyBait : State
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public override int Priority
        {
            get { return (int) CoolFishEngine.StatePriority.StateApplyBait; }
        }

        public override string Name
        {
            get { return "Applying bait"; }
        }

        /// <summary>
        ///     Runs this state and apply the lure.
        /// </summary>
        public override bool Run()
        {
            if (UserPreferences.Default.BaitIndex <= 0)
            {
                return false;
            }

            string result = DxHook.ExecuteScript(Resources.NeedToApplyBait, "AppliedBait");

            if (result == "1")
            {
                Logger.Info(Name);
                Thread.Sleep(1500);
                return true;
            }
            return false;
        }
    }
}