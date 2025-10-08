using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Game.Interfaces;
using Game.Behaviours.Players;
using Game.Behaviours.Platforms;

namespace Game.Behaviours.Managers {
    public class TwoPlayerLevel : MonoBehaviour, ILevel {
        
        private const float MAX_DIFFERENCE = 6f;
        private const float PLAYER_START_Y = 1.6f;

        [SerializeField] private PlatformMultiplayer platformMultiplayerPrefab;
        [SerializeField] private PlayerMultiplayer playerMultiplayerPrefab;

        private List<PlatformMultiplayer> platforms;
        private List<PlayerMultiplayer> players;
        
        private float highestPlatform;
        
        private int platformIndex = 0;
        
        private void OnClientConnected(ulong clientId) {
            NetworkManager manager = NetworkManager.Singleton;
            if (!manager.IsServer) return;
            if (manager.ConnectedClients.Count < 2) return;
            
            platforms = new List<PlatformMultiplayer>();
            players = new List<PlayerMultiplayer>();
            highestPlatform = -1f;
            InstantiatePlatform();
            float playerStartPosX = platforms[0].transform.position.x;
            foreach (ulong clientID in NetworkManager.Singleton.ConnectedClientsIds) {
                PlayerMultiplayer player = PlayerMultiplayer.Create(playerMultiplayerPrefab, new Vector2(playerStartPosX, PLAYER_START_Y), clientID);
                players.Add(player);
            }

            StartCoroutine(UpdateEverySecond());
        }
        
        private IEnumerator UpdateEverySecond() {
            while (true) {
                UpdatePlatforms();
                yield return new WaitForSeconds(1);
            }
        }
        
        private void UpdatePlatforms() {
            float highestCircle = float.MinValue;
            float lowestCircle = float.MaxValue;
            foreach (PlayerMultiplayer player in players) {
                if (player.transform.position.y > highestCircle) {
                    highestCircle = player.transform.position.y;
                }

                if (player.transform.position.y < lowestCircle) {
                    lowestCircle = player.transform.position.y;
                }
            }

            while (highestCircle + 15f > highestPlatform) {
                InstantiatePlatform();
            }

            for (int i = platforms.Count - 1; i >= 0; i--) {
                PlatformBase platform = platforms[i];
                if (lowestCircle - platform.transform.position.y > MAX_DIFFERENCE) {
                    platform.gameObject.GetComponent<NetworkObject>().Despawn();
                    platforms.RemoveAt(i);
                }
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
            Debug.Log("Num Players: " + players.Count);
            int activePlayers = 0;
            foreach (var player in players) {
                activePlayers += player.hasLost.Value ? 0 : 1;
            }
            Debug.Log("Num active Players: " + activePlayers);
            
            if (activePlayers > 0) return;
            
            Debug.Log("Server attempting to end game");
            foreach (var player in players) {
                player.EndGameClientRpc();
            }
            SceneLoader.LoadMultiplayerEndScreen();
        }
    }
}