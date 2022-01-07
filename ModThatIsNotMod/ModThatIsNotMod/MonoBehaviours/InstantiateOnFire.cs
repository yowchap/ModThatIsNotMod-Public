using StressLevelZero.Props.Weapons;
using System;
using UnityEngine;

namespace ModThatIsNotMod.MonoBehaviours
{
    public class InstantiateOnFire : MonoBehaviour
    {
        public InstantiateOnFire(IntPtr intPtr) : base(intPtr) { }

        private int uuid = -1;

        private void Awake()
        {
            Gun gun = GetComponentInParent<Gun>();
            if (gun != null)
                uuid = gun.GetInstanceID();
        }

        private void OnEnable() => Hooking.OnPostFireGun += OnGunFire;
        private void OnDisable() => Hooking.OnPostFireGun -= OnGunFire;

        /// <summary>
        /// Instantiates all children of self.
        /// </summary>
        private void OnGunFire(Gun __instance)
        {
            if (__instance.GetInstanceID() == uuid)
            {
                for (int i = 0; i < transform.childCount; i++)
                {
                    Transform t = transform.GetChild(i);
                    GameObject newObj = GameObject.Instantiate(t.gameObject);
                    newObj.transform.parent = null; // Set the parent to null so it doesn't follow the gun around
                    newObj.transform.position = t.position;
                    newObj.transform.rotation = t.rotation;
                    newObj.SetActive(true);
                }
            }
        }
    }
}
