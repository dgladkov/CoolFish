using System.Threading;
using CoolFishNS.Management.CoolManager.HookingLua;
using CoolFishNS.Properties;
using CoolFishNS.Utilities;
using NLog;

namespace CoolFishNS.Bots.FiniteStateMachine.States
{
    /// <summary>
    ///     Run this state if we want to use water walking or Angler's raft item to fish in open water
    /// </summary>
    public class StateUseRaft : State
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public override int Priority
        {
            get { return (int) CoolFishEngine.StatePriority.StateUseRaft; }
        }

        public override string Name
        {
            get { return "Using water walk"; }
        }

        /// <summary>
        ///     Execute Lua code to use Raft/Water Walking. See UseRaft.lua in Resources for code.
        /// </summary>
        public override bool Run()
        {
            if (!UserPreferences.Default.UseRaft)
            {
                return false;
            }
            string res = DxHook.ExecuteScript(Resources.UseRaft, "UsedRaft");

            if (res == "1")
            {
                Logger.Info(Name);
                Thread.Sleep(1000);
                return true;
            }
            return false;
        }
    }
}