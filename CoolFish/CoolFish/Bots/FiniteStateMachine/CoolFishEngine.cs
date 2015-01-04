using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CoolFishNS.Bots.FiniteStateMachine.States;
using CoolFishNS.Exceptions;
using CoolFishNS.Management;
using CoolFishNS.Management.CoolManager.HookingLua;
using CoolFishNS.Properties;
using CoolFishNS.Utilities;
using NLog;

namespace CoolFishNS.Bots.FiniteStateMachine
{
    /// <summary>
    ///     The main driving Engine of the Finite State Machine. This performs all the state running logic.
    /// </summary>
    public class CoolFishEngine
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private static readonly object LockObject = new object();

        private readonly SortedSet<State> _states;
        private Task _workerTask;

        public CoolFishEngine()
        {
            _states = new SortedSet<State>
            {
                new StateDoNothing(),
                new StateStopOrLogout(),
                new StateFish(),
                new StateBobbing(),
                new StateDoLoot(),
                new StateApplyLure(),
                new StateUseRaft(),
                new StateDoWhisper(),
                new StateUseRumsey(),
                new StateUseSpear(),
                new StateApplyBait()
            };
        }

        /// <summary>
        ///     True if the Engine is running. False otherwise.
        /// </summary>
        public bool Running { get; private set; }

        #region StatePriority enum

        /// <summary>
        ///     Simple priority list for all the states in our FSM
        /// </summary>
        public enum StatePriority
        {
            StateFish = 0,
            StateUseRumsey,
            StateUseSpear,
            StateApplyLure,
            StateApplyBait,
            StateUseRaft,
            StateDoNothing,
            StateBobbing,
            StateDoLoot,
            StateDoWhisper,
            StateStopOrLogout
        }

        #endregion

        /// <summary>
        ///     Starts the engine.
        /// </summary>
        public void StartEngine()
        {
            lock (LockObject)
            {
                if (Running)
                {
                    Logger.Info("The engine is running");
                    return;
                }

                Running = true;
                _workerTask = Task.Factory.StartNew(Run, TaskCreationOptions.LongRunning);
            }
        }

        /// <summary>
        ///     Stops the engine.
        /// </summary>
        public void StopEngine()
        {
            lock (LockObject)
            {
                if (!Running)
                {
                    Logger.Info("The engine is stopped");
                    return;
                }

                Running = false;
                _workerTask = null;
            }
        }

        private void InitOptions()
        {
            UserPreferences.Default.StopTime = UserPreferences.Default.StopOnTime
                ? (DateTime?) DateTime.Now.AddMinutes(UserPreferences.Default.MinutesToStop)
                : null;


            var builder = new StringBuilder();
            foreach (SerializableItem serializableItem in UserPreferences.Default.Items.Where(item => !string.IsNullOrWhiteSpace(item.Value)))
            {
                builder.Append("[\"");
                builder.Append(serializableItem.Value);
                builder.Append("\"] = true, ");
            }
            string items = builder.ToString();

            if (items.Length > 0)
            {
                items = items.Remove(items.Length - 2);
            }

            builder.Clear();

            builder.AppendLine("ItemsList = {" + items + "}");
            builder.AppendLine("LootLeftOnly = " +
                               UserPreferences.Default.LootOnlyItems.ToString().ToLower());
            builder.AppendLine("DontLootLeft = " +
                               UserPreferences.Default.DontLootLeft.ToString().ToLower());
            builder.AppendLine("LootQuality = " + UserPreferences.Default.LootQuality);
            builder.AppendLine(Resources.WhisperNotes);

            if (UserPreferences.Default.BaitItem != null)
            {
                builder.AppendLine("BaitItemId = " + UserPreferences.Default.BaitItem.Value);
                builder.AppendLine("BaitSpellId = " + UserPreferences.Default.BaitItem.ValueTwo);
            }
            builder.AppendLine("LootLog = {}; ");
            builder.AppendLine("NoLootLog = {}; ");

            DxHook.ExecuteScript(builder.ToString());
        }

        private void Pulse()
        {
            foreach (State state in _states)
            {
                if (!Running || !BotManager.LoggedIn)
                {
                    return;
                }
                if (state.Run())
                {
                    return;
                }
            }
        }

        private void Run()
        {
            Logger.Info("Started Engine");

            try
            {
                InitOptions();
                while (Running && BotManager.LoggedIn)
                {
                    Pulse();
                    Thread.Sleep(1000/60);
                }

                if (BotManager.LoggedIn)
                {
                    DxHook.ExecuteScript(
                        "if CoolFrame then CoolFrame:UnregisterAllEvents(); end print(\"|cff00ff00---Loot Log---\"); for key,value in pairs(LootLog) do local _, itemLink = GetItemInfo(key); print(itemLink .. \": \" .. value) end print(\"|cffff0000---DID NOT Loot Log---\"); for key,value in pairs(NoLootLog) do _, itemLink = GetItemInfo(key); print(itemLink .. \": \" .. value) end");
                }
            }
            catch (CodeInjectionFailedException ex)
            {
                const string msg = "Stopping bot because we could not execute code required to continue";
                if (DxHook.TriedHackyHook)
                {
                    Logger.Warn(msg, (Exception) ex);
                    Logger.Info(
                        "It seems you tried to create the hook CoolFish needs despite the mention of problems it could cause. This error is likely a result of that. It is recommended that you stop running the interfering program.");
                }
                else
                {
                    Logger.Error(msg, (Exception) ex);
                }
            }
            catch (HookNotAppliedException ex)
            {
                Logger.Warn("Stopping bot because required hook is no longer applied", (Exception) ex);
            }
            catch (AccessViolationException ex)
            {
                Logger.Warn("Stopping bot because we failed to read memory.", (Exception) ex);
            }
            catch (Exception ex)
            {
                Logger.Error("Unhandled error occurred. LoggedIn: " + BotManager.LoggedIn + " Attached: " + BotManager.IsAttached, ex);
            }

            Running = false;
            Logger.Info("Engine Stopped");
        }
    }
}