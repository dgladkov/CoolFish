using System;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using CoolFishNS.Management;
using CoolFishNS.Utilities;
using NLog;

namespace CoolFishNS
{
    /// <summary>
    ///     Interaction logic for App.xaml
    /// </summary>
    internal partial class App
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        internal static void StartUp()
        {
            AppDomain.CurrentDomain.UnhandledException += ErrorHandling.UnhandledException;
            TaskScheduler.UnobservedTaskException += ErrorHandling.TaskSchedulerOnUnobservedTaskException;
            Current.DispatcherUnhandledException += ErrorHandling.CurrentOnDispatcherUnhandledException;
            UserPreferences.LoadSettings();
            Utilities.Utilities.InitializeLoggers();
        }

        internal static void ShutDown()
        {
            UserPreferences.SaveSettings();
            LogManager.Shutdown();
        }

        [STAThread]
        public static void Main()
        {
            try
            {
                var screen = new SplashScreen("SplashScreen.png");
                screen.Show(false, true);
                var app = new App();
                StartUp();
                var window = new MainWindow(screen);
                app.Run(window);
            }
            catch (Exception ex)
            {
                Logger.Fatal("Exception thrown in Main", ex);
            }
            ShutDown();
        }
    }
}