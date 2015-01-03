using System.Threading;
using CoolFishNS.Management.CoolManager.HookingLua;
using CoolFishNS.Utilities;
using NLog;

namespace CoolFishNS.Bots.FiniteStateMachine.States
{
    /// <summary>
    ///     State which handles applying the Spear if we need it and have it
    /// </summary>
    public class StateUseSpear : State
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public override int Priority
        {
            get { return (int) CoolFishEngine.StatePriority.StateUseSpear; }
        }

        /// <summary>
        ///     Gets a value indicating whether we [need to run] this state or not.
        /// </summary>
        /// <value>
        ///     <c>true</c> if we [need to run]; otherwise, <c>false</c>.
        /// </value>
        private bool NeedToRun
        {
            get
            {
                string res =
                    DxHook.ExecuteScript("local startTime, duration,enable = GetItemCooldown(88535) " +
                                         " expires =  GetTime() - (startTime + duration) " +
                                         " if expires > 0 and enable == 1 then expires=1 else expires = 0 end; ",
                        "expires");


                return res == "1";
            }
        }

        public override string Name
        {
            get { return "Using Spear"; }
        }

        /// <summary>
        ///     Runs this state and apply the lure.
        /// </summary>
        public override bool Run()
        {
            if (!UserPreferences.Default.UseSpear)
            {
                return false;
            }

            if (!NeedToRun)
            {
                return false;
            }
            Logger.Info(Name);

            DxHook.ExecuteScript("SpellStopCasting() " +
                                 " CFweaponId = GetInventoryItemID(\"player\", 16); " +
                                 " EquipItemByName(88535);");
            Thread.Sleep(1000);

            DxHook.ExecuteScript("RunMacroText(\"/use 16 \"); ");


            Thread.Sleep(1500);

            DxHook.ExecuteScript("EquipItemByName(CFweaponId);");

            Thread.Sleep(500);
            return true;
        }
    }
}