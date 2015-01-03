using System;

namespace CoolFishNS.Management.CoolManager.Objects
{
    /// <summary>
    ///     Represents a player.
    /// </summary>
    public class WoWPlayer : WoWUnit
    {
        /// <summary>
        ///     Ctor
        /// </summary>
        /// <param name="baseAddress"></param>
        public WoWPlayer(IntPtr baseAddress)
            : base(baseAddress)
        {
        }

        /// <summary>
        ///     The player's experience points.
        /// </summary>
        public int Experience
        {
            get { return GetStorageField<int>(Offsets.WoWPlayerFields.XP); }
        }

        /// <summary>
        ///     The experience the player requires to advance to the next level.
        /// </summary>
        public int NextLevel
        {
            get { return GetStorageField<int>(Offsets.WoWPlayerFields.NextLevelXP); }
        }

        /// <summary>
        ///     The spellid of the spell currently being casted. 0 if not.
        ///     NOT Channeling spells
        /// </summary>
        public uint IsCasting
        {
            get { return BotManager.Memory.Read<uint>(BaseAddress + (int) Offsets.WoWPlayer.IsCasting); }
        }

        /// <summary>
        ///     The amount of experience the player has rested.
        ///     <!-- You can calculate the double experience rate, and thus shorten the time to the next level when using this in a calculation. -->
        /// </summary>
        public int RestExperience
        {
            get { return GetStorageField<int>(Offsets.WoWPlayerFields.RestState); }
        }


        /// <summary>
        ///     Returns true if in combat, false if not.
        /// </summary>
        public bool Combat
        {
            get { return HasUnitFlag(Offsets.UnitFlags.Combat); }
        }

        /// <summary>
        /// </summary>
        public uint Channeling
        {
            get { return BotManager.Memory.Read<uint>(BaseAddress + (int) Offsets.WoWPlayer.IsChanneling); }
        }

        /// <summary>
        ///     The name of the player.
        ///     TODO: Fix this
        /// </summary>
        public override string Name
        {
            get
            {
                return "Unknown Player";
                /*   var nMask =
                    BotManager.Memory.Read<uint>(
                        BaseAddress + (int) Offsets.WoWPlayer.NameStore + 0x8 + (int) Offsets.WoWPlayer.NameMask);
                var nBase =
                    BotManager.Memory.Read<uint>(
                        BaseAddress + (int) Offsets.WoWPlayer.NameStore + 0x8 + (int) Offsets.WoWPlayer.NameBase);

                ulong nShortGUID = Guid & 0xFFFFFFFF; // only need part of the GUID
                ulong nOffset = 12*(nMask & nShortGUID);

                var nCurrentObject = BotManager.Memory.Read<uint>((IntPtr) (nBase + nOffset + 0x8));
                nOffset = BotManager.Memory.Read<uint>((IntPtr) (nBase + nOffset));

                if ((nCurrentObject & 0x1) == 0x1)
                    return "Unknown Player";

                var nTestAgainstGUID = BotManager.Memory.Read<uint>((IntPtr) (nCurrentObject));

                while (nTestAgainstGUID != nShortGUID)
                {
                    nCurrentObject = BotManager.Memory.Read<uint>((IntPtr) (nCurrentObject + nOffset + 0x4));

                    if ((nCurrentObject & 0x1) == 0x1)
                        return "Unknown Player";

                    nTestAgainstGUID = BotManager.Memory.Read<uint>((IntPtr) (nCurrentObject));
                }

                return BotManager.Memory.ReadString((IntPtr) (nCurrentObject +  Offsets.WoWPlayer.NameString),
                    Encoding.UTF8);*/
            }
        }

        /// <summary>
        ///     Returns true if the unit is mounted, false if not.
        /// </summary>
        public bool Mounted
        {
            get { return MountDisplayID > 0; }
        }

        /// <summary>
        ///     Gets the descriptor struct.
        ///     Overload for when not casting uint.
        /// </summary>
        /// <typeparam name="T">struct</typeparam>
        /// <param name="field">Descriptor field</param>
        /// <returns>Descriptor field</returns>
        protected T GetStorageField<T>(Offsets.WoWPlayerFields field) where T : struct
        {
            return GetStorageField<T>((int) field);
        }
    }
}