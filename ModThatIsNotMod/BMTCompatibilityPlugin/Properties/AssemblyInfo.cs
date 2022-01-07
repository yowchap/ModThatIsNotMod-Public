using MelonLoader;
using System.Reflection;
using System.Resources;
using System.Runtime.InteropServices;

[assembly: AssemblyTitle(BackwardsCompatibilityPlugin.BuildInfo.Name)]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany(BackwardsCompatibilityPlugin.BuildInfo.Company)]
[assembly: AssemblyProduct(BackwardsCompatibilityPlugin.BuildInfo.Name)]
[assembly: AssemblyCopyright("Created by " + BackwardsCompatibilityPlugin.BuildInfo.Author)]
[assembly: AssemblyTrademark(BackwardsCompatibilityPlugin.BuildInfo.Company)]
[assembly: AssemblyCulture("")]
[assembly: ComVisible(false)]
//[assembly: Guid("")]
[assembly: AssemblyVersion(BackwardsCompatibilityPlugin.BuildInfo.Version)]
[assembly: AssemblyFileVersion(BackwardsCompatibilityPlugin.BuildInfo.Version)]
[assembly: NeutralResourcesLanguage("en")]
[assembly: MelonInfo(typeof(BackwardsCompatibilityPlugin.PluginMain), BackwardsCompatibilityPlugin.BuildInfo.Name, BackwardsCompatibilityPlugin.BuildInfo.Version, BackwardsCompatibilityPlugin.BuildInfo.Author, BackwardsCompatibilityPlugin.BuildInfo.DownloadLink)]


// Create and Setup a MelonModGame to mark a Mod as Universal or Compatible with specific Games.
// If no MelonModGameAttribute is found or any of the Values for any MelonModGame on the Mod is null or empty it will be assumed the Mod is Universal.
// Values for MelonModGame can be found in the Game's app.info file or printed at the top of every log directly beneath the Unity version.
[assembly: MelonGame("Stress Level Zero", "BONEWORKS")]