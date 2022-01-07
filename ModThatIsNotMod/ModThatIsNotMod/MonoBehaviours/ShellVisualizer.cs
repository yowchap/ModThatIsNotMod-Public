using System;
using UnityEngine;

namespace ModThatIsNotMod.MonoBehaviours
{
    public class ShellVisualizer : MonoBehaviour
    {
        public ShellVisualizer(IntPtr intPtr) : base(intPtr) { }


        private GameObject[] shells;


        private void Awake()
        {
            shells = new GameObject[transform.childCount];
            for (int i = 0; i < transform.childCount; i++)
                shells[i] = transform.GetChild(i).gameObject;

            gameObject.GetComponentInParent<ShellLoader>().visualizer = this;
        }

        public void OnShellInserted(int shell)
        {
            if (shell <= shells.Length)
                shells[shell - 1].SetActive(true);
        }

        public void OnGunFired(int shell)
        {
            if (shell < shells.Length)
                shells[shell].SetActive(false);
        }
    }
}
