using UnityEngine;
using Unity.Netcode;
using Game.Interfaces;
using Game.Behaviours.Players;
using Game.Behaviours.Platforms;

namespace Game.Behaviours.Managers {
    public class TwoPlayerLevel : MonoBehaviour, ILevel {

        [SerializeField] private PlatformMultiplayer platformMultiplayerPrefab;
        [SerializeField] private PlayerMultiplayer playerMultiplayerPrefab;

        private void Awake() {
            
        }

        private void OnClientConnected(ulong clientId) {
            
        }
        
        private void OnEnable() {
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        }

        private void OnDisable() {
            if (NetworkManager.Singleton == null) return;
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
        }

        public void UpdateScore() {
            
        }

        public void EndGame() {
            
        }
    }
}