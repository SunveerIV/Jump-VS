using UnityEngine;
using Game.Settings;

namespace Game.UI {
    public class FPSText : MonoBehaviour {

        private void Start() {
            gameObject.SetActive(UserSettings.ShowFPS);
        }
    }
}