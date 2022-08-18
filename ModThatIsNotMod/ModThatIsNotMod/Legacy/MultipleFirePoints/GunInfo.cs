using StressLevelZero.Props.Weapons;
using System.Collections.Generic;
using UnityEngine;
using static ModThatIsNotMod.Legacy.MFP.FirePoints;

namespace ModThatIsNotMod.Legacy.MFP
{
    public class GunInfo
    {
        public Gun gun;
        public int uuid;

        public Queue<FirePoint> firePoints;

        public float angleVariation;

        public int timesToFire;
        public int timesFired;
        public bool shouldFire = false;

        public bool shouldBeManual;
        public bool shouldBeAutomatic;


        public GunInfo(Gun gun, List<Transform> firePoints, int timesToFire = 1, float angleVariation = 0)
        {
            this.gun = gun;
            uuid = gun.gameObject.GetInstanceID();

            this.firePoints = new Queue<FirePoint>();
            for (int i = 1; i < firePoints.Count; i++)
            {
                this.firePoints.Enqueue(new FirePoint(firePoints[i], firePoints[i].localRotation));
            }
            this.firePoints.Enqueue(new FirePoint(firePoints[0], firePoints[0].localRotation));

            this.timesToFire = timesToFire;
            this.angleVariation = angleVariation;

            shouldBeManual = gun.isManual;
            shouldBeAutomatic = gun.isAutomatic;
        }
    }
}
