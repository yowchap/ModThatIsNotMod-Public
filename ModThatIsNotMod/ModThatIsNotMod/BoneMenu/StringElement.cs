using System;
using UnityEngine;

namespace ModThatIsNotMod.BoneMenu
{
    public class StringElement : GenericElement<string>
    {
        public StringElement(string displayText, Color color, string value, Action<string> action) : base(displayText, color)
        {
            this.value = value;
            this.action = action;
        }

        protected internal override void UpdateUiText()
        {
            if (tmpro != null)
                tmpro.text = displayText;
        }
    }
}
