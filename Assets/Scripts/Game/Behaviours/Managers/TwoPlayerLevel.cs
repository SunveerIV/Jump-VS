using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Game.Constants;
using Game.Interfaces;
using Game.Behaviours.Players;
using Game.Behaviours.Platforms;
using Game.Behaviours.Colliders;

namespace Game.Behaviours.Managers {
    public class TwoPlayerLevel : NetworkBehaviour, ILevelMultiplayer {

        [SerializeField] private PlatformMultiplayer platformMultiplayerPrefab;
        [SerializeField] private PlayerMultiplayer playerMultiplayerPrefab;
        [SerializeField] private KillCollider killColliderPrefab;
        [SerializeField] private Border borderPrefab;

        private Border borders;
        
        private bool clientInitialized;

        private Dictionary<int, PlatformMultiplayer> platforms;
        private List<PlayerMultiplayer> players;
        
        private float highestPlatform;
        
        private ushort platformIndex = 0;
        
        private void OnClientConnected(ulong clientId) {
            InitializeClient();
            InitializeServer();
        }

        private void InitializeClient() {
            if (!IsClient) return;
            if (clientInitialized) return;
            clientInitialized = true;
        }

        private void InitializeServer() {
            if (!IsServer) return;
            if (NetworkManager.Singleton.ConnectedClients.Count < 2) return;
            
            InitializeKillCollider();
            InitializeBorders();
            
            platforms = new Dictionary<int, PlatformMultiplayer>();
            players = new List<PlayerMultiplayer>();
            highestPlatform = -1f;
            InstantiatePlatform();
            float playerStartPosX = platforms[0].transform.position.x;
            foreach (ulong clientID in NetworkManager.Singleton.ConnectedClientsIds) {
                PlayerMultiplayer player = PlayerMultiplayer.Create(playerMultiplayerPrefab, new Vector2(playerStartPosX, Level.PLAYER_START_Y), this, clientID);
                players.Add(player);
            }
            StartCoroutine(UpdateEverySecond());
        }

        private void InitializeKillCollider() {
            KillCollider.Create(killColliderPrefab, true);
        }

        private void InitializeBorders() {
            borders = Border.Create(borderPrefab);
        }
        
        private IEnumerator UpdateEverySecond() {
            while (true) {
                UpdateBorders();
                UpdatePlatforms();
                yield return new WaitForSeconds(1);
            }
        }
        
        private void UpdateBorders() {
            float minY = float.MaxValue;
            float maxY = float.MinValue;
            foreach (var player in players) {
                float playerY = player.transform.position.y;
                if (playerY < minY) minY = playerY;
                if (playerY > maxY) maxY = playerY;
            }

            float averageHeight = (minY + maxY) / 2;
            borders.UpdateTransform(averageHeight, maxY - minY);
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
                PlatformMultiplayer platform = platforms[i];
                if (lowestCircle - platform.transform.position.y > Level.MAX_DIFFERENCE) {
                    platforms.Remove(platform.Index);
                    platform.gameObject.GetComponent<NetworkObject>().Despawn();
                }
            }
        }
        
        private void InstantiatePlatform() {
            highestPlatform += Level.DISTANCE_BETWEEN_PLATFORMS;
            PlatformMultiplayer platform = PlatformMultiplayer.Create(platformMultiplayerPrefab, new Vector2(Random.Range(-2f, 2f), highestPlatform), platformIndex);
            platforms.Add(platformIndex, platform);
            platformIndex++;
        }
        
        public PlatformMultiplayer GetPlatformAtIndex(int platformIndex) => platforms[platformIndex];
        
        private void OnEnable() {
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        }

        private void OnDisable() {
            if (NetworkManager.Singleton == null) return;
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
        }

        public void UpdateScore() {
            foreach (var player in players) {
                player.UpdateScoreTextClientRpc();
            }
        }

        public void EndGame() {
            int activePlayers = 0;
            foreach (var player in players) {
                activePlayers += player.HasLost ? 0 : 1;
            }
            
            if (activePlayers > 0) return;
            
            foreach (var player in players) {
                player.EndGameClientRpc();
            }
            SceneLoader.LoadMultiplayerEndScreen();
            Destroy(gameObject);
        }
    }
}