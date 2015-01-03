using System;
using System.Collections.Specialized;
using System.Text;

namespace CoolFishNS.Management.CoolManager.Objects
{
    /// <summary>
    ///     An unit, such as an NPC, but also a player.
    /// </summary>
    public class WoWUnit : WoWObject
    {
        /// <summary>
        ///     Instantiates a new instance of the WoWUnit object.
        /// </summary>
        /// <param name="baseAddress">The units base address.</param>
        public WoWUnit(IntPtr baseAddress)
            : base(baseAddress)
        {
        }


        /// <summary>
        ///     How fast the WoWUnit is traveling
        /// </summary>
        public float Speed
        {
            get
            {
                return
                    BotManager.Memory.Read<float>(BotManager.Memory.Read<IntPtr>(BaseAddress + (int) Offsets.WoWPlayer.Speed1) +
                                                  (int) Offsets.WoWPlayer.Speed2);
            }
        }

        /// <summary>
        ///     Returns the Unit Flags.
        /// </summary>
        private BitVector32 UnitFlags
        {
            get { return GetStorageField<BitVector32>(Offsets.WoWUnitFields.Flags); }
        }

        /// <summary>
        ///     Is this unit a critter?
        /// </summary>
        public bool Critter
        {
            get { return GetStorageField<int>(Offsets.WoWUnitFields.Critter) == 1; }
        }

        /// <summary>
        ///     Returns whether the unit is a unit.
        /// </summary>
        public bool IsUnit
        {
            get { return Type == ObjectManager.ObjectType.Unit; }
        }

        /// <summary>
        ///     Returns whether the unit is a Player.
        /// </summary>
        public bool IsPlayer
        {
            get { return Type == ObjectManager.ObjectType.Player; }
        }

        /// <summary>
        ///     The GUID of the object this unit is charmed by.
        /// </summary>
        public ulong CharmedBy
        {
            get { return GetStorageField<ulong>(Offsets.WoWUnitFields.CharmedBy); }
        }

        /// <summary>
        ///     The GUID of the object this unit is summoned by.
        /// </summary>
        public ulong SummonedBy
        {
            get { return GetStorageField<ulong>(Offsets.WoWUnitFields.SummonedBy); }
        }

        /// <summary>
        ///     The GUID of the object this unit was created by.
        /// </summary>
        public ulong CreatedBy
        {
            get { return GetStorageField<ulong>(Offsets.WoWUnitFields.CreatedBy); }
        }

        /// <summary>
        ///     The unit's health.
        /// </summary>
        public int Health
        {
            get { return GetStorageField<int>(Offsets.WoWUnitFields.Health); }
        }

        /// <summary>
        ///     The unit's maximum health.
        /// </summary>
        public int MaximumHealth
        {
            get { return GetStorageField<int>(Offsets.WoWUnitFields.MaxHealth); }
        }

        /// <summary>
        ///     The unit's health as a percentage of its total.
        /// </summary>
        public int HealthPercentage
        {
            get { return (100*Health)/MaximumHealth; }
        }

        /// <summary>
        ///     The unit's base health.
        /// </summary>
        public int BaseHealth
        {
            get { return GetStorageField<int>(Offsets.WoWUnitFields.BaseHealth); }
        }

        /// <summary>
        ///     The unit's base health.
        /// </summary>
        public int BaseMana
        {
            get { return GetStorageField<int>(Offsets.WoWUnitFields.BaseMana); }
        }

        /// <summary>
        ///     The unit's level.
        /// </summary>
        public int Level
        {
            get { return GetStorageField<int>(Offsets.WoWUnitFields.Level); }
        }

        /// <summary>
        ///     The name of the unit.
        /// </summary>
        public virtual string Name
        {
            get
            {
                var name1 = BotManager.Memory.Read<IntPtr>(BaseAddress + (int) Offsets.WoWUnit.Name1);
                if (name1 == IntPtr.Zero)
                {
                    return "UnknownUnit";
                }
                var name2 = BotManager.Memory.Read<IntPtr>(name1 + (int) Offsets.WoWUnit.Name2);
                if (name2 == IntPtr.Zero)
                {
                    return "UnknownUnit";
                }
                return BotManager.Memory.ReadString(name2, Encoding.UTF8);
            }
        }

        /// <summary>
        ///     The unit's DisplayID.
        /// </summary>
        public int DisplayID
        {
            get { return GetStorageField<int>(Offsets.WoWUnitFields.DisplayID); }
        }

