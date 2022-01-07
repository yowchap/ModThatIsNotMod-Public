using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// -------------------------------------------------------------------------------------------------------
// EVERYTHING HERE IS JUST REDIRECTING TO THE EQUIVALENT MTINM METHODS, IT DOESN'T DO ANYTHING ON IT'S OWN
// -------------------------------------------------------------------------------------------------------

namespace EasyMenu
{
    public static class Interfaces
    {
        public static MenuInterface AddNewInterface(string title, Color color)
        {
            return new MenuInterface(title, color);
        }

        public static MenuInterface AddNewInterface(string title, Color color, string subtitleText)
        {
            return new MenuInterface(title, color, subtitleText);
        }

        public static MenuInterface CreateCategoryInterface(string title, Color color)
        {
            return new MenuInterface(title, color, randomBoolJustIgnoreThisOk: false);
        }
    }

    public class MenuInterface
    {
        public ModThatIsNotMod.BoneMenu.MenuCategory menuCategory;


        public string text { get { return menuCategory.displayText; } }

        public Color color { get { return menuCategory.color; } }

        public List<MenuElement> menuElements { get; } = new List<MenuElement>();

        public MenuInterface(string title, Color color, string subtitleText = "")
        {
            menuCategory = ModThatIsNotMod.BoneMenu.MenuManager.CreateCategory(title, color);
        }

        public MenuInterface(string title, Color color, bool randomBoolJustIgnoreThisOk)
        {
            menuCategory = new ModThatIsNotMod.BoneMenu.MenuCategory(title, color);
        }

        public GameObject GetMenuObject()
        {
            return null;
        }

        public void SetMenuObject(GameObject gameObject)
        {

        }

        public string GetSubtitleText()
        {
            return "";
        }

        public void SetSubtitleText(string text)
        {

        }

        public void SetSubtitleObject(GameObject gameObject)
        {

        }

        public GameObject GetSubtitleObject()
        {
            return null;
        }

        public void RenderMenu()
        {

        }

        public void OpenMenu()
        {

        }

        public void CloseMenu()
        {

        }

        public IntElement CreateIntElement(string text, Color color, int minValue, int maxValue, int increment, int startValue, string units = "", string subtitleText = "")
        {
            IntElement intElement = new IntElement(text, color, minValue, maxValue, increment, startValue, units, subtitleText);
            menuCategory.AddElement(intElement.intElement);
            menuElements.Add(intElement);
            return intElement;
        }

        public IntElement CreateIntElement(string text, Color color, int minValue, int maxValue, int increment, int startValue, Action<int> onValueChanged, string units = "", string subtitleText = "")
        {
            IntElement intElement = new IntElement(text, color, minValue, maxValue, increment, startValue, onValueChanged, units, subtitleText);
            menuCategory.AddElement(intElement.intElement);
            this.menuElements.Add(intElement);
            return intElement;
        }

        public IntListElement CreateIntListElement(string text, Color color, List<int> options, int startValue, string units = "", string subtitleText = "")
        {
            return new IntListElement(text, color, options, startValue, units, subtitleText);
        }

        public IntListElement CreateIntListElement(string text, Color color, List<int> options, int startValue, Action<int> onValueChanged, string units = "", string subtitleText = "")
        {
            return new IntListElement(text, color, options, startValue, onValueChanged, units, subtitleText);
        }

        public FloatElement CreateFloatElement(string text, Color color, float minValue, float maxValue, float increment, float startValue, int decimalRestriction, string units = "", string subtitleText = "")
        {
            FloatElement floatElement = new FloatElement(text, color, minValue, maxValue, increment, startValue, decimalRestriction, units, subtitleText);
            menuCategory.AddElement(floatElement.floatElement);
            this.menuElements.Add(floatElement);
            return floatElement;
        }

