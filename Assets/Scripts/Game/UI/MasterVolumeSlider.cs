using UnityEngine;
using UnityEngine.UI;
using Game.Settings;

namespace Game.UI {
    public class MasterVolumeSlider : MonoBehaviour {

        [SerializeField] private Slider slider;

        private void Awake() {
            slider.onValueChanged.AddListener(OnSliderValueChanged);
            slider.value = UserSettings.MasterVolume;
            AudioListener.volume = UserSettings.MusicVolume;
        }

        private void OnSliderValueChanged(float value) {
            UserSettings.MasterVolume = value;
            AudioListener.volume = UserSettings.MasterVolume;
        }

        private void OnDestroy() {
            slider.onValueChanged.RemoveListener(OnSliderValueChanged);
        }
    }
}