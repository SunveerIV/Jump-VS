using UnityEngine;
using UnityEngine.UI;
using Game.Settings;

namespace Game.UI {
    public class MasterVolumeSlider : MonoBehaviour {

        [SerializeField] private Slider slider;

        private void Start() {
            slider.onValueChanged.AddListener(OnSliderValueChanged);
            slider.value = UserSettings.MasterVolume;
        }

        private void OnSliderValueChanged(float value) {
            UserSettings.MasterVolume = value;
            AudioListener.volume = UserSettings.MasterVolume; //After validation from the UserSettings class
        }

        private void OnDestroy() {
            slider.onValueChanged.RemoveListener(OnSliderValueChanged);
        }
    }
}