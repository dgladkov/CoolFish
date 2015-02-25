using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Threading;
using System.Windows.Forms;
using CoolFishNS.Management;
using CoolFishNS.Utilities;
using NLog;
using Octokit;

namespace CoolFishNS.GitHub
{
    internal static class GithubAPI
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        internal static GitHubClient Client = new GitHubClient(new ProductHeaderValue("CoolFish"));

        internal static int? GetLatestVersionId()
        {
            int? latestId = null;
            try
            {
                IReadOnlyList<Release> releases = Client.Release.GetAll("unknowndev", "CoolFish").Result;
                var latestRelease = new Version(Constants.Version.Value);

                foreach (Release release in releases)
                {
                        var version = new Version(release.TagName);
                        if (version > latestRelease)
                        {
                            latestId = release.Id;
                            latestRelease = version;
                        }
                }
            }
            catch (RateLimitExceededException ex)
            {
                Logger.Warn("Failed to check for a new version due to exceeding hourly requests. " + ex.Reset, ex);
            }
            catch (Exception ex)
            {
                Logger.Warn("Failed to check for new version", ex);
            }
            return latestId;
        }

        internal static string DownloadAsset(int id)
        {

                
                var assets = Client.Release.GetAssets("unknowndev", "CoolFish", id).Result;
                if (assets.Any())
                {
                    using (var client = new WebClient())
                    {
                        Logger.Info("Downloading File...");
                        client.DownloadFile(
                            new Uri(assets[0].BrowserDownloadUrl),
                            assets[0].Name);
                        return assets[0].Name;
                    }
                }
            return null;
        }
    }
}