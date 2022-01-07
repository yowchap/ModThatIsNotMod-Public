using HarmonyLib;
using ModThatIsNotMod.Internals;
using StressLevelZero.Interaction;
using StressLevelZero.UI;
using StressLevelZero.UI.Radial;
using StressLevelZero.VRMK;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using TMPro;
using UnityEngine;

namespace ModThatIsNotMod.BoneMenu
{
    public static class MenuManager
    {
        // Base objects
        private static GameObject baseMenuCanvas;
        private static GameObject baseMenuButton;


        // Scene instance objects
        private static GameObject menuInstance;
        private static Transform buttonContainer;
        private static AudioSource audioSource;

        private static GameObject firstCategoryHeader;
        private static GameObject secondCategoryHeader;
        private static TextMeshProUGUI firstCategoryHeaderText;
        private static TextMeshProUGUI secondCategoryHeaderText;


        // Menu interaction objects
        private static Hand menuHand { get { return Preferences.menuOnRightHand.value ? Player.rightHand : Player.leftHand; } }
        private static Hand heldItemHand = null;

        private static GameObject menuInteractor;

        private static SLZ_BodyBlender defaultBodyBlender;
        private static SLZ_BodyBlender otherBodyBlender;

        private static Vector3 prevFingertipPos;


        // Menu gesture settings
        private static Vector3 palmUpRange = new Vector3(0f, 0.6f);
        private static Vector3 palmForwardRange = new Vector3(0.5f, 1f);

        private static float leftThumbOutThreshold = -0.8f;
        private static float rightThumbOutThreshold = 0.8f;


        // Menu opening settings
        private static bool canOpenMenu = false;
        private static bool isItemMenuOpen = false;

        private static float lastTimeWhenMenuCouldOpen = 0;
        private static float timeToWaitBeforeMenuClose = 0.1f;

        public static float timeUntilButtonsCanBePressed { get; private set; } = 0;
        private static float categoryOpenTimeout = 0.1f;


        // Menu position settings
        private static float menuMovementSmoothing = 0.8f;
        internal static Vector3 menuOffset = Vector3.zero;


        // Object paths and names
        internal static readonly string fingerPointerName = "BoneMenuFingerCollider";

        private static readonly string rightFingertipPath = "r_Finger_01_02SHJnt/r_Finger_01_03SHJnt/r_Finger_01_04SHJnt";
        private static readonly string leftFingertipPath = "l_Finger_01_02SHJnt/l_Finger_01_03SHJnt/l_Finger_01_04SHJnt";

        private static readonly string bodyBlenderPath = "[SkeletonRig (GameWorld Brett)]/Brett@neutral";

        private static readonly string titleTextPath = "Logo/TitleText";

        private static readonly string firstCategoryHeaderPath = "CurrentCategoryUI/Categories/FirstCategory";
        private static readonly string secondCategoryHeaderPath = "CurrentCategoryUI/Categories/SecondCategory";


        // Menu categories
        private static List<MenuCategory> categories = new List<MenuCategory>();
        private static MenuCategory activeCategory = null;

        private static FunctionElement backFunction;


        internal static void InitialSetup()
        {
            // Load the menu ui from the asset bundle embedded resource
            AssetBundle bundle = null;
            Assembly assembly = Assembly.GetExecutingAssembly();
            using (Stream stream = assembly.GetManifestResourceStream(assembly.GetManifestResourceNames().First(x => x.Contains("bonemenu"))))
            {
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    stream.CopyTo(memoryStream);
                    bundle = AssetBundle.LoadFromMemory(memoryStream.ToArray());
                }
            }

            UnityEngine.Object[] objects = bundle.LoadAllAssets(); // Not sure why, but just doing bundle.Load() didn't work
            baseMenuCanvas = objects.Where(x => x.name == "BoneMenuCanvas").First().Cast<GameObject>();
            baseMenuButton = objects.Where(x => x.name == "Button").First().Cast<GameObject>();

            baseMenuCanvas.hideFlags = HideFlags.DontUnloadUnusedAsset;
            baseMenuButton.hideFlags = HideFlags.DontUnloadUnusedAsset;
            GameObject.DontDestroyOnLoad(baseMenuCanvas);
            GameObject.DontDestroyOnLoad(baseMenuButton);

