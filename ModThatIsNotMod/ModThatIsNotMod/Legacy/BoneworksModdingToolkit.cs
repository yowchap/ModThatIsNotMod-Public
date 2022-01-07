using MelonLoader;
using StressLevelZero.Data;
using StressLevelZero.Interaction;
using StressLevelZero.Pool;
using StressLevelZero.Props.Weapons;
using StressLevelZero.Rig;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

// -------------------------------------------------------------------------------------------------------
// EVERYTHING HERE IS JUST REDIRECTING TO THE EQUIVALENT MTINM METHODS, IT DOESN'T DO ANYTHING ON IT'S OWN
// -------------------------------------------------------------------------------------------------------

#pragma warning disable CS0618
namespace BoneworksModdingToolkit
{
    public static class Player
    {
        internal static void OnUpdate()
        {
            leftHand = ModThatIsNotMod.Player.leftHand;
            rightHand = ModThatIsNotMod.Player.rightHand;
        }

        [Obsolete("Use ModThatIsNotMod.Player.leftHand instead.")]
        public static Hand leftHand;

        [Obsolete("Use ModThatIsNotMod.Player.rightHand instead.")]
        public static Hand rightHand;

        [Obsolete("Use ModThatIsNotMod.Player.handsExist instead.")]
        public static bool handsExist => ModThatIsNotMod.Player.handsExist;

        [Obsolete("Use ModThatIsNotMod.Player.GetPlayerHead() instead.")]
        public static GameObject FindPlayer() => ModThatIsNotMod.Player.GetPlayerHead();

        [Obsolete("Use ModThatIsNotMod.Player.GetRigManager() instead.")]
        public static GameObject FindRigManager() => ModThatIsNotMod.Player.GetRigManager();

        [Obsolete("Use ModThatIsNotMod.Player.leftHand and ModThatIsNotMod.Player.rightHand instead.")]
        public static void TryGetHands(out Hand leftHand, out Hand rightHand) { ModThatIsNotMod.Player.FindObjectReferences(); leftHand = ModThatIsNotMod.Player.leftHand; rightHand = ModThatIsNotMod.Player.rightHand; }

        [Obsolete("Use ModThatIsNotMod.Player.leftController and ModThatIsNotMod.Player.rightController instead.")]
        public static void TryGetControllers(out Controller leftController, out Controller rightController) { ModThatIsNotMod.Player.FindObjectReferences(); leftController = ModThatIsNotMod.Player.leftController; rightController = ModThatIsNotMod.Player.rightController; }

        [Obsolete("Use ModThatIsNotMod.Player.GetGunInHand() instead.")]
        public static Gun GetGunInHand(Hand hand) => ModThatIsNotMod.Player.GetGunInHand(hand);

        [Obsolete("Use ModThatIsNotMod.Player.GetComponentInHand<SpawnGun>() instead.")]
        public static SpawnGun GetSpawnGunInHand(Hand hand) => ModThatIsNotMod.Player.GetComponentInHand<SpawnGun>(hand);

        [Obsolete("Use ModThatIsNotMod.Player.GetObjectInHand() instead.")]
        public static GameObject GetObjectInHand(Hand hand) => ModThatIsNotMod.Player.GetObjectInHand(hand);
    }

    public static class SpawnMenu
    {
        [Obsolete("Use ModThatIsNotMod.CustomItems.customItemsExist instead.")]
        public static bool customItemsExist => ModThatIsNotMod.CustomItems.customItemsExist;

        [Obsolete("Use ModThatIsNotMod.SpawnMenu.AddItem() instead.")]
        public static void AddItem(GameObject prefab, string title, int poolAmount, CategoryFilters category)
        {
            SpawnableObject spawnable = ModThatIsNotMod.CustomItems.CreateSpawnableObject(prefab, title, category, PoolMode.REUSEOLDEST, poolAmount);
            ModThatIsNotMod.SpawnMenu.AddItem(spawnable);
        }

        [Obsolete("Use ModThatIsNotMod.SpawnMenu.AddItem() instead.")]
        public static void AddItem(GameObject prefab, string title, int poolAmount, CategoryFilters category, bool replaceExistingItem)
        {
            if (replaceExistingItem)
                RemoveItem(title);
            AddItem(prefab, title, poolAmount, category);
        }

        [Obsolete("Use ModThatIsNotMod.SpawnMenu.AddItem() instead.")]
        public static void AddSpawnable(SpawnableObject spawnable) => ModThatIsNotMod.SpawnMenu.AddItem(spawnable);

        [Obsolete("Use ModThatIsNotMod.SpawnMenu.RemoveItem() instead.")]
        public static void RemoveItem(string title) => ModThatIsNotMod.SpawnMenu.RemoveItem(title);
    }

    public static class Shaders
    {
        [Obsolete("Use ModThatIsNotMod.CustomItems.dummyShaderReplacements instead.")]
        public static Dictionary<string, string> dummyShaderReplacements => ModThatIsNotMod.CustomItems.dummyShaderReplacements;

        [Obsolete("Use ModThatIsNotMod.CustomItems.FixObjectShaders() instead.")]
        public static void ReplaceDummyShaders(GameObject obj) => ModThatIsNotMod.CustomItems.FixObjectShaders(obj);

        [Obsolete("Use ModThatIsNotMod.CustomItems.FixObjectShaders() instead.")]
        public static void ReplaceWithValveVRStandard(GameObject obj) => ModThatIsNotMod.CustomItems.FixObjectShaders(obj);

        [Obsolete("Use ModThatIsNotMod.CustomItems.AddDummyShaderReplacement() instead.")]
        public static void AddDummyShaderReplacement(string dummyName, string inGameName) => ModThatIsNotMod.CustomItems.AddDummyShaderReplacement(dummyName, inGameName);
    }

