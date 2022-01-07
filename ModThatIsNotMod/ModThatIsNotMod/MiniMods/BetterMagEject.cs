using MelonLoader;
using ModThatIsNotMod.Internals;
using StressLevelZero.Props.Weapons;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ModThatIsNotMod.MiniMods
{
    internal static class BetterMagEject
    {
        private static float timeBetweenEmptyChecks = 0.15f; //How long to wait between checking if a guns mag is empty

        private static float timeToReleaseButton = 0.2f; //Maximum time you can hold the button for it to count as a press
        private static float timeToResetPresses = 0.5f; //Time after pressing a button until the press counter resets

        // The times that the button has to be released by to count as a press
        private static float leftBReleaseTime;
        private static float rightBReleaseTime;

        // The times to reset the counter of how many presses there were
        private static float leftBResetTime;
        private static float rightBResetTime;

        private static int leftPresses;
        private static int rightPresses;

        public static HashSet<int> neverAutoEjectGuns = new HashSet<int>();
        public static HashSet<int> alwaysAutoEjectGuns = new HashSet<int>();
        public static HashSet<int> disableEjectButtonGuns = new HashSet<int>();


        public static void Setup()
        {
            MelonCoroutines.Start(EjectEmptyMags());
        }

        public static void OnUpdate()
        {
            if (Player.handsExist && Player.controllersExist)
            {
                if (Preferences.enableMagEjectButton.value)
                    UpdateInputValues();

                EjectMags();
            }
        }

        /// <summary>
        /// Tries to eject mags if they're empty a few times each second.
        /// </summary>
        private static IEnumerator EjectEmptyMags()
        {
            yield return new WaitForSeconds(timeBetweenEmptyChecks);
            TryEjectMagazine(Player.GetGunInHand(Player.leftHand), false);
            yield return new WaitForSeconds(timeBetweenEmptyChecks);
            TryEjectMagazine(Player.GetGunInHand(Player.rightHand), false);

            MelonCoroutines.Start(EjectEmptyMags());
        }

        /// <summary>
        /// Trys to eject the magazine if the input button was pressed.
        /// </summary>
        private static void EjectMags()
        {
            if (Preferences.enableMagEjectButton.value)
            {
                if (leftPresses == Preferences.pressesToEjectMag.value)
                {
                    TryEjectMagazine(Player.GetGunInHand(Player.leftHand), true);
                    leftPresses = 0;
                }
                if (rightPresses == Preferences.pressesToEjectMag.value)
                {
                    TryEjectMagazine(Player.GetGunInHand(Player.rightHand), true);
                    rightPresses = 0;
                }
            }
        }

        /// <summary>
        /// Checks for which buttons have been pressed.
        /// </summary>
        private static void UpdateInputValues()
        {
            //Reset presses if it's been too long
            if (Time.time >= leftBResetTime)
                leftPresses = 0;
            if (Time.time >= rightBResetTime)
                rightPresses = 0;

            //Check which buttons are down
            if (Player.leftController.GetMenuButtonDown() && !Player.leftController.GetSecondaryMenuButtonDown())
            {
                leftBResetTime = Time.time + timeToResetPresses;
                leftBReleaseTime = Time.time + timeToReleaseButton;
            }
            if (Player.rightController.GetMenuButtonDown() && !Player.rightController.GetSecondaryMenuButtonDown())
            {
                rightBResetTime = Time.time + timeToResetPresses;
                rightBReleaseTime = Time.time + timeToReleaseButton;
            }

            //Check which buttons were released
            if (Player.leftController.GetMenuButtonUp() && !Player.leftController.GetSecondaryMenuButtonUp() && Time.time <= leftBReleaseTime)
                leftPresses++;
            if (Player.rightController.GetMenuButtonUp() && !Player.rightController.GetSecondaryMenuButtonUp() && Time.time <= rightBReleaseTime)
                rightPresses++;
        }

        /// <summary>
        /// Ejects the magazine if the gun is out of ammo or the button was pressed.
        /// </summary>
        private static void TryEjectMagazine(Gun gun, bool hadInput)
        {
            try
            {
                if (gun != null)
                {
                    int uuid = gun.GetInstanceID();

                    if (!Preferences.overrideMagEjectSettings.value)
                    {
                        if ((!hadInput && neverAutoEjectGuns.Contains(uuid)) || (hadInput && disableEjectButtonGuns.Contains(uuid)) || (!hadInput && !Preferences.autoEjectEmptyMags.value && !alwaysAutoEjectGuns.Contains(uuid)))
                            return;
                    }
                    else if (!hadInput && !Preferences.autoEjectEmptyMags.value)
                    {
                        return;
                    }

                    //Don't run if it's a spawn or balloon gun
                    string gunType = gun.GetIl2CppType().ToString();
                    if (gunType == typeof(SpawnGun).FullName || gunType == typeof(BalloonGun).FullName)
                        return;

                    // Try to eject the mag
                    if (gun.magazineSocket != null && gun.magazineSocket.hasMagazine)
                    {
                        if (hadInput || (gun.chamberedCartridge == null && gun.magazineSocket.GetMagazine().GetAmmoCount() == 0))
                            gun.magazineSocket.EjectMagazine();
                    }
                }
            }
            catch { }
        }
    }
}
