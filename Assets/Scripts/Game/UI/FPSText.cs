using System.Collections;
using UnityEngine;
using Game.Settings;
using TMPro;

namespace Game.UI {
    public class FPSText : MonoBehaviour {

        private const float UPDATE_INTERVAL = 1f;
        
        [SerializeField] private TextMeshProUGUI textFPS;

        private void Start() {
            
            if (UserSettings.ShowFPS) {
                gameObject.SetActive(true);
                StartCoroutine(ShowFPS());
            }
        }

        private IEnumerator ShowFPS() {
            while (true) {
                textFPS.text = Mathf.Round(1 / Time.deltaTime).ToString();
                yield return new WaitForSeconds(UPDATE_INTERVAL);
            }
        }
    }
}