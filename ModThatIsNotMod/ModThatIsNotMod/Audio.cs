using System.Linq;
using UnityEngine;
using UnityEngine.Audio;

namespace ModThatIsNotMod
{
    public static class Audio
    {
        private static bool hasFoundMixers => musicMixer != null && sfxMixer != null && gunshotMixer != null;

        public static AudioMixerGroup musicMixer { get; private set; }
        public static AudioMixerGroup sfxMixer { get; private set; }
        public static AudioMixerGroup gunshotMixer { get; private set; }


        /// <summary>
        /// Finds the music and sfx audio mixers.
        /// </summary>
        internal static void GetAudioMixers()
        {
            if (hasFoundMixers)
                return;

            AudioMixerGroup[] mixers = Resources.FindObjectsOfTypeAll<AudioMixerGroup>();
            musicMixer = mixers.Where(x => x.name == "Music").First();
            sfxMixer = mixers.Where(x => x.name == "SFX").First();
            gunshotMixer = mixers.Where(x => x.name == "GunShot").First();
        }
    }
}
