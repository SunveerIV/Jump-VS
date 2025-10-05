using UnityEngine;

namespace Game.Prefabs {
    public partial class PrefabContainer : MonoBehaviour {
        [SerializeField] private Player playerPrefab;
        public static Player PLAYER => instance.playerPrefab;
    }
}