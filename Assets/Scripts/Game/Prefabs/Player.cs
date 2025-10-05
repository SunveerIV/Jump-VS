using UnityEngine;
using Game.Behaviours.Players;

namespace Game.Prefabs {
    public partial class PrefabContainer : MonoBehaviour {
        [SerializeField] private PlayerSingleplayer playerSingleplayerPrefab;
        public static PlayerSingleplayer PLAYER_SINGLEPLAYER => instance.playerSingleplayerPrefab;
        
        [SerializeField] private PlayerMultiplayer playerMultiplayerPrefab;
        public static PlayerMultiplayer PLAYER_MULTIPLAYER => instance.playerMultiplayerPrefab;
    }
}