using System;
using UnityEngine;

namespace ModThatIsNotMod.BoneMenu
{
    public class IntElement : GenericElement<int>
    {
        private int increment;
        private int minValue;
        private int maxValue;

        private bool invokeOnValueChanged; // If this is false, you need to press the main button to invoke the action


        public IntElement(string displayText, Color color, int initialValue, Action<int> action, int increment = 1, int minValue = int.MinValue, int maxValue = int.MaxValue, bool invokeOnValueChanged = false) : base(displayText, color)
        {
            elementType = UiElementType.ArrowsButton;

            value = initialValue;

            this.action = action;

            this.increment = increment;
            this.minValue = minValue;
            this.maxValue = maxValue;

            this.invokeOnValueChanged = invokeOnValueChanged;
        }

        /// <summary>
        /// Decrements the value and optionally invokes the action
        /// </summary>
        protected internal override void OnLeftInputPressed()
        {
            int newValue = Mathf.Clamp(value - increment, minValue, maxValue);
            SetValue(newValue);

            if (invokeOnValueChanged)
                action?.Invoke(value);
        }

        /// <summary>
        /// Increments the value and optionally invokes the action
        /// </summary>
        protected internal override void OnRightInputPressed()
        {
            int newValue = Mathf.Clamp(value + increment, minValue, maxValue);
            SetValue(newValue);

            if (invokeOnValueChanged)
                action?.Invoke(value);
        }
    }
}