        public FloatElement CreateFloatElement(string text, Color color, float minValue, float maxValue, float increment, float startValue, int decimalRestriction, Action<float> onValueChanged, string units = "", string subtitleText = "")
        {
            FloatElement floatElement = new FloatElement(text, color, minValue, maxValue, increment, startValue, decimalRestriction, onValueChanged, units, subtitleText);
            menuCategory.AddElement(floatElement.floatElement);
            this.menuElements.Add(floatElement);
            return floatElement;
        }

        public FloatListElement CreateFloatListElement(string text, Color color, List<float> options, float startValue, string units = "", string subtitleText = "")
        {
            return new FloatListElement(text, color, options, startValue, units, subtitleText);
        }

        public FloatListElement CreateFloatListElement(string text, Color color, List<float> options, float startValue, Action<float> onValueChanged, string units = "", string subtitleText = "")
        {
            return new FloatListElement(text, color, options, startValue, onValueChanged, units, subtitleText);
        }

        public BoolElement CreateBoolElement(string text, Color color, bool startValue, string subtitleText = "")
        {
            BoolElement boolElement = new BoolElement(text, color, startValue, subtitleText);
            menuCategory.AddElement(boolElement.boolElement);
            this.menuElements.Add(boolElement);
            return boolElement;
        }

        public BoolElement CreateBoolElement(string text, Color color, bool startValue, Action<bool> onValueChanged, string subtitleText = "")
        {
            BoolElement boolElement = new BoolElement(text, color, startValue, onValueChanged, subtitleText);
            menuCategory.AddElement(boolElement.boolElement);
            this.menuElements.Add(boolElement);
            return boolElement;
        }

        public StringElement CreateStringElement(string text, Color color, List<string> options, string startValue = "", string subtitleText = "")
        {
            return new StringElement(text, color, options, startValue, subtitleText);
        }

        public StringElement CreateStringElement(string text, Color color, List<string> options, Action<string> onValueChanged, string startValue = "", string subtitleText = "")
        {
            return new StringElement(text, color, options, startValue, onValueChanged, subtitleText);
        }

        public FunctionElement CreateFunctionElement(string text, Color color, Action left, Action right, Action trigger)
        {
            FunctionElement functionElement = new FunctionElement(text, color, left, right, trigger, "");
            menuCategory.AddElement(functionElement.functionElement);
            this.menuElements.Add(functionElement);
            return functionElement;
        }

        public FunctionElement CreateFunctionElement(string text, Color color, Action left, Action right, Action trigger, string subtitleText)
        {
            FunctionElement functionElement = new FunctionElement(text, color, left, right, trigger, subtitleText);
            menuCategory.AddElement(functionElement.functionElement);
            this.menuElements.Add(functionElement);
            return functionElement;
        }

        public CategoryElement CreateCategoryElement(string text, Color color, MenuInterface categoryInterface, string subtitleText = "")
        {
            CategoryElement categoryElement = new CategoryElement(text, color, categoryInterface, this, subtitleText);
            this.menuElements.Add(categoryElement);
            return categoryElement;
        }

        public CategoryElement CreateCategoryElement(string text, Color color, MenuInterface categoryInterface, Action onOpen, string subtitleText = "")
        {
            CategoryElement categoryElement = new CategoryElement(text, color, categoryInterface, this, onOpen, subtitleText);
            this.menuElements.Add(categoryElement);
            return categoryElement;
        }

        private void SetElementInterface(MenuElement element)
        {

        }

        public void RemoveMenuElement(MenuElement element)
        {

        }
    }

    // Maybe completed types

    public abstract class MenuElement
    {
        public MenuElement(string text, Color color, string subtitleText = "")
        {

        }

        public void SetTextObject(GameObject gameObject)
        {

        }

        public GameObject GetTextObject()
        {
            return null;
        }

        public void SetSubtitleObject(GameObject gameObject)
        {

        }

        public GameObject GetSubtitleObject()
        {
            return null;
        }

        public void SetText(string text)
        {

        }

        public string GetText()
        {
            return "";
        }

        public void SetSubtitleText(string text)
        {

        }

        public string GetSubtitleText()
        {
            return "";
        }

        public void SetColor(Color color)
        {

        }

