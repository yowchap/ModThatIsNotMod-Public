using HarmonyLib;
using MelonLoader;
using System;
using System.Net.Http;
using System.Reflection;
using UnityEngine;

namespace ModThatIsNotMod.Internals
{
    internal static class Stats
    {
        private const string modName = "mtinm";

        private const string versionUrl = "https://stats.extraes.xyz/increment?mod=" + modName + "&key=" + BuildInfo.Version;
        private const string miscStatsUrlBase = "https://stats.extraes.xyz/increment?mod=" + modName + "&key=";

        private static readonly HttpClient client = new HttpClient();

        private static Type mapLoaderType;


        public static void ReportStats()
        {
            if (Preferences.disableReportingStats)
                return;

            string playerPrefsVersionKey = $"MTINM_ReportedVersion_{BuildInfo.Version}";
            string playerPrefsMiscKey = "MTINM_ReportedMiscStats";
            if (!PlayerPrefs.HasKey(playerPrefsVersionKey))
            {
                try { client.GetAsync("https://stats.extraes.xyz/register?mod=" + modName + "&key=" + BuildInfo.Version + "&value=0"); }
                catch { ModConsole.Msg("Failed to register version " + BuildInfo.Version, LoggingMode.DEBUG); }

                if (Preferences.hasRegisteredVersionLaunch)
                {
                    try
                    {
                        client.GetAsync("https://stats.extraes.xyz/register?mod=" + modName + "&key=" + BuildInfo.Version + "_fucked_up" + "&value=0");
                        client.GetAsync("https://stats.extraes.xyz/increment?mod=" + modName + "&key=" + BuildInfo.Version + "_fucked_up");
                        PlayerPrefs.SetInt(playerPrefsVersionKey, 1);
                    }
                    catch { ModConsole.Msg("Fucked up registering version " + BuildInfo.Version, LoggingMode.DEBUG); }
                }
                else
                {
                    try
                    {
                        client.GetAsync(versionUrl);
                        Preferences.hasRegisteredVersionLaunch.SetValue(true);
                        PlayerPrefs.SetInt(playerPrefsVersionKey, 1);
                    }
                    catch { ModConsole.Msg("Failed to report version stat", LoggingMode.DEBUG); }
                }
            }

            try { client.GetAsync("https://stats.extraes.xyz/register?mod=" + modName + "&key=" + BuildInfo.Version + "_not_unique" + "&value=0"); }
            catch { ModConsole.Msg("Failed to register non-unique version " + BuildInfo.Version, LoggingMode.DEBUG); }
            try { client.GetAsync(versionUrl + "_not_unique"); }
            catch { ModConsole.Msg("Failed to report non-unique version stat", LoggingMode.DEBUG); }

            if (!PlayerPrefs.HasKey(playerPrefsMiscKey) && !Preferences.isFirstLoad)
            {
                if (Preferences.hasRegisteredMiscStats)
                {
                    try
                    {
                        client.GetAsync("https://stats.extraes.xyz/register?mod=" + modName + "&key=" + BuildInfo.Version + "_fucked_up_misc_stats" + "&value=0");
                        client.GetAsync("https://stats.extraes.xyz/increment?mod=" + modName + "&key=" + BuildInfo.Version + "_fucked_up_misc_stats");
                        PlayerPrefs.SetInt(playerPrefsMiscKey, 1);
                    }
                    catch { ModConsole.Msg("Fucked up registering misc stats", LoggingMode.DEBUG); }
                }
                else
                {
                    try
                    {
                        if (Preferences.boneMenuGestureEnabled && Preferences.boneMenuRadialButtonEnabled)
                            client.GetAsync(miscStatsUrlBase + "bonemenu_both");
                        else if (Preferences.boneMenuGestureEnabled)
                            client.GetAsync(miscStatsUrlBase + "bonemenu_gesture");
                        else
                            client.GetAsync(miscStatsUrlBase + "bonemenu_radial");

                        if (Preferences.loggingMode == LoggingMode.DEBUG)
                            client.GetAsync(miscStatsUrlBase + "debug_logging");

                        if (!Preferences.allowEmbeddedCode)
                            client.GetAsync(miscStatsUrlBase + "disabled_embedded_code");

                        if (Preferences.tabloidMode)
                            client.GetAsync(miscStatsUrlBase + "tabloid_mode");

                        if (Preferences.autoSpawnAds)
                            client.GetAsync(miscStatsUrlBase + "happy_fun_time");

                        Preferences.hasRegisteredMiscStats.SetValue(true);
                        PlayerPrefs.SetInt(playerPrefsMiscKey, 1);
                    }
                    catch { ModConsole.Msg("Failed to report misc stats", LoggingMode.DEBUG); }
                }
            }
        }

