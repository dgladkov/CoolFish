using System;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using NLog;

namespace CoolFishNS.RemoteNotification.Analytics
{
    public class UserIdentity
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private static readonly object SyncRoot = new Object();
        private static readonly Regex Matcher = new Regex(@"\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}");
        private static volatile UserIdentity _instance;
        public readonly string UserId;
        public string ExternalIp;

        private UserIdentity()
        {
            UserId = GetUserId();
            ExternalIp = GetExternalIp();
        }

        public static UserIdentity Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (SyncRoot)
                    {
                        if (_instance == null)
                        {
                            _instance = new UserIdentity();
                        }
                    }
                }

                return _instance;
            }
        }

        private static string GetSaveFilePath()
        {
            return Path.Combine(Environment.GetFolderPath(
                Environment.SpecialFolder.ApplicationData), "UserIdentity.dat");
        }

        private static string GetUserId()
        {
            try
            {
                string path = GetSaveFilePath();
                if (File.Exists(path))
                {
                    return File.ReadAllText(path);
                }
                string id = Guid.NewGuid().ToString("N");
                File.WriteAllText(path, id);
                return id;
            }
            catch (Exception ex)
            {
                Logger.Error("Failed to retrieve id", ex);
                return Guid.NewGuid().ToString("N");
            }
        }


        private static string GetExternalIp()
        {
            try
            {
                using (var client = new WebClient {Proxy = WebRequest.DefaultWebProxy})
                {
                    string response = client.DownloadString("http://checkip.dyndns.org/");
                    Match match = Matcher.Match(response);
                    if (match.Success)
                    {
                        return match.Value;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Warn("Error performing web request", ex);
            }
            return "0.0.0.0";
        }
    }
}