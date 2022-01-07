using System;
using UnityEngine;

namespace ModThatIsNotMod.BoneMenu
{
    public abstract class GenericElement<T> : MenuElement
    {
        protected T value;
        protected Action<T> action;

        public GenericElement(string displayText, Color color) : base(displayText, color) { }

        protected internal override void OnMainInputPressed() => action?.Invoke(value);

        /// <summary>
        /// By default assigns the value and updates the text on the button
        /// </summary>
        public virtual void SetValue(T value)
        {
            this.value = value;
            UpdateUiText();
        }

        public virtual T GetValue()
        {
            return value;
        }

        public virtual void SetAction(Action<T> action) => this.action = action;

        /// <summary>
        /// By default sets the text to "Display Text: Value"
        /// </summary>
        protected internal override void UpdateUiText()
        {
            if (tmpro != null)
                tmpro.text = $"{displayText}: {value.ToString()}";
        }
    }
}
