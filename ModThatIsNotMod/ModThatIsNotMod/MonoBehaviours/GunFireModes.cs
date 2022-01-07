using MelonLoader;
using StressLevelZero.Props.Weapons;
using System;
using System.Collections;
using UnityEngine;

namespace ModThatIsNotMod.MonoBehaviours
{
    public class GunFireModes : MonoBehaviour
    {
        public GunFireModes(IntPtr intPtr) : base(intPtr) { }

        private Gun gun;
        private int uuid = -1;

        public FireMode[] allowedFireModes;

        public int burstShots = 3;
        public float burstTimeout = 0.2f;

        internal FireMode curFireMode { get; private set; }

        private bool shouldFire = false;
        private int burstShotsFired = 0;
        private float timeToReAllowFiring = 0;


        private void Awake()
        {
            CustomMonoBehaviourHandler.SetFieldValues(this);

            gun = GetComponent<Gun>();
            uuid = gun.GetInstanceID();

            curFireMode = allowedFireModes[0];

            MelonCoroutines.Start(CheckToStopBursts());
        }

        private void Update()
        {
            if (curFireMode == FireMode.Burst)
            {
                if (shouldFire)
                    gun.Fire();

                // Don't let the gun fire if the timeout isn't done
                // Pretty sure this won't work if the gun uses a pump slide grip but I doubt anyone will run into that issue
                gun._isSlideGrabbed = Time.time < timeToReAllowFiring;
            }
        }

        private void OnEnable() => Hooking.OnPostFireGun += OnGunFire;
        private void OnDisable() => Hooking.OnPostFireGun -= OnGunFire;
        private void OnGunFire(Gun __instance)
        {
            if (__instance.GetInstanceID() == uuid && curFireMode == FireMode.Burst)
            {
                shouldFire = ++burstShotsFired < burstShots;
                gun.isAutomatic = shouldFire;
                if (!shouldFire)
                {
                    burstShotsFired = 0;
                    timeToReAllowFiring = Time.time + burstTimeout;
                }
            }
        }

        /// <summary>
        /// Go to the next fire mode and play the dry fire sound so the player knows it worked
        /// </summary>
        public void CycleFireMode()
        {
            int index = Array.IndexOf(allowedFireModes, curFireMode);
            curFireMode = ++index < allowedFireModes.Length ? allowedFireModes[index] : allowedFireModes[0];

            gun.isAutomatic = curFireMode == FireMode.Burst || curFireMode == FireMode.FullAuto;

            shouldFire = false;
            burstShotsFired = 0;

            gun._isSlideGrabbed = false;

            if (gun.gunSFX != null)
                gun.gunSFX.DryFire();
        }

        private IEnumerator CheckToStopBursts()
        {
            yield return new WaitForSeconds(0.15f);

            if (curFireMode == FireMode.Burst)
            {
                // If there's no more ammo, stop burst firing
                if (gun.overrideMagazine == null)
                    if (gun.chamberedCartridge == null && gun.magazineSocket != null)
                        if (!gun.magazineSocket.hasMagazine || (gun.magazineSocket.hasMagazine && gun.magazineSocket.GetMagazine().GetAmmoCount() == 0))
                            shouldFire = false;

                if (!shouldFire)
                    burstShotsFired = 0;
            }

            MelonCoroutines.Start(CheckToStopBursts());
        }


        public enum FireMode
        {
            FullAuto,
            SemiAuto,
            Burst
        }
    }
}
