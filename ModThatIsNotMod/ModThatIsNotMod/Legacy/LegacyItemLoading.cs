using ModThatIsNotMod.Internals;
using ModThatIsNotMod.MonoBehaviours;
using StressLevelZero.Data;
using StressLevelZero.Pool;
using StressLevelZero.Props.Weapons;
using System;
using System.Collections.Generic;
using UnityEngine;
using static ModThatIsNotMod.MonoBehaviours.GunFireModes;

namespace ModThatIsNotMod.Legacy
{
    // --------------------------------------------------------------------------------------------------
    // DON'T MESS WITH ANY OF THIS STUFF, IT'S VERY OLD AND THINGS WILL PROBABLY BREAK IF IT GETS CHANGED
    // ModThatIsNotMod.Internals.ItemLoading is the up to date item loading
    // --------------------------------------------------------------------------------------------------

    internal static class LegacyItemLoading
    {
        private static readonly string itemSettingsName = "ItemSettings";

        private static readonly string categoryName = "Category";
        private static readonly string pooledAmountName = "PoolAmount";
        private static readonly string hideInMenuName = "HideInMenu";

        private static readonly string instantiateOnFireName = "InstantiateOnFire";

        private static readonly string noMuzzleFlashName = "NoMuzzleFlash";
        private static readonly string casingScaleName = "BulletCasingScale";

        private static readonly string burstFiringName = "BurstFiring";
        private static readonly string burstTimeoutName = "BurstTimeout";
        private static readonly string cycleFireModeGripName = "FireModeGrip";
        private static readonly string fullAutoName = "Auto";
        private static readonly string semiAutoName = "Semi";
        private static readonly string burstFireName = "Burst";

        private static readonly string disableEjectButtonName = "DisableEjectButton";
        private static readonly string noAutoEjectName = "NoAutoEject";
        private static readonly string alwaysAutoEjectName = "AlwaysAutoEject";


