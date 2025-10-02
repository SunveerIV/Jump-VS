using UnityEngine;

namespace Game.Utility.Prefab {
    public partial class PrefabContainer : MonoBehaviour {
        [SerializeField] private Player playerPrefab;
        public static Player PLAYER => instance.playerPrefab;
    }
}