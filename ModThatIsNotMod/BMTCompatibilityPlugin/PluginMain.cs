using MelonLoader;
using System;
using System.Reflection;

namespace BackwardsCompatibilityPlugin
{
    public static class BuildInfo
    {
        public const string Name = "Backwards Compatibility Plugin"; // Name of the Mod.  (MUST BE SET)
        public const string Author = "YOWChap"; // Author of the Mod.  (Set as null if none)
        public const string Company = null; // Company that made the Mod.  (Set as null if none)
        public const string Version = "1.1.0"; // Version of the Mod.  (MUST BE SET)
        public const string DownloadLink = null; // Download Link for the Mod.  (Set as null if none)
    }

    public class PluginMain : MelonPlugin
    {
        public override void OnPreInitialization()
        {
            AppDomain.CurrentDomain.AssemblyResolve += AssemblyResolveHandler;
        }

        private static Assembly AssemblyResolveHandler(object sender, ResolveEventArgs args)
        {
            // Redirect references to BMT, CIF, and EasyMenu to MTINM
            if (args.Name.StartsWith("BoneworksModdingToolkit, Version=") || args.Name.StartsWith("CustomItemsFramework, Version=") || args.Name.StartsWith("EasyMenu, Version="))
                return Type.GetType("ModThatIsNotMod.Main, ModThatIsNotMod").Assembly;
            return null;
        }
    }
}
