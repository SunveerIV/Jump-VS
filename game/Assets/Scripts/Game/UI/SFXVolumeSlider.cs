using UnityEngine;
using UnityEngine.UI;
using Game.Settings;

namespace Game.UI {
    public class SFXVolumeSlider : MonoBehaviour {

        [SerializeField] private Slider slider;

        private void Awake() {
            slider.onValueChanged.AddListener(OnSliderValueChanged);
            slider.value = UserSettings.SoundEffectsVolume;
        }

        private void OnSliderValueChanged(float value) {
            UserSettings.SoundEffectsVolume = value;
        }

        private void OnDestroy() {
            slider.onValueChanged.RemoveListener(OnSliderValueChanged);
        }
    }
}