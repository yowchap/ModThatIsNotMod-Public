using HarmonyLib;
using StressLevelZero.Pool;
using StressLevelZero.Props.Weapons;

namespace ModThatIsNotMod.Internals
{
    /// <summary>
    /// For some patches that are needed on multiple scripts
    /// </summary>
    internal static class HookingHelpers
    {
        public static void Setup()
        {
            Hooking.CreateHook(typeof(Gun).GetMethod("Fire", AccessTools.all), typeof(HookingHelpers).GetMethod("PreFireGun", AccessTools.all), true);
            Hooking.CreateHook(typeof(Gun).GetMethod("EjectCartridge", AccessTools.all), typeof(HookingHelpers).GetMethod("EjectCartridgePrefix", AccessTools.all), true);
            Hooking.CreateHook(typeof(CaseEject).GetMethod("ForceInDirection", AccessTools.all), typeof(HookingHelpers).GetMethod("ForceInDirectionPostfx", AccessTools.all));
            Hooking.CreateHook(typeof(Gun).GetMethod("CompleteSlidePull", AccessTools.all), typeof(HookingHelpers).GetMethod("OnCompleteSlidePull", AccessTools.all));
        }

        private static bool PreFireGun(Gun __instance)
        {
            return GunModifiersController.PreFireGun(__instance) && ShellLoadingController.PreFireGun(__instance);
        }

        private static void EjectCartridgePrefix(Gun __instance)
        {
            GunModifiersController.EjectCartridgePrefix(__instance);
            ShellLoadingController.EjectCartridgePrefix(__instance);
        }

        private static void ForceInDirectionPostfx(CaseEject __instance)
        {
            GunModifiersController.OnCaseEjectSpawned(__instance);
            ShellLoadingController.OnCaseEjectSpawned(__instance);
        }

        private static void OnCompleteSlidePull(Gun __instance)
        {
            GunSlideFixes.OnCompleteSlidePull(__instance);
            GunModifiersController.OnCompleteSlidePull(__instance);
        }
    }
}
