using System;
using UnityEngine;

namespace ModThatIsNotMod.BoneMenu
{
    public class FloatElement : GenericElement<float>
    {
        private float increment;
        private float minValue;
        private float maxValue;

        private bool invokeOnValueChanged; // If this is false, you need to press the main button to invoke the action


        public FloatElement(string displayText, Color color, float initialValue, Action<float> action, float increment = 1, float minValue = float.MinValue, float maxValue = float.MaxValue, bool invokeOnValueChanged = false) : base(displayText, color)
        {
            elementType = UiElementType.ArrowsButton;

            value = initialValue;

            this.action = action;

            this.increment = increment;
            this.minValue = minValue;
            this.maxValue = maxValue;

            this.invokeOnValueChanged = invokeOnValueChanged;
        }

        protected internal override void OnLeftInputPressed()
        {
            float newValue = Mathf.Clamp(value - increment, minValue, maxValue);
            SetValue(newValue);

            if (invokeOnValueChanged)
                action?.Invoke(value);
        }

        protected internal override void OnRightInputPressed()
        {
            float newValue = Mathf.Clamp(value + increment, minValue, maxValue);
            SetValue(newValue);

            if (invokeOnValueChanged)
                action?.Invoke(value);
        }

        protected internal override void UpdateUiText()
        {
            if (tmpro != null)
                tmpro.text = $"{displayText}: {value.ToString("0.####")}";
        }
    }
}
