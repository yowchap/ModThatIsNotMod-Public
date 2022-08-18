using CustomItemsFramework;
using HarmonyLib;
using MelonLoader;
using StressLevelZero.Props.Weapons;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static ModThatIsNotMod.Legacy.MFP.FirePoints;

namespace ModThatIsNotMod.Legacy.MFP
{
    public static class MultipleFirePoints
    {
        private static List<string> gunNames = new List<string>();
        private static List<GunInfo> guns = new List<GunInfo>();
        private static readonly string firePointsParentName = "MultipleFirePoints";
        private static readonly string burstFireParentName = "BurstFire";
        private static readonly string angleVariationParentName = "AngleVariation";


        public static void Setup(HarmonyLib.Harmony harmony)
        {
            if (AppDomain.CurrentDomain.GetAssemblies().Any(i => i.GetName().Name == "MultipleFirePoints"))
            {
                Type oldMfpType = AccessTools.TypeByName("YOWC.MultipleFirePoints.MultipleFirePointsMod");
                harmony.Patch(oldMfpType.GetMethod("OnItemAdded"), prefix: typeof(MultipleFirePoints).GetMethod("Patch").ToNewHarmonyMethod());
                harmony.Patch(oldMfpType.GetMethod("OnGunFired"), prefix: typeof(MultipleFirePoints).GetMethod("Patch").ToNewHarmonyMethod());

            }

            Hooking.OnPostFireGun += OnGunFired;
            CustomItemsMod.OnItemAdded += OnItemAdded;
        }

        public static bool Patch() => false;

        private static void OnItemAdded(GameObject obj)
        {
            Transform firePointsParent = obj.transform.Find(firePointsParentName);
            if (firePointsParent != null)
            {
                Gun gun = obj.GetComponent<Gun>();
                if (gun != null)
                    AddGun(gun, obj.name, firePointsParent);
            }
        }

        private static void OnGunFired(Gun gun)
        {
            string gunName = gun.name;
            if (ListContainsName(gunName))
            {
                GunInfo gunInfo = GetGunInfo(gun);
                if (gunInfo == null)
                {
                    List<Transform> firePoints = FirePoints.GetFirePoints(gun, firePointsParentName);
                    if (firePoints.Count < 2)
                        return;

                    int burstAmount = GetBurstFireAmount(gun.transform, burstFireParentName);
                    float angleVariation = GetAngleVariation(gun.transform, angleVariationParentName);

                    guns.Add(new GunInfo(gun, firePoints, burstAmount, angleVariation));
                    gunInfo = guns[guns.Count - 1];
                }

                //Get the next fire point
                FirePoint point = gunInfo.firePoints.Dequeue();
                gunInfo.firePoints.Enqueue(point);

                gun.firePointTransform = point.transform;

                //Apply random rotational offset
                Quaternion newRotation = point.baseRotation * Quaternion.AngleAxis(gunInfo.angleVariation, UnityEngine.Random.insideUnitSphere);
                gun.firePointTransform.localRotation = newRotation;

                gunInfo.timesFired++;

                if (gun.overrideMagazine == null)
                {
                    if (gun.chamberedCartridge == null && gun.magazineSocket != null)
                        if (!gun.magazineSocket.hasMagazine || (gun.magazineSocket.hasMagazine && gun.magazineSocket.GetMagazine().GetAmmoCount() == 0))
                        {
                            gunInfo.timesFired = gunInfo.timesToFire;
                            // ModConsole.Msg("aaaaaaaaaaaaaaaaaaaaaaa"); // Why is this even here lmao
                        }
                }

                if (gunInfo.timesFired >= gunInfo.timesToFire)
                {
                    gunInfo.shouldFire = false;
                    gun.isAutomatic = gunInfo.shouldBeAutomatic;
                    gunInfo.timesFired = 0;

                    if (gunInfo.shouldBeManual)
                    {
                        gun.isManual = true;
                        gun.cartridgeState = Gun.CartridgeStates.SPENT;
                    }
                }
                else
                {
                    gunInfo.shouldFire = true;
                    gun.isAutomatic = true;

                    gun.PullCartridge();
                    gun.Fire();
                }
            }
        }

        public static void AddGun(string name)
        {
            gunNames.Add(name);
        }

        private static void AddGun(Gun gun, string name, Transform firePointsParent)
        {
            gunNames.Add(name);
            List<Transform> firePoints = FirePoints.GetFirePoints(firePointsParent);

            int burstAmount = GetBurstFireAmount(gun.transform, burstFireParentName);
            float angleVariation = GetAngleVariation(gun.transform, angleVariationParentName);

            guns.Add(new GunInfo(gun, firePoints, burstAmount, angleVariation));

            ModConsole.Msg($"Automatically configured {name} with {firePoints.Count} fire points", LoggingMode.DEBUG);
        }

        private static bool ListContainsName(string name)
        {
            for (int i = 0; i < gunNames.Count; i++)
            {
                if (name.Contains(gunNames[i]))
                    return true;
            }
            return false;
        }

        private static GunInfo GetGunInfo(Gun gun)
        {
            int uuid = gun.gameObject.GetInstanceID();
            for (int i = 0; i < guns.Count; i++)
            {
                if (guns[i].uuid == uuid)
                    return guns[i];
            }
            return null;
        }

        public static int GetBurstFireAmount(Transform gun, string burstFireParentName)
        {
            int timesToFire = 1;
            Transform burstObj = gun.Find(burstFireParentName);
            if (burstObj != null)
                timesToFire = int.Parse(burstObj.GetChild(0).name, System.Globalization.CultureInfo.InvariantCulture);

            return timesToFire;
        }

        public static float GetAngleVariation(Transform gun, string angleVariationParentName)
        {
            float angleVariation = 0;
            Transform angleObj = gun.Find(angleVariationParentName);
            if (angleObj != null)
                angleVariation = float.Parse(angleObj.GetChild(0).name, System.Globalization.CultureInfo.InvariantCulture);

            return angleVariation;

        }
    }
}