            baseMenuCanvas.transform.Find("Scrollbar").gameObject.AddComponent<MenuScrollbar>();

            SetupBaseButton(baseMenuButton);
            SetupBackButton(baseMenuCanvas.transform.Find("BackButton").gameObject);
            SetupTitleBar(baseMenuCanvas);

            // This is so player models doesn't break the finger interactor
            MethodInfo bodyBlenderFillBonesMethod = typeof(SLZ_BodyBlender).GetMethod("AutoFillBones", AccessTools.all);
            MethodInfo bonesFilledHookMethod = typeof(MenuManager).GetMethod("OnBodyBlenderBonesFilled", AccessTools.all);
            Hooking.CreateHook(bodyBlenderFillBonesMethod, bonesFilledHookMethod);

            menuOffset = new Vector3(Preferences.menuOffsetX.value, Preferences.menuOffsetY.value, Preferences.menuOffsetZ.value);

            HeldItemMenuManager.InteractableHookingSetup();
        }

        internal static void SceneSetup()
        {
            defaultBodyBlender = Player.GetRigManager().transform.Find(bodyBlenderPath).GetComponent<SLZ_BodyBlender>();

            // Add a collider to the player's fingertip so they can use the menu
            menuInteractor = new GameObject(fingerPointerName);
            menuInteractor.name = fingerPointerName;

            SphereCollider sphereCollider = menuInteractor.AddComponent<SphereCollider>();
            sphereCollider.isTrigger = true;
            sphereCollider.radius = 0.02f;

            Rigidbody rb = menuInteractor.AddComponent<Rigidbody>();
            rb.useGravity = false;
            rb.isKinematic = true;
            rb.mass = 0.00001f;

            // Make a new instance of the menu
            menuInstance = GameObject.Instantiate(baseMenuCanvas, menuHand.palmPositionTransform.position, Quaternion.identity);
            buttonContainer = menuInstance.transform.Find("ButtonListPanel/ButtonContainer");
            menuInstance.transform.Find("BackButton").GetComponent<MenuElementInputManager>().SetElement(backFunction);

            SetupCategoryHeaders();

            audioSource = menuInstance.GetComponent<AudioSource>();
            audioSource.outputAudioMixerGroup = Audio.sfxMixer;

            AddRadialMenuButton();

            menuInstance.SetActive(false);
        }

        internal static void AddRadialMenuButton()
        {
            if (!Preferences.boneMenuRadialButtonEnabled.value)
                return;

            GameObject rig = Player.GetRigManager();
            if (rig != null)
            {
                Transform panel = rig.transform.Find("[UIRig]/PLAYERUI/panel_Default");
                if (panel != null)
                {
                    Transform buttonTransform = panel.transform.Find("button_Region_NW");
                    if (buttonTransform)
                    {
                        PageView pageView = panel.GetComponent<PageView>();
                        PageItemView pageItemView = buttonTransform.GetComponent<PageItemView>();
                        PopUpMenuView radialMenu = GameObject.FindObjectOfType<PopUpMenuView>();

                        Action action = new Action(() =>
                        {
                            if (menuInstance.activeSelf)
                                CloseMenu();
                            else
                                OpenMenu();

                            radialMenu.Deactivate();
                            radialMenu.ForceHideCursor();
                        });

                        pageItemView.m_Data = new PageItem("BoneMenu", PageItem.Directions.NORTHWEST, action);
                        pageView.m_HomePage.items.Add(pageItemView.m_Data);
                    }
                }
            }
        }

        /// <summary>
        /// Pretty sure L4rs did this stuff I don't understand it
        /// </summary>
        internal static void SetHeldItemMenuEnabled(bool enabled, Grip grip, Hand hand)
        {
            if (enabled)
            {
                HeldItemInfo heldItemInfo = HeldItemMenuManager.GetItemInfo(grip.interactable.GetInstanceID());

                isItemMenuOpen = true;
                heldItemHand = hand;

                menuInstance.transform.parent = heldItemInfo.transform;
                menuInstance.transform.localPosition = Vector3.zero;
                menuInstance.transform.localRotation = Quaternion.identity;

                OpenMenu();
                OpenCategory(heldItemInfo.category);
            }
            else
            {
                isItemMenuOpen = false;
                heldItemHand = null;

                menuInstance.transform.parent = null;

                CloseMenu();
            }
        }