        public Color GetColor()
        {
            return Color.white;
        }

        public abstract TextMeshPro Render(GameObject gameObject);

        public virtual void OnTrigger()
        {

        }

        public virtual void OnRight()
        {

        }

        public virtual void OnLeft()
        {

        }

        public void OnSelect()
        {

        }
    }

    public class CategoryElement : MenuElement
    {
        public ModThatIsNotMod.BoneMenu.MenuCategory menuCategory;


        public CategoryElement(string text, Color color, MenuInterface category, MenuInterface parentInterface, string subtitleText = "") : base(text, color, subtitleText)
        {
            //menuCategory = category.menuCategory.CreateSubCategory(text, color);
            parentInterface.menuCategory.AddElement(category.menuCategory);
            menuCategory = category.menuCategory;
            menuCategory.parentCategory = parentInterface.menuCategory;
        }

        public CategoryElement(string text, Color color, MenuInterface category, MenuInterface parentInterface, Action onOpen, string subtitleText = "") : base(text, color, subtitleText)
        {
            //menuCategory = category.menuCategory.CreateSubCategory(text, color);
            parentInterface.menuCategory.AddElement(category.menuCategory);
            menuCategory = category.menuCategory;
            menuCategory.parentCategory = parentInterface.menuCategory;
        }

        public MenuInterface GetInterface()
        {
            return null;
        }

        public override void OnTrigger()
        {

        }

        public override TextMeshPro Render(GameObject gameObject)
        {
            return null;
        }
    }

    public class BoolElement : MenuElement
    {
        public ModThatIsNotMod.BoneMenu.BoolElement boolElement;

        public BoolElement(string text, Color color, bool startValue, string subtitleText = "") : base(text, color, subtitleText)
        {
            boolElement = new ModThatIsNotMod.BoneMenu.BoolElement(text, color, startValue, null);
        }

        public BoolElement(string text, Color color, bool startValue, Action<bool> onValueChanged, string subtitleText = "") : base(text, color, subtitleText)
        {
            boolElement = new ModThatIsNotMod.BoneMenu.BoolElement(text, color, startValue, onValueChanged);
        }

        public override TextMeshPro Render(GameObject gameObject)
        {
            return null;
        }

        public bool GetValue()
        {
            return boolElement.GetValue();
        }

        public void SetValue(bool value)
        {
            boolElement.SetValue(value);
        }

        public override void OnLeft()
        {

        }

        public override void OnRight()
        {

        }
    }

    public class FloatElement : MenuElement
    {
        public ModThatIsNotMod.BoneMenu.FloatElement floatElement;


        public FloatElement(string text, Color color, float minValue, float maxValue, float increment, float startValue, int decimalRestriction, string units = "", string subtitleText = "") : base(text, color, subtitleText)
        {
            floatElement = new ModThatIsNotMod.BoneMenu.FloatElement(text, color, startValue, null, increment, minValue, maxValue, true);
        }

        public FloatElement(string text, Color color, float minValue, float maxValue, float increment, float startValue, int decimalRestriction, Action<float> onValueChanged, string units = "", string subtitleText = "") : base(text, color, subtitleText)
        {
            floatElement = new ModThatIsNotMod.BoneMenu.FloatElement(text, color, startValue, onValueChanged, increment, minValue, maxValue, true);
        }

        public void SetUnits(string units)
        {

        }

        public string GetUnits()
        {
            return "";
        }

        public void SetValue(float value)
        {
            floatElement.SetValue(value);
        }

        public float GetValue()
        {
            return floatElement.GetValue();
        }

        public override TextMeshPro Render(GameObject gameObject)
        {
            return null;
        }

        public override void OnLeft()
        {

        }

        public override void OnRight()
        {

        }
    }

    public class FunctionElement : MenuElement
    {
        public ModThatIsNotMod.BoneMenu.FunctionElement functionElement;


        public FunctionElement(string text, Color color, Action left, Action right, Action trigger, string subtitleText = "") : base(text, color, subtitleText)
        {
            functionElement = new ModThatIsNotMod.BoneMenu.FunctionElement(text, color, trigger);
        }

