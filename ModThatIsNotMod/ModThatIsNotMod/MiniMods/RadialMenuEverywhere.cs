using ModThatIsNotMod.Internals;
using StressLevelZero;
using StressLevelZero.Data;
using StressLevelZero.Pool;
using StressLevelZero.UI;
using StressLevelZero.UI.Radial;
using StressLevelZero.VRMK;
using System;
using UnityEngine;

namespace ModThatIsNotMod.MiniMods
{
    internal static class RadialMenuEverywhere
    {
        private static SpawnableObject poofBallSpawnable;
        private static SpawnableObject utilGunSpawnable;
        private static SpawnableObject nimbusSpawnable;

        private static PopUpMenuView popUpMenu;

        private static bool foundAmmoSpawnables = false;


        /// <summary>
        /// Adds ammo spawnables to the spawn menu and enables the util gun radial button.
        /// </summary>
        public static void OnSceneWasInitialized(int level)
        {
            BodyVitals bodyVitals = GameObject.FindObjectOfType<BodyVitals>();
            bodyVitals.quickmenuEnabled = true;
            bodyVitals.slowTimeEnabled = true;

            popUpMenu = GameObject.FindObjectOfType<PopUpMenuView>();
            if (popUpMenu != null)
                utilGunSpawnable = popUpMenu.utilityGunSpawnable;

            if (!foundAmmoSpawnables)
            {
                SpawnableObject[] spawnables = Resources.FindObjectsOfTypeAll<SpawnableObject>();
                for (int i = 0; i < spawnables.Length; i++)
                {
                    if (spawnables[i].title.Contains("Ammo Box"))
                    {
                        GameObject prefab = GameObject.Instantiate(spawnables[i].prefab);
                        prefab.GetComponentInChildren<AmmoPickup>().ammoCount = 2500;
                        GameObject.Destroy(prefab.GetComponent<Poolee>());
                        prefab.SetActive(true);
                        SpawnableObject newSpawnable = CustomItems.CreateSpawnableObject(prefab, spawnables[i].title + " 2500", CategoryFilters.OTHER, PoolMode.GROW, 10);
                        SpawnMenu.AddItem(newSpawnable);

                        foundAmmoSpawnables = true;
                    }
                    else if (spawnables[i].title == "Nimbus Gun")
                    {
                        nimbusSpawnable = spawnables[i];
                    }
                    else if (spawnables[i].title == "Confetti Poof Ball")
                    {
                        poofBallSpawnable = spawnables[i];
                    }
                }
            }

            if (Preferences.utilGunInRadialMenu)
            {
                if (level == 16 || level == 18 || level == 19 || level == 22 || level == 23)
                {
                    Transform pageViewTransform = Player.GetRigManager().transform.Find("[UIRig]/PLAYERUI/panel_Default");
                    if (pageViewTransform != null)
                    {
                        Transform utilGunButton = pageViewTransform.Find("button_Region_SE");
                        if (utilGunButton != null)
                        {
                            PageView pageView = pageViewTransform.GetComponent<PageView>();
                            for (int i = 0; i < pageView.m_HomePage.items.Count; i++)
                            {
                                if (pageView.m_HomePage.items[i].direction == PageItem.Directions.SOUTHEAST)
                                    return;

                                PageItemView pageItemView = utilGunButton.GetComponent<PageItemView>();
                                pageItemView.m_Data = new PageItem("Utility Gun", PageItem.Directions.SOUTHEAST, new Action(() => SpawnUtilGun(0.5f)));
                                pageView.m_HomePage.items.Add(pageItemView.m_Data);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Instantiates the utility gun in front of the player.
        /// </summary>
        public static void SpawnUtilGun(float distance)
        {
            if (utilGunSpawnable != null)
            {
                if (poofBallSpawnable != null)
                    GameObject.Instantiate(poofBallSpawnable.prefab, Player.GetPlayerHead().transform.position + Player.GetPlayerHead().transform.forward * distance, Player.GetPlayerHead().transform.rotation, null);

                GameObject newUtilGun = GameObject.Instantiate(utilGunSpawnable.prefab, Player.GetPlayerHead().transform.position + Player.GetPlayerHead().transform.forward * distance, Player.GetPlayerHead().transform.rotation, null);
                newUtilGun.SetActive(true);

                if (popUpMenu != null)
                    popUpMenu.Deactivate();
            }
        }

        /// <summary>
        /// Instantiates the nimbus gun in front of the player.
        /// </summary>
        public static void SpawnNimbusGun(float distance)
        {
            if (nimbusSpawnable != null)
            {
                if (poofBallSpawnable != null)
                    GameObject.Instantiate(poofBallSpawnable.prefab, Player.GetPlayerHead().transform.position + Player.GetPlayerHead().transform.forward * distance, Player.GetPlayerHead().transform.rotation, null);

                GameObject newUtilGun = GameObject.Instantiate(nimbusSpawnable.prefab, Player.GetPlayerHead().transform.position + Player.GetPlayerHead().transform.forward * distance, Player.GetPlayerHead().transform.rotation, null);
                newUtilGun.SetActive(true);

                if (popUpMenu != null)
                    popUpMenu.Deactivate();
            }
        }
    }
}
