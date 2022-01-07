using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ModThatIsNotMod.BoneMenu
{
    public abstract class MenuElement
    {
        public string displayText { get; protected set; }
        public Color color { get; protected set; }

        internal GameObject gameObject { get; private set; }
        protected TextMeshProUGUI tmpro;
        protected Image image;

        private float customFontSize = -1;

        public UiElementType elementType { get; protected internal set; } = UiElementType.StandardButton;


        public MenuElement(string displayText, Color color)
        {
            this.displayText = displayText;
            this.color = color;
        }

        protected internal virtual void OnMainInputPressed() { }
        protected internal virtual void OnLeftInputPressed() { }
        protected internal virtual void OnRightInputPressed() { }

        /// <summary>
        /// Most types of elements should override this to include their value.
        /// </summary>
        protected internal virtual void UpdateUiText()
        {
            if (tmpro != null)
                tmpro.text = displayText;
        }

        /// <summary>
        /// The default font size is automatically picked between 0.2f and 0.1f.
        /// Only change it if you really need to.
        /// </summary>
        public void SetFontSize(float size)
        {
            customFontSize = size;
            if (tmpro != null && size != -1)
            {
                tmpro.enableAutoSizing = false;
                tmpro.fontSize = size;
            }
        }

        /// <summary>
        /// Called whenever a button for this element is created.
        /// </summary>
        internal void AssignComponentReferences(GameObject gameObject, TextMeshProUGUI tmpro, Image image)
        {
            this.gameObject = gameObject;
            this.tmpro = tmpro;
            this.image = image;

            tmpro.alignment = TextAlignmentOptions.MidlineLeft;
            tmpro.color = color;

            SetFontSize(customFontSize);
        }
    }
}