        /// <summary>
        ///     The mount display of the mount the unit is mounted on.
        /// </summary>
        public int MountDisplayID
        {
            get { return GetStorageField<int>(Offsets.WoWUnitFields.MountDisplayID); }
        }

        /// <summary>
        ///     The GUID of the object this unit is targeting.
        /// </summary>
        public ulong Target
        {
            get { return GetStorageField<ulong>(Offsets.WoWUnitFields.Target); }
        }

        /// <summary>
        ///     Determines if the unit is dead.
        /// </summary>
        public bool Dead
        {
            get { return Health == 0 || Health == 1; }
        }

        /// <summary>
        ///     Is the unit lootable?
        /// </summary>
        public bool Lootable
        {
            get { return HasFlag(Offsets.UnitDynamicFlags.Lootable); }
        }

        private byte[] DisplayPower
        {
            get { return BitConverter.GetBytes(GetStorageField<uint>(Offsets.WoWUnitFields.DisplayPower)); }
        }

        /// <summary>Gets the type of the power.</summary>
        /// <value>The type of the power.</value>
        public ObjectManager.PowerType PowerType
        {
            get
            {
                var overridePowerId = GetStorageField<uint>(Offsets.WoWUnitFields.OverrideDisplayPowerID);
                if (overridePowerId == 0)
                    return (ObjectManager.PowerType) DisplayPower[3];
                return (ObjectManager.PowerType) overridePowerId;
            }
        }

        /// <summary>
        ///     Gets the race of the unit.
        /// </summary>
        public Offsets.RaceFlags Race
        {
            get { return (Offsets.RaceFlags) DisplayPower[0]; }
        }

        /// <summary>
        ///     Gets the class of the unit.
        /// </summary>
        public Offsets.ClassFlags Class
        {
            get { return (Offsets.ClassFlags) DisplayPower[1]; }
        }

        /*   /// <summary>
        /// Reads power of a unit
        /// credits to JuJuBoSc
        /// </summary>
        /// <param name="powerType"></param>
        /// <returns></returns>
        private int GetPowerIndexByPowerType(ObjectManager.PowerType powerType)
        {
            uint index =
                BotManager.Memory.Read<byte>(
                    BotManager.Memory.Read<IntPtr>(BaseAddress + 0xDC) + 0x49) +
                 powerType +
                16*
                
                BotManager.Memory.Read<byte>(
                    BotManager.Memory.Read<IntPtr>(BaseAddress + 0xDC) + 0x49);

            var result = BotManager.Memory.Read<int>((IntPtr) (Offsets.WoWUnit.PowerOffset + index*4), true);

            return result;
        }

        /// <summary>
        /// Reads power of a unit
        /// credits to JuJuBoSc
        /// </summary>
        /// <param name="powerType"></param>
        /// <returns></returns>
        public uint GetPowerByPowerType(ObjectManager.PowerType powerType)
        {
            int index = GetPowerIndexByPowerType(powerType);

            if (index == -1)
                return 0;

            return BotManager.Memory.Read<uint>(BaseAddress + index*4 + (int) Offsets.GetPowerbyType);
        }

        /// <summary>
        /// Reads maxpower of a unit
        /// credits to Tanaris4
        /// </summary>
        /// <param name="powerType"></param>
        /// <returns></returns>
        public uint GetMaxPowerByPowerType(ObjectManager.PowerType powerType)
        {
            //Here is base to get this working. Offsets need updating and put in offsets file

            int index = GetPowerIndexByPowerType(powerType);

            if (index == -1)
                return 0;

            return
                BotManager.Memory.Read<uint>(
                    BotManager.Memory.Read<IntPtr>(BaseAddress + 0xDC) + 4*index + 0x6C);
        }*/

        /// <summary>
        ///     Does the unit have this flag?
        /// </summary>
        /// <param name="flags"></param>
        /// <returns></returns>
        private bool HasFlag(Offsets.UnitDynamicFlags flags)
        {
            return DynamicFlags[(int) flags];
        }

        internal bool HasUnitFlag(Offsets.UnitFlags flag)
        {
            return UnitFlags[(int) flag];
        }

        /// <summary>
        ///     Gets the descriptor struct.
        ///     Overload for when not casting uint.
        /// </summary>
        /// <typeparam name="T">struct</typeparam>
        /// <param name="field">Descriptor field</param>
        /// <returns>Descriptor field</returns>
        protected T GetStorageField<T>(Offsets.WoWUnitFields field) where T : struct
        {
            return GetStorageField<T>((int) field);
        }
    }
}