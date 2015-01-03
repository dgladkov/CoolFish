using System;
using System.Collections.Specialized;
using System.Text;
using CoolFishNS.Management.CoolManager.Objects.Structs;

namespace CoolFishNS.Management.CoolManager.Objects
{
    /// <summary>
    ///     A Game Object, such as a herb, but also a treasure box.
    /// </summary>
    public class WoWGameObject : WoWObject
    {
        /// <summary>
        ///     Ctor.
        /// </summary>
        /// <param name="baseAddress"></param>
        public WoWGameObject(IntPtr baseAddress)
            : base(baseAddress)
        {
        }


        /// <summary>
        ///     The GameObject's Display ID.
        /// </summary>
        public int DisplayID
        {
            get { return GetStorageField<int>(Offsets.WoWGameObjectFields.DisplayID); }
        }

        public int AnimationState
        {
            get { return BotManager.Memory.Read<int>(BaseAddress + (int) Offsets.WowGameObject.AnimationState); }
        }

        /// <summary>
        ///     The GameObject's faction.
        /// </summary>
        public int Faction
        {
            get { return GetStorageField<int>(Offsets.WoWGameObjectFields.FactionTemplate); }
        }

        public BitVector32 Flags
        {
            get { return GetStorageField<BitVector32>(Offsets.WoWGameObjectFields.Flags); }
        }

        public bool IsBobbing
        {
            get { return BotManager.Memory.Read<bool>(BaseAddress + (int) Offsets.WowGameObject.AnimationState); }
        }

        public bool Locked
        {
            get { return Flags[1]; }
        }

        public bool InUse
        {
            get { return Flags[0]; }
        }

        public bool IsTransport
        {
            get { return Flags[3]; }
        }

        public bool InteractCondition
        {
            get { return Flags[2]; }
        }

        /// <summary>
        ///     The GameObject's level.
        /// </summary>
        public int Level
        {
            get { return GetStorageField<int>(Offsets.WoWGameObjectFields.Level); }
        }

        /// <summary>
        ///     The GUID of the object this object was created by.
        ///     <!-- Presumably, hasn't been double-checked. -->
        /// </summary>
        public WoWGUID CreatedBy
        {
            get { return GetStorageField<WoWGUID>(Offsets.WoWGameObjectFields.CreatedBy); }
        }

        /// <summary>
        ///     Returns the object's name.
        /// </summary>
        public string Name
        {
            get
            {
                return
                    BotManager.Memory.ReadString(
                        BotManager.Memory.Read<IntPtr>(
                            BotManager.Memory.Read<IntPtr>(BaseAddress + (int) Offsets.WowGameObject.Name1) +
                            (int) Offsets.WowGameObject.Name2), Encoding.UTF8);
            }
        }

        /// <summary>
        ///     Returns the GameObject's X position.
        /// </summary>
        public override float X
        {
            get { return BotManager.Memory.Read<float>(BaseAddress + (int) Offsets.WowObject.GameObjectX); }
        }

        /// <summary>
        ///     Returns the GameObject's Y position.
        /// </summary>
        public override float Y
        {
            get { return BotManager.Memory.Read<float>(BaseAddress + (int) Offsets.WowObject.GameObjectY); }
        }

        /// <summary>
        ///     Returns the GameObject's Z position.
        /// </summary>
        public override float Z
        {
            get { return BotManager.Memory.Read<float>(BaseAddress + (int) Offsets.WowObject.GameObjectZ); }
        }

        /// <summary>
        ///     Returns the <see cref="Point">Point</see> location of the Game Object.
        /// </summary>
        public new Point Location
        {
            get { return new Point(X, Y, Z); }
        }

        /// <summary>
        ///     The distance.
        /// </summary>
        public new float Distance
        {
            get { return (float) Point.Distance(ObjectManager.Me.Location, Location); }
        }

        /// <summary>
        ///     Gets the descriptor struct.
        ///     Overload for when not casting uint.
        /// </summary>
        /// <typeparam name="T">struct</typeparam>
        /// <param name="field">Descriptor field</param>
        /// <returns>Descriptor field</returns>
        protected T GetStorageField<T>(Offsets.WoWGameObjectFields field) where T : struct
        {
            return GetStorageField<T>((int) field);
        }
    }
}