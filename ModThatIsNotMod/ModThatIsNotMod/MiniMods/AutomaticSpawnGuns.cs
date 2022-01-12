using HarmonyLib;
using ModThatIsNotMod.Internals;
using StressLevelZero.Props.Weapons;

namespace ModThatIsNotMod.MiniMods
{
    internal static class AutomaticSpawnGuns
    {
        public static void Setup()
        {
            if (Preferences.automaticSpawnGuns)
                Hooking.CreateHook(typeof(SpawnGun).GetMethod("OnFire", AccessTools.all), typeof(AutomaticSpawnGuns).GetMethod("OnSpawnGunFire", AccessTools.all), true);
        }

        private static void OnSpawnGunFire(SpawnGun __instance)
        {
            __instance.isAutomatic = true;
        }
    }
}