    public static class SimpleFixes
    {
        [Obsolete("Use ModThatIsNotMod.CustomItems.FixObjectShaders() instead.")]
        public static void FixObjectShader(GameObject obj) => ModThatIsNotMod.CustomItems.FixObjectShaders(obj);
    }

    public static class Audio
    {
        internal static void AssignValues()
        {
            musicMixer = ModThatIsNotMod.Audio.musicMixer;
            sfxMixer = ModThatIsNotMod.Audio.sfxMixer;
        }

        [Obsolete("Use ModThatIsNotMod.Audio.musicMixer instead.")]
        public static AudioMixerGroup musicMixer;

        [Obsolete("Use ModThatIsNotMod.Audio.sfxMixer instead.")]
        public static AudioMixerGroup sfxMixer;
    }

    public static class AssetBundles
    {
        [Obsolete("Use ModThatIsNotMod.AssetBundles.userDataDir instead.")]
        public static string userDataDir = @"UserData\";

        [Obsolete("Use ModThatIsNotMod.AssetBundles.assetBundlesDir instead.")]
        public static string assetBundlesDir = @"UserData\AssetBundles\";

        [Obsolete("Use ModThatIsNotMod.AssetBundles.LoadFromUserData() instead.")]
        public static bool LoadBundle(string name, out AssetBundle bundle) => LoadBundle(name, true, out bundle);

        [Obsolete("Use ModThatIsNotMod.AssetBundles.LoadFromUserData() instead.")]
        public static bool LoadBundle(string name, bool inAssetBunldesFolder, out AssetBundle bundle)
        {
            userDataDir = MelonUtils.UserDataDirectory;
            assetBundlesDir = Path.Combine(userDataDir, "AssetBundles");

            string pathToBundle = inAssetBunldesFolder ? Path.Combine(assetBundlesDir, name) : Path.Combine(userDataDir, name);
            bundle = AssetBundle.LoadFromFile(pathToBundle);
            return bundle != null;
        }
    }

    public static class VersionChecking
    {
        [Obsolete("Use ModThatIsNotMod.VersionChecking.CheckModVersion() instead.")]
        public static void CheckVersion(MelonMod mod, string bonetomeLink) => ModThatIsNotMod.VersionChecking.CheckModVersion(mod, bonetomeLink);
    }

    public static class UI
    {
        private static float textQualityMultiplier = 2000f;
        private static Anchor[] anchors = new Anchor[9]
        {
            new Anchor(new Vector2(0, 1), new Vector2(0, 1)),
            new Anchor(new Vector2(0.5f, 1), new Vector2(0.5f, 1)),
            new Anchor(new Vector2(1, 1), new Vector2(1, 1)),
            new Anchor(new Vector2(0, 0.5f), new Vector2(0, 0.5f)),
            new Anchor(new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f)),
            new Anchor(new Vector2(1, 0.5f), new Vector2(1, 0.5f)),
            new Anchor(new Vector2(0, 0), new Vector2(0, 0)),
            new Anchor(new Vector2(0.5f, 0), new Vector2(0.5f, 0)),
            new Anchor(new Vector2(1, 0), new Vector2(1, 0))
        };

        [Obsolete("No longer supported.")]
        public static Canvas CreateWorldSpaceCanvas(Vector2 dimensions, Vector3 position, string objectName)
        {
            GameObject canvasGO = new GameObject(objectName);

            Canvas canvas = canvasGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace;

            RectTransform rectTransform = canvasGO.GetComponent<RectTransform>();
            rectTransform.position = position;
            rectTransform.sizeDelta = dimensions;

            return canvas;
        }

        [Obsolete("No longer supported.")]
        public static Text CreateText(Canvas canvas, string text, int fontSize, Vector2 textBoundsSize, Vector2 pivotPoint, Vector2 positionOffset, AnchorType anchorType = AnchorType.UpperLeft, TextAnchor alignment = TextAnchor.UpperLeft)
        {
            GameObject textGO = new GameObject(canvas.name + " Text " + canvas.transform.childCount);
            textGO.transform.parent = canvas.transform;
            textGO.transform.localPosition = Vector3.zero;

            Text textComp = textGO.AddComponent<Text>();
            textComp.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            textComp.text = text;
            textComp.fontSize = fontSize * 20;

            textComp.horizontalOverflow = HorizontalWrapMode.Overflow;
            textComp.verticalOverflow = VerticalWrapMode.Overflow;
            textComp.alignment = alignment;

            RectTransform rectTransform = textGO.GetComponent<RectTransform>();
            rectTransform.localScale = Vector3.one / textQualityMultiplier;
            rectTransform.sizeDelta = textBoundsSize * textQualityMultiplier;

            rectTransform.anchorMin = anchors[(int)anchorType].minAnchor;
            rectTransform.anchorMax = anchors[(int)anchorType].maxAnchor;
            rectTransform.pivot = pivotPoint;

            rectTransform.anchoredPosition = positionOffset;

            return textComp;
        }

        public enum AnchorType
        {
            UpperLeft = 0,
            UpperCenter = 1,
            UpperRight = 2,
            MiddleLeft = 3,
            MiddleCenter = 4,
            MiddleRight = 5,
            LowerLeft = 6,
            LowerCenter = 7,
            LowerRight = 8
        }
        private struct Anchor
        {
            public Vector2 minAnchor;
            public Vector2 maxAnchor;

            public Anchor(Vector2 minAnchor, Vector2 maxAnchor)
            {
                this.minAnchor = minAnchor;
                this.maxAnchor = maxAnchor;
            }
        }
    }
}
