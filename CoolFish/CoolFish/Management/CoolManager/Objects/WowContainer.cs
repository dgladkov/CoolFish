using System;

namespace CoolFishNS.Management.CoolManager.Objects
{
    /// <summary>
    ///     Contains information about WoWContainers.
    /// </summary>
    public class WoWContainer : WoWObject
    {
        /// <summary>
        ///     Ctor
        /// </summary>
        /// <param name="baseAddress"></param>
        public WoWContainer(IntPtr baseAddress)
            : base(baseAddress)
        {
        }

        /// <summary>
        ///     The amount of slots this container has.
        /// </summary>
        public uint Slots
        {
            get { return GetStorageField<uint>(Offsets.WoWContainerFields.Slots); }
        }

        /// <summary>
        ///     The slot this container occupies on the character.
        /// </summary>
        public uint NumSlots
        {
            get { return GetStorageField<uint>(Offsets.WoWContainerFields.NumSlots); }
        }

        /// <summary>
        ///     Gets the descriptor struct.
        ///     Overload for when not casting uint.
        /// </summary>
        /// <typeparam name="T">struct</typeparam>
        /// <param name="field">Descriptor field</param>
        /// <returns>Descriptor field</returns>
        protected T GetStorageField<T>(Offsets.WoWContainerFields field) where T : struct
        {
            return GetStorageField<T>((int) field);
        }
    }
}