        internal static void OnUpdate()
        {
            // Gesture opening
            if (menuHand != null && Preferences.boneMenuGestureEnabled.value)
            {
                bool isMenuHandInOpenPose = IsHandInOpenMenuPose(menuHand);
                bool isOtherHandInOpenPose = IsHandInOpenMenuPose(menuHand.otherHand);
                bool isMenuHandInClosePose = IsHandInCloseMenuPose(menuHand);

                canOpenMenu = isMenuHandInOpenPose && isOtherHandInOpenPose;
                if (canOpenMenu)
                    OpenMenu();
                else if (isMenuHandInOpenPose || !isMenuHandInClosePose)
                    lastTimeWhenMenuCouldOpen = Time.unscaledTime;
                else if (Time.unscaledTime >= lastTimeWhenMenuCouldOpen + timeToWaitBeforeMenuClose && !isItemMenuOpen)
                    CloseMenu();
            }

            // Menu follow hand
            if (menuInstance != null && !isItemMenuOpen)
                FollowPlayer();

            // Set interactor finger
            // THIS WOULD HAVE BEEN SO MUCH EASIER IF PLAYER MODELS HADN'T BROKEN IT >:(
            SLZ_BodyBlender blender = otherBodyBlender != null ? otherBodyBlender : defaultBodyBlender;
            if (menuInteractor != null && blender != null)
            {
                SLZ_BodyBlender.Bones bones = blender.bones;
                Transform rootFingerBone;
                Transform endFingerBone;
                if (isItemMenuOpen && heldItemHand != null)
                {
                    rootFingerBone = heldItemHand.handedness == StressLevelZero.Handedness.RIGHT ? bones.lfIndex1 : bones.rtIndex1;
                    endFingerBone = heldItemHand.handedness == StressLevelZero.Handedness.RIGHT ? rootFingerBone.transform.Find(leftFingertipPath) : rootFingerBone.transform.Find(rightFingertipPath);
                }
                else
                {
                    rootFingerBone = Preferences.menuOnRightHand.value ? bones.lfIndex1 : bones.rtIndex1;
                    endFingerBone = Preferences.menuOnRightHand.value ? rootFingerBone.transform.Find(leftFingertipPath) : rootFingerBone.transform.Find(rightFingertipPath);
                }
                Vector3 posDiff = endFingerBone.position - prevFingertipPos;
                menuInteractor.transform.position = endFingerBone.position + posDiff;
                prevFingertipPos = endFingerBone.position;
            }
        }

        public static MenuCategory CreateCategory(string name, Color color)
        {
            MenuCategory newCategory = new MenuCategory(name, color);
            categories.Add(newCategory);
            return newCategory;
        }

        /// <summary>
        /// More L4rs stuff I think
        /// </summary>
        public static void CreateItemMenu(Interactable interactable, Transform transform, MenuCategory category, bool isToggle = true)
        {
            HeldItemMenuManager.AddItemMenu(interactable, transform, category, isToggle);
        }

        internal static void PlayButtonPressSound()
        {
            if (audioSource != null)
                audioSource.Play();
        }

        internal static void SendHapticFeedback(float amplitude = 0.2f)
        {
            // Frequency: 0-320
            // Amplitude: 0-1
            if (isItemMenuOpen)
                heldItemHand.otherHand.controller.HapticAction(0, 0.125f, 50, amplitude);
            else menuHand.otherHand.controller.HapticAction(0, 0.125f, 50, amplitude);
        }

        internal static GameObject CreateNewButton(MenuElement element)
        {
            if (menuInstance != null)
            {
                GameObject newButton = GameObject.Instantiate(baseMenuButton, buttonContainer);
                newButton.AddComponent<MenuElementInputManager>().SetElement(element);
                newButton.SetActive(false);

                return newButton;
            }
            return null;
        }

