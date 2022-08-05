using StressLevelZero.Interaction;
using StressLevelZero.Props.Weapons;
using StressLevelZero.Rig;
using UnityEngine;

namespace ModThatIsNotMod
{
    public static class Player
    {
        private static readonly string rigManagerName = "[RigManager (Default Brett)]";
        private static readonly string playerHeadPath = "[PhysicsRig]/Head/PlayerTrigger"; // The path to the head relative to the rig manager

        private static GameObject rigManager;
        private static GameObject playerHead;

        public static Hand leftHand { get; private set; }
        public static Hand rightHand { get; private set; }
        public static bool handsExist => leftHand != null && rightHand != null;

        public static Controller leftController { get; private set; }
        public static Controller rightController { get; private set; }

        public static BaseController leftBaseController { get; private set; }
        public static BaseController rightBaseController { get; private set; }

        public static bool controllersExist => leftController != null && rightController != null;
        public static bool baseControllersExist => leftBaseController != null && rightBaseController != null;

        private static ControllerRig controllerRig;



        /// <summary>
        /// Finds and assigns the hand and controller components to their variables.
        /// Hands can be used to find held objects and controllers for input.
        /// </summary>
        internal static void FindObjectReferences()
        {
            Hand[] hands = GameObject.FindObjectsOfType<Hand>();
            foreach (Hand hand in hands)
            {
                if (hand.name.ToLower().Contains("left"))
                    leftHand = hand;
                else if (hand.name.ToLower().Contains("right"))
                    rightHand = hand;
            }

            Controller[] controllers = GameObject.FindObjectsOfType<Controller>();
            foreach (Controller controller in controllers)
            {
                if (controller.name.ToLower().Contains("left"))
                    leftController = controller;
                else if (controller.name.ToLower().Contains("right"))
                    rightController = controller;
            }

            BaseController[] baseControllers = GameObject.FindObjectsOfType<BaseController>();
            foreach (BaseController baseController in baseControllers)
            {
                if (baseController.name.ToLower().Contains("left"))
                    leftBaseController = baseController;
                else if (baseController.name.ToLower().Contains("right"))
                    rightBaseController = baseController;
            }
            controllerRig = GameObject.FindObjectOfType<ControllerRig>();
        }

        /// <summary>
        /// Returns the root gameobject in the player rig manager.
        /// </summary>
        public static GameObject GetRigManager()
        {
            if (rigManager == null)
                rigManager = GameObject.Find(rigManagerName);

            return rigManager;
        }

        /// <summary>
        /// Returns the gameobject of the player's head.
        /// </summary>
        public static GameObject GetPlayerHead()
        {
            if (playerHead == null)
            {
                GameObject rig = GetRigManager();
                if (rig != null)
                    playerHead = rig.transform.Find(playerHeadPath).gameObject;
            }
            return playerHead;
        }

        /// <summary>
        /// Generic method for getting any component on the object the player is holding.
        /// </summary>
        public static T GetComponentInHand<T>(Hand hand) where T : Component
        {
            T value = null;
            if (hand != null)
            {
                GameObject heldObject = hand.m_CurrentAttachedObject;
                if (heldObject != null)
                {
                    value = heldObject.GetComponentInParent<T>();
                    if (!value)
                        value = heldObject.GetComponentInChildren<T>();
                }
            }
            return value;
        }

        // Figured I'd include this since getting a gun is probably the most common use of GetComponentInHand
        public static Gun GetGunInHand(Hand hand) => GetComponentInHand<Gun>(hand);

        /// <summary>
        /// Returns the object the given hand is holding or null if the hand is null.
        /// </summary>
        public static GameObject GetObjectInHand(Hand hand) => hand != null ? hand.m_CurrentAttachedObject : null;


        public static void RotatePlayer(float rotation)
        {
            if (controllerRig != null)
            {
                controllerRig._rotationDebt = rotation;
                controllerRig.ApplyRotation();
            }
        }

    }
}
