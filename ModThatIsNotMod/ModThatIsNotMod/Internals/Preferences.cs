using MelonLoader;
using System;
using System.IO;
using UnityEngine;

namespace ModThatIsNotMod.Internals
{
    internal static class Preferences
    {
        private static readonly string category = "ModThatIsNotMod";
        private static readonly string debugCategory = "ModThatIsNotModDebug";
        private static readonly string filePath = "UserData/ModThatIsNotMod.cfg";


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

        public static ModPref<bool> notificationsOnRightHand = new ModPref<bool>(category, "NotificationsOnRightHand", false);
        public static ModPref<float> notificationOffsetX = new ModPref<float>(category, "NotificationOffsetX", 0.025f);
        public static ModPref<float> notificationOffsetY = new ModPref<float>(category, "NotificationOffsetY", 0.15f);
        public static ModPref<float> notificationOffsetZ = new ModPref<float>(category, "NotificationOffsetZ", 0.05f);

        public static ModPref<bool> automaticSpawnGuns = new ModPref<bool>(category, "AutomaticSpawnGuns", false);
        public static ModPref<bool> utilGunInRadialMenu = new ModPref<bool>(category, "UtilGunInRadialMenu", true);
        public static ModPref<string[]> infiniteAmmoGuns = new ModPref<string[]>(category, "InfiniteAmmoGuns", new string[] { "Gun names here", "CaSe SeNsItIvE" });

        // Meme prefs
        public static ModPref<bool> tabloidMode = new ModPref<bool>(category, "TabloidMode", false);
        public static ModPref<bool> autoSpawnAds = new ModPref<bool>(category, "HappyFunTime", false);
        public static ModPref<Vector2> timeBetweenAds = new ModPref<Vector2>(category, "HappyFunTimeDelay", new Vector2(30, 300));


        public static void Initialize()
        {
            MelonPreferences_Category defaultCategoryObj = MelonPreferences.CreateCategory(category);
            MelonPreferences_Category debugCategoryObj = MelonPreferences.CreateCategory(debugCategory);

            if (File.Exists(filePath))
            {
                defaultCategoryObj.SetFilePath(filePath);
                debugCategoryObj.SetFilePath(filePath);
                CreatePrefs();
                LoadPrefs();
                MelonPreferences.Save();
            }
            else
            {
                CreatePrefs();
                LoadPrefs();
                defaultCategoryObj.SetFilePath(filePath);
                debugCategoryObj.SetFilePath(filePath);
                MelonPreferences.Save();

                ModConsole.Msg(ConsoleColor.Green, "Your preferences for ModThatIsNotMod have been moved from MelonPreferences.cfg to ModThatIsNotMod.cfg for organizational purposes.", LoggingMode.MINIMAL);
                Notifications.SendNotification("Your preferences for ModThatIsNotMod have been moved from\nMelonPreferences.cfg\nto\nModThatIsNotMod.cfg\nfor organizational purposes.", 15);
            }
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

            notificationsOnRightHand.CreateEntry();
            notificationOffsetX.CreateEntry();
            notificationOffsetY.CreateEntry();
            notificationOffsetZ.CreateEntry();

            automaticSpawnGuns.CreateEntry();
            utilGunInRadialMenu.CreateEntry();
            infiniteAmmoGuns.CreateEntry();

            // Meme prefs
            tabloidMode.CreateEntry(true);
            autoSpawnAds.CreateEntry(true);
            timeBetweenAds.CreateEntry(!autoSpawnAds.ReadValue());
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

            notificationsOnRightHand.ReadValue();
            notificationOffsetX.ReadValue();
            notificationOffsetY.ReadValue();
            notificationOffsetZ.ReadValue();

            automaticSpawnGuns.ReadValue();
            utilGunInRadialMenu.ReadValue();
            infiniteAmmoGuns.ReadValue();

            // Random prefs
            tabloidMode.ReadValue();
            if (tabloidMode)
                ModConsole.Msg(ConsoleColor.Magenta, "TABLOID MODE POGGERS", LoggingMode.MINIMAL);

            autoSpawnAds.ReadValue();
            if (autoSpawnAds)
                ModConsole.Msg(ConsoleColor.Magenta, "HAPPY FUN TIME MODE POGGERS", LoggingMode.MINIMAL);

            timeBetweenAds.ReadValue();
        }
    }
}
