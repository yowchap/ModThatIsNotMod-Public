using ModThatIsNotMod.MonoBehaviours;
using StressLevelZero.Data;
using StressLevelZero.Pool;
using StressLevelZero.Props.Weapons;
using System.Linq;
using UnityEngine;

namespace ModThatIsNotMod.Internals
{
    internal static class InfiniteAmmoGuns
    {
        /// <summary>
        /// Adds a new version of the gun with inifinite ammo to the spawn menu if the name is in the modprefs array
        /// </summary>
        public static void CheckForInfiniteAmmoVersion(GameObject obj, string name, CategoryFilters category, PoolMode poolMode, int pooledAmount, bool hideInMenu)
        {
            if (Preferences.infiniteAmmoGuns.value.Contains(name))
            {
                name += " Infinite";
                obj = GameObject.Instantiate(obj);
                obj.name = name;

                Gun gun = obj.GetComponent<Gun>();
                gun.overrideMagazine = gun.magazineSocket.magazineData;

                CustomItems.InvokeOnItemLoaded(obj);
                obj.AddComponent<CustomItem>();
                SpawnableObject spawnable = CustomItems.CreateSpawnableObject(obj, name, category, PoolMode.REUSEOLDEST, pooledAmount);
                if (!hideInMenu)
                    SpawnMenu.AddItem(spawnable);
            }
        }
    }
}
