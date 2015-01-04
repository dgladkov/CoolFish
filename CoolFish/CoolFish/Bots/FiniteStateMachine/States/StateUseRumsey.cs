using System.Threading;
using CoolFishNS.Management.CoolManager.HookingLua;
using CoolFishNS.Properties;
using CoolFishNS.Utilities;
using NLog;

namespace CoolFishNS.Bots.FiniteStateMachine.States
{
    /// <summary>
    ///     State which handles applying the Rumsey  if we need it and have it
    /// </summary>
    public class StateUseRumsey : State
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public override int Priority
        {
            get { return (int) CoolFishEngine.StatePriority.StateUseRumsey; }
        }

        public override string Name
        {
            get { return "Using Rumsey"; }
        }

        /// <summary>
        ///     Runs this state and apply the lure.
        /// </summary>
        public override bool Run()
        {
            if (!UserPreferences.Default.UseRumsey)
            {
                return false;
            }

            string res = DxHook.ExecuteScript(Resources.NeedToRunUseRumsey, "UsedRumsey");

            if (res == "1")
            {
                Logger.Info(Name);
                Thread.Sleep(1500);
                return true;
            }
            return false;
        }
    }
}