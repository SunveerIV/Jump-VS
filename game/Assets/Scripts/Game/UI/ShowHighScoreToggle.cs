using UnityEngine;
using UnityEngine.UI;
using Game.Settings;

namespace Game.UI {
    public class ShowHighScoreToggle : MonoBehaviour {

        [SerializeField] private Toggle toggle;
        
        private void Start() {
            toggle.onValueChanged.AddListener(OnSliderValueChanged);
            toggle.isOn = UserSettings.ShowHighScore;
        }

        private void OnSliderValueChanged(bool value) {
            UserSettings.ShowHighScore = value;
        }

        private void OnDestroy() {
            toggle.onValueChanged.RemoveListener(OnSliderValueChanged);
        }
    }
}