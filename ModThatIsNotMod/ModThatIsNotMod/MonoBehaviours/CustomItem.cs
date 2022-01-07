using System;
using UnityEngine;

namespace ModThatIsNotMod.MonoBehaviours
{
    internal class CustomItem : MonoBehaviour
    {
        public CustomItem(IntPtr intPtr) : base(intPtr) { }

        private void Start() => CustomItems.customItemInstances.Add(gameObject);
    }
}
