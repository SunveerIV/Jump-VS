using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Game.UI;
using Game.Constants;
using Game.Interfaces;
using Game.Behaviours.Players;
using Game.Behaviours.Platforms;
using Game.Behaviours.Colliders;

namespace Game.Behaviours.Managers {
    public class TwoPlayerLevel : NetworkBehaviour, ILevel {

        [SerializeField] private PlatformMultiplayer platformMultiplayerPrefab;
        [SerializeField] private PlayerMultiplayer playerMultiplayerPrefab;
        [SerializeField] private SingleplayerCanvas singleplayerCanvasPrefab;
        [SerializeField] private KillCollider killColliderPrefab;

        private SingleplayerCanvas gui;
        private bool clientInitialized;

        private Dictionary<int, PlatformMultiplayer> platforms;
        private List<PlayerMultiplayer> players;
        
        private float highestPlatform;
        
        private int platformIndex = 0;
        
        private void OnClientConnected(ulong clientId) {
            InitializeClient();
            InitializeServer();
        }

        private void InitializeClient() {
            if (!IsClient) return;
            if (clientInitialized) return;
            clientInitialized = true;

            gui = SingleplayerCanvas.Create(singleplayerCanvasPrefab);
            KillCollider.Create(killColliderPrefab);
        }

        private void InitializeServer() {
            if (!IsServer) return;
            if (NetworkManager.Singleton.ConnectedClients.Count < 2) return;
            
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
            UpdateScoreClientRpc();
        }

        [ServerRpc]
        private void UpdateScoreServerRpc() {
            
        }

        [ClientRpc]
        private void UpdateScoreClientRpc() {
            Debug.Log("Attempting to set score on client");
            PlayerMultiplayer[] players = FindObjectsByType<PlayerMultiplayer>(FindObjectsSortMode.None);
            bool player0IsMe = players[0].OwnerClientId == NetworkManager.Singleton.LocalClientId;
            float myScore;
            float otherScore;
            if (player0IsMe) {
                myScore = players[0].Score;
                otherScore = players[1].Score;
            }
            else {
                myScore = players[1].Score;
                otherScore = players[0].Score;
            }
            
            gui.ScoreText = myScore - otherScore;
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
            Destroy(gameObject);
        }
    }
}