using UnityEngine;
using UnityEngine.UI;
using Game.Settings;

namespace Game.UI {
    public class ShowFPSToggle : MonoBehaviour {

        [SerializeField] private Toggle toggle;

        private void Start() {
            toggle.onValueChanged.AddListener(OnSliderValueChanged);
            toggle.isOn = UserSettings.ShowFPS;
        }

        private void OnSliderValueChanged(bool value) {
            UserSettings.ShowFPS = value;
        }

        private void OnDestroy() {
            toggle.onValueChanged.RemoveListener(OnSliderValueChanged);
        }
    }
}