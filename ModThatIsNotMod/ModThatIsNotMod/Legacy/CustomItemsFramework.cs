using System;
using UnityEngine;

// -------------------------------------------------------------------------------------------------------
// EVERYTHING HERE IS JUST REDIRECTING TO THE EQUIVALENT MTINM METHODS, IT DOESN'T DO ANYTHING ON IT'S OWN
// -------------------------------------------------------------------------------------------------------

namespace CustomItemsFramework
{
    public class CustomItemsMod
    {
        internal static void Setup()
        {
            ModThatIsNotMod.CustomItems.OnItemLoaded += CustomItems_OnItemLoaded;
        }

        private static void CustomItems_OnItemLoaded(GameObject obj) => OnItemAdded?.Invoke(obj);

        public static event Action<GameObject> OnItemAdded;
        public static GameObject GetCustomItem(string name) => ModThatIsNotMod.CustomItems.GetCustomGameObject(name);
    }
}