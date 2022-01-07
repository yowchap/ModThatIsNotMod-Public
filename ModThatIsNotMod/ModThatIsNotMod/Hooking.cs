using HarmonyLib;
using MelonLoader;
using StressLevelZero.Interaction;
using StressLevelZero.Props.Weapons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace ModThatIsNotMod
{
    public static class Hooking
    {
        private static HarmonyLib.Harmony baseHarmony;

        private static Queue<DelayedHookData> delayedHooks = new Queue<DelayedHookData>();

        public static event Action<Gun> OnPreFireGun;
        public static event Action<Gun> OnPostFireGun;

        public static event Action<GameObject, Hand> OnGrabObject;
        public static event Action<GameObject, Hand> OnReleaseObject;

        public static event Action<Grip, Hand> OnGripAttached;
        public static event Action<Grip, Hand> OnGripDetached;

        public static event Action<float> OnPlayerDamageRecieved;
        public static event Action<bool> OnPlayerDeathImminent;
        public static event Action OnPlayerDeath;

        public static event Action OnReloadScene;
        public static event Action OnSaveSpotCheckpoint;


        internal static void SetHarmony(HarmonyLib.Harmony harmony) => Hooking.baseHarmony = harmony;

        /// <summary>
        /// Creates default hooks and runs delayed ones.
        /// </summary>
        internal static void InitHooks()
        {
            CreateHook(typeof(Gun).GetMethod("OnFire", AccessTools.all), typeof(Hooking).GetMethod("OnFirePrefix", AccessTools.all), true);
            CreateHook(typeof(Gun).GetMethod("OnFire", AccessTools.all), typeof(Hooking).GetMethod("OnFirePostfix", AccessTools.all));
            CreateHook(typeof(Hand).GetMethod("AttachObject", AccessTools.all), typeof(Hooking).GetMethod("OnAttachObjectPostfix", AccessTools.all));
            CreateHook(typeof(Hand).GetMethod("DetachObject", AccessTools.all), typeof(Hooking).GetMethod("OnDetachObjectPostfix", AccessTools.all));
            CreateHook(typeof(Grip).GetMethod("OnAttachedToHand", AccessTools.all), typeof(Hooking).GetMethod("OnGripAttachedPostfix", AccessTools.all));
            CreateHook(typeof(Grip).GetMethod("OnDetachedFromHand", AccessTools.all), typeof(Hooking).GetMethod("OnGripDetachedPostfix", AccessTools.all));
            CreateHook(typeof(Data_Manager).GetMethod("RELOADSCENE", AccessTools.all), typeof(Hooking).GetMethod("OnReloadScenePostfix", AccessTools.all));
            CreateHook(typeof(Data_Manager).GetMethod("SAVESPOTCHECKPOINT", AccessTools.all), typeof(Hooking).GetMethod("OnSaveSpotCheckpointPostfix", AccessTools.all));

            Player_Health.add_OnPlayerDamageReceived(OnPlayerDamageRecieved);
            Player_Health.add_OnDeathImminent(OnPlayerDeathImminent);
            Player_Health.add_OnPlayerDeath(OnPlayerDeath); // TODO: Test this because mara said it's not working (update: it really doesn't work wtf)

            while (delayedHooks.Count > 0)
            {
                DelayedHookData data = delayedHooks.Dequeue();
                CreateHook(data.original, data.hook, data.isPrefix);
            }
        }

        /// <summary>
        /// Hooks the method and debug logs some info.
        /// </summary>
        public static void CreateHook(MethodInfo original, MethodInfo hook, bool isPrefix = false)
        {
            if (baseHarmony == null)
            {
                delayedHooks.Enqueue(new DelayedHookData(original, hook, isPrefix));
                return;
            }

            Assembly callingAssembly = Assembly.GetCallingAssembly();
            MelonMod callingMod = MelonHandler.Mods.FirstOrDefault(x => x.Assembly.FullName == callingAssembly.FullName);
            HarmonyLib.Harmony harmony = callingMod != null ? callingMod.HarmonyInstance : baseHarmony;

            HarmonyMethod prefix = isPrefix ? new HarmonyMethod(hook) : null;
            HarmonyMethod postfix = isPrefix ? null : new HarmonyMethod(hook);
            harmony.Patch(original, prefix: prefix, postfix: postfix);

            ModConsole.Msg($"New {(isPrefix ? "PREFIX" : "POSTFIX")} on {original.DeclaringType.Name}.{original.Name} to {hook.DeclaringType.Name}.{hook.Name}", LoggingMode.DEBUG);
        }

        private static void OnFirePrefix(Gun __instance) => OnPreFireGun?.Invoke(__instance);
        private static void OnFirePostfix(Gun __instance) => OnPostFireGun?.Invoke(__instance);

        private static void OnAttachObjectPostfix(GameObject objectToAttach, Hand __instance) => OnGrabObject?.Invoke(objectToAttach, __instance);
        private static void OnDetachObjectPostfix(GameObject objectToDetach, Hand __instance) => OnReleaseObject?.Invoke(objectToDetach, __instance);

        private static void OnGripAttachedPostfix(Grip __instance, Hand hand) => OnGripAttached?.Invoke(__instance, hand);
        private static void OnGripDetachedPostfix(Grip __instance, Hand hand) => OnGripDetached?.Invoke(__instance, hand);

        private static void OnReloadScenePostfix() => OnReloadScene?.Invoke();
        private static void OnSaveSpotCheckpointPostfix() => OnSaveSpotCheckpoint?.Invoke();


        struct DelayedHookData
        {
            public MethodInfo original;
            public MethodInfo hook;
            public bool isPrefix;

            public DelayedHookData(MethodInfo original, MethodInfo hook, bool isPrefix)
            {
                this.original = original;
                this.hook = hook;
                this.isPrefix = isPrefix;
            }
        }
    }
}
