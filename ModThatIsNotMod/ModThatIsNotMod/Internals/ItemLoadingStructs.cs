using StressLevelZero.Data;
using System.Collections.Generic;
using UnityEngine;

namespace ModThatIsNotMod.Internals
{
    internal struct LoadedMelonData
    {
        public string filePath;
        public string fileHash;
        public string author;
        public AssetBundle bundle;
        public int itemsLoaded;
        public List<LoadedItemData> loadedItems;
        public string errorMessage;

        public LoadedMelonData(int itemsLoaded, List<LoadedItemData> loadedItems)
        {
            filePath = "MISSING FILENAME";
            fileHash = "MISSING HASH";
            author = "UNKNOWN";
            bundle = null;
            this.itemsLoaded = itemsLoaded;
            this.loadedItems = loadedItems;
            errorMessage = "UNKNOWN ERROR";
        }
    }

    internal struct LoadedItemData
    {
        public string itemName;
        public CategoryFilters category;
        public bool hidden;

        public LoadedItemData(string itemName, CategoryFilters category, bool hidden)
        {
            this.itemName = itemName;
            this.category = category;
            this.hidden = hidden;
        }
    }
}
