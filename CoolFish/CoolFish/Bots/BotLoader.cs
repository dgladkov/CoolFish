using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using CoolFishNS.Utilities;
using NLog;

namespace CoolFishNS.Bots
{
    internal class BotLoader
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private static readonly Lazy<string> BotsDirectory = new Lazy<string>(() => Path.Combine(Constants.ApplicationPath.Value, "Bots"));
        private const string OldCoolFishBot = "\\CoolFishBot.dll";
        private const string OldCoolFishBotPdb = "\\CoolFishBot.pdb";

        internal List<IBot> LoadBots()
        {
            var botList = new List<IBot>();
            if (!Directory.Exists(BotsDirectory.Value))
            {
                Directory.CreateDirectory(BotsDirectory.Value);
                Logger.Warn("Bot directory did not exist. No bots loaded.");
                return botList;
            }
            DeleteLegacy();

            var bots = Directory.GetFiles(BotsDirectory.Value, "*.dll");

            foreach (string bot in bots)
            {
                try
                {
                    Assembly asm = Assembly.LoadFrom(bot);
                    foreach (Type bin in asm.GetTypes())
                    {
                        if (bin.IsClass && !bin.IsAbstract && typeof(IBot).IsAssignableFrom(bin))
                        {
                            var instance = Activator.CreateInstance(bin) as IBot;
                            botList.Add(instance);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Warn("Failed to load file: " + bot, ex);
                }
            }
            return botList;
        }

        private void DeleteLegacy()
        {
            try
            {
                if (File.Exists(BotsDirectory + OldCoolFishBot))
                {
                    File.Delete(BotsDirectory + OldCoolFishBot);
                }
                if (File.Exists(BotsDirectory + OldCoolFishBotPdb))
                {
                    File.Delete(BotsDirectory + OldCoolFishBotPdb);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
            }
        }
    }
}
