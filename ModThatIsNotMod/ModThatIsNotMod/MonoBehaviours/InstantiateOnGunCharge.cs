using System;
using UnityEngine;

namespace ModThatIsNotMod.MonoBehaviours
{
    public class InstantiateOnGunCharge : MonoBehaviour
    {
        public InstantiateOnGunCharge(IntPtr intPtr) : base(intPtr) { }


        public bool objectsFollowGun = true;


        private void Awake()
        {
            CustomMonoBehaviourHandler.SetFieldValues(this);

            SimpleGunModifiers modifiers = GetComponentInParent<SimpleGunModifiers>();
            if (modifiers != null)
                modifiers.instantiateOnGunCharge = this;
        }

        /// <summary>
        /// Instantiates all children of self.
        /// </summary>
        internal GameObject OnGunCharge()
        {
            Transform newObjsParent = new GameObject().transform;
            newObjsParent.parent = transform.parent;
            newObjsParent.position = transform.position;

            for (int i = 0; i < transform.childCount; i++)
            {
                Transform t = transform.GetChild(i);
                GameObject newObj = GameObject.Instantiate(t.gameObject);
                newObj.transform.parent = newObjsParent;
                newObj.transform.position = t.position;
                newObj.transform.rotation = t.rotation;
                newObj.SetActive(true);
            }

            newObjsParent.parent = objectsFollowGun ? transform.parent : null;
            return newObjsParent.gameObject;
        }
    }
}
