using UnityEngine;

namespace ModThatIsNotMod.BoneMenu
{
    // L4rs did this stuff so I have no clue what it's used for :)
    internal class HeldItemInfo
    {
        public HeldItemInfo(Transform transform, MenuCategory category, bool isToggle = true)
        {
            this.transform = transform;
            this.category = category;
            this.isToggle = isToggle;
            toggleState = true;
        }

        public Transform transform { get; private set; }
        public MenuCategory category { get; private set; }
        public bool isToggle { get; private set; }
        public bool toggleState { get; set; }
    }
}
