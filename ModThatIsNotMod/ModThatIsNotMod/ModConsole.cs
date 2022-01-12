using MelonLoader;
using ModThatIsNotMod.Internals;
using System;

internal static class ModConsole
{
    public static MelonLogger.Instance logger { get; private set; } = new MelonLogger.Instance("MTINM");
    public static MelonLogger.Instance loggerDebug { get; private set; } = new MelonLogger.Instance("MTINM_DEBUG");

    public static void Msg(object obj, LoggingMode minLoggingMode = LoggingMode.NORMAL)
    {
        MelonLogger.Instance instance = minLoggingMode == LoggingMode.DEBUG ? loggerDebug : logger;
        if (Preferences.loggingMode >= minLoggingMode)
            instance.Msg(obj.ToString());
    }

    public static void Msg(string txt, LoggingMode minLoggingMode = LoggingMode.NORMAL)
    {
        MelonLogger.Instance instance = minLoggingMode == LoggingMode.DEBUG ? loggerDebug : logger;
        if (Preferences.loggingMode >= minLoggingMode)
            instance.Msg(txt);
    }

    public static void Msg(ConsoleColor txtcolor, object obj, LoggingMode minLoggingMode = LoggingMode.NORMAL)
    {
        MelonLogger.Instance instance = minLoggingMode == LoggingMode.DEBUG ? loggerDebug : logger;
        if (Preferences.loggingMode >= minLoggingMode)
            instance.Msg(txtcolor, obj.ToString());
    }

    public static void Msg(ConsoleColor txtcolor, string txt, LoggingMode minLoggingMode = LoggingMode.NORMAL)
    {
        MelonLogger.Instance instance = minLoggingMode == LoggingMode.DEBUG ? loggerDebug : logger;
        if (Preferences.loggingMode >= minLoggingMode)
            instance.Msg(txtcolor, txt);
    }

    public static void Msg(string txt, LoggingMode minLoggingMode = LoggingMode.NORMAL, params object[] args)
    {
        MelonLogger.Instance instance = minLoggingMode == LoggingMode.DEBUG ? loggerDebug : logger;
        if (Preferences.loggingMode >= minLoggingMode)
            instance.Msg(txt, args);
    }

    public static void Msg(ConsoleColor txtcolor, string txt, LoggingMode minLoggingMode = LoggingMode.NORMAL, params object[] args)
    {
        MelonLogger.Instance instance = minLoggingMode == LoggingMode.DEBUG ? loggerDebug : logger;
        if (Preferences.loggingMode >= minLoggingMode)
            instance.Msg(txtcolor, txt, args);
    }
}

internal enum LoggingMode
{
    MINIMAL,
    NORMAL,
    VERBOSE,
    DEBUG
}
