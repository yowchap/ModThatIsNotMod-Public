using StressLevelZero.Interaction;
using System.Collections.Generic;
using UnityEngine;

namespace ModThatIsNotMod.BoneMenu
{
    // This is from L4rs so idk what most of it's doing
    internal static class HeldItemMenuManager
    {
        private static Dictionary<int, HeldItemInfo> assignedHeldItems = new Dictionary<int, HeldItemInfo>(); // int is for instance IDs


        public static void AddItemMenu(Interactable interactable, Transform transform, MenuCategory category, bool isToggle)
        {
            assignedHeldItems.Add(interactable.GetInstanceID(), new HeldItemInfo(transform, category, isToggle));
        }

        public static bool HasItemInfo(int uuid) => assignedHeldItems.ContainsKey(uuid);
        public static HeldItemInfo GetItemInfo(int uuid) => assignedHeldItems[uuid];


        public static void InteractableHookingSetup()
        {
            Hooking.OnGripAttached += OnGripAttached;
            Hooking.OnGripDetached += OnGripDetached;
        }

        private static void OnGripAttached(Grip grip, Hand hand)
        {
            if (grip.interactable != null && HeldItemMenuManager.HasItemInfo(grip.interactable.GetInstanceID()))
            {
                HeldItemInfo heldItemInfo = HeldItemMenuManager.GetItemInfo(grip.interactable.GetInstanceID());
                if (heldItemInfo.isToggle)
                {
                    MenuManager.SetHeldItemMenuEnabled(heldItemInfo.toggleState, grip, hand);
                    heldItemInfo.toggleState = !heldItemInfo.toggleState;
                }
                else
                {
                    MenuManager.SetHeldItemMenuEnabled(true, grip, hand);
                }
            }
        }

        private static void OnGripDetached(Grip grip, Hand hand)
        {
            if (grip.interactable != null && HeldItemMenuManager.HasItemInfo(grip.interactable.GetInstanceID()))
            {
                if (!HeldItemMenuManager.GetItemInfo(grip.interactable.GetInstanceID()).isToggle)
                    MenuManager.SetHeldItemMenuEnabled(false, grip, hand);
            }
        }
    }
}
