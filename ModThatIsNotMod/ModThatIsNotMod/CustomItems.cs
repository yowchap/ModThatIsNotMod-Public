using ModThatIsNotMod.Internals;
using StressLevelZero.Data;
using StressLevelZero.Pool;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ModThatIsNotMod
{
    public static class CustomItems
    {
        internal static Dictionary<string, string> dummyShaderReplacements { get; private set; } = new Dictionary<string, string>()
        {
            {"Hidden/InternalErrorShader", "Valve/vr_standard" },
            {"Valve/vr_standard2", "Valve/vr_standard" },
            {"Valve/vr_standard", "Valve/vr_standard" },
            {"Standard", "Valve/vr_standard" },
            {"SDK/AdditiveHDR", "SLZ/Additive HDR" },
            {"SDK/CubeReflection", "SLZ/Cube Reflection" },
            {"SDK/GibSkin", "SLZ/GibSkin" },
            {"SDK/GibSkinCrablet", "SLZ/GibSkinCrablet" },
            {"SDK/GibSkinMAS", "SLZ/GibSkinMAS" },
            {"SDK/GibSkinMASEmissive", "SLZ/GibSkinMAS_Emissive" },
            {"SDK/GibSkinMASTransparent", "SLZ/GibSkinMAS Transparent" },
            {"SDK/GibSkinWire", "SLZ/GibSkinWire" },
            {"SDK/Glass", "SLZ/Glass" },
            {"SDK/GlowingFalloff", "SLZ/Glowing Falloff" },
            {"SDK/GridGlitch", "SLZ/Grid Glitch" },
            {"SDK/Highlighter", "SLZ/Highlighter" },
            {"SDK/HolographicPortal", "SLZ/Holographic Portal" },
            {"SDK/HolographicProjection", "SLZ/Holographic Projection" },
            {"SDK/HolographicVisor", "SLZ/Holographic Visor" },
            {"SDK/HolographicWall", "SLZ/Holographic Wall" },
            {"SDK/HolographicWallGlitchy", "SLZ/Holographic Wall Glitchy" },
            {"SDK/IconBillboard", "SLZ/Icon Billboard" },
            {"SDK/Laser", "SLZ/Laser" },
            {"SDK/LitHologram", "SLZ/Lit Hologram" },
            {"SDK/MultiplicativeDecal", "SLZ/Multiplicative Decal" },
            {"SDK/Multiply", "SLZ/Multiply" },
            {"SDK/Nothin", "SLZ/Nothin" },
            {"SDK/ParticleMotionVector", "SLZ/Particle Motion Vector" },
            {"SDK/ReflexScope", "Shader Forge/reflexScope" },
            {"SDK/ShadowOnly", "SLZ/ShadowOnly" },
            {"SDK/SimpleFluorescence", "SLZ/Simple Fluorescence" },
            {"SDK/SmokePoof", "SLZ/SmokePoof" },
            {"SDK/StochasticHologram", "SLZ/Stochastic Hologram" },
            {"SDK/StylizedFXShield", "Stylized FX/Shield" },
            {"SDK/SwirlClouds", "SLZ/SwirlClouds" },
            {"SDK/TextureArrays", "SLZ/Texture Arrays" },
            {"SDK/Void", "SLZ/Void" },
            {"SDK/VoidBleed", "SLZ/VoidBleed" },
            {"SDK/VRTerrain", "SLZ/VR Terrain" },
            {"SDK/WhiteDimension", "SLZ/White Dimension" },
            {"SDK/ZeroLabVRDissolve", "ZeroLab/VR_Dissolve" },
            {"SDK/ZoomLens", "SLZ/ZoomLens" }
        };

        private static Dictionary<string, CustomSpawnableData> customSpawnables = new Dictionary<string, CustomSpawnableData>();

        internal static List<GameObject> customItemInstances = new List<GameObject>();

        public static bool customItemsExist => customSpawnables.Count > 0;

        public static event Action<GameObject> OnItemLoaded;
        public static event Action<SpawnableObject> OnSpawnableCreated;

        /// <summary>
        /// Returns the spawnable object for the custom item.
        /// </summary>
        public static SpawnableObject GetCustomSpawnable(string name)
        {
            if (customSpawnables.TryGetValue(name, out CustomSpawnableData spawnableData))
                return spawnableData.spawnable;
            return null;
        }

        /// <summary>
        /// Returns a new instance of the custom item.
        /// </summary>
        public static GameObject GetCustomGameObject(string name)
        {
            SpawnableObject spawnable = GetCustomSpawnable(name);
            if (spawnable != null)
            {
                GameObject instance = GameObject.Instantiate(spawnable.prefab);
                // Might need to enable the gameobject here, needs testing
                return instance;
            }
            return null;
        }

        public static SpawnableObject GetRandomCustomSpawnable()
        {
            List<CustomSpawnableData> values = Enumerable.ToList(customSpawnables.Values);
            CustomSpawnableData spawnableData = values[Random.Range(0, values.Count)];
            return spawnableData.spawnable;
        }

        /// <summary>
        /// Get an array of all custom items with the same name.
        /// TODO: Test this
        /// </summary>
        public static GameObject[] GetCustomItemInstances(string name, bool onlyActiveItems = false)
        {
            IEnumerable<GameObject> allInstances = customItemInstances.Where(x => x != null && SimpleHelpers.GetCleanObjectName(x.name) == name);
            if (onlyActiveItems)
                return allInstances.Where(x => x.activeInHierarchy).ToArray();
            return allInstances.ToArray();
        }

        public static GameObject SpawnFromPool(string name, Vector3 position, Quaternion rotation) => GlobalPool.Spawn(name, position, rotation);
        public static GameObject SpawnFromPool(string name, Vector3 position, Quaternion rotation, Vector3 scale) => GlobalPool.Spawn(name, position, rotation, scale);

        /// <summary>
        /// Creates a new SpawnableObject with the given data, stops the GameObject from being unloaded, and optionally registers a pool for the new SpawnableObject.
        /// </summary>
        public static SpawnableObject CreateSpawnableObject(GameObject prefab, string name, CategoryFilters category, PoolMode poolMode, int pooledAmount, bool isHidden = false, bool registerPool = true)
        {
            GameObject.DontDestroyOnLoad(prefab);
            prefab.hideFlags = HideFlags.DontUnloadUnusedAsset; // Stops the object from being unloaded when it's not in use, such as between scenes

            SpawnableObject spawnable = ScriptableObject.CreateInstance<SpawnableObject>();
            spawnable.name = $"{name}_SpawnableObject";

            spawnable.prefab = prefab;
            spawnable.title = name;
            spawnable.category = category;
            spawnable.mode = poolMode;
            spawnable.pooledAmount = pooledAmount;
            spawnable.isHidden = isHidden;

            #region Misc spawnable settings
            spawnable._uuid = Guid.NewGuid().ToString();
            spawnable.description = "";
            spawnable.warmupAmount = 1;
            spawnable.spawnDistance = 0;
            spawnable.spawnStackDistance = 0;
            spawnable.spawnFrequency = 0;
            spawnable.spawnStackCount = 0;
            #endregion

            if (registerPool)
                PoolManager.RegisterPool(spawnable); // A pool needs to be registered for the spawnable to used in the spawngun, scene zones, etc

            AddSpawnableToDictionary(spawnable, registerPool);

            spawnable.hideFlags = HideFlags.DontUnloadUnusedAsset;

            OnSpawnableCreated?.Invoke(spawnable);

            return spawnable;
        }

        /// <summary>
        /// Replaces all dummy and missing shaders with real ones.
        /// </summary>
        public static void FixObjectShaders(GameObject obj)
        {
            if (obj != null)
            {
                foreach (Renderer renderer in obj.GetComponentsInChildren<Renderer>(true))
                {
                    try // I think this try/catch might not be needed because of unhollower updates but don't feel like testing
                    {
                        foreach (Material mat in renderer.sharedMaterials)
                        {
                            if (dummyShaderReplacements.ContainsKey(mat.shader.name))
                                mat.shader = Shader.Find(dummyShaderReplacements[mat.shader.name]);
                            else if (Shader.Find(mat.shader.name.Replace("SDK", "SLZ")) != null)
                                mat.shader = Shader.Find(mat.shader.name.Replace("SDK", "SLZ"));
                        }
                    }
                    catch { }
                }
            }
        }

        public static void AddDummyShaderReplacement(string dummyName, string inGameName)
        {
            if (!dummyShaderReplacements.ContainsKey(dummyName))
                dummyShaderReplacements.Add(dummyName, inGameName);
        }

        public static void LoadItemsFromBundle(AssetBundle bundle)
        {
            ItemLoading.LoadFromBundle(bundle);
        }

        internal static void DestroySpawnable(string title)
        {
            if (customSpawnables.ContainsKey(title))
            {
                GameObject.Destroy(customSpawnables[title].spawnable);
                customSpawnables.Remove(title);
            }
        }

        internal static void OnLevelWasInitialized()
        {
            customItemInstances.Clear();
            RecreatePools();
        }

        /// <summary>
        /// Registers pools for all custom spawnables.
        /// </summary>
        internal static void RecreatePools()
        {
            foreach (var kvp in customSpawnables)
            {
                if (kvp.Value.registerPool)
                    PoolManager.RegisterPool(kvp.Value.spawnable);
            }
        }

        internal static void InvokeOnItemLoaded(GameObject obj) => OnItemLoaded?.Invoke(obj);

        private static void AddSpawnableToDictionary(SpawnableObject spawnable, bool registerPool)
        {
            if (!customSpawnables.ContainsKey(spawnable.title))
                customSpawnables.Add(spawnable.title, new CustomSpawnableData(spawnable, registerPool));
        }

        /// <summary>
        /// Used in RecreatePools to determine which custom spawnables need to have a pool registered.
        /// </summary>
        private struct CustomSpawnableData
        {
            public SpawnableObject spawnable;
            public bool registerPool;

            public CustomSpawnableData(SpawnableObject spawnable, bool registerPool)
            {
                this.spawnable = spawnable;
                this.registerPool = registerPool;
            }
        }
    }
}
