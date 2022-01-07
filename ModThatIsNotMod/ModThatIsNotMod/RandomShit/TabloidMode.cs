using ModThatIsNotMod.Internals;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ModThatIsNotMod.RandomShit
{
    /// <summary>
    /// Yes.
    /// </summary>
    internal static class TabloidMode
    {
        /// <summary>
        /// Yes.
        /// </summary>
        public static void DoTabloidStuff()
        {
            if (!Preferences.tabloidMode.value)
                return;

            Canvas canvas = CreateCanvas();

            CreateText(canvas, "BONES WORK MOD", Color.red, 45, new Vector2(-569, 214), 18f);
            CreateText(canvas, "THIS MOD DOESN'T REALLY EXIST", Color.red, 50, new Vector2(225, 133), 0);
            CreateText(canvas, "OR DOES IT", Color.red, 30, new Vector2(812, 61), 0);
            CreateText(canvas, "MELONLOADER BETA v2.5", Color.red, 45, new Vector2(-855, 515), 0);
            CreateText(canvas, "HELLO WORLD", Color.red, 27.5f, new Vector2(-156, 506), 0);
            CreateText(canvas, "WHAT YOU SEE HERE IS NOT BONEWORKS", Color.red, 43, new Vector2(135, 510), 0);
            CreateText(canvas, "X", Color.red, 220, new Vector2(225, 400), 20);
            CreateText(canvas, "SO HOW SMALL CAN THIS GUI TEXT REALLY GO?", Color.red, 12.5f, new Vector2(437, 280), 0);
            CreateText(canvas, "JAVELIN MOD V1.0", Color.red, 60, new Vector2(-880, 71), 0);
            CreateText(canvas, "WHO ARE YOU???", Color.red, 30, new Vector2(341, -15), 0);
            CreateText(canvas, "YOUR AD HERE FOR 4.99/MONTH", Color.red, 60, new Vector2(-810, -185), 16);
            CreateText(canvas, "BONETOME.COM FOR COOL NEW MODS", Color.red, 60, new Vector2(-150, -140), 0);
            CreateText(canvas, "NOT A HALF LIFE GAME", Color.red, 45, new Vector2(490, -125), 0);
            CreateText(canvas, "WHO AM I?????", Color.red, 30, new Vector2(-820, -325), 0);
            CreateText(canvas, "STRENGTH MOD: UNIVERSAL", Color.red, 40, new Vector2(-370, -280), 0);
            CreateText(canvas, "NO, THIS ISN'T PROJECT 4", Color.red, 45, new Vector2(-5, -270), 0);
            CreateText(canvas, "DAY 152192: CHROMIUM FINALLY FINISHED A MOD", Color.red, 25, new Vector2(460, -225), 0);
            CreateText(canvas, "SHIRTLESS BRANDON", Color.red, 40, new Vector2(750, -300), 0);
            CreateText(canvas, "MOD", Color.red, 40, new Vector2(976, -337), 0);
            CreateText(canvas, "AND WHY IS THIS CAKE ON FIRE???", Color.red, 25, new Vector2(185, -390), 0);
            CreateText(canvas, "IS ANYTHING EVEN REAL?", Color.red, 45, new Vector2(-855, -470), 0);
            CreateText(canvas, "UNOFFICIAL MULTIPLAYER MOD", Color.red, 45, new Vector2(-855, -515), 0);
            CreateText(canvas, "NOT ALL WEAPONS/ITEMS ARE OFFICIAL", Color.red, 45, new Vector2(160, -515), 0);
            CreateText(canvas, "HOW DID I TURN THE TEXT GREEN", Color.green, 45, new Vector2(-355, -530), 0);
        }

        private static Canvas CreateCanvas()
        {
            GameObject canvasGO = new GameObject("TabloidMomentCanvas");
            Canvas canvas = canvasGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            CanvasScaler scaler = canvasGO.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.matchWidthOrHeight = 1;

            return canvas;
        }

        private static void CreateText(Canvas canvas, string text, Color color, float fontSize, Vector2 position, float zRotation)
        {
            GameObject textGO = new GameObject("TabloidMomentText");
            textGO.transform.parent = canvas.transform;
            textGO.transform.localPosition = position;
            textGO.transform.localRotation = Quaternion.Euler(0, 0, zRotation);

            TextMeshProUGUI tmpro = textGO.AddComponent<TextMeshProUGUI>();
            tmpro.text = text;
            tmpro.color = color;
            tmpro.fontSize = fontSize;
            tmpro.overflowMode = TextOverflowModes.Overflow;
            tmpro.enableWordWrapping = false;
        }
    }
}
