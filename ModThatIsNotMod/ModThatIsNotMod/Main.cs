using MelonLoader;
using ModThatIsNotMod.BoneMenu;
using ModThatIsNotMod.Internals;
using ModThatIsNotMod.Legacy;
using ModThatIsNotMod.MiniMods;
using ModThatIsNotMod.MonoBehaviours;
using ModThatIsNotMod.Pooling;
using ModThatIsNotMod.RandomShit;

namespace ModThatIsNotMod
{
    internal static class BuildInfo
    {
        public const string Name = "ModThatIsNotMod"; // Name of the Mod.  (MUST BE SET)
        public const string Author = "YOWChap"; // Author of the Mod.  (Set as null if none)
        public const string Company = null; // Company that made the Mod.  (Set as null if none)
        public const string Version = "0.3.1"; // Version of the Mod.  (MUST BE SET)
        public const string DownloadLink = null; // Download Link for the Mod.  (Set as null if none)
    }

    internal class Main : MelonMod
    {
        private readonly string packageUrl = "https://boneworks.thunderstore.io/package/gnonme/ModThatIsNotMod/";
        private int levelsLoaded = 0;

        public override void OnApplicationStart()
        {
            // Stuff that should happen first I guess
            VersionChecking.CheckModVersion(this, packageUrl);
            Preferences.Initialize();
            BackwardsCompatibility.Setup();

            Hooking.SetHarmony(HarmonyInstance);
            WarningSilencers.SilenceWarningMessages();

            // Creating directories
            ItemLoading.CreateDirectories();

            // Other stuff
            Hooking.InitHooks();
            HookingHelpers.Setup();
            SpawnMenu.HookPopulateSpawnMenu();

            AmmoPouchGrabbing.Setup();
            AutomaticSpawnGuns.Setup();
            BetterMagEject.Setup();
            GunSlideFixes.Setup();

            CustomMonoBehaviourHandler.Setup();
            GunModifiersController.Setup();
            ShellLoadingController.Setup();

            MenuManager.InitialSetup();
            DefaultMenu.CreateDefaultElements();

            Notifications.LoadBundleAndSetup();

            AdManager.StartCoroutines();

            Stats.ReportStats();
            Stats.SetupCustomMapsHook();
        }

        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            SimplePoolManager.ClearPools();
            Player.FindObjectReferences();
            EmbeddedModController.OnSceneWasLoaded(buildIndex, sceneName);
        }

        public override void OnSceneWasInitialized(int buildIndex, string sceneName)
        {
            ModConsole.Msg($"Initialized {buildIndex}/{sceneName}", LoggingMode.DEBUG);
            Player.FindObjectReferences();
            RadialMenuEverywhere.OnSceneWasInitialized(buildIndex);
            Audio.GetAudioMixers();
            BackwardsCompatibility.OnLevelWasInitialized();

            if (++levelsLoaded == 1)
                ItemLoading.LoadMelons();
            else if (levelsLoaded > 2 && Preferences.reloadItemsOnLevelChange)
                ItemLoading.ReloadMelons();

            CustomItems.OnLevelWasInitialized();

            GunModifiersController.SceneSetup();

            MenuManager.SceneSetup();

            TabloidMode.DoTabloidStuff();
            AdManager.CreateBaseAd();

            Notifications.SetupNotificationsForScene(buildIndex);

            EmbeddedModController.OnSceneWasInitialized(buildIndex, sceneName);
        }

        public override void OnUpdate()
        {
            MenuManager.OnUpdate();
            BetterMagEject.OnUpdate();
            Notifications.UpdateNotifications();
            BackwardsCompatibility.OnUpdate();
            EmbeddedModController.OnUpdate();
        }

        public override void OnLateUpdate()
        {
            EmbeddedModController.OnLateUpdate();
        }

        public override void OnFixedUpdate()
        {
            EmbeddedModController.OnFixedUpdate();
        }

        public override void OnApplicationQuit()
        {
            EmbeddedModController.OnApplicationQuit();
        }
    }
}
