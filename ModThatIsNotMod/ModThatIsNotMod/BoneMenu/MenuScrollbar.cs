using System;
using UnityEngine;
using UnityEngine.UI;

namespace ModThatIsNotMod.BoneMenu
{
    internal class MenuScrollbar : MonoBehaviour
    {
        public MenuScrollbar(IntPtr intPtr) : base(intPtr) { }


        internal Scrollbar scrollbar { get; private set; }
        private RectTransform handle;

        private float maxDistanceFromCenter;

        internal float targetValue = 1;
        private float smoothing = 0.05f;


        private void Start()
        {
            maxDistanceFromCenter = gameObject.GetComponent<RectTransform>().sizeDelta.y / 2f;
            scrollbar = gameObject.GetComponent<Scrollbar>();
            handle = transform.Find("Sliding Area/Handle").GetComponent<RectTransform>();
        }

        private void Update()
        {
            scrollbar.value = Mathf.Lerp(scrollbar.value, targetValue, smoothing);
        }

        private void OnTriggerStay(Collider other)
        {
            if (other.gameObject.name.Contains(MenuManager.fingerPointerName))
            {
                // I would comment exactly what all of these are doing but I honestly don't remember and it's only been 30 seconds since I finsihed writing it
                // Hopefully it doesn't randomly stop working
                float distFromCenter = (transform.position.y - other.transform.position.y) / transform.lossyScale.y;
                float distBetweenHandleAnchors = handle.anchorMax.y - handle.anchorMin.y;
                float fixedMaxDistFromCenter = maxDistanceFromCenter - (distBetweenHandleAnchors * maxDistanceFromCenter);
                float normalizedDistance = distFromCenter / fixedMaxDistFromCenter;
                targetValue = 0.5f - (normalizedDistance * 0.5f);

                MenuManager.SendHapticFeedback(0.1f);
            }
        }
    }
}
