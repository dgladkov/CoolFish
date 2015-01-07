using System.Collections.Generic;
using CoolFishNS.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CoolFishTests
{
    [TestClass]
    public class UserPreferencesTests
    {
        [TestMethod]
        public void TestHappyCase()
        {
            UserPreferences.LoadSettings();
            Assert.IsNotNull(UserPreferences.Default);
            Assert.IsNotNull(UserPreferences.Default.Items);
            Assert.IsNotNull(UserPreferences.Default.Plugins);
        }

        [TestMethod]
        public void TestPersistSettings()
        {
            UserPreferences.LoadSettings();
            UserPreferences.Default.LootOnlyItems = true;
            UserPreferences.Default.LogoutOnStop = true;
            UserPreferences.Default.UseRaft = true;
            UserPreferences.Default.StopOnTime = true;
            UserPreferences.Default.StopOnNoLures = true;
            UserPreferences.Default.StopOnBagsFull = true;
            UserPreferences.Default.CloseWoWOnStop = true;
            UserPreferences.Default.ShutdownPcOnStop = true;
            UserPreferences.Default.DontLootLeft = true;
            UserPreferences.Default.LootQuality = 10;
            UserPreferences.Default.SoundOnWhisper = true;
            UserPreferences.Default.UseRumsey = true;
            UserPreferences.Default.UseSpear = true;
            UserPreferences.Default.DoFishing = false;
            UserPreferences.Default.DoBobbing = false;
            UserPreferences.Default.DoLoot = false;
            var testItem = new SerializableItem {Value = "testItem"};
            UserPreferences.Default.Items = new List<SerializableItem> {testItem};

            // Save new different preferences to file
            UserPreferences.SaveSettings();

            // Reload the defaults and assert that the defaults have been loaded
            UserPreferences.LoadDefaults();
            Assert.IsFalse(UserPreferences.Default.LootOnlyItems);
            Assert.IsFalse(UserPreferences.Default.LogoutOnStop);
            Assert.IsFalse(UserPreferences.Default.UseRaft);
            Assert.IsFalse(UserPreferences.Default.StopOnTime);
            Assert.IsFalse(UserPreferences.Default.StopOnNoLures);
            Assert.IsFalse(UserPreferences.Default.StopOnBagsFull);
            Assert.IsFalse(UserPreferences.Default.CloseWoWOnStop);
            Assert.IsFalse(UserPreferences.Default.ShutdownPcOnStop);
            Assert.IsFalse(UserPreferences.Default.DontLootLeft);
            Assert.AreEqual(1, UserPreferences.Default.LootQuality);
            Assert.IsFalse(UserPreferences.Default.SoundOnWhisper);
            Assert.IsFalse(UserPreferences.Default.UseRumsey);
            Assert.IsFalse(UserPreferences.Default.UseSpear);
            // These normally default to true
            Assert.IsTrue(UserPreferences.Default.DoFishing);
            Assert.IsTrue(UserPreferences.Default.DoBobbing);
            Assert.IsTrue(UserPreferences.Default.DoLoot);

            // Load our previously persisted settings and assert that the non-defaults were loaded
            UserPreferences.LoadSettings();
            Assert.IsTrue(UserPreferences.Default.LootOnlyItems);
            Assert.IsTrue(UserPreferences.Default.LogoutOnStop);
            Assert.IsTrue(UserPreferences.Default.UseRaft);
            Assert.IsTrue(UserPreferences.Default.StopOnTime);
            Assert.IsTrue(UserPreferences.Default.StopOnNoLures);
            Assert.IsTrue(UserPreferences.Default.StopOnBagsFull);
            Assert.IsTrue(UserPreferences.Default.CloseWoWOnStop);
            Assert.IsTrue(UserPreferences.Default.ShutdownPcOnStop);
            Assert.IsTrue(UserPreferences.Default.DontLootLeft);
            Assert.AreEqual(10, UserPreferences.Default.LootQuality);
            Assert.IsTrue(UserPreferences.Default.SoundOnWhisper);
            Assert.IsTrue(UserPreferences.Default.UseRumsey);
            Assert.IsTrue(UserPreferences.Default.UseSpear);
            // We changed these to false since default is true
            Assert.IsFalse(UserPreferences.Default.DoFishing);
            Assert.IsFalse(UserPreferences.Default.DoBobbing);
            Assert.IsFalse(UserPreferences.Default.DoLoot);
        }
    }
}