using System;
using UnityEngine;

namespace ModThatIsNotMod.BoneMenu
{
    public class BoolElement : GenericElement<bool>
    {
        public BoolElement(string displayText, Color color, bool value, Action<bool> action) : base(displayText, color) { SetValue(value); SetAction(action); }

        protected internal override void OnMainInputPressed()
        {
            // Invert the value and invoke the callback
            SetValue(!value);
            action?.Invoke(value);
        }
    }
}
