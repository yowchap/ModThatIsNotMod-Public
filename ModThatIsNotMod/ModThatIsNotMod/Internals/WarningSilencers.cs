using HarmonyLib;
using System.Reflection;
using UnhollowerBaseLib;

namespace ModThatIsNotMod.Internals
{
    /// <summary>
    /// Makes the unhollower shut up about unsupported return types and parameters because those messages are annoying.
    /// </summary>
    internal static class WarningSilencers
    {
        public static void SilenceWarningMessages()
        {
            MethodInfo logSupportMethod = typeof(LogSupport).GetMethod("Warning", AccessTools.all);
            if (Preferences.silenceUnhollowerWarnings && logSupportMethod != null)
                Hooking.CreateHook(logSupportMethod, typeof(WarningSilencers).GetMethod("UnhollowerWarningPrefix", AccessTools.all), true);
        }
        private static bool UnhollowerWarningPrefix(string __0) => !__0.Contains("unsupported return type") && !__0.Contains("unsupported parameter");
    }
}
