using System;

namespace CoolFishNS.Management.CoolManager.Objects
{
    /// <summary>
    ///     Player corpses.
    /// </summary>
    public class WoWCorpse : WoWUnit
    {
        /// <summary>
        ///     Ctor.
        /// </summary>
        /// <param name="baseAddress"></param>
        public WoWCorpse(IntPtr baseAddress)
            : base(baseAddress)
        {
        }

        /// <summary>
        ///     The corpse's owner.
        /// </summary>
        public ulong Owner
        {
            get { return GetStorageField<ulong>(Offsets.WoWCorpseFields.Owner); }
        }

        /// <summary>
        ///     The Corpses Display ID.
        /// </summary>
        public int DisplayId
        {
            get { return GetStorageField<int>(Offsets.WoWCorpseFields.DisplayID); }
        }

        /// <summary>
        ///     Gets the descriptor struct.
        ///     Overload for when not casting uint.
        /// </summary>
        /// <typeparam name="T">struct</typeparam>
        /// <param name="field">Descriptor field</param>
        /// <returns>Descriptor field</returns>
        protected T GetStorageField<T>(Offsets.WoWCorpseFields field) where T : struct
        {
            return GetStorageField<T>((int) field);
        }
    }
}