        /// <summary>
        /// Just moves the menu to follow the hand of the player
        /// </summary>
        private static void FollowPlayer()
        {
            menuInstance.transform.position = Vector3.Lerp(menuInstance.transform.position, menuHand.palmPositionTransform.position + new Vector3(0, 0.4f, 0), menuMovementSmoothing);
            menuInstance.transform.Translate(menuOffset, Space.Self);
            Vector3 lookPos = menuInstance.transform.position - Player.GetPlayerHead().transform.position;
            lookPos.y = 0;
            menuInstance.transform.rotation = Quaternion.Lerp(menuInstance.transform.rotation, Quaternion.LookRotation(lookPos), menuMovementSmoothing);
        }

        public static void OpenCategory(MenuCategory category)
        {
            // This should stop you from accidentally pressing a button immediately after opening a category, but apparently it doesn't work for some people so idk ¯\_(ツ)_/¯
            timeUntilButtonsCanBePressed = Time.unscaledTime + categoryOpenTimeout;

            DisableAllButtons();

            // Make button gameobjects for every element in the new category
            foreach (MenuElement element in category.elements)
            {
                if (element.gameObject != null)
                {
                    element.gameObject.SetActive(true);
                }
                else
                {
                    GameObject newButton = CreateNewButton(element);
                    newButton.SetActive(true);
                }
            }

            // Set the header text at the top of the menu
            firstCategoryHeader.SetActive(true);
            if (category.parentCategory != null)
            {
                secondCategoryHeader.SetActive(true);
                secondCategoryHeaderText.text = category.displayText;
                firstCategoryHeaderText.text = category.parentCategory.displayText;
            }
            else
            {
                secondCategoryHeader.SetActive(false);
                firstCategoryHeaderText.text = category.displayText;
            }

            activeCategory = category;
        }

        public static void OpenMenu()
        {
            if (menuInstance != null && !menuInstance.activeSelf)
            {
                menuInstance.SetActive(true);
                GoToRootMenu();
            }
        }

        public static void OpenPreviousCategory()
        {
            if (activeCategory == null)
                return;

            if (activeCategory.parentCategory != null)
                OpenCategory(activeCategory.parentCategory);
            else if (!isItemMenuOpen)
                GoToRootMenu();
        }

        public static void GoToRootMenu()
        {
            DisableAllButtons();
            activeCategory = null;

            foreach (MenuCategory category in categories)
            {
                if (category.gameObject != null)
                {
                    category.gameObject.SetActive(true);
                }
                else
                {
                    GameObject newCategoryButton = CreateNewButton(category);
                    newCategoryButton.SetActive(true);
                }
            }

            firstCategoryHeader.SetActive(false);
            secondCategoryHeader.SetActive(false);
        }

        public static void CloseMenu()
        {
            activeCategory = null;
            if (menuInstance != null)
                menuInstance.SetActive(false);
        }

        private static void DisableAllButtons()
        {
            for (int i = 0; i < buttonContainer.childCount; i++)
                buttonContainer.GetChild(i).gameObject.SetActive(false);
        }

        /// <summary>
        /// Used in harmony patch to fix a compatibility issue with player models
        /// </summary>
        private static void OnBodyBlenderBonesFilled(SLZ_BodyBlender __instance)
        {
            otherBodyBlender = __instance;
        }

        /// <summary>
        /// Checks if the hand is rotated the right way for the menu to open.
        /// Palm should face up, with the fingers pointing away from the player and slightly down.
        /// </summary>
        private static bool IsHandInOpenMenuPose(Hand hand)
        {
            Transform palm = hand.palmPositionTransform;
            float upDot = Vector3.Dot(Vector3.up, palm.forward);
            float forwardDot = Vector3.Dot(Player.GetPlayerHead().transform.forward, palm.forward);
            float thumbDot = Vector3.Dot(Player.GetPlayerHead().transform.right, palm.up);

            bool isThumbAngleCorrect = false;
            if (hand == Player.rightHand)
                isThumbAngleCorrect = thumbDot > rightThumbOutThreshold;
            else
                isThumbAngleCorrect = thumbDot < leftThumbOutThreshold;

            bool isPalmUpAngleCorrect = upDot > palmUpRange.x && upDot < palmUpRange.y;
            bool isPalmForwardAngleCorrect = forwardDot > palmForwardRange.x && forwardDot < palmForwardRange.y;

            return isThumbAngleCorrect && isPalmUpAngleCorrect && isPalmForwardAngleCorrect;
        }

