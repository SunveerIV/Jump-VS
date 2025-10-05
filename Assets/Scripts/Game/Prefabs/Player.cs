using UnityEngine;
using Game.Behaviours.Players;

namespace Game.Prefabs {
    public partial class PrefabContainer : MonoBehaviour {
        [SerializeField] private Player playerPrefab;
        public static Player PLAYER => instance.playerPrefab;
    }
}