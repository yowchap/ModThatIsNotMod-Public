using System;
using UnityEngine;

namespace ModThatIsNotMod.BoneMenu
{
    internal class MenuElementInputReceiver : MonoBehaviour
    {
        public MenuElementInputReceiver(IntPtr intPtr) : base(intPtr) { }


        private static readonly float minHeightForValidPress = -1.5f; // TODO: Fix this, it doesn't work

        private static float minPressLength = 0.05f;
        private float minTimeToReleasePress = float.MaxValue;

        private MenuElementInputManager inputManager;


        private void Start()
        {
            inputManager = gameObject.GetComponentInParent<MenuElementInputManager>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (Time.unscaledTime >= MenuManager.timeUntilButtonsCanBePressed)
            {
                // TODO: Make sure the button hasn't been masked out
                if (other.gameObject.name.Contains(MenuManager.fingerPointerName) && transform.localPosition.y >= minHeightForValidPress) // TODO: add max height check
                {
                    Vector3 localFingertipPos = transform.InverseTransformPoint(other.transform.position);
                    if (localFingertipPos.z <= transform.localPosition.z) // Hopefully prevent presses from the back
                        minTimeToReleasePress = Time.unscaledTime + minPressLength;
                }
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (other.gameObject.name.Contains(MenuManager.fingerPointerName) && Time.unscaledTime >= minTimeToReleasePress)
            {
                inputManager.OnInputPressed(gameObject.name);
                minTimeToReleasePress = float.MaxValue; // So it won't keep detecting a press
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.name.Contains(MenuManager.fingerPointerName))
                minTimeToReleasePress = float.MaxValue;
        }
    }
}
