using System;
using System.Collections.Generic;
using UnityEngine;

namespace ModThatIsNotMod.BoneMenu
{
    public class MenuCategory : MenuElement
    {
        public List<MenuElement> elements { get; private set; } = new List<MenuElement>();
        internal MenuCategory parentCategory = null;


        internal MenuCategory(string text, Color color) : base(text, color)
        {
            elementType = UiElementType.CategoryButton;
        }

        protected internal override void OnMainInputPressed() => MenuManager.OpenCategory(this);


        /// <summary>
        /// Removes any elements with matching display text
        /// </summary>
        public void RemoveElement(string displayText)
        {
            for (int i = elements.Count - 1; i >= 0; i--)
            {
                if (elements[i].displayText == displayText)
                    elements.RemoveAt(i);
            }
        }

        public MenuCategory CreateSubCategory(string name, Color color)
        {
            MenuCategory newCategory = new MenuCategory(name, color);
            newCategory.parentCategory = this;
            elements.Add(newCategory);
            return newCategory;
        }

        /// <summary>
        /// Built in types have a dedicated function to create and add at the same time but this way if you make a new button type you can still add it.
        /// </summary>
        public void AddElement(MenuElement element)
        {
            elements.Add(element);
        }

        public void CreateFunctionElement(string displayText, Color color, Action action)
        {
            FunctionElement functionButton = new FunctionElement(displayText, color, action);
            AddElement(functionButton);
        }

        public void CreateBoolElement(string displayText, Color color, bool value, Action<bool> action)
        {
            BoolElement boolButton = new BoolElement(displayText, color, value, action);
            AddElement(boolButton);
        }

        public void CreateIntElement(string displayText, Color color, int initialValue, Action<int> action, int increment = 1, int minValue = int.MinValue, int maxValue = int.MaxValue, bool invokeOnValueChanged = false)
        {
            IntElement intButton = new IntElement(displayText, color, initialValue, action, increment, minValue, maxValue, invokeOnValueChanged);
            AddElement(intButton);
        }

        public void CreateEnumElement(string displayText, Color color, Enum value, Action<Enum> action)
        {
            EnumElement enumElement = new EnumElement(displayText, color, value, action);
            AddElement(enumElement);
        }

        public void CreateStringElement(string displayText, Color color, string value, Action<string> action)
        {
            StringElement stringElement = new StringElement(displayText, color, value, action);
            AddElement(stringElement);
        }

        public void CreateFloatElement(string displayText, Color color, float initialValue, Action<float> action, float increment = 1, float minValue = int.MinValue, float maxValue = int.MaxValue, bool invokeOnValueChanged = false)
        {
            FloatElement intButton = new FloatElement(displayText, color, initialValue, action, increment, minValue, maxValue, invokeOnValueChanged);
            AddElement(intButton);
        }

        public void CreateColorElement(string displayText, Color color, Action<Color> action)
        {
            ColorElement colorElement = new ColorElement(displayText, color, action);
            AddElement(colorElement);
        }
    }
}
