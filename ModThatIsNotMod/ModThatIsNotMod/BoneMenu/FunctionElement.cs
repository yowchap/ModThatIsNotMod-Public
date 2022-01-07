using System;
using UnityEngine;

namespace ModThatIsNotMod.BoneMenu
{
    public class FunctionElement : MenuElement
    {
        private Action action;


        public FunctionElement(string displayText, Color color, Action action) : base(displayText, color) { SetAction(action); }

        public void SetAction(Action action) => this.action = action;

        protected internal override void OnMainInputPressed() => action?.Invoke();
    }
}
