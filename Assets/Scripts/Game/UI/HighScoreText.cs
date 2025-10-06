using Game.Settings;
using UnityEngine;
using TMPro;

namespace Game.UI {
    public class HighScoreText : MonoBehaviour {
        
        [SerializeField] private TextMeshProUGUI highScoreText;

        private void Start() {
            if (UserSettings.ShowHighScore) {
                highScoreText.text = "High Score: " + PlayerPrefs.GetFloat("High Score", 0);
            }
        }
    }
}