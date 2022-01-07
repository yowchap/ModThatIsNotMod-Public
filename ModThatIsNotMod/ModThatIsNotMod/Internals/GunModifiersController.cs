using HarmonyLib;
using ModThatIsNotMod.MonoBehaviours;
using StressLevelZero.Combat;
using StressLevelZero.Player;
using StressLevelZero.Pool;
using StressLevelZero.Props.Weapons;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using static ModThatIsNotMod.MonoBehaviours.SimpleGunModifiers;
using static StressLevelZero.Pool.PoolSpawner;

namespace ModThatIsNotMod.Internals
{
    internal static class GunModifiersController
    {
        private static Dictionary<int, SimpleGunModifiers> gunMods = new Dictionary<int, SimpleGunModifiers>();

        private static MuzzleFlashType nextMuzzleFlashType;
        private static Queue<Vector3> ejectedCartridgeScales = new Queue<Vector3>();

        private static PlayerInventory playerInventory;


        public static void Setup()
        {
            MethodInfo spawnMuzzleFlareInfo = typeof(PoolSpawner).GetMethod("SpawnMuzzleFlare", AccessTools.all, null, new Type[] { typeof(Vector3), typeof(Quaternion), typeof(MuzzleFlareType) }, null);
            Hooking.CreateHook(spawnMuzzleFlareInfo, typeof(GunModifiersController).GetMethod("SpawnMuzzleFlarePrefix", AccessTools.all), true);
            Hooking.OnPreFireGun += PreOnFireGun;
        }

        public static void SceneSetup()
        {
            playerInventory = GameObject.FindObjectOfType<PlayerInventory>();
        }

        /// <summary>
        /// Saves the gun modifiers to be accessed later
        /// </summary>
        public static void RegisterGun(SimpleGunModifiers modifiers)
        {
            Gun gun = modifiers.gameObject.GetComponent<Gun>();
            int uuid = gun.GetInstanceID();
            if (!gunMods.ContainsKey(uuid))
                gunMods.Add(uuid, modifiers);
            else
                gunMods[uuid] = modifiers;
        }

        /// <summary>
        /// Stops the gun from firing if it needs to be charged or the player doesn't have enough extra ammo (for guns that use more than 1 per shot)
        /// </summary>
        public static bool PreFireGun(Gun gun)
        {
            int uuid = gun.GetInstanceID();
            if (gunMods.ContainsKey(uuid))
            {
                SimpleGunModifiers modifiers = gunMods[uuid];
                bool chargeStatus = CheckChargeStatus(modifiers, gun);
                bool ammoCountStatus = CheckAmmoCountStatus(modifiers);

                if (!ammoCountStatus)
                    gun.blinkHighlightCoroutine = gun.StartCoroutine(gun.CoBlinkHighlight(gun.magazineHelperRenderer));

                return chargeStatus && ammoCountStatus;
            }

            return true;
        }

        /// <summary>
        /// Returns true if the gun is fully charged or doesn't use charging, false otherwise
        /// </summary>
        private static bool CheckChargeStatus(SimpleGunModifiers modifiers, Gun gun)
        {
            if (modifiers.requiresCharging && gun.chamberedCartridge != null)
            {
                if (!modifiers.isCharging && !modifiers.isChargeComplete)
                {
                    modifiers.BeginCharge(gun.triggerGrip.GetHand());
                    return false;
                }
                else
                {
                    return Time.time >= modifiers.chargeCompleteTime;
                }
            }

            return true;
        }

        /// <summary>
        /// Checks that the player has enough ammo to fire the gun
        /// </summary>
        private static bool CheckAmmoCountStatus(SimpleGunModifiers modifiers)
        {
            if (modifiers.ammoPerShot == 1)
                return true;

            if (modifiers.magazineData != null)
            {
                Weight weight = modifiers.magazineData.weight;
                if (playerInventory.GetAmmo(weight) >= modifiers.ammoPerShot - 1)
                    return true;
                return false;
            }
            return true;
        }

        /// <summary>
        /// Determines the type of muzzle flash to use
        /// </summary>
        private static void PreOnFireGun(Gun gun)
        {
            int uuid = gun.GetInstanceID();
            if (gunMods.ContainsKey(uuid))
            {
                SimpleGunModifiers modifiers = gunMods[uuid];
                nextMuzzleFlashType = modifiers.muzzleFlashType;

                if (modifiers.magazineData != null)
                    playerInventory.RemoveAmmo(modifiers.magazineData.weight, modifiers.ammoPerShot - 1);
            }
            else
            {
                nextMuzzleFlashType = MuzzleFlashType.Default;
            }
        }

        /// <summary>
        /// Spawns the correct muzzle flash type
        /// </summary>
        private static bool SpawnMuzzleFlarePrefix(Vector3 Position, Quaternion Direction, MuzzleFlareType muzzleFlareType)
        {
            try
            {
                if (nextMuzzleFlashType == MuzzleFlashType.Silenced)
                    SpawnMuzzleFlare(Position, Direction, Vector3.one * 0.15f, muzzleFlareType);

                return nextMuzzleFlashType == MuzzleFlashType.Default;
            }
            catch { return true; }
        }

        /// <summary>
        /// Stops the slide from locking back if that's been disabled on the gun
        /// </summary>
        public static void OnCompleteSlidePull(Gun gun)
        {
            int uuid = gun.GetInstanceID();
            if (gunMods.ContainsKey(uuid))
            {
                SimpleGunModifiers modifiers = gunMods[uuid];
                if (modifiers.disableSlideLock && gun.chamberedCartridge == null && !gun._isSlideGrabbed)
                {
                    gun.isFiring = false;
                    gun.SlideRelease();
                }
            }
        }

        /// <summary>
        /// Determines the ejected cartridge scale
        /// </summary>
        public static void EjectCartridgePrefix(Gun gun)
        {
            if (gun.shellSpawnTransform != null && gun.chamberedCartridge != null && gun.chamberedCartridge.ammoVariables.cartridgeType != Cart.NONE)
            {
                int uuid = gun.GetInstanceID();
                if (gunMods.ContainsKey(uuid))
                {
                    SimpleGunModifiers modifiers = gunMods[uuid];
                    Vector3 scale = modifiers.ejectedCartridgeScale;
                    ejectedCartridgeScales.Enqueue(scale);
                }
            }
        }

        /// <summary>
        /// Sets the scale of the ejected cartridge
        /// </summary>
        public static void OnCaseEjectSpawned(CaseEject ejectedCase)
        {
            Vector3 scale = Vector3.one;
            if (ejectedCartridgeScales.Count > 0)
                scale = ejectedCartridgeScales.Dequeue();

            ejectedCase.transform.localScale = scale;
        }
    }
}