        /// <summary>
        /// Finds the settings for each item and creates a spawnable object with them.
        /// Adds the items to the spawn menu unless they're marked as hidden.
        /// </summary>
        public static LoadedMelonData LoadItemsFromMelon(GameObject customItemsGO)
        {
            LoadedMelonData loadedMelonData = new LoadedMelonData(0, new List<LoadedItemData>());

            for (int i = 0; i < customItemsGO.transform.childCount; i++)
            {
                GameObject currentItem = customItemsGO.transform.GetChild(i).gameObject;

                // Obviously don't add the item settings object to the spawn menu
                if (currentItem.name == itemSettingsName)
                    continue;

                #region Backwards compatibility for gun modifications
                if (currentItem.GetComponent<Gun>())
                {
                    // InstantiateOnFire
                    currentItem.transform.Find(instantiateOnFireName)?.gameObject.AddComponent<InstantiateOnFire>();

                    #region SimpleGunModifiers
                    bool hasModifiers = false;
                    SimpleGunModifiers.MuzzleFlashType muzzleFlashType = SimpleGunModifiers.MuzzleFlashType.Default;
                    Vector3 casingScale = Vector3.one;

                    // Muzzle flash
                    if (currentItem.transform.Find(noMuzzleFlashName))
                    {
                        muzzleFlashType = SimpleGunModifiers.MuzzleFlashType.None;
                        hasModifiers = true;
                    }
                    else if (currentItem.GetComponent<Gun>().isSilenced)
                    {
                        muzzleFlashType = SimpleGunModifiers.MuzzleFlashType.Silenced;
                        hasModifiers = true;
                    }

                    // Bullet casings
                    if (currentItem.transform.Find(casingScaleName))
                    {
                        casingScale = Vector3FromString(currentItem.transform.Find(casingScaleName).GetChild(0).name);
                        hasModifiers = true;
                    }

                    // Apply component
                    if (hasModifiers)
                    {
                        SimpleGunModifiers simpleGunModifiers = currentItem.AddComponent<SimpleGunModifiers>();
                        simpleGunModifiers.muzzleFlashType = muzzleFlashType;
                        simpleGunModifiers.ejectedCartridgeScale = casingScale;
                        CustomMonoBehaviourHandler.SaveCustomMonoBehaviour<SimpleGunModifiers>(currentItem);
                    }
                    #endregion

                    #region GunFireModes
                    if (currentItem.transform.Find(burstFiringName))
                    {
                        int burstAmount = int.Parse(currentItem.transform.Find(burstFiringName).GetChild(0).name);
                        Transform toggleGripTransform = currentItem.transform.Find(cycleFireModeGripName);
                        List<FireMode> fireModes = new List<FireMode>() { FireMode.Burst };
                        if (toggleGripTransform != null)
                        {
                            for (int j = 0; j < toggleGripTransform.childCount; j++)
                            {
                                if (toggleGripTransform.GetChild(j).name == fullAutoName)
                                    fireModes.Add(FireMode.FullAuto);
                                if (toggleGripTransform.GetChild(j).name == semiAutoName)
                                    fireModes.Add(FireMode.SemiAuto);
                            }
                            toggleGripTransform.gameObject.AddComponent<CycleFireModeGrip>();
                        }

                        float timeout = 0.2f;
                        Transform burstTimeoutObj = currentItem.transform.Find(burstTimeoutName);
                        if (burstTimeoutObj != null)
                            timeout = int.Parse(burstTimeoutObj.GetChild(0).name) / 1000f;

                        GunFireModes gunFireModes = currentItem.AddComponent<GunFireModes>();
                        gunFireModes.burstShots = burstAmount;
                        gunFireModes.burstTimeout = timeout;
                        gunFireModes.allowedFireModes = fireModes.ToArray();

                        CustomMonoBehaviourHandler.SaveCustomMonoBehaviour<GunFireModes>(currentItem);
                    }
                    else if (currentItem.transform.Find(cycleFireModeGripName))
                    {
                        Transform toggleGripTransform = currentItem.transform.Find(cycleFireModeGripName);
                        List<FireMode> fireModes = new List<FireMode>();
                        if (toggleGripTransform != null)
                        {
                            for (int j = 0; j < toggleGripTransform.childCount; j++)
                            {
                                if (toggleGripTransform.GetChild(j).name == fullAutoName)
                                    fireModes.Add(FireMode.FullAuto);
                                if (toggleGripTransform.GetChild(j).name == semiAutoName)
                                    fireModes.Add(FireMode.SemiAuto);
                                if (toggleGripTransform.GetChild(j).name == burstFireName)
                                    fireModes.Add(FireMode.Burst);
                            }
                            toggleGripTransform.gameObject.AddComponent<CycleFireModeGrip>();
                        }

                        GunFireModes gunFireModes = currentItem.AddComponent<GunFireModes>();
                        gunFireModes.allowedFireModes = fireModes.ToArray();
                        CustomMonoBehaviourHandler.SaveCustomMonoBehaviour<GunFireModes>(currentItem);
                    }
                    #endregion

                    #region MagEjectSettings
                    bool disableEjectButton = false;
                    MagEjectSettings.AutoEjectMode autoEjectMode = MagEjectSettings.AutoEjectMode.Default;

                    if (currentItem.transform.Find(disableEjectButtonName))
                        disableEjectButton = true;
                    if (currentItem.transform.Find(noAutoEjectName))
                        autoEjectMode = MagEjectSettings.AutoEjectMode.Never;
                    else if (currentItem.transform.Find(alwaysAutoEjectName))
                        autoEjectMode = MagEjectSettings.AutoEjectMode.Always;

                    if (autoEjectMode != MagEjectSettings.AutoEjectMode.Default || disableEjectButton)
                    {
                        MagEjectSettings magEjectSettings = currentItem.AddComponent<MagEjectSettings>();
                        magEjectSettings.disableMagEjectButton = disableEjectButton;
                        magEjectSettings.autoEjectMode = autoEjectMode;
                        CustomMonoBehaviourHandler.SaveCustomMonoBehaviour<MagEjectSettings>(currentItem);
                    }
                    #endregion
                }
                #endregion

                LegacyItemSettings itemSettings = GetItemSettings(customItemsGO, currentItem);
                if (itemSettings != null)
                {
                    CategoryFilters category = itemSettings.category;
                    int pooledAmount = itemSettings.pooledAmount;

                    InfiniteAmmoGuns.CheckForInfiniteAmmoVersion(currentItem, currentItem.name, category, PoolMode.REUSEOLDEST, pooledAmount, itemSettings.hideInMenu);
                    ItemLoading.SetAudioMixers(currentItem);

                    CustomItems.InvokeOnItemLoaded(currentItem);
                    currentItem.AddComponent<CustomItem>();
                    SpawnableObject spawnable = CustomItems.CreateSpawnableObject(currentItem, currentItem.name, category, PoolMode.REUSEOLDEST, pooledAmount, isHidden: itemSettings.hideInMenu);
                    if (!itemSettings.hideInMenu)
                        SpawnMenu.AddItem(spawnable);

                    loadedMelonData.loadedItems.Add(new LoadedItemData(currentItem.name, category, itemSettings.hideInMenu));
                    loadedMelonData.itemsLoaded++;
                }
                else
                {
                    int category = 7; //7 is the number for props
                    int pooledAmount = 10;

                    Transform hideObj = currentItem.transform.Find(hideInMenuName); //If this object exists, the item shouldn't be added to the spawn menu

                    //Get category for the spawn menu
                    try
                    {
                        Transform categoryObj = currentItem.transform.Find(categoryName);
                        if (categoryObj != null)
                            category = Convert.ToInt32(categoryObj.GetChild(0).name);
                        else if (currentItem.transform.GetChild(currentItem.transform.childCount - 1).GetChild(0) != null) //For compatibility with very old asset bundles. DON'T TOUCH
                            category = Convert.ToInt32(currentItem.transform.GetChild(currentItem.transform.childCount - 1).GetChild(0).name);
                    }
                    catch { }

                    //Get the max amount of the item that can be spawned at once
                    Transform poolAmountObj = currentItem.transform.Find(pooledAmountName);
                    if (poolAmountObj != null)
                        pooledAmount = Convert.ToInt32(poolAmountObj.GetChild(0).name);

                    InfiniteAmmoGuns.CheckForInfiniteAmmoVersion(currentItem, currentItem.name, (CategoryFilters)category, PoolMode.REUSEOLDEST, pooledAmount, hideObj != null);

                    CustomItems.InvokeOnItemLoaded(currentItem);
                    currentItem.AddComponent<CustomItem>();
                    SpawnableObject spawnable = CustomItems.CreateSpawnableObject(currentItem, currentItem.name, (CategoryFilters)category, PoolMode.REUSEOLDEST, pooledAmount, isHidden: hideObj != null);
                    if (hideObj == null)
                        SpawnMenu.AddItem(spawnable);

                    loadedMelonData.loadedItems.Add(new LoadedItemData(currentItem.name, (CategoryFilters)category, hideObj != null));
                    loadedMelonData.itemsLoaded++;
                }
            }

            return loadedMelonData;
        }

