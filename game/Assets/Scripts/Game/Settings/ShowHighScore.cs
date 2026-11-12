using UnityEngine;

namespace Game.Settings {
    public static partial class UserSettings {
        private const string SHOW_HIGH_SCORE_KEY = "ShowHighScore";
        private const int DEFAULT_SHOW_HIGH_SCORE = 1;

        public static bool ShowHighScore {
            get => PlayerPrefs.GetInt(SHOW_HIGH_SCORE_KEY, DEFAULT_SHOW_HIGH_SCORE) == 1;
            set => PlayerPrefs.SetInt(SHOW_HIGH_SCORE_KEY, value ? 1 : 0);
        }
    }
}