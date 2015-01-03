using System;
using System.Collections.Generic;
using System.Linq;
using CoolFishNS.Management.CoolManager.Objects.Structs;
using NLog;

namespace CoolFishNS.Management.CoolManager.Objects
{
    /// <summary>
    ///     Manages all the WoWObjects
    /// </summary>
    public static class ObjectManager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        ///     Expensive call to get a list of all Objects. This will be a currently up to date list.
        ///     Cache this result if you don't need an updated list each call
        /// </summary>
        public static List<WoWObject> Objects
        {
            get
            {
                var objects = new List<WoWObject>();

                WoWGUID playerguid = PlayerGuid;
                var currentObject =
                    new WoWObject(
                        BotManager.Memory.Read<IntPtr>(CurrentManager + (int) Offsets.ObjectManager.FirstObject));

                while (((currentObject.BaseAddress.ToInt64() & 1) == 0) && currentObject.BaseAddress != IntPtr.Zero)
                {
                    switch (currentObject.Type)
                    {
                        case ObjectType.Unit:
                            objects.Add(new WoWUnit(currentObject.BaseAddress));
                            break;

                        case ObjectType.Item:
                            objects.Add(new WoWItem(currentObject.BaseAddress));
                            break;

                        case ObjectType.Container:
                            objects.Add(new WoWContainer(currentObject.BaseAddress));
                            break;

                        case ObjectType.Corpse:
                            objects.Add(new WoWCorpse(currentObject.BaseAddress));
                            break;

                        case ObjectType.GameObject:
                            objects.Add(new WoWGameObject(currentObject.BaseAddress));
                            break;

                        case ObjectType.Dynamicobject:
                            objects.Add(new WoWDynamicObject(currentObject.BaseAddress));
                            break;
                        case ObjectType.Player:
                            if (!currentObject.Guid.Equals(playerguid))
                            {
                                objects.Add(new WoWPlayer(currentObject.BaseAddress));
                            }
                            break;
                        default:
                            objects.Add(currentObject);
                            break;
                    }


                    currentObject.BaseAddress =
                        BotManager.Memory.Read<IntPtr>(
                            currentObject.BaseAddress + (int) Offsets.ObjectManager.NextObject);
                }

                return objects;
            }
        }

        /// <summary>
        ///     Expensive call to get the local player. This will be currently updated.
        ///     Cache this result if the local player is not changing a lot.
        /// </summary>
        public static WoWPlayerMe Me
        {
            get
            {
                var pointer = BotManager.Memory.Read<IntPtr>(Offsets.Addresses["PlayerPointer"]);
                if (pointer.Equals(IntPtr.Zero))
                {
                    return null;
                }
                return new WoWPlayerMe(pointer);
            }
        }


        internal static IntPtr CurrentManager
        {
            get { return BotManager.Memory.Read<IntPtr>(Offsets.Addresses["s_curMgr"]); }
        }

        internal static WoWGUID PlayerGuid
        {
            get { return BotManager.Memory.Read<WoWGUID>(CurrentManager + (int) Offsets.ObjectManager.LocalGuid); }
        }

        #region <Enums>

        #region ObjectType enum

        /// <summary>
        ///     Name/Number correlation for objects in World of Warcraft
        /// </summary>
        public enum ObjectType
        {
            Object = 0,
            Item = 1,
            Container = 2,
            Unit = 3,
            Player = 4,
            GameObject = 5,
            Dynamicobject = 6,
            Corpse = 7,
            Areatrigger = 8,
            Sceneobject = 9,
            NumClientObjectTypes = 0xA
        }

        #endregion

        #region PowerType enum

        /// <summary>
        ///     Name/Type correlation for power types in World of Warcraft
        /// </summary>
        public enum PowerType
        {
            Mana = 0,
            Rage = 1,
            Focus = 2,
            Energy = 3,
            Happiness = 4,
            Runes = 5,
            RunicPower = 6,
            SoulShards = 7,
            Eclipse = 8,
            HolyPower = 9,
            Alternate = 10,
            DarkForce = 11,
            LightForce = 12,
            ShadowOrbs = 13,
            BurningEmbers = 14,
            DemonicFury = 15,
            ArcaneCharges = 16
        };

        #endregion

        #endregion <Enums>

        private static ObjectType GetWoWTypeFromClassType(Type t)
        {
            if (t == typeof (WoWItem))
            {
                return ObjectType.Item;
            }
            if (t == typeof (WoWContainer))
            {
                return ObjectType.Container;
            }
            if (t == typeof (WoWUnit))
            {
                return ObjectType.Unit;
            }
            if (t == typeof (WoWPlayer) || t == typeof (WoWPlayerMe))
            {
                return ObjectType.Player;
            }
            if (t == typeof (WoWGameObject))
            {
                return ObjectType.GameObject;
            }
            if (t == typeof (WoWDynamicObject))
            {
                return ObjectType.Dynamicobject;
            }
            if (t == typeof (WoWCorpse))
            {
                return ObjectType.Corpse;
            }
            return ObjectType.Object;
        }

        /// <summary>
        ///     Gets a first object matching the passed boolean predicate function
        /// </summary>
        /// <typeparam name="T">The WoWObject (or subclass) type desired</typeparam>
        /// <param name="predicate">The boolean function to check objects against</param>
        /// <param name="conversionFunction">
        ///     Function to convert a WoWObject to the T object type (Can just instantiate a new
        ///     object of type T).
        /// </param>
        /// <returns></returns>
        public static T GetSpecificObject<T>(Predicate<T> predicate, Func<WoWObject, T> conversionFunction) where T : WoWObject
        {
            if (predicate == null)
            {
                return null;
            }

            ObjectType type = GetWoWTypeFromClassType(typeof (T));
            var currentObject =
                new WoWObject(
                    BotManager.Memory.Read<IntPtr>(CurrentManager + (int) Offsets.ObjectManager.FirstObject));

            while (((currentObject.BaseAddress.ToInt64() & 1) == 0) && currentObject.BaseAddress != IntPtr.Zero)
            {
                if (currentObject.Type == type)
                {
                    T obj = conversionFunction(currentObject);

                    if (predicate(obj))
                    {
                        return obj;
                    }
                }

                currentObject.BaseAddress =
                    BotManager.Memory.Read<IntPtr>(
                        currentObject.BaseAddress + (int) Offsets.ObjectManager.NextObject);
            }

            return null;
        }

        /// <summary>
        ///     Expensive call to get objects of the specified type.
        ///     Cache this result if you don't need an updated list each call
        /// </summary>
        /// <typeparam name="T">Type of objects to get. Must inherit from WoWObject</typeparam>
        /// <returns></returns>
        public static List<T> GetObjectsOfType<T>() where T : WoWObject
        {
            return (from t1 in Objects.AsParallel() let t = t1.GetType() where t == typeof (T) select t1).OfType<T>().ToList();
        }
    }
}