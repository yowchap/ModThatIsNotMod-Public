using HarmonyLib;
using ModThatIsNotMod.MonoBehaviours;
using StressLevelZero.Interaction;
using StressLevelZero.Props.Weapons;
using StressLevelZero.SFX;
using System.Collections.Generic;

namespace ModThatIsNotMod.Internals
{
    internal static class GunSlideFixes
    {
        private static HashSet<int> regularSlideGuns = new HashSet<int>();
        private static HashSet<int> pumpSlideGuns = new HashSet<int>();

        private static Dictionary<int, GunFireModes> potentialBurstGuns = new Dictionary<int, GunFireModes>();
        private static HashSet<int> nonBurstGuns = new HashSet<int>();

        private static Dictionary<int, PumpSlideGrip> gunToPumpSlide = new Dictionary<int, PumpSlideGrip>();
        private static Dictionary<int, PumpSlideGrip> gunSfxToPumpSlide = new Dictionary<int, PumpSlideGrip>();
        private static Dictionary<int, float> pumpSlideDistances = new Dictionary<int, float>();


        public static void Setup()
        {
            Hooking.CreateHook(typeof(GunSFX).GetMethod("SlideRelease", AccessTools.all), typeof(GunSlideFixes).GetMethod("OnSlideRelease", AccessTools.all));
            Hooking.CreateHook(typeof(PumpSlideGrip).GetMethod("OnAttachedToHand", AccessTools.all), typeof(GunSlideFixes).GetMethod("PreAttachedToHand", AccessTools.all), true);

            Hooking.OnPostFireGun += OnPostFireGun;
        }

        /// <summary>
        /// Makes guns always play the slide release sound
        /// </summary>
        public static void OnCompleteSlidePull(Gun gun)
        {
            int uuid = gun.GetInstanceID();
            if (!regularSlideGuns.Contains(uuid) && !pumpSlideGuns.Contains(uuid) && gun.GetComponentInChildren<PumpSlideGrip>() != null)
                pumpSlideGuns.Add(uuid);
            else if (!regularSlideGuns.Contains(uuid) && !pumpSlideGuns.Contains(uuid))
                regularSlideGuns.Add(uuid);

            bool isBurstGun = false;
            if (potentialBurstGuns.TryGetValue(uuid, out GunFireModes fireModes))
            {
                isBurstGun = fireModes.curFireMode == GunFireModes.FireMode.Burst;
            }
            else if (!nonBurstGuns.Contains(uuid))
            {
                GunFireModes newFireModes = gun.GetComponent<GunFireModes>();
                if (newFireModes != null)
                {
                    potentialBurstGuns.Add(uuid, newFireModes);
                    isBurstGun = newFireModes.curFireMode == GunFireModes.FireMode.Burst;
                }
                else
                {
                    nonBurstGuns.Add(uuid);
                }
            }

            bool isPumpGun = pumpSlideGuns.Contains(uuid);

            if (isPumpGun || (gun._isSlideGrabbed && !isBurstGun))
                gun.isFiring = false;
        }

        /// <summary>
        /// Locks the pump on shotguns if there's a round chambered
        /// </summary>
        private static void OnSlideRelease(GunSFX __instance)
        {
            int uuid = __instance.GetInstanceID();
            PumpSlideGrip pump = null;
            if (!regularSlideGuns.Contains(uuid) && !gunSfxToPumpSlide.TryGetValue(uuid, out pump))
            {
                pump = __instance.GetComponentInChildren<PumpSlideGrip>();
                if (pump == null)
                {
                    regularSlideGuns.Add(uuid);
                    return;
                }
                gunSfxToPumpSlide.Add(uuid, pump);
            }
            else if (regularSlideGuns.Contains(uuid)) { return; }

            if (pump.gun.chamberedCartridge != null)
            {
                uuid = pump.GetInstanceID();
                if (!pumpSlideDistances.ContainsKey(uuid))
                    pumpSlideDistances.Add(uuid, pump.slideDistance);
                pump.slideDistance = 0;
            }
        }

        /// <summary>
        /// Resets pump slide distances so they can be moved again after firing.
        /// </summary>
        private static void OnPostFireGun(Gun gun)
        {
            int uuid = gun.GetInstanceID();
            PumpSlideGrip pump = null;
            if (!regularSlideGuns.Contains(uuid) && !gunToPumpSlide.TryGetValue(uuid, out pump))
            {
                pump = gun.GetComponentInChildren<PumpSlideGrip>();
                if (pump == null)
                {
                    regularSlideGuns.Add(uuid);
                    return;
                }
                gunToPumpSlide.Add(uuid, pump);
            }
            else if (regularSlideGuns.Contains(uuid)) { return; }

            uuid = pump.GetInstanceID();
            if (pumpSlideDistances.ContainsKey(uuid))
                pump.slideDistance = pumpSlideDistances[uuid];
        }

        /// <summary>
        /// Reset the pump slide distance when you grab it DEFINITELY NOT BECAUSE STUFF BREAKS OTHERWISE THIS IS A 100% INTENDED FEATURE
        /// </summary>
        private static void PreAttachedToHand(PumpSlideGrip __instance)
        {
            int uuid = __instance.GetInstanceID();
            if (pumpSlideDistances.ContainsKey(uuid))
                __instance.slideDistance = pumpSlideDistances[uuid];
        }
    }
}
