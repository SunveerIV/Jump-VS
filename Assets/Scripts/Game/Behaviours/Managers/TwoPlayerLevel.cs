using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Game.Constants;
using Game.Interfaces;
using Game.Behaviours.Players;
using Game.Behaviours.Platforms;
using Game.Behaviours.Colliders;
using Game.UI;
using Game.Utility;
using JumpVS.Core;

namespace Game.Behaviours.Managers {
    public class TwoPlayerLevel : NetworkBehaviour, ILevel {

        [SerializeField] private PlatformMultiplayer platformMultiplayerPrefab;
        [SerializeField] private PlayerMultiplayer playerMultiplayerPrefab;
        [SerializeField] private SingleplayerCanvas singleplayerCanvasPrefab;
        [SerializeField] private KillCollider killColliderPrefab;
        [SerializeField] private BorderManager borderPrefab;
        
        private readonly Team team1 = new Team(0);
        private readonly Team team2 = new Team(1);

        private BorderManager borders;
        private SingleplayerCanvas gui;
        
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
            InitializeGui();
        }

        private void InitializeServer() {
            if (!IsServer) return;
            if (NetworkManager.Singleton.ConnectedClients.Count != 2) return;
            
            InitializeKillCollider();
            InitializeBorders();
            
            platforms = new Dictionary<int, PlatformMultiplayer>();
            players = new List<PlayerMultiplayer>();
            highestPlatform = -1f;
            InstantiatePlatform();
            float playerStartPosX = platforms[0].transform.position.x;

            Vector2 startPos = new Vector2(playerStartPosX, Level.PLAYER_START_Y);
            
            var player1 = PlayerMultiplayer.Create(playerMultiplayerPrefab, startPos, this, NetworkManager.Singleton.ConnectedClientsIds[0], team1);
            var player2 = PlayerMultiplayer.Create(playerMultiplayerPrefab, startPos, this, NetworkManager.Singleton.ConnectedClientsIds[1], team2);

            players.Add(player1);
            players.Add(player2);
            
            StartCoroutine(UpdateEverySecond());
        }
        
        private void InitializeGui() {
            gui = SingleplayerCanvas.Create(singleplayerCanvasPrefab);
        }

        private void InitializeKillCollider() {
            KillCollider.Create(killColliderPrefab, true);
        }

        private void InitializeBorders() {
            borders = BorderManager.Create(borderPrefab);
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
                    platform.NetworkObject.Despawn();
                }
            }
        }
        
        private void InstantiatePlatform() {
            highestPlatform += Level.DISTANCE_BETWEEN_PLATFORMS;
            PlatformMultiplayer platform = PlatformMultiplayer.Create(platformMultiplayerPrefab, new Vector2(Random.Range(-2f, 2f), highestPlatform), platformIndex);
            platforms.Add(platformIndex, platform);
            platformIndex++;
        }
        
        private void OnEnable() {
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        }

        private void OnDisable() {
            if (NetworkManager.Singleton == null) return;
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
        }

        public void UpdateScore() {
            UpdateScoreTextClientRpc();
        }
        
        [ClientRpc]
        public void UpdateScoreTextClientRpc() {
            this.ExecuteAtEndOfFrame(() => {
                PlayerMultiplayer[] players = FindObjectsByType<PlayerMultiplayer>(FindObjectsSortMode.None);
                bool player0IsMe = players[0].OwnerClientId == NetworkManager.Singleton.LocalClientId;
                float myScore = player0IsMe ? players[0].Score : players[1].Score;
                float otherScore = player0IsMe ? players[1].Score : players[0].Score;
            
                gui.ScoreText = myScore - otherScore;
            });
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