using MelonLoader;
using ModThatIsNotMod.Internals;
using StressLevelZero.Interaction;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ModThatIsNotMod
{
    public static class Notifications
    {
        private static GameObject baseNotificationObj;
        internal static GameObject notificationObj;

        private static float movementSmoothing = 0.8f;
        internal static Vector3 positionOffset = new Vector3(0.025f, 0.15f, 0.05f);

        private static TextMeshProUGUI messageText;
        private static TextMeshProUGUI queueText;
        internal static Image timerImage;

        private static AudioSource audioSource;

        private static Queue<NotificationData> notificationQueue = new Queue<NotificationData>();
        internal static NotificationData activeNotification;

        private static int curScene = -1;

        private static bool isValidScene => curScene != -1 && curScene != 0 && curScene != 24 && curScene != 25; // introStart, empty_scene, loadingScene
        private static Hand notificationHand { get { return Preferences.notificationsOnRightHand ? Player.rightHand : Player.leftHand; } }


        internal static void LoadBundleAndSetup()
        {
            AssetBundle bundle = null;
            Assembly assembly = Assembly.GetExecutingAssembly();
            using (Stream stream = assembly.GetManifestResourceStream(assembly.GetManifestResourceNames().First(x => x.Contains("notifications"))))
            {
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    stream.CopyTo(memoryStream);
                    bundle = AssetBundle.LoadFromMemory(memoryStream.ToArray());
                }
            }

            UnityEngine.Object[] objects = bundle.LoadAllAssets(); // Not sure if just Load() would work here but it didn't with the bonemenu bundle for whatever reason and I don't feel like testing
            baseNotificationObj = objects.Where(x => x.name == "PlayerNotifications").First().Cast<GameObject>();
            baseNotificationObj.hideFlags = HideFlags.DontUnloadUnusedAsset;
            GameObject.DontDestroyOnLoad(baseNotificationObj);

            baseNotificationObj.GetComponentInChildren<Canvas>().sortingOrder = 100;

            positionOffset = new Vector3(Preferences.notificationOffsetX, Preferences.notificationOffsetY, Preferences.notificationOffsetZ);
        }

        internal static void SetupNotificationsForScene(int buildIndex)
        {
            activeNotification = null;

            curScene = buildIndex;

            if (isValidScene)
            {
                notificationObj = GameObject.Instantiate(baseNotificationObj);
                notificationObj.transform.localPosition = positionOffset;
                notificationObj.transform.localScale = new Vector3(0.75f, 0.75f, 1);

                messageText = notificationObj.transform.Find("NotificationCanvas/NotificationMessage").GetComponent<TextMeshProUGUI>();
                queueText = notificationObj.transform.Find("NotificationCanvas/QueueNumber").GetComponent<TextMeshProUGUI>();
                timerImage = notificationObj.GetComponentInChildren<Image>();
                audioSource = notificationObj.GetComponent<AudioSource>();
                audioSource.outputAudioMixerGroup = Audio.sfxMixer;

                notificationObj.SetActive(false);
            }
        }

        internal static void UpdateNotifications()
        {
            if (isValidScene && notificationObj != null)
            {
                if (activeNotification != null)
                    activeNotification.DecreaseTime();
                else if (notificationQueue.Count > 0)
                    DisplayNextNotification();

                notificationObj.transform.position = Vector3.Lerp(notificationObj.transform.position, notificationHand.palmPositionTransform.position, movementSmoothing);
                notificationObj.transform.Translate(positionOffset, Space.Self);
                Vector3 lookPos = notificationObj.transform.position - Player.GetPlayerHead().transform.position;
                lookPos.y = 0;
                notificationObj.transform.rotation = Quaternion.Lerp(notificationObj.transform.rotation, Quaternion.LookRotation(lookPos), movementSmoothing);
            }
        }

        private static void DisplayNextNotification()
        {
            activeNotification = notificationQueue.Dequeue();
            messageText.text = activeNotification.message;
            queueText.text = notificationQueue.Count > 0 ? notificationQueue.Count.ToString() : "";
            timerImage.fillAmount = 1;

            messageText.color = activeNotification.color;
            queueText.color = activeNotification.color;
            timerImage.color = activeNotification.color;

            notificationObj.SetActive(true);

            audioSource.Play();
            MelonCoroutines.Start(CoSendNotificationHaptics());
        }

        private static IEnumerator CoSendNotificationHaptics()
        {
            notificationHand.controller.HapticAction(0, 0.05f, 50, 0.3f);
            yield return new WaitForSecondsRealtime(0.2f);
            notificationHand.controller.HapticAction(0, 0.1f, 50, 0.3f);
        }

        public static NotificationData SendNotification(string message, float duration) => SendNotification(message, duration, Color.white);

        public static NotificationData SendNotification(string message, float duration, Color color)
        {
            NotificationData data = new NotificationData(message, duration, color);
            notificationQueue.Enqueue(data);
            return data;
        }
    }

    public class NotificationData
    {
        public string message { get; private set; }
        public float duration { get; private set; }
        public Color color { get; private set; }

        public float timeRemaining { get; private set; }

        internal NotificationData(string message, float duration, Color color)
        {
            this.message = message;
            this.duration = duration;
            this.color = color;

            timeRemaining = duration;
        }

        internal void DecreaseTime()
        {
            timeRemaining -= Time.deltaTime * Time.timeScale;
            Notifications.timerImage.fillAmount = Mathf.Clamp01(timeRemaining / duration);
            if (timeRemaining <= 0)
            {
                Notifications.notificationObj.SetActive(false);
                Notifications.activeNotification = null;

                Notifications.UpdateNotifications();
            }
        }

        public void End()
        {
            timeRemaining = 0;
            DecreaseTime();
        }
    }
}
