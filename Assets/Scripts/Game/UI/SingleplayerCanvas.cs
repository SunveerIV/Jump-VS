using UnityEngine;
using TMPro;
using Game.Prefabs;

namespace Game.UI {
    public class SingleplayerCanvas : MonoBehaviour {
        
        [SerializeField] private TextMeshProUGUI scoreText;

        public float ScoreText {
            set => scoreText.text = value.ToString();
        }

        public static SingleplayerCanvas Create() {
            SingleplayerCanvas canvas = Instantiate(PrefabContainer.SINGLEPLAYER_CANVAS);
            canvas.transform.SetParent(Camera.main.transform);
            canvas.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
            canvas.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);
            return canvas;
        }
    }
}