        /// <summary>
        /// Finds the settings for the given item.
        /// </summary>
        internal static LegacyItemSettings GetItemSettings(GameObject customItems, GameObject item)
        {
            Transform rootSettingsObj = customItems.transform.Find(itemSettingsName);
            if (rootSettingsObj == null)
                return null;

            Transform thisSettingsObj = rootSettingsObj.Find(item.name);
            if (thisSettingsObj == null)
                return new LegacyItemSettings(true);

            LegacyItemSettings settings = new LegacyItemSettings(false);

            settings.hideInMenu = GetHideInMenu(thisSettingsObj);
            if (settings.hideInMenu)
                return settings;

            settings.category = GetCategory(thisSettingsObj);
            settings.pooledAmount = GetPoolAmount(thisSettingsObj);

            return settings;
        }

        /// <summary>
        /// Gets if the item is hidden.
        /// </summary>
        private static bool GetHideInMenu(Transform settingsObj)
        {
            return settingsObj.Find(hideInMenuName) != null;
        }

        /// <summary>
        /// Gets the size of the item pool.
        /// </summary>
        private static int GetPoolAmount(Transform settingsObj)
        {
            Transform poolAmountRoot = settingsObj.Find(pooledAmountName);

            if (poolAmountRoot != null && int.TryParse(poolAmountRoot.GetChild(0).name, out int poolAmount))
                return poolAmount;
            else
                return 10;
        }

        /// <summary>
        /// Gets the category of the item.
        /// </summary>
        private static CategoryFilters GetCategory(Transform settingsObj)
        {
            Transform categoryRoot = settingsObj.Find(categoryName);

            if (categoryRoot != null && int.TryParse(categoryRoot.GetChild(0).name, out int categoryNum))
                return (CategoryFilters)categoryNum;
            else
                return CategoryFilters.PROPS;
        }

        // Messy but it works ¯\_(ツ)_/¯
        private static Vector3 Vector3FromString(string str)
        {
            string[] vArr = str.Split(new char[] { ',' });
            System.Globalization.CultureInfo c = System.Globalization.CultureInfo.InvariantCulture;
            return new Vector3(float.Parse(vArr[0].Trim(), c), float.Parse(vArr[1].Trim(), c), float.Parse(vArr[2].Trim(), c));
        }
    }

    internal class LegacyItemSettings
    {
        public CategoryFilters category;
        public int pooledAmount;
        public bool hideInMenu;

        public LegacyItemSettings(bool hideInMenu)
        {
            this.hideInMenu = hideInMenu;
        }
    }
}
