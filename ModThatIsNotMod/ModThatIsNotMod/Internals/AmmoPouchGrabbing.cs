using HarmonyLib;
using MelonLoader;
using ModThatIsNotMod.MonoBehaviours;
using StressLevelZero.Interaction;
using StressLevelZero.Player;
using StressLevelZero.Props.Weapons;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ModThatIsNotMod.Internals
{
    internal static class AmmoPouchGrabbing
    {
        private static List<Magazine> spawnedMags = new List<Magazine>();
        private static PlayerInventory playerInventory;


        public static void Setup()
        {
            Hooking.CreateHook(typeof(AmmoPouch).GetMethod("OnSpawnGrab", AccessTools.all), typeof(AmmoPouchGrabbing).GetMethod("OnSpawnGrabPostfix", AccessTools.all));
            MelonCoroutines.Start(CleanupMags());
        }

        /// <summary>
        /// Checks if the grabbed mag is the right one for the currently held gun.
        /// If it's correct, leaves it. Otherwise it instantiates the right mag and grabs it.
        /// </summary>
        private static void OnSpawnGrabPostfix(Hand hand)
        {
            try
            {
                if (playerInventory == null)
                    playerInventory = Player.GetRigManager().GetComponent<PlayerInventory>();

                Magazine grabbedMag = Player.GetComponentInHand<Magazine>(hand);
                if (grabbedMag == null)
                    return;

                Hand gunHand = hand.otherHand;
                Gun heldGun = Player.GetGunInHand(gunHand);
                if (heldGun == null)
                    return;

                if (heldGun.magazineSocket != null && heldGun.magazineSocket.magazineData != null && heldGun.magazineSocket.magazineData.spawnableObject != null)
                {
                    if (heldGun.magazineSocket.magazineData.spawnableObject.title != grabbedMag.magazineData.spawnableObject.title) // If the player grabbed the wrong mag
                    {
                        // Yoink a new mag from the pool except SIKE it's not actually from the pool because nullables are CRINGE AND NOBODY LIKES THEM and for some reason CustomItems.SpawnFromPool() doesn't work either :(
                        GameObject newMag = GameObject.Instantiate(heldGun.magazineSocket.magazineData.spawnableObject.prefab);
                        CustomItems.FixObjectShaders(newMag); // This should be handled when the bundle is loaded but apparently not
                        if (!newMag.GetComponent<CustomItem>())
                            newMag.AddComponent<CustomItem>();
                        newMag.SetActive(true);

                        // Reset the mag's ammo but don't let the player gain any more
                        Magazine magComp = newMag.GetComponent<Magazine>();
                        magComp.ResetMagazine();
                        playerInventory.RegisterMagazine(magComp);
                        magComp.IsAmmoClaimed = true;

                        spawnedMags.Add(magComp);

                        Grip magGrip = newMag.GetComponent<Grip>();
                        if (magGrip == null) // Realisticly this should never be null since you need a grip on a mag, but whatever ¯\_(ツ)_/¯
                            return;

                        newMag.transform.rotation = magGrip.GetRotatedGripTargetTransformWorld(hand).rotation; // Wacky quaternion magic or something idk

                        // Position the mag in the palm of the hand
                        Vector3 offset = magGrip.targetTransform != null ? magGrip.targetTransform.localPosition : Vector3.zero;
                        newMag.transform.position = hand.palmPositionTransform.position - offset;

                        //Drop the mag that was taken from the pouch and destroy it
                        grabbedMag.GetComponent<InteractableHost>().Drop();
                        hand.DetachObject(grabbedMag.gameObject);
                        hand.DetachJoint(true);
                        GameObject.Destroy(grabbedMag.gameObject);

                        magGrip.Snatch(hand, true); // Grab the new mag
                    }
                }
            }
            catch { }
        }

        /// <summary>
        /// Every 10 seconds, removes all custom mags that are more than 1m from the players head and not held or in a gun.
        /// </summary>
        private static IEnumerator CleanupMags()
        {
            yield return new WaitForSeconds(10);

            try
            {
                for (int i = spawnedMags.Count - 1; i >= 0; i--)
                {
                    if (spawnedMags[i] == null || (!spawnedMags[i].isMagazineInserted && spawnedMags[i].grip.GetHand() == null && (Player.GetPlayerHead().transform.position - spawnedMags[i].transform.position).sqrMagnitude >= 1))
                    {
                        if (spawnedMags[i])
                            GameObject.Destroy(spawnedMags[i].gameObject);
                        spawnedMags.RemoveAt(i);
                    }
                }
            }
            catch { }

            MelonCoroutines.Start(CleanupMags());
        }
    }
}
