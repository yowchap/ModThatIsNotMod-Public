using ModThatIsNotMod.Internals;
using StressLevelZero.Combat;
using StressLevelZero.Interaction;
using StressLevelZero.Props.Weapons;
using System;
using UnityEngine;

namespace ModThatIsNotMod.MonoBehaviours
{
    public class SimpleGunModifiers : MonoBehaviour
    {
        public SimpleGunModifiers(IntPtr intPtr) : base(intPtr) { }


        public MuzzleFlashType muzzleFlashType = MuzzleFlashType.Default;
        public Vector3 ejectedCartridgeScale = Vector3.one;

        public int ammoPerShot = 1;

        public bool disableSlideLock = false;

        public bool requiresCharging = false;
        public float chargingTime = 1;

        internal bool isCharging { get; private set; } = false;
        internal bool isChargeComplete { get; private set; } = false;
        internal float chargeCompleteTime { get; private set; } = 0;

        internal InstantiateOnGunCharge instantiateOnGunCharge = null;
        private GameObject chargingEffectInstance = null;

        private Gun gun;
        private Hand hand;

        internal MagazineData magazineData { get; private set; }


        private void Awake()
        {
            CustomMonoBehaviourHandler.SetFieldValues(this);
            GunModifiersController.RegisterGun(this);

            gun = gameObject.GetComponent<Gun>();
            if (gun.overrideMagazine != null)
                magazineData = gun.overrideMagazine;
            else if (gun.magazineSocket != null)
                magazineData = gun.magazineSocket.magazineData;
        }

        /// <summary>
        /// Handles gun charging
        /// </summary>
        private void Update()
        {
            if (isCharging)
            {
                if (hand != null && gun.triggerGrip.GetHand() == hand && hand.controller.GetPrimaryInteractionButton())
                {
                    if (Time.time >= chargeCompleteTime)
                    {
                        gun.Fire();

                        isCharging = false;
                        isChargeComplete = true;
                    }
                    else
                    {
                        isChargeComplete = false;
                    }
                }
                else
                {
                    isCharging = false;
                    isChargeComplete = false;
                    if (chargingEffectInstance != null)
                        GameObject.Destroy(chargingEffectInstance);
                }
            }
            else
            {
                isChargeComplete = hand != null && gun.triggerGrip.GetHand() == hand && hand.controller.GetPrimaryInteractionButton();
            }
        }

        public void BeginCharge(Hand hand)
        {
            isCharging = true;
            isChargeComplete = false;
            chargeCompleteTime = Time.time + chargingTime;

            this.hand = hand;

            chargingEffectInstance = instantiateOnGunCharge?.OnGunCharge();
        }

        public enum MuzzleFlashType
        {
            Default,
            Silenced,
            None
        }
    }
}
