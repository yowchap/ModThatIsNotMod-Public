using System;
using System.Collections.Generic;
using StressLevelZero.Props.Weapons;
using UnityEngine;

namespace ModThatIsNotMod.Legacy.MFP
{
	public static class FirePoints
	{
		public static List<Transform> GetFirePoints(Gun gun, string firePointsParentName)
		{
			List<Transform> firePoints = new List<Transform>();

			string firePointName = "FirePoint ";
			Transform nextPoint = gun.transform.Find(firePointName + "(1)");
			int index = 2;
			while (nextPoint != null)
			{
				firePoints.Add(nextPoint);
				nextPoint = gun.transform.Find(firePointName + $"({index})");
				index++;
			}

			if (firePoints.Count == 0)
			{
				Transform firePointsParent = gun.transform.Find(firePointsParentName);
				if (firePointsParent != null)
					firePoints = GetFirePoints(firePointsParent);
			}

			return firePoints;
		}

		public static List<Transform> GetFirePoints(Transform firePointsParent)
		{
			List<Transform> firePoints = new List<Transform>();
			for (int i = 0; i < firePointsParent.childCount; i++)
			{
				firePoints.Add(firePointsParent.GetChild(i));
			}
			return firePoints;
		}

		public struct FirePoint
		{
			public Transform transform;
			public Quaternion baseRotation;

			public FirePoint(Transform firePoint, Quaternion baseRotation)
			{
				this.baseRotation = baseRotation;
				this.transform = firePoint;
			}
		}
	}
}
