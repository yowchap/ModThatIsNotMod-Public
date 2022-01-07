using StressLevelZero.Interaction;
using StressLevelZero.Props.Weapons;
using System;
using System.Reflection;
using UnityEngine;

// -------------------------------------------------------------------------------------------------------
// EVERYTHING HERE IS JUST REDIRECTING TO THE EQUIVALENT MTINM METHODS, IT DOESN'T DO ANYTHING ON IT'S OWN
// -------------------------------------------------------------------------------------------------------

namespace BoneworksModdingToolkit.BoneHook
{
    public static class CustomHooks
    {
        [Obsolete("Use ModThatIsNotMod.Hooking.CreateHook() instead.")]
        public static void CreateHook(MethodInfo original, MethodInfo hook) => ModThatIsNotMod.Hooking.CreateHook(original, hook);

        [Obsolete("Use ModThatIsNotMod.Hooking.CreateHook() instead.")]
        public static void CreateHook(MethodInfo original, MethodInfo hook, bool prefix) => ModThatIsNotMod.Hooking.CreateHook(original, hook, prefix);
    }

    public static class PlayerHooks
    {
        internal static void Setup()
        {
            ModThatIsNotMod.Hooking.OnPlayerDeathImminent += Hooking_OnPlayerDeathImminent;
            ModThatIsNotMod.Hooking.OnPlayerDamageRecieved += Hooking_OnPlayerDamageRecieved;
            ModThatIsNotMod.Hooking.OnPlayerDeath += Hooking_OnPlayerDeath;
            ModThatIsNotMod.Hooking.OnGrabObject += Hooking_OnGrabObject;
            ModThatIsNotMod.Hooking.OnReleaseObject += Hooking_OnReleaseObject;
        }

        private static void Hooking_OnPlayerDeathImminent(bool value) => OnDeathImminent?.Invoke(value);
        private static void Hooking_OnPlayerDamageRecieved(float value) => OnPlayerDamageReceived?.Invoke(value);
        private static void Hooking_OnPlayerDeath() => OnPlayerDeath?.Invoke();
        private static void Hooking_OnGrabObject(GameObject obj, Hand hand) { OnPlayerGrabObject?.Invoke(obj); OnPlayerGrabObjectHand?.Invoke(obj, hand); }
        private static void Hooking_OnReleaseObject(GameObject obj, Hand hand) { OnPlayerReleaseObject?.Invoke(obj); OnPlayerReleaseObjectHand?.Invoke(obj, hand); }

        [Obsolete("Use ModThatIsNotMod.Hooking.OnPlayerDeathImminent instead.")]
        public static event Action<bool> OnDeathImminent;

        [Obsolete("Use ModThatIsNotMod.Hooking.OnPlayerDamageRecieved instead.")]
        public static event Action<float> OnPlayerDamageReceived;

        [Obsolete("Use ModThatIsNotMod.Hooking.OnPlayerDeath instead.")]
        public static event Action OnPlayerDeath;

        [Obsolete("Use ModThatIsNotMod.Hooking.OnGrabObject instead.")]
        public static event Action<GameObject> OnPlayerGrabObject;

        [Obsolete("Use ModThatIsNotMod.Hooking.OnGrabObject instead.")]
        public static event Action<GameObject, Hand> OnPlayerGrabObjectHand;

        [Obsolete("Use ModThatIsNotMod.Hooking.OnReleaseObject instead.")]
        public static event Action<GameObject> OnPlayerReleaseObject;

        [Obsolete("Use ModThatIsNotMod.Hooking.OnReleaseObject instead.")]
        public static event Action<GameObject, Hand> OnPlayerReleaseObjectHand;
    }

    public static class GunHooks
    {
        internal static void Setup()
        {
            ModThatIsNotMod.Hooking.OnPostFireGun += Hooking_OnPostGunFire;
        }

        private static void Hooking_OnPostGunFire(Gun gun) => OnGunFire?.Invoke(gun);

        [Obsolete("Use ModThatIsNotMod.Hooking.OnPostGunFire instead.")]
        public static event Action<Gun> OnGunFire;
    }

    public static class MiscHooks
    {
        internal static void Setup()
        {
            ModThatIsNotMod.Hooking.OnReloadScene += Hooking_OnReloadScene;
            ModThatIsNotMod.Hooking.OnSaveSpotCheckpoint += Hooking_OnSaveSpotCheckpoint;
        }

        private static void Hooking_OnReloadScene() => OnSceneReload?.Invoke();
        private static void Hooking_OnSaveSpotCheckpoint() => OnSavespotSave?.Invoke();

        [Obsolete("Use ModThatIsNotMod.Hooking.OnReloadScene instead.")]
        public static event Action OnSceneReload;

        [Obsolete("Use ModThatIsNotMod.Hooking.OnSaveSpotCheckpoint instead.")]
        public static event Action OnSavespotSave;
    }
}
