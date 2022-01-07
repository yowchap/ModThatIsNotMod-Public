using ModThatIsNotMod.Internals;
using StressLevelZero.Interaction;
using StressLevelZero.Props.Weapons;
using System;
using UnityEngine;

namespace ModThatIsNotMod.MonoBehaviours
{
    public class ShellLoader : MonoBehaviour
    {
        public ShellLoader(IntPtr intPtr) : base(intPtr) { }


        public int ammoPerShell = 1;
        public int maxShells = 6;

        internal int shells { get; private set; } = 0;
        internal bool requiresShellEject = false;
        internal bool hasPumpSlide = false;

        internal ShellVisualizer visualizer;
        private Gun gun;


        private void Awake()
        {
            CustomMonoBehaviourHandler.SetFieldValues(this);
            ShellLoadingController.RegisterGun(this);

            gun = gameObject.GetComponent<Gun>();
            hasPumpSlide = gameObject.GetComponentInChildren<PumpSlideGrip>() != null;
        }

        public bool InsertShell()
        {
            int newValue = shells + ammoPerShell;
            shells = Mathf.Clamp(newValue, 0, maxShells * ammoPerShell);
            bool successfulInsert = newValue <= maxShells * ammoPerShell;
            if (successfulInsert && visualizer != null)
                visualizer.OnShellInserted(shells);

            return successfulInsert;
        }

        public void OnGunFired()
        {
            if (shells > 0)
                shells--;

            if (visualizer != null)
                visualizer.OnGunFired(shells);
        }

        public void SetChamberedBulletActive(bool active)
        {
            gun.chamberedBulletGameObject?.SetActive(active);
            gun.chamberedCartridgeGameObject?.SetActive(active);
        }
    }
}