        public override void OnLeft()
        {

        }

        public override void OnRight()
        {

        }

        public override void OnTrigger()
        {

        }

        public override TextMeshPro Render(GameObject gameObject)
        {
            return null;
        }
    }

    public class IntElement : MenuElement
    {
        public ModThatIsNotMod.BoneMenu.IntElement intElement;


        public IntElement(string text, Color color, int minValue, int maxValue, int increment, int startValue, string units = "", string subtitleText = "") : base(text, color, subtitleText)
        {
            intElement = new ModThatIsNotMod.BoneMenu.IntElement(text, color, startValue, null, increment, minValue, maxValue, true);
        }

        public IntElement(string text, Color color, int minValue, int maxValue, int increment, int startValue, Action<int> onValueChanged, string units = "", string subtitleText = "") : base(text, color, subtitleText)
        {
            intElement = new ModThatIsNotMod.BoneMenu.IntElement(text, color, startValue, onValueChanged, increment, minValue, maxValue, true);
        }

        public void SetValue(int value)
        {
            intElement.SetValue(value);
        }

        public int GetValue()
        {
            return intElement.GetValue();
        }

        public void SetUnits(string units)
        {

        }

        public string GetUnits()
        {
            return "";
        }

        public override TextMeshPro Render(GameObject gameObject)
        {
            return null;
        }

        public override void OnLeft()
        {

        }

        public override void OnRight()
        {

        }
    }

    // Definitely not completed types

    public class StringElement : MenuElement
    {
        public StringElement(string text, Color color, List<string> options, string startValue, string subtitleText = "") : base(text, color, subtitleText)
        {

        }

        public StringElement(string text, Color color, List<string> options, string startValue, Action<string> onValueChanged, string subtitleText = "") : base(text, color, subtitleText)
        {

        }

        public void SetOptions(List<string> options)
        {

        }

        public List<string> GetOptions()
        {
            return null;
        }

        public void SetValue(string value)
        {

        }

        public override void OnLeft()
        {

        }

        public override void OnRight()
        {

        }

        public string GetValue()
        {
            return "";
        }

        public override TextMeshPro Render(GameObject gameObject)
        {
            return null;
        }
    }

    public class IntListElement : MenuElement
    {
        public IntListElement(string text, Color color, List<int> options, int startValue, string units = "", string subtitleText = "") : base(text, color, subtitleText)
        {

        }

        public IntListElement(string text, Color color, List<int> options, int startValue, Action<int> onValueChanged, string units = "", string subtitleText = "") : base(text, color, subtitleText)
        {

        }

        public void SetUnits(string units)
        {

        }

        public string GetUnits()
        {
            return "";
        }

        public void SetValue(int value)
        {

        }

        public void SetOptions(List<int> options)
        {

        }

        public List<int> GetOptions()
        {
            return null;
        }

        public float GetValue()
        {
            return 0;
        }

        public override void OnLeft()
        {

        }

        public override void OnRight()
        {

        }

        public override TextMeshPro Render(GameObject gameObject)
        {
            return null;
        }
    }

    public class FloatListElement : MenuElement
    {
        public FloatListElement(string text, Color color, List<float> options, float startValue, string units = "", string subtitleText = "") : base(text, color, subtitleText)
        {

        }

        public FloatListElement(string text, Color color, List<float> options, float startValue, Action<float> onValueChanged, string units = "", string subtitleText = "") : base(text, color, subtitleText)
        {

        }

        public void SetUnits(string units)
        {

        }

        public string GetUnits()
        {
            return "";
        }

        public void SetValue(float value)
        {

        }

        public void SetOptions(List<float> options)
        {

        }

        public List<float> GetOptions()
        {
            return null;
        }

        public float GetValue()
        {
            return 0;
        }

        public override void OnLeft()
        {

        }

        public override void OnRight()
        {

        }

        public override TextMeshPro Render(GameObject gameObject)
        {
            return null;
        }
    }
}
