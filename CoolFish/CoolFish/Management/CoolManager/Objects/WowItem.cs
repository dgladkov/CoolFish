using System;

namespace CoolFishNS.Management.CoolManager.Objects
{
    /// <summary>
    ///     An item.
    /// </summary>
    public class WoWItem : WoWObject
    {
        /// <summary>
        ///     Ctor.
        /// </summary>
        /// <param name="baseAddress"></param>
        public WoWItem(IntPtr baseAddress)
            : base(baseAddress)
        {
        }

        /// <summary>
        ///     The item's remaining durability.
        /// </summary>
        public int Durability
        {
            get { return GetStorageField<int>(Offsets.WoWItemFields.Durability); }
        }

        /// <summary>
        ///     The item's maximum durability.
        /// </summary>
        public int MaximumDurability
        {
            get { return GetStorageField<int>(Offsets.WoWItemFields.MaxDurability); }
        }

        /// <summary>
        ///     The amount of items stacked.
        /// </summary>
        public int StackCount
        {
            get { return GetStorageField<int>(Offsets.WoWItemFields.StackCount); }
        }

        /// <summary>
        ///     The amount of charges this item has.
        /// </summary>
        public int Charges
        {
            get { return GetStorageField<int>(Offsets.WoWItemFields.SpellCharges); }
        }

        /// <summary>
        ///     Does the item have charges?
        /// </summary>
        public bool HasCharges
        {
            get { return Charges > 0; }
        }

        /// <summary>
        ///     Gets the descriptor struct.
        ///     Overload for when not casting uint.
        /// </summary>
        /// <typeparam name="T">struct</typeparam>
        /// <param name="field">Descriptor field</param>
        /// <returns>Descriptor field</returns>
        protected T GetStorageField<T>(Offsets.WoWItemFields field) where T : struct
        {
            return GetStorageField<T>((int) field);
        }
    }
}