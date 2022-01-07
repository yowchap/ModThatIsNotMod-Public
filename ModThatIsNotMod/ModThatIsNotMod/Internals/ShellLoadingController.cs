using HarmonyLib;
using ModThatIsNotMod.MonoBehaviours;
using StressLevelZero.Combat;
using StressLevelZero.Interaction;
using StressLevelZero.Pool;
using StressLevelZero.Props.Weapons;
using System.Collections.Generic;
using UnityEngine;

namespace ModThatIsNotMod.Internals
{
    internal static class ShellLoadingController
    {
        private static Dictionary<int, ShellLoader> shellLoaders = new Dictionary<int, ShellLoader>();
        private static Dictionary<int, ShellLoader> shellLoadersMagSocket = new Dictionary<int, ShellLoader>();

        private static Queue<bool> enableEjectedCasesQueue = new Queue<bool>();


        public static void Setup()
        {
            Hooking.CreateHook(typeof(MagazineSocket).GetMethod("OnPlugLocked", AccessTools.all), typeof(ShellLoadingController).GetMethod("PostOnPlugLocked", AccessTools.all));
            Hooking.CreateHook(typeof(Gun).GetMethod("UpdateArt", AccessTools.all), typeof(ShellLoadingController).GetMethod("UpdateArtPostfix", AccessTools.all));
            Hooking.CreateHook(typeof(Gun).GetMethod("PullCartridge", AccessTools.all), typeof(ShellLoadingController).GetMethod("PrePullCartridge", AccessTools.all), true);
            Hooking.OnPreFireGun += OnPreFireGun;
            Hooking.OnPostFireGun += OnPostFireGun;
        }

        public static void RegisterGun(ShellLoader shellLoader)
        {
            Gun gun = shellLoader.gameObject.GetComponent<Gun>();
            int uuid = gun.GetInstanceID();
            if (!shellLoaders.ContainsKey(uuid))
                shellLoaders.Add(uuid, shellLoader);
            else
                shellLoaders[uuid] = shellLoader;

            MagazineSocket magazineSocket = gun.magazineSocket;
            int magSocketUuid = magazineSocket.GetInstanceID();
            if (!shellLoadersMagSocket.ContainsKey(magSocketUuid))
                shellLoadersMagSocket.Add(magSocketUuid, shellLoader);
            else
                shellLoadersMagSocket[magSocketUuid] = shellLoader;
        }

        /// <summary>
        /// Disables the renderers and colliders on the magazine and ejects it.
        /// This way it looks like the shell was loaded but the mag socket is still empty so you can put another in.
        /// </summary>
        private static void PostOnPlugLocked(MagazineSocket __instance)
        {
            int uuid = __instance.GetInstanceID();
            if (shellLoadersMagSocket.ContainsKey(uuid))
            {
                ShellLoader loader = shellLoadersMagSocket[uuid];

                if (loader.InsertShell())
                {
                    Magazine magazine = __instance.GetMagazine();
                    MeshRenderer[] meshRenderers = magazine.gameObject.GetComponentsInChildren<MeshRenderer>(true);
                    Collider[] colliders = magazine.gameObject.GetComponentsInChildren<Collider>(true);

                    for (int i = 0; i < meshRenderers.Length; i++)
                        meshRenderers[i].enabled = false;
                    for (int i = 0; i < colliders.Length; i++)
                        colliders[i].enabled = false;
                }

                __instance.EjectMagazine();
            }
        }

        /// <summary>
        /// Stops the gun from firing if there are no shells and plays the dry fire sound.
        /// </summary>
        public static bool PreFireGun(Gun gun)
        {
            int uuid = gun.GetInstanceID();
            if (shellLoaders.ContainsKey(uuid))
            {
                ShellLoader loader = shellLoaders[uuid];
                if (loader.shells > 0)
                {
                    return true;
                }
                else
                {
                    if (gun.gunSFX != null && gun.triggerGrip.GetHand() != null && gun.triggerGrip.GetHand().controller.GetPrimaryInteractionButtonDown())
                        gun.gunSFX.DryFire();
                    return false;
                }
            }

            return true;
        }

        private static void OnPreFireGun(Gun gun)
        {
            int uuid = gun.GetInstanceID();
            if (shellLoaders.ContainsKey(uuid))
            {
                ShellLoader loader = shellLoaders[uuid];
                loader.requiresShellEject = true;
                loader.OnGunFired();
            }
        }

        private static void OnPostFireGun(Gun gun)
        {
            int uuid = gun.GetInstanceID();
            if (shellLoaders.ContainsKey(uuid))
            {
                ShellLoader loader = shellLoaders[uuid];
                if (loader.shells == 0 && !loader.hasPumpSlide)
                {
                    gun.PlayAnimationState(Gun.AnimationStates.SLIDELOCKED);
                }
            }
        }

        /// <summary>
        /// Doesn't let the gun load a cartridge unless shells have been put in.
        /// This is needed since technically all shell loading guns have infinite ammo.
        /// </summary>
        private static bool PrePullCartridge(Gun __instance)
        {
            int uuid = __instance.GetInstanceID();
            if (shellLoaders.ContainsKey(uuid))
            {
                ShellLoader loader = shellLoaders[uuid];
                return loader.shells > 0;
            }
            return true;
        }

        public static void EjectCartridgePrefix(Gun gun)
        {
            if (gun.shellSpawnTransform != null && gun.chamberedCartridge != null && gun.chamberedCartridge.ammoVariables.cartridgeType != Cart.NONE)
            {
                int uuid = gun.GetInstanceID();
                if (shellLoaders.ContainsKey(uuid))
                {
                    ShellLoader loader = shellLoaders[uuid];
                    enableEjectedCasesQueue.Enqueue(loader.requiresShellEject);
                    loader.requiresShellEject = false;
                }
            }
        }

        /// <summary>
        /// Hides ejected shells in some circumstances, because the gun will have infinite ammo.
        /// </summary>
        public static void OnCaseEjectSpawned(CaseEject ejectedCase)
        {
            bool active = true;
            if (enableEjectedCasesQueue.Count > 0)
                active = enableEjectedCasesQueue.Dequeue();
            ejectedCase.gameObject.SetActive(active);
        }

        /// <summary>
        /// Hides the chambered bullet in the gun unless the player has put a shell in.
        /// </summary>
        private static void UpdateArtPostfix(Gun __instance)
        {
            int uuid = __instance.GetInstanceID();
            if (shellLoaders.ContainsKey(uuid))
            {
                ShellLoader loader = shellLoaders[uuid];
                loader.SetChamberedBulletActive(loader.shells > 0 || loader.requiresShellEject);
            }
        }
    }
}
