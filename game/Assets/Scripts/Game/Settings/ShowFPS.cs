using UnityEngine;

namespace Game.Settings {
    public static partial class UserSettings {
        private const string SHOW_FPS_KEY = "ShowFPS";
        private const int DEFAULT_SHOW_FPS = 0;

        public static bool ShowFPS {
            get => PlayerPrefs.GetInt(SHOW_FPS_KEY, DEFAULT_SHOW_FPS) == 1;
            set => PlayerPrefs.SetInt(SHOW_FPS_KEY, value ? 1 : 0);
        }
    }
}