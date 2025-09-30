using UnityEngine;
using UnityEngine.UI;
using Game.Settings;

namespace Game.UI {
    public class MusicVolumeSlider : MonoBehaviour {

        [SerializeField] private Slider slider;

        private void Start() {
            slider.onValueChanged.AddListener(OnSliderValueChanged);
            slider.value = UserSettings.MusicVolume;
        }

        private void OnSliderValueChanged(float value) {
            UserSettings.MusicVolume = value;
        }

        private void OnDestroy() {
            slider.onValueChanged.RemoveListener(OnSliderValueChanged);
        }
    }
}