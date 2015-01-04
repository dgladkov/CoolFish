using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using CoolFishNS.Management;
using CoolFishNS.Utilities;
using NLog;
using MessageBox = System.Windows.Forms.MessageBox;

namespace CoolFishNS.Bots.CoolFishBot
{
    /// <summary>
    ///     Interaction logic for CoolFishBotSettings.xaml
    /// </summary>
    internal partial class CoolFishBotSettings
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public CoolFishBotSettings()
        {
            InitializeComponent();
        }


        private void StopTimeMinutesTB_TextChanged(object sender, TextChangedEventArgs e)
        {
            double minutes;
            if (double.TryParse(StopTimeMinutesTB.Text, out minutes))
            {
                try
                {
                    DateTime date = DateTime.Now.AddMinutes(minutes);

                    DateLBL.Content = date.ToString();
                }
                catch (Exception)
                {
                    StopTimeMinutesTB.Text = string.Empty;
                    DateLBL.Content = "Invalid Date";
                }
            }
            else
            {
                StopTimeMinutesTB.Text = string.Empty;
                DateLBL.Content = "Invalid Date";
            }
        }

        private void CheckDataGrid(DataGrid grid)
        {
            IEditableCollectionView collection = grid.Items;

            if (collection.IsEditingItem)
            {
                collection.CommitEdit();
            }
            if (collection.IsAddingNew)
            {
                collection.CommitNew();
            }
        }

        private void SaveButton_OnClick(object sender, RoutedEventArgs e)
        {
            CheckDataGrid(ItemsGrid);

            if (!BotManager.ActiveBot.IsRunning)
            {
                SaveControlSettings();
                UserPreferences.Default.SaveSettings();
                Logger.Info("CoolFishBot settings saved.");
                Close();
            }
            else
            {
                MessageBox.Show("Can't save settings while the bot is running. Please stop the bot first.");
            }
        }

        private void CloseButton_OnClick(object sender, RoutedEventArgs e)
        {
            CheckDataGrid(ItemsGrid);
            Close();
        }


        public void FillDataGrid(DataGrid grid, IEnumerable items)
        {
            try
            {
                grid.ItemsSource = null;
                grid.ItemsSource = items;
            }
            catch (InvalidOperationException)
            {
                // Shouldn't happen anymore, but in case it does silently fail since there is no impact
            }
            catch (Exception ex)
            {
                Logger.Error("Error updating datagrid", ex);
            }
        }

        private void UpdateControlSettings()
        {
            LootOnlyItemsCB.IsChecked = UserPreferences.Default.LootOnlyItems;
            StopTimeMinutesTB.Text = UserPreferences.Default.MinutesToStop.ToString();
            LogoutCB.IsChecked = UserPreferences.Default.LogoutOnStop;
            UseRaftCB.IsChecked = UserPreferences.Default.UseRaft;
            StopTimeCB.IsChecked = UserPreferences.Default.StopOnTime;
            StopNoLuresCB.IsChecked = UserPreferences.Default.StopOnNoLures;
            StopFullBagsCB.IsChecked = UserPreferences.Default.StopOnBagsFull;
            CloseAppsCB.IsChecked = UserPreferences.Default.CloseWoWOnStop;
            ShutdownCB.IsChecked = UserPreferences.Default.ShutdownPcOnStop;
            DontLootCB.IsChecked = UserPreferences.Default.DontLootLeft;
            QualityCMB.SelectedIndex = UserPreferences.Default.LootQuality + 1;
            SoundWhisperCB.IsChecked = UserPreferences.Default.SoundOnWhisper;
            UseRumseyCB.IsChecked = UserPreferences.Default.UseRumsey;
            UseSpearCB.IsChecked = UserPreferences.Default.UseSpear;
            CastFishingCB.IsChecked = UserPreferences.Default.DoFishing;
            ClickBobberCB.IsChecked = UserPreferences.Default.DoBobbing;
            DoLootingCB.IsChecked = UserPreferences.Default.DoLoot;
            NoLureCB.IsChecked = UserPreferences.Default.NoLure;
            BaitCMB.SelectedIndex = UserPreferences.Default.BaitIndex;
            FillDataGrid(ItemsGrid, UserPreferences.Default.Items);
        }

        private void SaveControlSettings()
        {
            UserPreferences.Default.LootOnlyItems = LootOnlyItemsCB.IsChecked.GetValueOrDefault();
            UserPreferences.Default.LogoutOnStop = LogoutCB.IsChecked.GetValueOrDefault();
            UserPreferences.Default.UseRaft = UseRaftCB.IsChecked.GetValueOrDefault();
            UserPreferences.Default.StopOnTime = StopTimeCB.IsChecked.GetValueOrDefault();
            UserPreferences.Default.StopOnNoLures = StopNoLuresCB.IsChecked.GetValueOrDefault();
            UserPreferences.Default.StopOnBagsFull = StopFullBagsCB.IsChecked.GetValueOrDefault();
            UserPreferences.Default.CloseWoWOnStop = CloseAppsCB.IsChecked.GetValueOrDefault();
            UserPreferences.Default.ShutdownPcOnStop = ShutdownCB.IsChecked.GetValueOrDefault();
            UserPreferences.Default.DontLootLeft = DontLootCB.IsChecked.GetValueOrDefault();
            UserPreferences.Default.LootQuality = QualityCMB.SelectedIndex - 1;
            UserPreferences.Default.SoundOnWhisper = SoundWhisperCB.IsChecked.GetValueOrDefault();
            UserPreferences.Default.UseRumsey = UseRumseyCB.IsChecked.GetValueOrDefault();
            UserPreferences.Default.UseSpear = UseSpearCB.IsChecked.GetValueOrDefault();
            UserPreferences.Default.DoFishing = CastFishingCB.IsChecked.GetValueOrDefault();
            UserPreferences.Default.DoBobbing = ClickBobberCB.IsChecked.GetValueOrDefault();
            UserPreferences.Default.DoLoot = DoLootingCB.IsChecked.GetValueOrDefault();
            UserPreferences.Default.NoLure = NoLureCB.IsChecked.GetValueOrDefault();
            UserPreferences.Default.Items = new List<SerializableItem>(ItemsGrid.ItemsSource.Cast<SerializableItem>());
            double result;
            if (double.TryParse(StopTimeMinutesTB.Text, out result))
            {
                UserPreferences.Default.MinutesToStop = result;
            }
            else
            {
                Logger.Warn("Invalid Stop Time.");
                UserPreferences.Default.MinutesToStop = -1;
            }

            UserPreferences.Default.BaitIndex = BaitCMB.SelectedIndex;
            UserPreferences.Default.BaitItem = BaitCMB.SelectedItem as NullableKeyValuePair<string, uint, uint>;
        }


        private void UIElement_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void SettingsWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                var col1 = new DataGridTextColumn {Binding = new Binding("Value"), Header = "ItemId or Name", Width = 150};
                col1.SetValue(NameProperty, "ItemColumn");

                ItemsGrid.Columns.Add(col1);

                BaitCMB.ItemsSource = null;
                BaitCMB.ItemsSource = Constants.Baits;

                UpdateControlSettings();
            }
            catch (Exception ex)
            {
                Logger.Error("Error thrown while updating controls", ex);
            }
        }
    }
}