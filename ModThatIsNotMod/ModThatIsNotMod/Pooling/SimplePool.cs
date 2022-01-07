using System;
using System.Collections.Generic;
using UnityEngine;

namespace ModThatIsNotMod.Pooling
{
    public class SimplePool : MonoBehaviour
    {
        public SimplePool(IntPtr intPtr) : base(intPtr) { }


        private Queue<SimplePoolee> despawnedPoolees = new Queue<SimplePoolee>();
        private Queue<SimplePoolee> spawnedPoolees = new Queue<SimplePoolee>();

        private SimplePoolee poolee;
        private int pooledAmount;
        private PoolMode poolMode;


        internal void InitPool(GameObject prefab, int pooledAmount, PoolMode poolMode)
        {
            poolee = prefab.GetComponent<SimplePoolee>() ?? prefab.AddComponent<SimplePoolee>();
            this.pooledAmount = pooledAmount;
            this.poolMode = poolMode;

            for (int i = 0; i < pooledAmount; i++)
                despawnedPoolees.Enqueue(NewPoolee());
        }

        private SimplePoolee NewPoolee()
        {
            SimplePoolee newPoolee = Instantiate(poolee, transform.position, Quaternion.identity, transform);
            newPoolee.gameObject.SetActive(false);
            newPoolee.pool = this;

            return newPoolee;
        }

        public SimplePoolee SpawnPoolee()
        {
            SimplePoolee p = null;

            if (despawnedPoolees.Count > 0)
            {
                p = despawnedPoolees.Dequeue();
            }
            else
            {
                switch (poolMode)
                {
                    case PoolMode.Grow:
                        p = NewPoolee();
                        break;
                    case PoolMode.ReuseOldest:
                        if (spawnedPoolees.Count > 0)
                        {
                            spawnedPoolees.Dequeue().Despawn();
                            p = despawnedPoolees.Dequeue();
                        }
                        break;
                }
            }

            p.transform.parent = null;
            spawnedPoolees.Enqueue(p);
            p.OnSpawned();
            return p;
        }

        internal void ReturnPoolee(SimplePoolee p)
        {
            // Reset poolee transform
            p.transform.parent = transform;
            p.transform.position = transform.position;
            p.transform.rotation = Quaternion.identity;

            // Zero out velocity
            Rigidbody rb = p.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }

            // Disable and allow it to be respawned
            p.gameObject.SetActive(false);
            despawnedPoolees.Enqueue(p);
        }

        public enum PoolMode
        {
            ReuseOldest,
            Grow
        }
    }
}
