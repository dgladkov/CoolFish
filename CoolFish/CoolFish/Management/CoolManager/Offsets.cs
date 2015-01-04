using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using CoolFishNS.Properties;
using NLog;

namespace CoolFishNS.Management.CoolManager
{
    /// <summary>
    ///     A collection of offsets used for memory reading and writing.
    /// </summary>
    public static class Offsets
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private static Dictionary<string, IntPtr> _addresses = new Dictionary<string, IntPtr>();

        /// <summary>
        ///     The offsets that the bot uses to read from the WoW process.
        ///     Returns a copy of the internally found offsets to assure it is unmodified
        /// </summary>
        public static Dictionary<string, IntPtr> Addresses
        {
            get { return new Dictionary<string, IntPtr>(_addresses); }
        }

        /// <summary>
        ///     Find the offsets for the <see cref="Process" /> opened by <see cref="BotManager.Memory" />
        /// </summary>
        /// <param name="proc">A process to find offsets</param>
        /// <returns>
        ///     true if we find all offsets successfully; otherwise false. <see cref="Offsets.Addresses" /> will still
        ///     contain the offsets that were found despite error.
        /// </returns>
        internal static bool FindOffsets(Process proc)
        {
            try
            {
                if (proc == null || proc.HasExited)
                {
                    Logger.Info("Invalid process");
                    return false;
                }
                var addresses = new Dictionary<string, IntPtr>(_addresses.Count);
                var fp = new FindPattern(new MemoryStream(Encoding.UTF8.GetBytes(Resources.Patterns)), proc);
                var baseAddr = (int) proc.MainModule.BaseAddress;

                foreach (var pattern in fp.Patterns)
                {
                    switch (pattern.Key)
                    {
                        case "FrameScript_ExecuteBuffer":
                        case "FrameScript_GetLocalizedText":
                        case "ClntObjMgrGetActivePlayerObj":
                            addresses.Add(pattern.Key, fp.Get(pattern.Key));
                            break;
                        default:
                            addresses.Add(pattern.Key, fp.Get(pattern.Key) - baseAddr);
                            break;
                    }
                }

                if (Logger.IsDebugEnabled)
                {
                    Logger.Debug("Base: 0x" + baseAddr.ToString("X"));
                    foreach (var address in addresses)
                    {
                        Logger.Debug(address.Key + ": 0x" + (address.Value - baseAddr).ToString("X"));
                    }
                }

                _addresses = addresses;
                return fp.NotFoundCount == 0;
            }
            catch (FileNotFoundException ex)
            {
                if (ex.FileName.Contains("fasmdll_managed"))
                {
                    Logger.Fatal(
                        "You have not downloaded a required prerequisite for CoolFish. Please visit the following download page for the Visual C++ Redistributable: http://www.microsoft.com/en-us/download/details.aspx?id=40784 (Download the vcredist_x86.exe when asked)");
                }
                else
                {
                    throw;
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Exception thrown while finding offsets. ", ex);
            }
            _addresses = new Dictionary<string, IntPtr>();
            return false;
        }

        /****
         * Missing XML Documentation warnings by ReSharper are being suppressed in
         * this file with the use of the #pragma statement below.
         */

#pragma warning disable 1591

        /// <summary>
        ///     Memory locations specific to the WoWPlayer type.
        ///     Version: 5.4
        /// </summary>
        internal enum WoWPlayer
        {
            NameStore = 0xC8B574, //6.0.2
            NameMask = 0x24,
            NameBase = 0x15,
            NameString = 0x21,

            IsCasting = 0xF38, //6.0.2
            IsChanneling = 0xF58, //6.0.2
            Speed1 = 0x124,
            Speed2 = 0x88
        }

        /// <summary>
        ///     Memory locations for ClickToMove
        ///     Version: 5.3
        /// </summary>
        internal enum CTM
        {
            CTM_Push = 0x1C,
            CTM_X = 0x84,
            CTM_Y = CTM_X + 0x4,
            CTM_Z = CTM_X + 0x8,
            CTM_GUID = 0x20,
            CTM_Distance = 0xC
        }

        /// <summary>
        ///     Memory locations for reading WoWUnit stuff
        ///     Version: 5.4.0
        /// </summary>
        internal enum WoWUnit
        {
            // PowerOffset = 0xC7C91C, TODO: out of date
            Name1 = 0x9AC,
            Name2 = 0x6C,
        }

        /// <summary>
        ///     Memory locations specific to the ObjectManager.
        ///     Version: 6.0.2
        /// </summary>
        internal enum ObjectManager
        {
            LocalGuid = 0xF8,
            FirstObject = 0xD8,
            NextObject = 0x3C,
        }


        /// <summary>
        ///     Memory locations specific to the WowObject type.
        ///     Version: 5.4.0
        /// </summary>
        internal enum WowObject
        {
            X = 0x830,
            Y = X + 0x4,
            Z = Y + 0x4,
            RotationOffset = Z + 0x4,
            GameObjectX = 0x1F4, //5.4.2
            GameObjectY = GameObjectX + 0x4,
            GameObjectZ = GameObjectX + 0x8,
            Pitch = X + 0x14,
            Rotation = X + 0x10,
            TargetGuid = 0x13,
            VisibleGuid = 0x28,
            Type = 0x0C
        }

        /// <summary>
        ///     Memory locations specific to the WowGameObject type.
        ///     Version: 6.0.2
        /// </summary>
        internal enum WowGameObject : uint
        {
            Name1 = 0x26C,
            Name2 = 0xB4,
            AnimationState = 0x104,
        }

        #region <Flags>

        [Flags]
        public enum ClassFlags : uint
        {
            None = 0,
            Warrior = 1,
            Paladin = 2,
            Hunter = 3,
            Rogue = 4,
            Priest = 5,
            DeathKnight = 6,
            Shaman = 7,
            Mage = 8,
            Warlock = 9,
            Monk = 10,
            Druid = 11,
        }

        [Flags]
        public enum RaceFlags : uint
        {
            Human = 1,
            Orc,
            Dwarf,
            NightElf,
            Undead,
            Tauren,
            Gnome,
            Troll,
            Goblin,
            BloodElf,
            Draenei,
            FelOrc,
            Naga,
            Broken,
            Skeleton = 15,
            Pandaren = 24,
        }

        [Flags]
        internal enum CorpseFlags
        {
            CORPSE_FLAG_NONE = 0x00,
            CORPSE_FLAG_BONES = 0x01,
            CORPSE_FLAG_UNK1 = 0x02,
            CORPSE_FLAG_UNK2 = 0x04,
            CORPSE_FLAG_HIDE_HELM = 0x08,
            CORPSE_FLAG_HIDE_CLOAK = 0x10,
            CORPSE_FLAG_LOOTABLE = 0x20
        }

        [Flags]
        internal enum UnitDynamicFlags
        {
            None = 0,
            Lootable = 0x1,
            TrackUnit = 0x2,
            TaggedByOther = 0x4,
            TaggedByMe = 0x8,
            SpecialInfo = 0x10,
            Dead = 0x20,
            ReferAFriendLinked = 0x40,
            IsTappedByAllThreatList = 0x80,
        }

        [Flags]
        internal enum UnitFlags : uint
        {
            None = 0,
            Sitting = 0x1,

            //SelectableNotAttackable_1 = 0x2,
            Influenced = 0x4, // Stops movement packets

            PlayerControlled = 0x8, // 2.4.2
            Totem = 0x10,
            Preparation = 0x20, // 3.0.3
            PlusMob = 0x40, // 3.0.2

            //SelectableNotAttackable_2 = 0x80,
            NotAttackable = 0x100,

            //Flag_0x200 = 0x200,
            Looting = 0x400,

            PetInCombat = 0x800, // 3.0.2
            PvPFlagged = 0x1000,
            Silenced = 0x2000, //3.0.3

            //Flag_14_0x4000 = 0x4000,
            //Flag_15_0x8000 = 0x8000,
            //SelectableNotAttackable_3 = 0x10000,
            Pacified = 0x20000, //3.0.3

            Stunned = 0x40000,
            CanPerformAction_Mask1 = 0x60000,
            Combat = 0x80000, // 3.1.1
            TaxiFlight = 0x100000, // 3.1.1
            Disarmed = 0x200000, // 3.1.1
            Confused = 0x400000, //  3.0.3
            Fleeing = 0x800000,
            Possessed = 0x1000000, // 3.1.1
            NotSelectable = 0x2000000,
            Skinnable = 0x4000000,
            Mounted = 0x8000000,

            //Flag_28_0x10000000 = 0x10000000,
            Dazed = 0x20000000,

            Sheathe = 0x40000000,

            //Flag_31_0x80000000 = 0x80000000,
        }

        #endregion <Flags>

        #region <Descriptors>

        public enum WoWObjectFields
        {
            Guid = 0x0,
            Data = 0x10,
            Type = 0x20,
            EntryID = 0x24,
            DynamicFlags = 0x28,
            Scale = 0x2C,
        };

        public enum WoWItemFields
        {
            Owner = 0x30,
            ContainedIn = 0x40,
            Creator = 0x50,
            GiftCreator = 0x60,
            StackCount = 0x70,
            Expiration = 0x74,
            SpellCharges = 0x78,
            DynamicFlags = 0x8C,
            Enchantment = 0x90,
            PropertySeed = 0x12C,
            RandomPropertiesID = 0x130,
            Durability = 0x134,
            MaxDurability = 0x138,
            CreatePlayedTime = 0x13C,
            ModifiersMask = 0x140,
            Context = 0x144,
        };

        public enum WoWContainerFields
        {
            Slots = 0x148,
            NumSlots = 0x388,
        };

        public enum WoWUnitFields
        {
            Charm = 0x30,
            Summon = 0x40,
            Critter = 0x50,
            CharmedBy = 0x60,
            SummonedBy = 0x70,
            CreatedBy = 0x80,
            DemonCreator = 0x90,
            Target = 0xA0,
            BattlePetCompanionGUID = 0xB0,
            BattlePetDBID = 0xC0,
            ChannelObject = 0xC8,
            ChannelSpell = 0xD8,
            SummonedByHomeRealm = 0xDC,
            Sex = 0xE0,
            DisplayPower = 0xE4,
            OverrideDisplayPowerID = 0xE8,
            Health = 0xEC,
            Power = 0xF0,
            MaxHealth = 0x108,
            MaxPower = 0x10C,
            PowerRegenFlatModifier = 0x124,
            PowerRegenInterruptedFlatModifier = 0x13C,
            Level = 0x154,
            EffectiveLevel = 0x158,
            FactionTemplate = 0x15C,
            VirtualItemID = 0x160,
            Flags = 0x16C,
            Flags2 = 0x170,
            Flags3 = 0x174,
            AuraState = 0x178,
            AttackRoundBaseTime = 0x17C,
            RangedAttackRoundBaseTime = 0x184,
            BoundingRadius = 0x188,
            CombatReach = 0x18C,
            DisplayID = 0x190,
            NativeDisplayID = 0x194,
            MountDisplayID = 0x198,
            MinDamage = 0x19C,
            MaxDamage = 0x1A0,
            MinOffHandDamage = 0x1A4,
            MaxOffHandDamage = 0x1A8,
            AnimTier = 0x1AC,
            PetNumber = 0x1B0,
            PetNameTimestamp = 0x1B4,
            PetExperience = 0x1B8,
            PetNextLevelExperience = 0x1BC,
            ModCastingSpeed = 0x1C0,
            ModSpellHaste = 0x1C4,
            ModHaste = 0x1C8,
            ModRangedHaste = 0x1CC,
            ModHasteRegen = 0x1D0,
            CreatedBySpell = 0x1D4,
            NpcFlags = 0x1D8,
            EmoteState = 0x1E0,
            Stats = 0x1E4,
            StatPosBuff = 0x1F8,
            StatNegBuff = 0x20C,
            Resistances = 0x220,
            ResistanceBuffModsPositive = 0x23C,
            ResistanceBuffModsNegative = 0x258,
            ModBonusArmor = 0x274,
            BaseMana = 0x278,
            BaseHealth = 0x27C,
            ShapeshiftForm = 0x280,
            AttackPower = 0x284,
            AttackPowerModPos = 0x288,
            AttackPowerModNeg = 0x28C,
            AttackPowerMultiplier = 0x290,
            RangedAttackPower = 0x294,
            RangedAttackPowerModPos = 0x298,
            RangedAttackPowerModNeg = 0x29C,
            RangedAttackPowerMultiplier = 0x2A0,
            MinRangedDamage = 0x2A4,
            MaxRangedDamage = 0x2A8,
            PowerCostModifier = 0x2AC,
            PowerCostMultiplier = 0x2C8,
            MaxHealthModifier = 0x2E4,
            HoverHeight = 0x2E8,
            MinItemLevelCutoff = 0x2EC,
            MinItemLevel = 0x2F0,
            MaxItemLevel = 0x2F4,
            WildBattlePetLevel = 0x2F8,
            BattlePetCompanionNameTimestamp = 0x2FC,
            InteractSpellID = 0x300,
            StateSpellVisualID = 0x304,
            StateAnimID = 0x308,
            StateAnimKitID = 0x30C,
            StateWorldEffectID = 0x310,
            ScaleDuration = 0x320,
            LooksLikeMountID = 0x324,
            LooksLikeCreatureID = 0x328,
        };

        public enum WoWPlayerFields
        {
            DuelArbiter = 0x32C,
            WowAccount = 0x33C,
            LootTargetGUID = 0x34C,
            PlayerFlags = 0x35C,
            PlayerFlagsEx = 0x360,
            GuildRankID = 0x364,
            GuildDeleteDate = 0x368,
            GuildLevel = 0x36C,
            HairColorID = 0x370,
            RestState = 0x374,
            ArenaFaction = 0x378,
            DuelTeam = 0x37C,
            GuildTimeStamp = 0x380,
            QuestLog = 0x384,
            VisibleItems = 0xF3C,
            PlayerTitle = 0x1020,
            FakeInebriation = 0x1024,
            VirtualPlayerRealm = 0x1028,
            CurrentSpecID = 0x102C,
            TaxiMountAnimKitID = 0x1030,
            AvgItemLevelTotal = 0x1034,
            AvgItemLevelEquipped = 0x1038,
            CurrentBattlePetBreedQuality = 0x103C,
            InvSlots = 0x1040,
            FarsightObject = 0x1BC0,
            KnownTitles = 0x1BD0,
            Coinage = 0x1BF8,
            XP = 0x1C00,
            NextLevelXP = 0x1C04,
            Skill = 0x1C08,
            CharacterPoints = 0x2308,
            MaxTalentTiers = 0x230C,
            TrackCreatureMask = 0x2310,
            TrackResourceMask = 0x2314,
            MainhandExpertise = 0x2318,
            OffhandExpertise = 0x231C,
            RangedExpertise = 0x2320,
            CombatRatingExpertise = 0x2324,
            BlockPercentage = 0x2328,
            DodgePercentage = 0x232C,
            ParryPercentage = 0x2330,
            CritPercentage = 0x2334,
            RangedCritPercentage = 0x2338,
            OffhandCritPercentage = 0x233C,
            SpellCritPercentage = 0x2340,
            ShieldBlock = 0x235C,
            ShieldBlockCritPercentage = 0x2360,
            Mastery = 0x2364,
            Amplify = 0x2368,
            Multistrike = 0x236C,
            MultistrikeEffect = 0x2370,
            Readiness = 0x2374,
            Speed = 0x2378,
            Lifesteal = 0x237C,
            Avoidance = 0x2380,
            Sturdiness = 0x2384,
            Cleave = 0x2388,
            Versatility = 0x238C,
            VersatilityBonus = 0x2390,
            PvpPowerDamage = 0x2394,
            PvpPowerHealing = 0x2398,
            ExploredZones = 0x239C,
            RestStateBonusPool = 0x26BC,
            ModDamageDonePos = 0x26C0,
            ModDamageDoneNeg = 0x26DC,
            ModDamageDonePercent = 0x26F8,
            ModHealingDonePos = 0x2714,
            ModHealingPercent = 0x2718,
            ModHealingDonePercent = 0x271C,
            ModPeriodicHealingDonePercent = 0x2720,
            WeaponDmgMultipliers = 0x2724,
            WeaponAtkSpeedMultipliers = 0x2730,
            ModSpellPowerPercent = 0x273C,
            ModResiliencePercent = 0x2740,
            OverrideSpellPowerByAPPercent = 0x2744,
            OverrideAPBySpellPowerPercent = 0x2748,
            ModTargetResistance = 0x274C,
            ModTargetPhysicalResistance = 0x2750,
            LocalFlags = 0x2754,
            LifetimeMaxRank = 0x2758,
            SelfResSpell = 0x275C,
            PvpMedals = 0x2760,
            BuybackPrice = 0x2764,
            BuybackTimestamp = 0x2794,
            YesterdayHonorableKills = 0x27C4,
            LifetimeHonorableKills = 0x27C8,
            WatchedFactionIndex = 0x27CC,
            CombatRatings = 0x27D0,
            PvpInfo = 0x2850,
            MaxLevel = 0x28E0,
            RuneRegen = 0x28E4,
            NoReagentCostMask = 0x28F4,
            GlyphSlots = 0x2904,
            Glyphs = 0x291C,
            GlyphSlotsEnabled = 0x2934,
            PetSpellPower = 0x2938,
            Researching = 0x293C,
            ProfessionSkillLine = 0x2964,
            UiHitModifier = 0x296C,
            UiSpellHitModifier = 0x2970,
            HomeRealmTimeOffset = 0x2974,
            ModPetHaste = 0x2978,
            SummonedBattlePetGUID = 0x297C,
            OverrideSpellsID = 0x298C,
            LfgBonusFactionID = 0x2990,
            LootSpecID = 0x2994,
            OverrideZonePVPType = 0x2998,
            ItemLevelDelta = 0x299C,
            BagSlotFlags = 0x29A0,
            BankBagSlotFlags = 0x29B0,
            InsertItemsLeftToRight = 0x29CC,
        };

        public enum WoWGameObjectFields
        {
            CreatedBy = 0x30,
            DisplayID = 0x40,
            Flags = 0x44,
            ParentRotation = 0x48,
            FactionTemplate = 0x58,
            Level = 0x5C,
            PercentHealth = 0x60,
            SpellVisualID = 0x64,
            StateSpellVisualID = 0x68,
            StateAnimID = 0x6C,
            StateAnimKitID = 0x70,
            StateWorldEffectID = 0x74,
        };

        public enum WoWDynamicObjectFields
        {
            Caster = 0x30,
            TypeAndVisualID = 0x40,
            SpellID = 0x44,
            Radius = 0x48,
            CastTime = 0x4C,
        };

        public enum WoWCorpseFields
        {
            Owner = 0x30,
            PartyGUID = 0x40,
            DisplayID = 0x50,
            Items = 0x54,
            SkinID = 0xA0,
            FacialHairStyleID = 0xA4,
            Flags = 0xA8,
            DynamicFlags = 0xAC,
            FactionTemplate = 0xB0,
        };

        public enum WoWAreaTriggerFields
        {
            Caster = 0x30,
            Duration = 0x40,
            SpellID = 0x44,
            SpellVisualID = 0x48,
            ExplicitScale = 0x4C,
        };

        public enum WoWSceneObjectFields
        {
            ScriptPackageID = 0x30,
            RndSeedVal = 0x34,
            CreatedBy = 0x38,
            SceneType = 0x48,
        };

        #endregion <Descriptors>

#pragma warning restore 1591

        // ReSharper restore InconsistentNaming
    }
}