using UnityEngine;

namespace Game.Settings {
    public static partial class UserSettings {
        private const string MASTER_VOLUME_KEY = "MasterVolume";
        private const float DEFAULT_MASTER_VOLUME = 1.0f;
        
        public static float MasterVolume {
            get => PlayerPrefs.GetFloat(MASTER_VOLUME_KEY, DEFAULT_MASTER_VOLUME);
            set => PlayerPrefs.SetFloat(MASTER_VOLUME_KEY, Mathf.Clamp01(value));
        }
        
        
        
        private const string MUSIC_VOLUME_KEY = "MusicVolume";
        private const float DEFAULT_MUSIC_VOLUME = 1.0f;
        
        public static float MusicVolume {
            get => PlayerPrefs.GetFloat(MUSIC_VOLUME_KEY, DEFAULT_MUSIC_VOLUME);
            set => PlayerPrefs.SetFloat(MUSIC_VOLUME_KEY, Mathf.Clamp01(value));
        }
        
        
        
        private const string SOUND_EFFECTS_VOLUME_KEY = "SoundEffectsVolume";
        private const float DEFAULT_SOUND_EFFECTS_VOLUME = 1.0f;
        
        public static float SoundEffectsVolume {
            get => PlayerPrefs.GetFloat(SOUND_EFFECTS_VOLUME_KEY, DEFAULT_SOUND_EFFECTS_VOLUME);
            set => PlayerPrefs.SetFloat(SOUND_EFFECTS_VOLUME_KEY, Mathf.Clamp01(value));
        }
    }
}