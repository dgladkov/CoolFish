using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using NLog;
using NLog.Common;
using NLog.Targets;

namespace CoolFishNS.Targets
{
    public class TextBoxTarget : TargetWithLayout
    {
        private readonly TextBox _box;

        public TextBoxTarget(TextBox box)
        {
            _box = box;
        }

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
            if (_box == null || logEvent.Level == LogLevel.Trace || Application.Current == null)
            {
                return;
            }

            Application.Current.Dispatcher.InvokeAsync(() =>
            {
                try
                {
                    _box.AppendText(Layout.Render(logEvent) + Environment.NewLine);
                    _box.ScrollToEnd();
                }
                catch
                {
                }
            }, DispatcherPriority.Background);
        }
    }
}