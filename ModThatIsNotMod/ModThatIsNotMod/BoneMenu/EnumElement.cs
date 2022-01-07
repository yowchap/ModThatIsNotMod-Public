using System;
using UnityEngine;

namespace ModThatIsNotMod.BoneMenu
{
    public class EnumElement : GenericElement<Enum>
    {
        public EnumElement(string displayText, Color color, Enum value, Action<Enum> action) : base(displayText, color)
        {
            SetValue(value);
            SetAction(action);
        }

        protected internal override void OnMainInputPressed()
        {
            // Set the value to the next item of the enum and invoke the action
            SetValue(GetNextValue());
            action?.Invoke(value);
        }

        /// <summary>
        /// Returns the next element of the enum, or the first if it needs to loop back
        /// </summary>
        private Enum GetNextValue()
        {
            Array values = Enum.GetValues(value.GetType());
            int nextIndex = Array.IndexOf(values, value) + 1;
            return nextIndex == values.Length ? (Enum)values.GetValue(0) : (Enum)values.GetValue(nextIndex);
        }
    }
}
