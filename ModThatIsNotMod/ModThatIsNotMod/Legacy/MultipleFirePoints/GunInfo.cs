using System;
using System.Collections.Generic;
using StressLevelZero.Props.Weapons;
using UnityEngine;

namespace ModThatIsNotMod.Legacy
{
	public class GunInfo
	{
		public Gun gun;
		public int uuid;
		public Queue<FirePoints.FirePoint> firePoints;
		public float angleVariation;
		public int timesToFire;
		public int timesFired;
		public bool shouldFire = false;
		public bool shouldBeManual;
		public bool shouldBeAutomatic;
		public GunInfo(Gun gun, List<Transform> firePoints, int timesToFire = 1, float angleVariation = 0f)
		{
			this.gun = gun;
			uuid = gun.gameObject.GetInstanceID();
			this.firePoints = new Queue<FirePoints.FirePoint>();
			for (int i = 1; i < firePoints.Count; i++)
			{
				this.firePoints.Enqueue(new FirePoints.FirePoint(firePoints[i], firePoints[i].localRotation));
			}
			this.firePoints.Enqueue(new FirePoints.FirePoint(firePoints[0], firePoints[0].localRotation));
			this.timesToFire = timesToFire;
			this.angleVariation = angleVariation;
			shouldBeManual = gun.isManual;
			shouldBeAutomatic = gun.isAutomatic;
		}
	}
}
