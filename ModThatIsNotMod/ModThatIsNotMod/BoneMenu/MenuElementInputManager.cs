using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ModThatIsNotMod.BoneMenu
{
    internal class MenuElementInputManager : MonoBehaviour
    {
        public MenuElementInputManager(IntPtr intPtr) : base(intPtr) { }


        // Ok so basically, don't mess with these variables because it works and that's all that matters.
        // I wasn't able to use TMP in an asset bundle when this was made so I have to manually set up pretty much everything.

        private static readonly Vector4 categoryButtonTextMargins = new Vector4(0, 0, 0, 0);
        private static readonly Vector4 standardButtonTextMargins = new Vector4(0, 0, -0.1f, 0);
        private static readonly Vector4 arrowButtonTextMargins = new Vector4(0, 0, 0.6f, 0);

        private static readonly string categoryIconName = "CategoryIcon";

        private static readonly string standardButtonName = "StandardButton";
        private static readonly string arrowsButtonName = "ArrowsButton";

        private static readonly string mainInputColName = "Col_ButtonMain";
        private static readonly string leftInputColName = "Col_ButtonLeft";
        private static readonly string rightInputColName = "Col_ButtonRight";


        private TextMeshProUGUI tmpro;

        private MenuElement element;


        public void SetElement(MenuElement element)
        {
            if (tmpro == null)
                GetTmpro();

            this.element = element;

            SetupCollidersAndText(element.elementType);

            element.AssignComponentReferences(gameObject, tmpro, gameObject.GetComponentInChildren<Image>());
            element.UpdateUiText();
        }

        private void Start()
        {
            GetTmpro();

            foreach (BoxCollider col in gameObject.GetComponentsInChildren<BoxCollider>(true))
                col.gameObject.AddComponent<MenuElementInputReceiver>();
        }

        private void GetTmpro()
        {
            tmpro = gameObject.GetComponentInChildren<TextMeshProUGUI>();
            tmpro.alignment = TextAlignmentOptions.MidlineLeft;
        }

        public void OnInputPressed(string inputName)
        {
            MenuManager.PlayButtonPressSound();
            MenuManager.SendHapticFeedback();

            if (inputName == mainInputColName)
                element.OnMainInputPressed();
            else if (inputName == leftInputColName)
                element.OnLeftInputPressed();
            else if (inputName == rightInputColName)
                element.OnRightInputPressed();
        }

        /// <summary>
        /// I'm not even gonna try to explain how this works because I didn't bother to comment when I was making it and now I don't really remember.
        /// Basically just don't mess with it.
        /// </summary>
        private void SetupCollidersAndText(UiElementType elementType)
        {
            Transform[] transforms = transform.GetComponentsInChildren<Transform>(true);
            switch (elementType)
            {
                case UiElementType.StandardButton:
                    transforms.Where(x => x.name == standardButtonName).First().gameObject.SetActive(true);
                    tmpro.margin = standardButtonTextMargins;
                    break;
                case UiElementType.ArrowsButton:
                    transforms.Where(x => x.name == arrowsButtonName).First().gameObject.SetActive(true);
                    tmpro.margin = arrowButtonTextMargins;
                    break;
                case UiElementType.BackButton:
                    transforms.Where(x => x.name == standardButtonName).First().gameObject.SetActive(true);
                    element.SetFontSize(0);
                    break;
                case UiElementType.CategoryButton:
                    transforms.Where(x => x.name == standardButtonName).First().gameObject.SetActive(true);
                    transforms.Where(x => x.name == categoryIconName).First().gameObject.SetActive(true);
                    tmpro.margin = categoryButtonTextMargins;
                    break;
            }
        }
    }

    // Used for setting up the colliders and text margins for different button types iirc
    public enum UiElementType
    {
        StandardButton,
        ArrowsButton,
        BackButton,
        CategoryButton
    }
}
