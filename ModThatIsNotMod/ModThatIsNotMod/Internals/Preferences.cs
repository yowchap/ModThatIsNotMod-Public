using MelonLoader;
using System;
using UnityEngine;

namespace ModThatIsNotMod.Internals
{
    internal static class Preferences
    {
        private static readonly string category = "ModThatIsNotMod";
        private static readonly string debugCategory = "ModThatIsNotModDebug";


        public static ModPref<LoggingMode> loggingMode = new ModPref<LoggingMode>(debugCategory, "LoggingMode", LoggingMode.NORMAL);

        // Debug prefs
        public static ModPref<bool> allowEmbeddedCode = new ModPref<bool>(debugCategory, "AllowEmbeddedCode", true);
        public static ModPref<bool> exportEmbeddedAssemblies = new ModPref<bool>(debugCategory, "ExportEmbeddedAssemblies", true);
        public static ModPref<bool> silenceUnhollowerWarnings = new ModPref<bool>(debugCategory, "SilenceUnhollowerWarnings", true);
        public static ModPref<bool> reloadItemsOnLevelChange = new ModPref<bool>(debugCategory, "ReloadItemsOnLevelChange", false);

        // Regular prefs
        public static ModPref<bool> enableMagEjectButton = new ModPref<bool>(category, "MagEjectButtonEnabled", true);
        public static ModPref<int> pressesToEjectMag = new ModPref<int>(category, "PressesToEjectMag", 1);
        public static ModPref<bool> autoEjectEmptyMags = new ModPref<bool>(category, "AutoEjectEmptyMags", false);
        public static ModPref<bool> overrideMagEjectSettings = new ModPref<bool>(category, "OverrideMagEjectSettings", false);

        public static ModPref<bool> boneMenuGestureEnabled = new ModPref<bool>(category, "BoneMenuGestureEnabled", false);
        public static ModPref<bool> boneMenuRadialButtonEnabled = new ModPref<bool>(category, "BoneMenuRadialButtonEnabled", true);

        public static ModPref<bool> menuOnRightHand = new ModPref<bool>(category, "MenuOnRightHand", false);
        public static ModPref<float> menuOffsetX = new ModPref<float>(category, "MenuOffsetX", 0.05f);
        public static ModPref<float> menuOffsetY = new ModPref<float>(category, "MenuOffsetY", -0.05f);
        public static ModPref<float> menuOffsetZ = new ModPref<float>(category, "MenuOffsetZ", 0.05f);

        public static ModPref<bool> automaticSpawnGuns = new ModPref<bool>(category, "AutomaticSpawnGuns", false);
        public static ModPref<bool> utilGunInRadialMenu = new ModPref<bool>(category, "UtilGunInRadialMenu", true);
        public static ModPref<string[]> infiniteAmmoGuns = new ModPref<string[]>(category, "InfiniteAmmoGuns", new string[] { "Gun names here", "CaSe SeNsItIvE" });

        // Meme prefs
        public static ModPref<bool> tabloidMode = new ModPref<bool>(category, "TabloidMode", false);
        public static ModPref<bool> autoSpawnAds = new ModPref<bool>(category, "HappyFunTime", false);
        public static ModPref<Vector2> timeBetweenAds = new ModPref<Vector2>(category, "HappyFunTimeDelay", new Vector2(30, 300));


        public static void Initialize()
        {
            MelonPreferences.CreateCategory(category);
            MelonPreferences.CreateCategory(debugCategory);

            CreatePrefs();
            LoadPrefs();
        }

        private static void CreatePrefs()
        {
            // Debug prefs
            loggingMode.CreateEntry();
            allowEmbeddedCode.CreateEntry();
            exportEmbeddedAssemblies.CreateEntry();
            silenceUnhollowerWarnings.CreateEntry();
            reloadItemsOnLevelChange.CreateEntry();

            // Regular prefs
            enableMagEjectButton.CreateEntry();
            pressesToEjectMag.CreateEntry();
            autoEjectEmptyMags.CreateEntry();
            overrideMagEjectSettings.CreateEntry();

            boneMenuGestureEnabled.CreateEntry();
            boneMenuRadialButtonEnabled.CreateEntry();

            menuOnRightHand.CreateEntry();
            menuOffsetX.CreateEntry();
            menuOffsetY.CreateEntry();
            menuOffsetZ.CreateEntry();

            automaticSpawnGuns.CreateEntry();
            utilGunInRadialMenu.CreateEntry();
            infiniteAmmoGuns.CreateEntry();

            // Meme prefs
            tabloidMode.CreateEntry(true);
            autoSpawnAds.CreateEntry(true);
            timeBetweenAds.CreateEntry(!autoSpawnAds.ReadValue());

            MelonPreferences.Save();
        }

        private static void LoadPrefs()
        {
            // Debug prefs
            loggingMode.ReadValue();
            ModConsole.Msg($"Using {loggingMode.value} logging mode", LoggingMode.MINIMAL);

            allowEmbeddedCode.ReadValue();
            exportEmbeddedAssemblies.ReadValue();
            silenceUnhollowerWarnings.ReadValue();
            reloadItemsOnLevelChange.ReadValue();

            // Normal prefs
            enableMagEjectButton.ReadValue();
            pressesToEjectMag.ReadValue();
            autoEjectEmptyMags.ReadValue();
            overrideMagEjectSettings.ReadValue();

            boneMenuGestureEnabled.ReadValue();
            boneMenuRadialButtonEnabled.ReadValue();

            menuOnRightHand.ReadValue();
            menuOffsetX.ReadValue();
            menuOffsetY.ReadValue();
            menuOffsetZ.ReadValue();

            automaticSpawnGuns.ReadValue();
            utilGunInRadialMenu.ReadValue();
            infiniteAmmoGuns.ReadValue();

            // Random prefs
            tabloidMode.ReadValue();
            if (tabloidMode.value)
                ModConsole.Msg(ConsoleColor.Magenta, "TABLOID MODE POGGERS", LoggingMode.MINIMAL);

            autoSpawnAds.ReadValue();
            if (autoSpawnAds.value)
                ModConsole.Msg(ConsoleColor.Magenta, "HAPPY FUN TIME MODE POGGERS", LoggingMode.MINIMAL);

            timeBetweenAds.ReadValue();
        }
    }
}
