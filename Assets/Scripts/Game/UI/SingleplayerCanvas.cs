using UnityEngine;
using TMPro;

namespace Game.UI {
    public class SingleplayerCanvas : MonoBehaviour {
        
        [SerializeField] private TextMeshProUGUI scoreText;

        public float ScoreText {
            set => scoreText.text = value.ToString();
        }

        public static SingleplayerCanvas Create(SingleplayerCanvas prefab) {
            SingleplayerCanvas canvas = Instantiate(prefab);
            canvas.transform.SetParent(Camera.main.transform);
            canvas.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);
            return canvas;
        }
    }
}