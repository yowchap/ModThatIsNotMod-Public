using MelonLoader;
using System.Reflection;
using System.Resources;
using System.Runtime.InteropServices;

[assembly: AssemblyTitle(ModThatIsNotMod.BuildInfo.Name)]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany(ModThatIsNotMod.BuildInfo.Company)]
[assembly: AssemblyProduct(ModThatIsNotMod.BuildInfo.Name)]
[assembly: AssemblyCopyright("Created by " + ModThatIsNotMod.BuildInfo.Author)]
[assembly: AssemblyTrademark(ModThatIsNotMod.BuildInfo.Company)]
[assembly: AssemblyCulture("")]
[assembly: ComVisible(false)]
//[assembly: Guid("")]
[assembly: AssemblyVersion(ModThatIsNotMod.BuildInfo.Version)]
[assembly: AssemblyFileVersion(ModThatIsNotMod.BuildInfo.Version)]
[assembly: NeutralResourcesLanguage("en")]
[assembly: MelonInfo(typeof(ModThatIsNotMod.Main), ModThatIsNotMod.BuildInfo.Name, ModThatIsNotMod.BuildInfo.Version, ModThatIsNotMod.BuildInfo.Author, ModThatIsNotMod.BuildInfo.DownloadLink)]


// Create and Setup a MelonModGame to mark a Mod as Universal or Compatible with specific Games.
// If no MelonModGameAttribute is found or any of the Values for any MelonModGame on the Mod is null or empty it will be assumed the Mod is Universal.
// Values for MelonModGame can be found in the Game's app.info file or printed at the top of every log directly beneath the Unity version.
[assembly: MelonGame("Stress Level Zero", "BONEWORKS")]
[assembly: MelonIncompatibleAssemblies("BoneworksModdingToolkit", "CustomItemsFramework", "MagEjectButton", "AutomaticSpawnGun", "FireModes")]
[assembly: MelonPriorityAttribute(-1000000)]