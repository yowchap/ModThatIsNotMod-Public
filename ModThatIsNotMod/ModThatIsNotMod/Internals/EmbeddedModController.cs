using System;
using System.Collections.Generic;

namespace ModThatIsNotMod.Internals
{
    internal static class EmbeddedModController
    {
        private static List<EmbeddedMod> embeddedMods = new List<EmbeddedMod>();


        public static void RegisterMod(EmbeddedMod mod)
        {
            mod.OnAssemblyLoaded();
            embeddedMods.Add(mod);
        }


        public static void OnSceneWasLoaded(int buildIndex, string sceneName) { foreach (EmbeddedMod mod in embeddedMods) { try { mod.OnSceneWasLoaded(buildIndex, sceneName); } catch (Exception e) { ModConsole.Msg(ConsoleColor.Red, "[ERROR] " + e.Message + e.StackTrace, LoggingMode.MINIMAL); } } }
        public static void OnSceneWasInitialized(int buildIndex, string sceneName) { foreach (EmbeddedMod mod in embeddedMods) { try { mod.OnSceneWasInitialized(buildIndex, sceneName); } catch (Exception e) { ModConsole.Msg(ConsoleColor.Red, "[ERROR] " + e.Message + e.StackTrace, LoggingMode.MINIMAL); } } }
        public static void OnUpdate() { foreach (EmbeddedMod mod in embeddedMods) { try { mod.OnUpdate(); } catch (Exception e) { ModConsole.Msg(ConsoleColor.Red, "[ERROR] " + e.Message + e.StackTrace, LoggingMode.MINIMAL); } } }
        public static void OnLateUpdate() { foreach (EmbeddedMod mod in embeddedMods) { try { mod.OnLateUpdate(); } catch (Exception e) { ModConsole.Msg(ConsoleColor.Red, "[ERROR] " + e.Message + e.StackTrace, LoggingMode.MINIMAL); } } }
        public static void OnFixedUpdate() { foreach (EmbeddedMod mod in embeddedMods) { try { mod.OnFixedUpdate(); } catch (Exception e) { ModConsole.Msg(ConsoleColor.Red, "[ERROR] " + e.Message + e.StackTrace, LoggingMode.MINIMAL); } } }
        public static void OnApplicationQuit() { foreach (EmbeddedMod mod in embeddedMods) { try { mod.OnApplicationQuit(); } catch (Exception e) { ModConsole.Msg(ConsoleColor.Red, "[ERROR] " + e.Message + e.StackTrace, LoggingMode.MINIMAL); } } }
    }
}
