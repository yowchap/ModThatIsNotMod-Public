using StressLevelZero.Data;
using StressLevelZero.Pool;
using System.Collections.Generic;

namespace ModThatIsNotMod.Internals
{
    /// <summary>
    /// This data will be set in the Custom Items SDK.
    /// </summary>
    internal class ItemSettings
    {
        public string author = "";
        public List<Item> items = new List<Item>();

        public class Item
        {
            public string name = "";
            public CategoryFilters category = CategoryFilters.PROPS;
            public PoolMode poolMode = PoolMode.REUSEOLDEST;
            public int pooledAmount = 0;
            public bool hideInMenu = false;
        }

        /// <summary>
        /// Searches for an item with the given name.
        /// </summary>
        /// <returns>True if the item is found.</returns>
        public bool TryGetItem(string itemName, out Item item)
        {
            item = null;
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i].name == itemName)
                {
                    item = items[i];
                    return true;
                }
            }
            return false;
        }
    }
}
