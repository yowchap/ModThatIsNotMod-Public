using System;
using System.Collections.Generic;
using StressLevelZero.Props.Weapons;
using UnityEngine;

namespace ModThatIsNotMod.Legacy
{
	public static class FirePoints
	{
		public static List<Transform> GetFirePoints(Gun gun, string firePointsParentName)
		{
			List<Transform> list = new List<Transform>();
			string str = "FirePoint ";
			Transform transform = gun.transform.Find(str + "(1)");
			int num = 2;
			while (transform != null)
			{
				list.Add(transform);
				transform = gun.transform.Find(str + string.Format("({0})", num));
				num++;
			}
			if (list.Count == 0)
			{
				Transform transform2 = gun.transform.Find(firePointsParentName);
				if (transform2 != null)
					list = GetFirePoints(transform2);
			}
			return list;
		}

		public static List<Transform> GetFirePoints(Transform firePointsParent)
		{
			List<Transform> list = new List<Transform>();
			for (int i = 0; i < firePointsParent.childCount; i++)
			{
				list.Add(firePointsParent.GetChild(i));
			}
			return list;
		}

		public struct FirePoint
		{
			public FirePoint(Transform firePoint, Quaternion baseRotation)
			{
				this.baseRotation = baseRotation;
				this.transform = firePoint;
			}

			public Transform transform;

			public Quaternion baseRotation;
		}
	}
}
