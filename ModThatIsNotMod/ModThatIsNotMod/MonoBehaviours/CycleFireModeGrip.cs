using StressLevelZero.Interaction;
using System;
using UnityEngine;

namespace ModThatIsNotMod.MonoBehaviours
{
    public class CycleFireModeGrip : MonoBehaviour
    {
        public CycleFireModeGrip(IntPtr intPtr) : base(intPtr) { }

        private int uuid = -1;
        private GunFireModes fireModes;


        private void Awake()
        {
            try { uuid = gameObject.GetComponent<Grip>().GetInstanceID(); } catch { }
            fireModes = gameObject.GetComponentInParent<GunFireModes>();

            Hooking.OnGripAttached += OnGripAttached;
        }

        private void OnGripAttached(Grip grip, Hand hand)
        {
            if (grip.GetInstanceID() == uuid)
                fireModes.CycleFireMode();
        }
    }
}
