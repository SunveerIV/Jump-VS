using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Game.Interfaces;
using Game.Behaviours.Players;
using Game.Behaviours.Platforms;

namespace Game.Behaviours.Managers {
    public class TwoPlayerLevel : MonoBehaviour, ILevel {
        
        private const float PLAYER_START_Y = 1.6f;

        [SerializeField] private PlatformMultiplayer platformMultiplayerPrefab;
        [SerializeField] private PlayerMultiplayer playerMultiplayerPrefab;

        private List<PlatformMultiplayer> platforms;
        
        private float highestPlatform;
        
        private int platformIndex = 0;
        
        private void OnClientConnected(ulong clientId) {
            NetworkManager manager = NetworkManager.Singleton;
            if (!manager.IsServer) return;
            if (manager.ConnectedClients.Count < 2) return;
            
            platforms = new List<PlatformMultiplayer>();
            highestPlatform = -1f;
            InstantiatePlatform();
            float playerStartPosX = platforms[0].transform.position.x;
            foreach (ulong clientID in NetworkManager.Singleton.ConnectedClientsIds) {
                PlayerMultiplayer.Create(playerMultiplayerPrefab, new Vector2(playerStartPosX, PLAYER_START_Y), clientID);
            }
        }
        
        private void InstantiatePlatform() {
            highestPlatform += 2f;
            PlatformMultiplayer platform = PlatformMultiplayer.Create(platformMultiplayerPrefab, new Vector2(Random.Range(-2f, 2f), highestPlatform), platformIndex);
            platformIndex++;
            platforms.Add(platform);
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