        public static void ReportItemStats(int melonsLoaded)
        {
            if (Preferences.disableReportingStats)
                return;

            string playerPrefsItemsKey = "MTINM_ItemsLoadedReported";
            if (!PlayerPrefs.HasKey(playerPrefsItemsKey) || Mathf.Abs(PlayerPrefs.GetInt(playerPrefsItemsKey) - melonsLoaded) >= 10)
            {
                try
                {
                    if (melonsLoaded == 0)
                        client.GetAsync(miscStatsUrlBase + "no_items");
                    if (melonsLoaded >= 10)
                        client.GetAsync(miscStatsUrlBase + "10_items");
                    if (melonsLoaded >= 25)
                        client.GetAsync(miscStatsUrlBase + "25_items");
                    if (melonsLoaded >= 50)
                        client.GetAsync(miscStatsUrlBase + "50_items");
                    if (melonsLoaded >= 75)
                        client.GetAsync(miscStatsUrlBase + "75_items");
                    if (melonsLoaded >= 100)
                        client.GetAsync(miscStatsUrlBase + "100_items");
                    if (melonsLoaded >= 150)
                        client.GetAsync(miscStatsUrlBase + "150_items");
                    if (melonsLoaded >= 200)
                        client.GetAsync(miscStatsUrlBase + "200_items");

                    PlayerPrefs.SetInt(playerPrefsItemsKey, melonsLoaded);
                }
                catch { ModConsole.Msg("Failed to report item stats", LoggingMode.DEBUG); }
            }
        }

        public static void SetupCustomMapsHook()
        {
            if (Preferences.disableReportingStats)
                return;

            // Tabloid wanted this so here it is
            foreach (MelonMod mod in MelonHandler.Mods)
            {
                if (mod.Info.Name == "Custom Maps")
                {
                    try
                    {
                        Assembly assembly = mod.Assembly;
                        Type customMapsType = assembly.GetType("CustomMaps.CustomMaps");
                        mapLoaderType = assembly.GetType("CustomMaps.MapLoader");
                        EventInfo eventInfo = customMapsType.GetEvent("OnCustomMapLoad", AccessTools.all);
                        MethodInfo hookMethod = typeof(Stats).GetMethod("OnCustomMapLoaded", AccessTools.all);
                        eventInfo.AddEventHandler(null, Delegate.CreateDelegate(eventInfo.EventHandlerType, hookMethod));
                        ModConsole.Msg("Hooked custom map loading for stats", LoggingMode.DEBUG);
                    }
                    catch { ModConsole.Msg("Custom maps stats hooking failed", LoggingMode.DEBUG); }

                    break;
                }
            }
        }

        private static void OnCustomMapLoaded(string name)
        {
            string mapName = name;
            if (name == "map.bcm")
            {
                try
                {
                    FieldInfo mapInfoField = mapLoaderType.GetField("mapInfo", AccessTools.all);
                    dynamic mapInfo = mapInfoField.GetValue(null);
                    mapName = mapInfo.mapName;
                }
                catch { }
            }

            try
            {
                client.GetAsync("https://stats.extraes.xyz/register?mod=custom_map_stats&key=" + mapName + "&value=0");
            }
            catch { ModConsole.Msg("Failed to register map stats for " + mapName, LoggingMode.DEBUG); }

            try
            {
                client.GetAsync("https://stats.extraes.xyz/increment?mod=custom_map_stats&key=total_loads");
                client.GetAsync("https://stats.extraes.xyz/increment?mod=custom_map_stats&key=" + mapName);
            }
            catch { ModConsole.Msg("Failed to update map stats for " + mapName, LoggingMode.DEBUG); }
        }
    }
}
