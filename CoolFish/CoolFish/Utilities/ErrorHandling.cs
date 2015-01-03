using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Threading;
using NLog;

namespace CoolFishNS.Utilities
{
    internal static class ErrorHandling
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        internal static void TaskSchedulerOnUnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs unobservedTaskExceptionEventArgs)
        {
            Logger.Error("Unhandled error has occurred on another thread. This may cause an unstable state of the application.",
                (Exception) unobservedTaskExceptionEventArgs.Exception);
        }

        internal static void CurrentOnDispatcherUnhandledException(object sender,
            DispatcherUnhandledExceptionEventArgs dispatcherUnhandledExceptionEventArgs)
        {
            Logger.Fatal("An unhandled error has occurred. Please send the log file to the developer.",
                dispatcherUnhandledExceptionEventArgs.Exception);
            dispatcherUnhandledExceptionEventArgs.Handled = true;
        }

        internal static void UnhandledException(object sender, UnhandledExceptionEventArgs ex)
        {
#if DEBUG
            Logger.Fatal("Unhandled Exception", (Exception) ex.ExceptionObject);
            MessageBox.Show(ex.ExceptionObject.ToString());
            return;
#endif

            try
            {
                const string msg = "An unhandled error has occurred. Please send the log file to the developer. The application will now exit.";
                var e = (Exception) ex.ExceptionObject;
                Logger.Fatal(msg, e);
                MessageBox.Show(msg);
            }
            catch (Exception exception)
            {
                Logger.Fatal("Failed to handle exception", exception);
            }
            App.ShutDown();
            Environment.Exit(-1);
        }
    }
}