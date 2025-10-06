using Game.UI;
using UnityEngine;

namespace Game.Prefabs {
    public partial class PrefabContainer : MonoBehaviour {
        [SerializeField] private SingleplayerCanvas singleplayerCanvasPrefab;
        public static SingleplayerCanvas SINGLEPLAYER_CANVAS => instance.singleplayerCanvasPrefab;
    }
}