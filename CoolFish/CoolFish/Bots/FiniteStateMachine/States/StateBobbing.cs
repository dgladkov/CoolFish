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
    ///     This state handles if the fishing bobber actively has a fish on the line.
    /// </summary>
    public class StateBobbing : State
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private static readonly Random Random = new Random();

        public override int Priority
        {
            get { return (int) CoolFishEngine.StatePriority.StateBobbing; }
        }

        public override string Name
        {
            get { return "Caught a fish."; }
        }

        private static bool IsBobber(WoWGameObject objectToCheck)
        {
            return objectToCheck.CreatedBy.Equals(ObjectManager.PlayerGuid) && objectToCheck.IsBobbing;
        }

        /// <summary>
        ///     Interact with the bobber so we can catch the fish
        /// </summary>
        public override bool Run()
        {
            if (!UserPreferences.Default.DoBobbing)
            {
                return false;
            }

            WoWGameObject bobber = ObjectManager.GetSpecificObject(IsBobber, WoWObject.ToWoWGameObject);

            if (bobber == null)
            {
                return false;
            }

            Logger.Info(Name);

            Thread.Sleep(Random.Next(500, 1750));
            BotManager.Memory.Write(Offsets.Addresses["MouseOverGUID"], bobber.Guid);
            Logger.Info("Clicking bobber");
            DxHook.ExecuteScript("InteractUnit(\"mouseover\");");
            Thread.Sleep(1000);
            return true;
        }
    }
}