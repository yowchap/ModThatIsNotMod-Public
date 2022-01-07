using ModThatIsNotMod.MiniMods;
using StressLevelZero.Props.Weapons;
using System;
using UnityEngine;

namespace ModThatIsNotMod.MonoBehaviours
{
    public class MagEjectSettings : MonoBehaviour
    {
        public MagEjectSettings(IntPtr intPtr) : base(intPtr) { }


        public AutoEjectMode autoEjectMode = AutoEjectMode.Default;
        public bool disableMagEjectButton = false;


        private void Awake()
        {
            CustomMonoBehaviourHandler.SetFieldValues(this);

            int uuid = gameObject.GetComponent<Gun>().GetInstanceID();

            if (autoEjectMode == AutoEjectMode.Always)
                BetterMagEject.alwaysAutoEjectGuns.Add(uuid);
            else if (autoEjectMode == AutoEjectMode.Never)
                BetterMagEject.neverAutoEjectGuns.Add(uuid);

            if (disableMagEjectButton)
                BetterMagEject.disableEjectButtonGuns.Add(uuid);
        }

        public enum AutoEjectMode
        {
            Default,
            Never,
            Always
        }
    }
}
