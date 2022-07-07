using MelonLoader;
using ModThatIsNotMod.Legacy;
using ModThatIsNotMod.MonoBehaviours;
using Newtonsoft.Json;
using StressLevelZero.Combat;
using StressLevelZero.Data;
using StressLevelZero.SFX;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace ModThatIsNotMod.Internals
{
    internal static class ItemLoading
    {
        private static readonly string customItemsDir = @"CustomItems\";
        private static readonly string exportedAssembliesDir = @"ExportedAssemblies\";

        private static readonly string customItemsExt = ".melon";
        private static readonly string ignoreFolderSuffix = "_ignore";

        private static readonly string customItemsGOName = "CustomItems";
        private static readonly string embeddedAssemblyName = "ModAssembly"; // Extension in unity needs to be .bytes

        private static bool hasLoadedEmbeddedAssemblies = false;

        private static List<LoadedMelonData> loadedMelons = new List<LoadedMelonData>();


        /// <summary>
        /// Finds and loads every .melon file in CustomItems and its subfolders. Logs info about success and failures.
        /// </summary>
        public static void LoadMelons(bool isReload = false)
        {
            string[] melonPaths = GetMelonFilePaths(Path.Combine(MelonUtils.UserDataDirectory, customItemsDir));
            List<LoadedMelonData> failedMelons = new List<LoadedMelonData>(); // Separate list for failed items so they're all at the end of the logging and easier to see
            int melonsLoaded = 0;
            int itemsLoaded = 0;

            if (!isReload)
                ModConsole.Msg(ConsoleColor.DarkYellow, "Loading custom items, this may take a while", LoggingMode.MINIMAL);
            else
                ModConsole.Msg(ConsoleColor.DarkYellow, "Reloading modified .melon files, this may take a while", LoggingMode.MINIMAL);

            foreach (string file in melonPaths)
            {
                string localPath = Path.Combine(customItemsDir, file);

                // Try to load the .melon file
                LoadedMelonData loadedMelonData = new LoadedMelonData();
                try { loadedMelonData = LoadMelon(localPath); }
                catch(Exception ex) { failedMelons.Add(new LoadedMelonData() { filePath = file, errorMessage = "Exception thrown", exceptionThrown = ex.ToString() }); continue; }

                if (loadedMelonData.itemsLoaded > 0)
                {
                    // Log info about what items were loaded
                    ModConsole.Msg(ConsoleColor.Blue, $"Loaded {loadedMelonData.itemsLoaded} items from {file} by {loadedMelonData.author}", LoggingMode.MINIMAL);
                    if (Preferences.loggingMode >= LoggingMode.NORMAL)
                    {
                        foreach (LoadedItemData item in loadedMelonData.loadedItems)
                        {
                            string comma = Preferences.loggingMode >= LoggingMode.VERBOSE ? "," : "";
                            string categoryAttribute = Preferences.loggingMode >= LoggingMode.VERBOSE ? item.category.ToString() : "";
                            string hiddenAttribute = Preferences.loggingMode >= LoggingMode.VERBOSE ? (item.hidden ? "(Hidden)" : "") : "";
                            ModConsole.Msg($"  - {item.itemName}{comma} {categoryAttribute} {hiddenAttribute}");
                        }
                    }

                    melonsLoaded++;
                    itemsLoaded += loadedMelonData.itemsLoaded;
                }
                else if (loadedMelonData.itemsLoaded != -1)
                {
                    if (loadedMelonData.errorMessage == new LoadedMelonData().errorMessage)
                        failedMelons.Add(new LoadedMelonData() { filePath = file, errorMessage = "No items loaded" });
                    else
                        failedMelons.Add(loadedMelonData);
                }

                loadedMelons.Add(loadedMelonData);
            }

            // Log failed .melons
            foreach (LoadedMelonData data in failedMelons)
            {
                ModConsole.Msg(ConsoleColor.Red, $"Failed to load {data.filePath}: {data.errorMessage}", LoggingMode.MINIMAL);
                if(data.exceptionThrown != "")
                    ModConsole.Msg(ConsoleColor.Red, data.exceptionThrown, LoggingMode.MINIMAL);
            }
                

            // Log overall loading stats
            if (melonsLoaded > 0 || failedMelons.Count > 0)
            {
                ModConsole.Msg(ConsoleColor.DarkYellow, $"Total .melon files loaded: {melonsLoaded}", LoggingMode.MINIMAL);
                ModConsole.Msg(ConsoleColor.DarkYellow, $"Total items loaded: {itemsLoaded}", LoggingMode.MINIMAL);
                if (failedMelons.Count > 0)
                    ModConsole.Msg(ConsoleColor.DarkYellow, $"Total failed .melon files: {failedMelons.Count}", LoggingMode.MINIMAL);
            }
            else if (!isReload) { ModConsole.Msg(ConsoleColor.DarkYellow, $"No .melon files found in {Path.Combine(MelonUtils.UserDataDirectory, customItemsDir)}", LoggingMode.MINIMAL); }
            else { ModConsole.Msg(ConsoleColor.DarkYellow, "No modified .melon files detected", LoggingMode.MINIMAL); }

            if (!isReload)
                Stats.ReportItemStats(loadedMelons.Count);

            hasLoadedEmbeddedAssemblies = true;
        }

        /// <summary>
        /// Unloads all custom items, removes (hopefully) all references to them, and loads the .melons again
        /// Doesn't reload embedded assemblies since that would require cross appdomain reference fuckery
        /// </summary>
        public static void ReloadMelons() => LoadMelons(true);

        /// <summary>
        /// Finds a LoadedMelonData with the same file path
        /// </summary>
        private static LoadedMelonData GetLoadedMelon(string path)
        {
            foreach (LoadedMelonData data in loadedMelons)
                if (data.filePath == path)
                    return data;

            return new LoadedMelonData(-1, new List<LoadedItemData>());
        }

        /// <summary>
        /// Deletes any LoadedMelonData's with the same file path
        /// </summary>
        private static void DeleteLoadedMelon(string path)
        {
            for (int i = loadedMelons.Count - 1; i >= 0; i--)
                if (loadedMelons[i].filePath == path)
                    loadedMelons.RemoveAt(i);
        }

        /// <summary>
        /// Loads the asset bundle and adds all items in it to the spawn menu.
        /// </summary>
        private static LoadedMelonData LoadMelon(string path)
        {
            LoadedMelonData loadedMelonData = new LoadedMelonData(0, new List<LoadedItemData>());

            string fullPath = Path.Combine(MelonUtils.UserDataDirectory, path);
            string fileHash = MelonHashChecking.GetMelonHash(fullPath);

            LoadedMelonData existingData = GetLoadedMelon(path);
            if (existingData.itemsLoaded != -1 && existingData.fileHash != fileHash) // If there's a .melon file with a different hash
            {
                // Remove references to existing items and unload bundle
                foreach (LoadedItemData item in existingData.loadedItems)
                {
                    SpawnMenu.RemoveItem(item.itemName);
                    CustomItems.DestroySpawnable(item.itemName);
                }
                existingData.bundle.Unload(true);
            }
            else if (existingData.itemsLoaded != -1) // If there's a melon file with the same hash, the -1 is used to not show as an error in the console
            {
                loadedMelonData.itemsLoaded = -1;
                return loadedMelonData;
            }

            DeleteLoadedMelon(existingData.filePath);

            // Load the bundle and do stuff
            if (AssetBundles.LoadFromUserData(path, out AssetBundle bundle))
                loadedMelonData = LoadFromBundle(bundle);

            loadedMelonData.fileHash = fileHash;
            loadedMelonData.filePath = path;
            return loadedMelonData;
        }

        public static LoadedMelonData LoadFromBundle(AssetBundle bundle)
        {
            LoadedMelonData loadedMelonData = new LoadedMelonData(0, new List<LoadedItemData>());

            bundle.hideFlags = HideFlags.DontUnloadUnusedAsset;

            if (bundle.Contains(embeddedAssemblyName) && !hasLoadedEmbeddedAssemblies)
                LoadAssemblyFromMelon(bundle.LoadAsset(embeddedAssemblyName).Cast<TextAsset>().bytes, bundle.name);

            // Find the root custom items gameobject
            UnityEngine.Object customItemsObj = bundle.LoadAsset(customItemsGOName);
            GameObject customItemsGO = null;
            if (customItemsObj != null)
                customItemsGO = customItemsObj.Cast<GameObject>();
            else
                return new LoadedMelonData() { filePath = bundle.name, errorMessage = "No CustomItems GameObject" };

            CustomMonoBehaviourHandler.AddMonoBehavioursFromJson(customItemsGO);
            CustomItems.FixObjectShaders(customItemsGO);

            LocalizedText settingsComp = customItemsGO.GetComponent<LocalizedText>(); // Settings should be in the LocalizedText component
            if (settingsComp != null)
                loadedMelonData = LoadItemsFromMelon(customItemsGO, settingsComp);
            else
                loadedMelonData = LegacyItemLoading.LoadItemsFromMelon(customItemsGO);

            loadedMelonData.bundle = bundle;

            return loadedMelonData;
        }

        /// <summary>
        /// Finds the settings for each item and creates a spawnable object with them.
        /// Adds the items to the spawn menu unless they're marked as hidden.
        /// </summary>
        private static LoadedMelonData LoadItemsFromMelon(GameObject customItemsGO, LocalizedText settingsComp)
        {
            LoadedMelonData loadedMelonData = new LoadedMelonData(0, new List<LoadedItemData>());

            ItemSettings allSettings = JsonConvert.DeserializeObject<ItemSettings>(settingsComp.key); // Convert the json text to an ItemSettings object
            loadedMelonData.author = allSettings.author == "" ? loadedMelonData.author : allSettings.author;

            for (int i = 0; i < customItemsGO.transform.childCount; i++)
            {
                GameObject itemGO = customItemsGO.transform.GetChild(i).gameObject;

                // Check if the current child is in the item settings
                if (allSettings.TryGetItem(itemGO.name, out ItemSettings.Item settings))
                {
                    InfiniteAmmoGuns.CheckForInfiniteAmmoVersion(itemGO, settings.name, settings.category, settings.poolMode, settings.pooledAmount, settings.hideInMenu);
                    SetAudioMixers(itemGO);

                    CustomItems.InvokeOnItemLoaded(itemGO);
                    itemGO.AddComponent<CustomItem>();

                    // Remove any existing items with the same name to avoid conflicts
                    SpawnMenu.RemoveItem(settings.name);
                    CustomItems.DestroySpawnable(settings.name);

                    SpawnableObject spawnable = CustomItems.CreateSpawnableObject(itemGO, settings.name, settings.category, settings.poolMode, settings.pooledAmount, isHidden: settings.hideInMenu);

                    // Add the item to the spawn menu unless it's marked as hidden
                    if (!settings.hideInMenu)
                        SpawnMenu.AddItem(spawnable);

                    loadedMelonData.loadedItems.Add(new LoadedItemData(settings.name, settings.category, settings.hideInMenu));
                    loadedMelonData.itemsLoaded++;
                }
            }

            return loadedMelonData;
        }

        /// <summary>
        /// Loads the assembly from bytes, logs a warning, registers the monobehaviours, and starts the embedded mod controller
        /// </summary>
        private static void LoadAssemblyFromMelon(byte[] bytes, string bundleName)
        {
            Assembly assembly = Assembly.Load(bytes);

            string message = $"[NOTICE] {bundleName} contains executable code";
            if (Preferences.exportEmbeddedAssemblies)
            {
                message += ", exporting .dll";
                File.WriteAllBytes(Path.Combine(MelonUtils.UserDataDirectory, exportedAssembliesDir, assembly.GetName().Name + ".dll"), bytes);
            }
            ModConsole.Msg(ConsoleColor.DarkYellow, message);

            CustomMonoBehaviourHandler.RegisterAllInAssembly(assembly);

            Type[] types = assembly.GetTypes().Where(x => x.IsSubclassOf(typeof(EmbeddedMod))).ToArray();
            if (types.Length > 0)
            {
                Type embeddedModType = types[0];
                EmbeddedMod instance = (EmbeddedMod)Activator.CreateInstance(embeddedModType);
                EmbeddedModController.RegisterMod(instance);
            }
        }

        /// <summary>
        /// Sets the audio mixers of the some common scripts on the item to the ingame ones
        /// </summary>
        internal static void SetAudioMixers(GameObject obj)
        {
            AudioSource[] audioSources = obj.GetComponentsInChildren<AudioSource>();
            foreach (AudioSource source in audioSources)
            {
                if (source.outputAudioMixerGroup == null)
                    source.outputAudioMixerGroup = Audio.sfxMixer;
            }

            GunSFX gunSFX = obj.GetComponentInChildren<GunSFX>();
            if (gunSFX != null)
            {
                gunSFX.interactionOutputMixer = Audio.sfxMixer;
                gunSFX.gunshotOutputMixer = Audio.gunshotMixer;
            }

            ImpactSFX impactSFX = obj.GetComponentInChildren<ImpactSFX>();
            if (impactSFX != null)
                impactSFX.outputMixer = Audio.sfxMixer;

            StabSlash stabSlash = obj.GetComponentInChildren<StabSlash>();
            if (stabSlash != null && stabSlash.bladeAudio != null)
                stabSlash.bladeAudio.outputMixer = Audio.sfxMixer;

            AmbientSFX ambientSFX = obj.GetComponentInChildren<AmbientSFX>();
            if (ambientSFX != null)
                ambientSFX.mixerGroup = Audio.sfxMixer;
        }

        /// <summary>
        /// Recursively finds all .melon files in the given path.
        /// </summary>
        private static string[] GetMelonFilePaths(string path)
        {
            List<string> files = new List<string>();

            foreach (string file in Directory.GetFiles(path))
            {
                // Add the part of the file path after the CustomItems folder to the list if it's a .melon file
                if (Path.GetExtension(file) == customItemsExt)
                    files.Add(file.Split(new string[] { customItemsDir }, StringSplitOptions.None)[1]);
            }

            foreach (string dir in Directory.GetDirectories(path))
            {
                // Search through any folders that don't end with _ignore and add the contents of them to the list
                if (!dir.EndsWith(ignoreFolderSuffix))
                    files.AddRange(GetMelonFilePaths(dir));
            }

            return files.ToArray();
        }

        /// <summary>
        /// Creates the CustomItems and ExportedAssemblies folders in UserData. I really shouldn't need to have this since people can just take about 10 seconds to make it manually themselves but noooo that's too much effort and there might be something "special" about it. Like what does that even mean??? It's a folder, what on earth could possibly be so special about it that they can't just make it themselves?
        /// </summary>
        internal static void CreateDirectories()
        {
            Directory.CreateDirectory(Path.Combine(MelonUtils.UserDataDirectory, customItemsDir));
            Directory.CreateDirectory(Path.Combine(MelonUtils.UserDataDirectory, exportedAssembliesDir));
        }
    }
}
