using System;
using System.Collections.Generic;
using UnityEngine;
using static ModThatIsNotMod.Pooling.SimplePool;

namespace ModThatIsNotMod.Pooling
{
    /// <summary>
    /// Adamdev had some issue with this so probably just stick to the vanilla pools
    /// </summary>
    public static class SimplePoolManager
    {
        public static Dictionary<string, SimplePool> pools { get; private set; } = new Dictionary<string, SimplePool>();


        internal static void ClearPools() => pools.Clear();

        public static void CreatePool(GameObject prefab, string name, int pooledAmount, PoolMode poolMode)
        {
            if (pools.ContainsKey(name))
            {
                ModConsole.Msg(ConsoleColor.Red, $"[ERROR] SimplePool with name \"{name}\" already exists");
                return;
            }

            SimplePool newPool = new GameObject($"{name} Pool").AddComponent<SimplePool>();
            newPool.InitPool(prefab, pooledAmount, poolMode);
            pools.Add(name, newPool);
        }

        public static SimplePool GetPool(string name) => pools.ContainsKey(name) ? pools[name] : null;
    }
}
