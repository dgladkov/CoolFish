using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CoolFishNS.Bots;
using CoolFishNS.Management;
using CoolFishNS.PluginSystem;
using CoolFishNS.RemoteNotification.Analytics;
using CoolFishNS.Targets;
using CoolFishNS.Utilities;
using NLog;
using NLog.Config;
using NLog.Targets.Wrappers;
using MessageBox = System.Windows.Forms.MessageBox;

namespace CoolFishNS
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly IList<IBot> _bots = new List<IBot>();
        private readonly AnalyticsManager _manager;
        private readonly ICollection<CheckBox> _pluginCheckBoxesList = new Collection<CheckBox>();
        private readonly SplashScreen _splashScreen;
        private Process[] _processes = new Process[0];

        public MainWindow(SplashScreen splashScreen)
        {
            _splashScreen = splashScreen;
            _manager = new AnalyticsManager();
            InitializeComponent();
        }

        private void UpdateControlSettings()
        {
            foreach (var plugin in PluginManager.Plugins)
            {
                var cb = new CheckBox {Content = plugin.Key};
                cb.Checked += changedCheck;
                cb.Unchecked += changedCheck;
                cb.IsChecked = UserPreferences.Default.Plugins.ContainsKey(plugin.Key) &&
                               (UserPreferences.Default.Plugins[plugin.Key].IsEnabled == true);

                _pluginCheckBoxesList.Add(cb);
            }

            LogLevelCMB.SelectedIndex = UserPreferences.Default.LogLevel;
            ScriptsLB.ItemsSource = _pluginCheckBoxesList;
        }

        private void SaveControlSettings()
        {
            foreach (var script in _pluginCheckBoxesList)
            {
                UserPreferences.Default.Plugins[script.Content.ToString()] = new SerializablePlugin
                {
                    FileName = script.Content.ToString(),
                    IsEnabled = script.IsChecked
                };
            }
            UserPreferences.Default.LogLevel = LogLevelCMB.SelectedIndex;
        }

        private void RefreshProcesses()
        {
            ProcessCB.Items.Clear();

            _processes = BotManager.GetWowProcesses();

            foreach (var process in _processes)
            {
                try
                {
                    ProcessCB.Items.Add("Id: " + process.Id + " Name: " + process.MainWindowTitle);
                }
                catch (Exception ex)
                {
                    Logger.Trace("Error adding process", ex);
                }
            }
        }

        private void OnCloseWindow(object sender, MouseButtonEventArgs e)
        {
            _manager.SendAnalyticsEvent((DateTime.Now - Utilities.Utilities.StartTime).TotalMilliseconds,
                "ApplicationClose");
            Application.Current.Shutdown();
        }

        private void OnDragMoveWindow(object sender, MouseButtonEventArgs e)
        {
            try
            {
                DragMove();
            }
            catch (InvalidOperationException ex)
            {
                if (Logger.IsTraceEnabled)
                {
                    Logger.Trace("Error moving window", (Exception) ex);
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Error moving window", ex);
            }
        }

        private void OnMinimizeWindow(object sender, MouseButtonEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void LogLevelCMB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                var ordinal = LogLevelCMB.SelectedIndex == -1 ? 2 : LogLevelCMB.SelectedIndex;
                Utilities.Utilities.Reconfigure(ordinal);
            }
            catch (Exception ex)
            {
                Logger.Error("Exception thrown while changing log level", ex);
            }
        }

        private void MainWindow1_ContentRendered(object sender, EventArgs e)
        {
            UpdateControlSettings();
            StartupTask();
            _splashScreen.Close(new TimeSpan(0));
        }

        private async void StartupTask()
        {
            await Task.Run(() =>
            {
                const string endMessage =
                    "\nHey All, \n\n This is a message to let you know this is the end of CoolFish. If you are reading this, then it means that this version of CoolFish does not work and I do not intend on updating it. As of the time of this writing, I'm planning on working on something new. It might be a fish bot or something else depending on what I decide. It will likely be a bot or plugin within another framework that someone else builds. \n\n You can keep track of what I am up to by checking my twitter: https://twitter.com/TheUnknownDev \n\n If you are looking for alternative fish bots you can check ownedcore.com in the bots section.";
                Logger.Fatal(endMessage);
                MessageBox.Show(endMessage);
            });
        }

        private void Hide_BTN_Click(object sender, RoutedEventArgs e)
        {
            if (_processes.Length > ProcessCB.SelectedIndex && ProcessCB.SelectedIndex >= 0)
            {
                var window = _processes[ProcessCB.SelectedIndex].MainWindowHandle;
                NativeImports.HideWindow(window);
            }
            else
            {
                Logger.Warn("Please pick a process to hide");
            }
        }

        private void Show_BTN_Click(object sender, RoutedEventArgs e)
        {
            if (_processes.Length > ProcessCB.SelectedIndex && ProcessCB.SelectedIndex >= 0)
            {
                var window = _processes[ProcessCB.SelectedIndex].MainWindowHandle;
                if (window == IntPtr.Zero)
                {
                    var windows = NativeImports.GetProcessWindows(_processes[ProcessCB.SelectedIndex].Id);
                    foreach (var ptr in windows)
                    {
                        NativeImports.ShowWindow(ptr);
                    }
                }
                else
                {
                    NativeImports.ShowWindow(window);
                }
            }
            else
            {
                Logger.Warn("Please pick a process to show");
            }
        }

        #region EventHandlers

        private void btn_Attach_Click(object sender, EventArgs e)
        {
        }

        private void ComboBox_DropDownOpened_1(object sender, EventArgs e)
        {
        }


        private void MetroWindow_Closing_1(object sender, CancelEventArgs e)
        {
            SaveControlSettings();
        }

        private void StartBTN_Click(object sender, RoutedEventArgs e)
        {
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
        }

        private void HelpBTN_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Process.Start("http://unknowndev.github.io/CoolFish/");
            }
            catch (Exception ex)
            {
                TabControlTC.SelectedItem = MainTab;
                Logger.Info("http://unknowndev.github.io/CoolFish/");
            }
        }

        private void MainTab_Click(object sender, RoutedEventArgs e)
        {
            TabControlTC.SelectedItem = MainTab;
        }

        private void DonateBTN_Click(object sender, MouseButtonEventArgs e)
        {
            TabControlTC.SelectedItem = MainTab;
            try
            {
                Process.Start(Properties.Resources.PaypalLink);
            }
            catch (Exception ex)
            {
                Logger.Info(Properties.Resources.PaypalLink);
            }
        }

        private void DonateTab_Click(object sender, RoutedEventArgs e)
        {
            TabControlTC.SelectedItem = DonateTab;
        }

        private void SecretBTN_Click(object sender, RoutedEventArgs e)
        {
            TabControlTC.SelectedItem = MainTab;
            Logger.Info(Properties.Resources.SecretBTNMessage);
        }

        private void MetroWindow_Loaded_1(object sender, RoutedEventArgs e)
        {
            var textbox = new TextBoxTarget(OutputText)
            {
                Layout = @"[${date:format=h\:mm\:ss.ff tt}] [${level:uppercase=true}] ${message}"
            };
            var asyncWrapper = new AsyncTargetWrapper(textbox);
            LogManager.Configuration.LoggingRules.Add(new LoggingRule("*",
                LogLevel.FromOrdinal(UserPreferences.Default.LogLevel), asyncWrapper));
            LogManager.ReconfigExistingLoggers();
        }


        private void PluginsBTN_Click(object sender, RoutedEventArgs e)
        {
            TabControlTC.SelectedItem = PluginTab;
            ScriptsLB_SelectionChanged(sender, null);
        }

        private void changedCheck(object sender, RoutedEventArgs routedEventArgs)
        {
            var box = (CheckBox) sender;

            PluginManager.Plugins[box.Content.ToString()].Enabled = box.IsChecked == true;
        }

        private void ConfigBTN_Click(object sender, RoutedEventArgs e)
        {
            var item = ScriptsLB.SelectedItem;

            if (item != null)
            {
                var cb = (CheckBox) item;

                var plugin = PluginManager.Plugins.ContainsKey(cb.Content.ToString())
                    ? PluginManager.Plugins[cb.Content.ToString()]
                    : null;

                if (plugin != null)
                {
                    try
                    {
                        plugin.Plugin.OnConfig();
                    }
                    catch (Exception ex)
                    {
                        Logger.Error("An Error occurred trying to configure the plugin: " + plugin.Plugin.Name, ex);
                    }
                }
            }
        }

        private void ScriptsLB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var value = ScriptsLB.SelectedItem;

            if (value != null)
            {
                var cb = (CheckBox) value;
                var p = PluginManager.Plugins[cb.Content.ToString()].Plugin;

                DescriptionBox.Text = p.Description;
                AuthorTB.Text = "Author: " + p.Author;
                VersionTB.Text = "Version: " + p.Version;
            }
        }

        private void btn_Settings_Click(object sender, RoutedEventArgs e)
        {
        }

        private void BotBaseCB_DropDownOpened(object sender, EventArgs e)
        {
        }

        private void BotBaseCB_DropDownClosed(object sender, EventArgs e)
        {
        }

        #endregion
    }
}