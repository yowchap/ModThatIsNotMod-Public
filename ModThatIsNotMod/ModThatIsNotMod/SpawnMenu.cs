using HarmonyLib;
using MelonLoader;
using StressLevelZero.Data;
using StressLevelZero.Pool;
using StressLevelZero.Props.Weapons;
using StressLevelZero.UI.Radial;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace ModThatIsNotMod
{
    public static class SpawnMenu
    {
        private static List<SpawnableObject> customSpawnables = new List<SpawnableObject>();

        public static event Action<SpawnableObject> OnItemAddedToMenu;
        public static GameObject lastSpawnedPoolee { get; private set; }


        public static void AddItem(SpawnableObject spawnable)
        {
            RemoveItem(spawnable);
            customSpawnables.Add(spawnable);
            OnItemAddedToMenu?.Invoke(spawnable);
        }

        public static void RemoveItem(SpawnableObject spawnable) => RemoveItem(spawnable.title);
        public static void RemoveItem(string title)
        {
            for (int i = customSpawnables.Count - 1; i >= 0; i--)
            {
                if (customSpawnables[i].title == title)
                    customSpawnables.RemoveAt(i);
            }
        }

        internal static void HookPopulateSpawnMenu()
        {
            // Hook populating the spawn menu
            MethodInfo original = typeof(SpawnablesPanelView).GetMethod("PopulateMenu", AccessTools.all);
            MethodInfo hook = typeof(SpawnMenu).GetMethod("PrePopulateSpawnMenu", AccessTools.all);
            Hooking.CreateHook(original, hook, true);

            // Hook after spawn gun fires
            original = typeof(SpawnGun).GetMethod("OnFire", AccessTools.all);
            hook = typeof(SpawnMenu).GetMethod("OnFireSpawnGun", AccessTools.all);
            Hooking.CreateHook(original, hook);
        }

        /// <summary>
        /// Adds all custom spawnable objects into the spawn gun right before the menu opens.
        /// </summary>
        private static void PrePopulateSpawnMenu(SpawnablesPanelView __instance)
        {
            foreach (SpawnableObject spawnable in customSpawnables)
            {
                if (!__instance.SpawnablesQuickMap.ContainsKey(spawnable.category))
                    __instance.SpawnablesQuickMap.Add(spawnable.category, new Il2CppSystem.Collections.Generic.List<SpawnableObject>());

                if (!__instance.SpawnablesQuickMap[spawnable.category].Contains(spawnable))
                    __instance.SpawnablesQuickMap[spawnable.category].Add(spawnable);
            }
        }

        private static void OnFireSpawnGun(SpawnGun __instance)
        {
            if (__instance._selectedSpawnable != null && __instance._selectedMode == UtilityModes.SPAWNER)
            {
                string spawnableTitle = __instance._selectedSpawnable.title;
                if (PoolManager.DynamicPools.ContainsKey(spawnableTitle))
                    MelonCoroutines.Start(CoSetLastSpawnedPoolee(PoolManager.DynamicPools[spawnableTitle]));
            }
        }

        private static IEnumerator CoSetLastSpawnedPoolee(Pool pool)
        {
            yield return new WaitForSeconds(2 / 90f); // TODO: Not really a todo but hopefully this gets fixed (update many months later, iirc "this" is referring to yield return null not working)
            if (pool._spawnedObjects.Count > 0)
            {
                Poolee poolee = pool._spawnedObjects[pool._spawnedObjects.Count - 1];
                if (poolee != null)
                    lastSpawnedPoolee = poolee.gameObject;
            }
        }
    }
}