        /// <summary>
        /// Checks if the hand is rotated correctly to close the menu.
        /// The hand should be palm down, with the fingers away from the player and slightly up (or maybe down idr it's just muscle memory at this point).
        /// </summary>
        private static bool IsHandInCloseMenuPose(Hand hand)
        {
            Transform palm = hand.palmPositionTransform;
            float downDot = Vector3.Dot(Vector3.down, palm.forward);
            float thumbDot = Vector3.Dot(Player.GetPlayerHead().transform.right, palm.up);

            bool isThumbAngleCorrect = false;
            if (hand == Player.rightHand)
                isThumbAngleCorrect = thumbDot < leftThumbOutThreshold;
            else
                isThumbAngleCorrect = thumbDot > rightThumbOutThreshold;

            bool isPalmAngleCorrect = downDot > palmUpRange.x && downDot < palmUpRange.y;

            return isThumbAngleCorrect && isPalmAngleCorrect;
        }


        /* -------------------------------------------------------------------------------------------------------------------------------------
         * 
         * Everything below this point should basically just be ignored because it's really bad.
         * This was done before TMP components could be loaded in asset bundles so every value has to be set manually for them.
         * Changing stuff here will most likely break the menu so just don't.
         * 
        ------------------------------------------------------------------------------------------------------------------------------------- */


        private static void SetupBaseButton(GameObject button)
        {
            GameObject text = button.transform.Find("Text").gameObject;

            // TMP couldn't be loaded in asset bundles when I made this so I have to set everything manually
            TextMeshProUGUI tmpro = text.AddComponent<TextMeshProUGUI>();
            tmpro.enableAutoSizing = true;
            tmpro.fontSizeMin = 0.1f;
            tmpro.fontSizeMax = 0.2f;
            tmpro.alignment = TextAlignmentOptions.MidlineLeft;
            tmpro.enableWordWrapping = true;
            tmpro.color = Color.white;
        }

        private static void SetupBackButton(GameObject button)
        {
            GameObject text = button.transform.Find("Text").gameObject;
            TextMeshProUGUI tmpro = text.AddComponent<TextMeshProUGUI>();

            backFunction = new FunctionElement("Back", Color.grey, OpenPreviousCategory);
            backFunction.elementType = UiElementType.BackButton;

            button.AddComponent<MenuElementInputManager>().SetElement(backFunction);
        }

        private static void SetupTitleBar(GameObject menu)
        {
            // Title text
            Transform titleTextObj = menu.transform.Find(titleTextPath);

            TextMeshProUGUI tmpro = titleTextObj.gameObject.AddComponent<TextMeshProUGUI>();
            tmpro.fontSize = 0.125f;
            tmpro.enableWordWrapping = true;
            tmpro.color = new Color(0.5568628f, 0.9137255f, 0.972549f);
            tmpro.fontStyle = FontStyles.Bold;
            tmpro.text = "BoneMenu";

            // Category headers
            AddTextToCategoryHeader(menu.transform.Find(firstCategoryHeaderPath));
            AddTextToCategoryHeader(menu.transform.Find(secondCategoryHeaderPath));
        }

        private static void AddTextToCategoryHeader(Transform header)
        {
            GameObject textObj = header.Find("Text").gameObject;

            TextMeshProUGUI tmpro = textObj.AddComponent<TextMeshProUGUI>();
            tmpro.fontSize = 0.075f;
            tmpro.enableWordWrapping = false;
            tmpro.color = new Color(0.6603774f, 0.6603774f, 0.6603774f);
        }

        private static void SetupCategoryHeaders()
        {
            firstCategoryHeader = menuInstance.transform.Find(firstCategoryHeaderPath).gameObject;
            secondCategoryHeader = menuInstance.transform.Find(secondCategoryHeaderPath).gameObject;

            firstCategoryHeaderText = firstCategoryHeader.GetComponentInChildren<TextMeshProUGUI>();
            secondCategoryHeaderText = secondCategoryHeader.GetComponentInChildren<TextMeshProUGUI>();

            firstCategoryHeader.SetActive(false);
            secondCategoryHeader.SetActive(false);
        }
    }
}
