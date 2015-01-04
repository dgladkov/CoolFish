using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
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
            foreach (CheckBox script in _pluginCheckBoxesList)
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

            foreach (Process process in _processes)
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
            _manager.SendAnalyticsEvent((DateTime.Now - Utilities.Utilities.StartTime).TotalMilliseconds, "ApplicationClose");
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
                int ordinal = LogLevelCMB.SelectedIndex == -1 ? 2 : LogLevelCMB.SelectedIndex;
                Utilities.Utilities.Reconfigure(ordinal);
            }
            catch (Exception ex)
            {
                Logger.Error("Exception thrown while changing log level", ex);
            }
        }

        private void MainWindow1_ContentRendered(object sender, EventArgs e)
        {
            OutputText.Text = Utilities.Utilities.GetNews() + Environment.NewLine;
            Logger.Info("CoolFish Version: " + Constants.Version.Value);
            BotBaseCB_DropDownOpened(null, null);
            BotBaseCB.SelectedIndex = 0;
            UpdateControlSettings();

            Task.Run(() =>
            {
                BotManager.StartUp();
                _manager.SendAnalyticsEvent(0, "ApplicationStart");
                Updater.Update();
                Process[] procs = BotManager.GetWowProcesses();
                if (procs.Length == 1)
                {
                    BotManager.AttachToProcess(procs.First());
                }
            });
            _splashScreen.Close(new TimeSpan(0));
        }

        #region EventHandlers

        private void btn_Attach_Click(object sender, EventArgs e)
        {
            if (_processes.Length > ProcessCB.SelectedIndex && ProcessCB.SelectedIndex >= 0) // return if we have an invalid process
            {
                BotManager.AttachToProcess(_processes[ProcessCB.SelectedIndex]);
            }
            else
            {
                Logger.Warn("Please pick a process to attach to.");
            }
        }

        private void ComboBox_DropDownOpened_1(object sender, EventArgs e)
        {
            try
            {
                RefreshProcesses();
            }
            catch (Exception ex)
            {
                Logger.Error("Error refreshing processes", ex);
            }
        }


        private void MetroWindow_Closing_1(object sender, CancelEventArgs e)
        {
            SaveControlSettings();
        }

        private void StartBTN_Click(object sender, RoutedEventArgs e)
        {
            SaveControlSettings();
            BotManager.StartActiveBot();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            BotManager.StopActiveBot();
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
            var textbox = new TextBoxTarget(OutputText) {Layout = @"[${date:format=h\:mm\:ss.ff tt}] [${level:uppercase=true}] ${message}"};
            var asyncWrapper = new AsyncTargetWrapper(textbox);
            LogManager.Configuration.LoggingRules.Add(new LoggingRule("*", LogLevel.FromOrdinal(UserPreferences.Default.LogLevel), asyncWrapper));
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
            object item = ScriptsLB.SelectedItem;

            if (item != null)
            {
                var cb = (CheckBox) item;

                PluginContainer plugin = PluginManager.Plugins.ContainsKey(cb.Content.ToString())
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
            object value = ScriptsLB.SelectedItem;

            if (value != null)
            {
                var cb = (CheckBox) value;
                IPlugin p = PluginManager.Plugins[cb.Content.ToString()].Plugin;

                DescriptionBox.Text = p.Description;
                AuthorTB.Text = "Author: " + p.Author;
                VersionTB.Text = "Version: " + p.Version;
            }
        }

        private void btn_Settings_Click(object sender, RoutedEventArgs e)
        {
            BotManager.Settings();
        }

        private void BotBaseCB_DropDownOpened(object sender, EventArgs e)
        {
            BotBaseCB.Items.Clear();
            _bots.Clear();
            foreach (var pair in BotManager.LoadedBots)
            {
                BotBaseCB.Items.Add(pair.Value.Name + " (" + pair.Value.Version + ") by " +
                                    pair.Value.Author);
                _bots.Add(pair.Value);
            }
        }

        private void BotBaseCB_DropDownClosed(object sender, EventArgs e)
        {
            if (BotBaseCB.SelectedIndex == -1)
            {
                BotBaseCB.SelectedIndex = 0;
            }

            BotManager.SetActiveBot(_bots[BotBaseCB.SelectedIndex]);
        }

        #endregion
    }
}