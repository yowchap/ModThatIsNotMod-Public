using System;
using UnityEngine;

namespace ModThatIsNotMod.Pooling
{
    public class SimplePoolee : MonoBehaviour
    {
        public SimplePoolee(IntPtr intPtr) : base(intPtr) { }


        public SimplePool pool { get; internal set; }

        public float timeActive { get => gameObject.activeInHierarchy ? Time.time - timeSpawned : 0; }
        private float timeSpawned;


        internal void OnSpawned()
        {
            timeSpawned = Time.time;
        }

        public void Despawn() => pool.ReturnPoolee(this);
    }
}
