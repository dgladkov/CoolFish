using System;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Forms;
using CoolFishNS.GitHub;
using CoolFishNS.Management;
using NLog;

namespace CoolFishNS.Utilities
{
    /// <summary>
    ///     All Web Request related functions are in this class (Updater and News).
    /// </summary>
    internal static class Updater
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        internal static string GetNews()
        {
            try
            {
                using (var client = new WebClient {Proxy = WebRequest.DefaultWebProxy})
                {
                    return client.DownloadString("http://unknowndev.github.io/CoolFish/Message.txt");
                }
            }
            catch (Exception ex)
            {
                Logger.Warn("Could not connect to news feed. Website is down?", ex);
            }
            return string.Empty;
        }

        internal static async Task Update()
        {
#if DEBUG
            return;
#endif

            await Task.Run(() =>
            {
                try
                {
                    Logger.Info("Checking for a new version of CoolFish");
                    var latestInfo = GithubAPI.GetLatestVersionId();
                    if (latestInfo.HasValue)
                    {
                        Logger.Info("A new version of CoolFish was found. Downloading the latest version.");
                        MessageBox.Show("A new version of CoolFish was found. We will now update to the latest version", "Update Required",
                            MessageBoxButtons.OK);
                        GithubAPI.DownloadAsset(latestInfo.Value);

                        Logger.Info("Download Complete.");
                        MessageBox.Show("The download has completed. Please extract the zip file in the CoolFish directory. This application will now close.");
                        BotManager.SafeShutdown();
                    }
                    else
                    {
                        Logger.Info("No new versions available.");
                    }
                }
                catch (Exception ex)
                {
                    Logger.Warn("Exception thrown while trying to check for a new version", ex);
                }
            });
        }
    }
}