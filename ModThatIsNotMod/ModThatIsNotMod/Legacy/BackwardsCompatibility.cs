namespace ModThatIsNotMod.Legacy
{
    internal static class BackwardsCompatibility
    {
        public static void Setup()
        {
            BoneworksModdingToolkit.BoneHook.PlayerHooks.Setup();
            BoneworksModdingToolkit.BoneHook.GunHooks.Setup();
            BoneworksModdingToolkit.BoneHook.MiscHooks.Setup();
            CustomItemsFramework.CustomItemsMod.Setup();
        }

        public static void OnLevelWasInitialized()
        {
            BoneworksModdingToolkit.Audio.AssignValues();
        }

        public static void OnUpdate()
        {
            BoneworksModdingToolkit.Player.OnUpdate();
        }
    }
}