using MelonLoader;
using System;
using System.IO;
using UnityEngine;

namespace ModThatIsNotMod
{
    public static class AssetBundles
    {
        [Obsolete("Use MelonUtils.UserDataDirectory instead.")]
        public static readonly string userDataDir = MelonUtils.UserDataDirectory;


        /// <summary>
        /// Loads the AssetBundle from UserData.
        /// </summary>
        /// <param name="file">The name/local path of the file.</param>
        /// <returns>True if the bundle isn't null.</returns>
        public static bool LoadFromUserData(string file, out AssetBundle bundle)
        {
            bundle = AssetBundle.LoadFromFile(Path.Combine(MelonUtils.UserDataDirectory, file));
            return bundle != null;
        }
    }
}
