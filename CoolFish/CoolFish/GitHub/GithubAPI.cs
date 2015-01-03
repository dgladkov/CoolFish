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

        internal static Gist CreateGist(string description, string filename, string contents)
        {
            try
            {
                var gist = new NewGist {Description = description, Public = false};
                gist.Files.Add(filename, contents);
                return Client.Gist.Create(gist).Result;
            }
            catch (RateLimitExceededException ex)
            {
                Logger.Warn("Failed to create gist due to exceeding hourly requests. " + ex.Reset, ex);
            }
            catch (Exception ex)
            {
                Logger.Warn("Failed to create Gist", ex);
            }
            return null;
        }

        internal static Tuple<int, string> GetLatestVersionInfo()
        {
            int latestId = -1;
            var latestTag = (string) null;
            try
            {
                IReadOnlyList<Release> releases = Client.Release.GetAll("unknowndev", "CoolFish").Result;
                var latestRelease = new Version(Constants.Version.Value);

                foreach (Release release in releases)
                {
                    try
                    {
                        var version = new Version(release.TagName);
                        if (version > latestRelease)
                        {
                            latestTag = release.TagName;
                            latestId = release.Id;
                            latestRelease = version;
                        }
                    }
                    catch (Exception)
                    {
                        //Skip this release because the tagname is probably bad
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
            return latestId > -1 ? new Tuple<int, string>(latestId, latestTag) : null;
        }

        internal static void DownloadAsset(int id, string tag)
        {
            try
            {
                ;
                IReadOnlyList<ReleaseAsset> assets = Client.Release.GetAssets("unknowndev", "CoolFish", id).Result;
                if (assets.Any())
                {
                    using (var client = new WebClient())
                    {
                        Logger.Info("Downloading File...");
                        client.DownloadFile(
                            new Uri(string.Format("https://github.com/unknowndev/CoolFish/releases/download/{0}/{1}", tag, assets[0].Name)),
                            assets[0].Name);
                        ClientOnDownloadFileCompleted(assets[0].Name);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Error downloading new version", ex);
                MessageBox.Show("An error occurred while downloading the new version. Please try again or visit the website to download it manually.");
            }
        }

        private static void ClientOnDownloadFileCompleted(string fileName)
        {
            try
            {
                Logger.Info("Download Complete.");
                BotManager.DetachFromProcess();
                App.ShutDown();
                DeleteOldSetupFiles();
                ZipFile.ExtractToDirectory(fileName, Constants.ApplicationPath.Value);
                MessageBox.Show("The update was complete. CoolFish will restart now.");
                Process.Start("CoolFish.exe");
            }
            catch (Exception ex)
            {
                Logger.Error("Failed to extract and run the latest version. Please download and run the latest version manually.", ex);
                MessageBox.Show("Failed to extract and run the latest version. Please download and run the latest version manually.");
            }

            Process.GetCurrentProcess().Kill();
        }

        private static void DeleteOldSetupFiles()
        {
            var info = new DirectoryInfo(Constants.ApplicationPath.Value);
            string backup = Constants.ApplicationPath.Value + "\\Backup\\";
            if (!Directory.Exists(backup))
            {
                Directory.CreateDirectory(backup);
            }

            // Clean up old setup files if they exist
            IEnumerable<FileInfo> files =
                info.GetFiles().Where(file => !file.Name.Equals(Constants.UserPreferencesFileName) && !file.Name.EndsWith(".zip"));

            foreach (FileInfo file in files)
            {
                string newFile = backup + file.Name + ".bak";
                if (File.Exists(newFile))
                {
                    File.Delete(newFile);
                }
                file.MoveTo(newFile);
            }
        }
    }
}