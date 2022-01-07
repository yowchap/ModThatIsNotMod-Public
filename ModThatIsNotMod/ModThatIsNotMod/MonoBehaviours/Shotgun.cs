using StressLevelZero.Combat;
using StressLevelZero.Pool;
using StressLevelZero.Props.Weapons;
using System;
using UnityEngine;

namespace ModThatIsNotMod.MonoBehaviours
{
    public class Shotgun : MonoBehaviour
    {
        public Shotgun(IntPtr intPtr) : base(intPtr) { }

        private int uuid = -1;
        private Gun gun;

        private Quaternion baseFirePointLocalRot;

        public int shots = 8;
        public float maxSpreadAngle = 2f;


        private void Awake()
        {
            CustomMonoBehaviourHandler.SetFieldValues(this);

            gun = GetComponent<Gun>();
            uuid = gun.GetInstanceID();
            baseFirePointLocalRot = gun.firePointTransform.localRotation;
        }

        private void OnEnable() => Hooking.OnPreFireGun += OnGunFire;
        private void OnDisable() => Hooking.OnPreFireGun -= OnGunFire;

        /// <summary>
        /// Instantiate more bullets at random angles
        /// </summary>
        private void OnGunFire(Gun __instance)
        {
            if (__instance.GetInstanceID() == uuid)
            {
                BulletObject bullet = gun.chamberedCartridge;
                gun.firePointTransform.localRotation = GetRandomFirePointAngle();
                for (int i = 0; i < shots - 1; i++)
                    PoolSpawner.SpawnProjectile(gun.firePointTransform.position, GetRandomFiringAngle(), bullet, gun.name, null);
            }
        }

        private Quaternion GetRandomFirePointAngle() => baseFirePointLocalRot * Quaternion.AngleAxis(maxSpreadAngle, UnityEngine.Random.insideUnitSphere);
        private Quaternion GetRandomFiringAngle() => gun.firePointTransform.rotation * Quaternion.AngleAxis(maxSpreadAngle, UnityEngine.Random.insideUnitSphere);
    }
}
