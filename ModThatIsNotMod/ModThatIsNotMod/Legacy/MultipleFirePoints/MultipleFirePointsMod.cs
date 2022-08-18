using System;
using System.Collections.Generic;
using System.Diagnostics;
using BoneworksModdingToolkit.BoneHook;
using CustomItemsFramework;
using MelonLoader;
using StressLevelZero.Props.Weapons;
using UnityEngine;
using System.Reflection;
using HarmonyLib;
using System.Linq;

namespace ModThatIsNotMod.Legacy
{
	public static class MultipleFirePointsMod
	{
		public static event Action<Gun> OnGunFire;
		private static List<string> gunNames = new List<string>();
		private static List<GunInfo> guns = new List<GunInfo>();
		private static readonly string firePointsParentName = "MultipleFirePoints";
		private static readonly string burstFireParentName = "BurstFire";
		private static readonly string angleVariationParentName = "AngleVariation";

		public static void Setup(HarmonyLib.Harmony harmony)
		{
			if (AppDomain.CurrentDomain.GetAssemblies().Any(i => i.GetName().Name == "MultipleFirePoints"))
            {
				Type t = AccessTools.TypeByName("YOWC.MultipleFirePoints.MultipleFirePointsMod");
				harmony.Patch(t.GetMethod("OnItemAdded"), typeof(MultipleFirePointsMod).GetMethod("Patch").ToNewHarmonyMethod());
				harmony.Patch(t.GetMethod("OnGunFired"), typeof(MultipleFirePointsMod).GetMethod("Patch").ToNewHarmonyMethod());

			}
			Hooking.OnPostFireGun += OnGunFired;
			CustomItemsMod.OnItemAdded += OnItemAdded;	
		}
		public static bool Patch() => false;

		private static void OnItemAdded(GameObject obj)
		{
			Transform transform = obj.transform.Find(firePointsParentName);
			if (transform != null)
			{
				Gun gun = obj.GetComponent<Gun>();
				if (gun != null)
					AddGun(gun, obj.name, transform);
			}
		}

		private static void OnGunFired(Gun gun)
		{
			string name = gun.name;
			if (ListContainsName(name))
			{
				GunInfo gunInfo = GetGunInfo(gun);
				if (gunInfo == null)
				{
					List<Transform> firePoints = FirePoints.GetFirePoints(gun, firePointsParentName);
					if (firePoints.Count < 2)
						return;

					int burstFireAmount = GetBurstFireAmount(gun.transform, burstFireParentName);
					float angleVariation = GetAngleVariation(gun.transform, angleVariationParentName);
					guns.Add(new GunInfo(gun, firePoints, burstFireAmount, angleVariation));
					gunInfo = guns[guns.Count - 1];
				}
				FirePoints.FirePoint firePoint = gunInfo.firePoints.Dequeue();
				gunInfo.firePoints.Enqueue(firePoint);
				gun.firePointTransform = firePoint.transform;
				Quaternion localRotation = firePoint.baseRotation * Quaternion.AngleAxis(gunInfo.angleVariation, UnityEngine.Random.insideUnitSphere);
				gun.firePointTransform.localRotation = localRotation;
				gunInfo.timesFired++;
				if (gunInfo.timesFired == 1)
					OnGunFire?.Invoke(gun);
				if (gunInfo.timesFired >= gunInfo.timesToFire)
				{
					gunInfo.shouldFire = false;
					gun.isAutomatic = gunInfo.shouldBeAutomatic;
					gunInfo.timesFired = 0;
					bool shouldBeManual = gunInfo.shouldBeManual;
					if (shouldBeManual)
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
			int burstFireAmount = GetBurstFireAmount(gun.transform, burstFireParentName);
			float angleVariation = GetAngleVariation(gun.transform, angleVariationParentName);
			guns.Add(new GunInfo(gun, firePoints, burstFireAmount, angleVariation));
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
			int instanceID = gun.gameObject.GetInstanceID();
			for (int i = 0; i < guns.Count; i++)
			{
				if (guns[i].uuid == instanceID)
					return guns[i];
			}
			return null;
		}

		public static int GetBurstFireAmount(Transform gun, string burstFireParentName)
		{
			Transform transform = gun.Find(burstFireParentName);
			return transform != null ? Convert.ToInt32(transform.GetChild(0).name) : 1;
		}

		public static float GetAngleVariation(Transform gun, string angleVariationParentName)
		{
			Transform transform = gun.Find(angleVariationParentName);
			return transform != null ? (float)Convert.ToDouble(transform.GetChild(0).name) : 0f;

		}
	}
}
