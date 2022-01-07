using System;
using UnityEngine;

namespace ModThatIsNotMod.BoneMenu
{
    class ColorElement : GenericElement<Color>
    {
        private float currentHue = 0f;


        public ColorElement(string displayText, Color value, Action<Color> action) : base(displayText, value)
        {
            elementType = UiElementType.ArrowsButton;
            this.value = value;
            this.action = action;

            UpdateUiText();
        }

        protected internal override void OnLeftInputPressed()
        {
            // Decreases the hue a bit
            currentHue = Mathf.Clamp(currentHue - 0.05f, 0f, 1f);
            SetValue(Color.HSVToRGB(currentHue, 1, 1));

            action?.Invoke(value);
        }

        protected internal override void OnRightInputPressed()
        {
            // Increases the hue
            currentHue = Mathf.Clamp(currentHue + 0.05f, 0f, 1f);
            SetValue(Color.HSVToRGB(currentHue, 1, 1));

            action?.Invoke(value);
        }

        protected internal override void UpdateUiText()
        {
            // Set the text color to the value instead of showing the value as a string like usual
            if (tmpro != null)
            {
                tmpro.text = displayText;
                tmpro.color = value;
            }
        }
    }
}
