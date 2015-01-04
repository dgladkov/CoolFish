using System;
using System.Text;
using CoolFishNS.RemoteNotification.Payloads;
using CoolFishNS.Utilities;
using NLog;
using NLog.Common;
using NLog.Targets;

namespace CoolFishNS.Targets
{
    public class RemoteTarget : TargetWithLayout
    {
        private readonly SQSManager manager = new SQSManager();

        protected override void Write(AsyncLogEventInfo[] logEvents)
        {
            foreach (AsyncLogEventInfo logEvent in logEvents)
            {
                Write(logEvent);
            }
        }

        protected override void Write(AsyncLogEventInfo logEvent)
        {
            Write(logEvent.LogEvent);
        }

        protected override void Write(LogEventInfo logEvent)
        {
            if ((logEvent.Level == LogLevel.Error || logEvent.Level == LogLevel.Fatal) && logEvent.Exception != null)
            {
                SendPayload(logEvent.Level, logEvent.FormattedMessage, logEvent.Exception);
            }
        }

        private void SendPayload(LogLevel level, string message, Exception exception)
        {
            if (exception == null)
            {
                return;
            }
            string exceptionType;
            var stackBuilder = new StringBuilder();

            do
            {
                exceptionType = exception.GetType().Name;
                stackBuilder.AppendLine(exception.Message);
                stackBuilder.AppendLine(exception.StackTrace);
                exception = exception.InnerException;
            } while (exception != null);


            manager.SendLoggingPayload(new LoggingPayload
            {
                StackTrace = stackBuilder.ToString(),
                Message = message,
                LogLevel = level.Name,
                ExceptionType = exceptionType
            });
        }
    }
}