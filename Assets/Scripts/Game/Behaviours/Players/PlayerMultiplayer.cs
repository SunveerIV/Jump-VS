using System.Collections;
using UnityEngine;
using Unity.Netcode;
using Game.Utility;
using Game.UI;
using Game.Constants;
using Game.Behaviours.Directors;
using Game.Behaviours.Managers;
using Game.Behaviours.Platforms;
using Game.Interfaces;

namespace Game.Behaviours.Players {
    public class PlayerMultiplayer : NetworkBehaviour, IPlayer, ILaunchable {
        
        [Header("Prefabs")] 
        [SerializeField] private LineDirector lineDirectorPrefab;
        [SerializeField] private SingleplayerCanvas singleplayerCanvasPrefab;

        [Header("Audio")] 
        [SerializeField] private AudioClip stickSound;
        [SerializeField] private AudioClip bounceSound;

        [Header("Components")] 
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private Rigidbody2D rb;
        
        //Network Variables
        private readonly NetworkVariable<bool> hasLost = new(false);
        private readonly NetworkVariable<float> score = new(0);
        
        //Cached References
        private SingleplayerCanvas gui;
        private IStickable stickable;
        private ILevelMultiplayer level;
        
        //Scoring
        private float previousScore;
        private int previousPlatformIndex;
        private int cachedBounces;
        
        private bool isAttachedToPlatform;
        private float minYToRaiseCamera;
        private bool clientInitialized;
        
        public float Score => score.Value;
        public bool HasLost => hasLost.Value;
        
        public static PlayerMultiplayer Create(PlayerMultiplayer prefab, Vector3 position, ILevelMultiplayer level, ulong clientID) {
            PlayerMultiplayer player = Instantiate(prefab, position, Quaternion.identity);
            player.level = level;
            player.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientID, true);
            return player;
        }

        public override void OnNetworkSpawn() {
            base.OnNetworkSpawn();
            IgnoreCollisionsWithOtherPlayers();
            InitializeGui();
            minYToRaiseCamera = float.MinValue;
        }

        private void IgnoreCollisionsWithOtherPlayers() {
            PlayerMultiplayer[] players = FindObjectsByType<PlayerMultiplayer>(FindObjectsSortMode.None);
            for (int i = 0; i < players.Length; i++) {
                for (int j = i + 1; j < players.Length; j++) {
                    Physics2D.IgnoreCollision(players[i].GetComponent<Collider2D>(), players[j].GetComponent<Collider2D>());
                }
            }
        }

        private void InitializeGui() {
            if (!IsOwner) return;
            if (clientInitialized) return;
            clientInitialized = true;
            gui = SingleplayerCanvas.Create(singleplayerCanvasPrefab);
        }

        private void Update() {
            RaiseCamera();
            InstantiateDirector();
            RemainStuckToPlatform();
        }

        private void RaiseCamera() {
            if (!IsOwner) return;
            if (transform.position.y < minYToRaiseCamera) return;

            Camera mainCamera = Camera.main;
            minYToRaiseCamera = transform.position.y;
            Vector3 currentCameraPos = mainCamera.transform.position;
            currentCameraPos.y = transform.position.y;
            mainCamera.transform.position = currentCameraPos;
        }
        
        private void InstantiateDirector() {
            if (!IsOwner) return;
            if (isAttachedToPlatform && Input.GetMouseButtonDown(0)) {
                LineDirector.Create(lineDirectorPrefab, this);
            }
        }

        private void RemainStuckToPlatform() {
            if (!IsOwner) return;
            RemainStuckToPlatformServerRpc();
        }
        
        [ServerRpc]
        private void RemainStuckToPlatformServerRpc() {
            if (stickable == null) return;
            
            Vector3 playerPos = transform.position;
            playerPos.x = stickable.transform.position.x;
            transform.position = playerPos;
        }
        
        public void Launch(Vector3 directorPosition) {
            isAttachedToPlatform = false;
            LaunchServerRpc(directorPosition);
        }

        [ServerRpc]
        private void LaunchServerRpc(Vector3 directorPosition) {
            stickable = null;
            rb.linearVelocity = (directorPosition - transform.position) * Player.VELOCITY_AMPLIFIER;
        }
        
        private void StickToPlatform(IPlatform newPlatform) {
            isAttachedToPlatform = true;
            StickToPlatformServerRpc(newPlatform.Index);
        }

        [ServerRpc]
        private void StickToPlatformServerRpc(int platformIndex) {
            
            rb.linearVelocity = Vector2.zero;
            PlatformMultiplayer newPlatform = FindFirstObjectByType<TwoPlayerLevel>().GetPlatformAtIndex(platformIndex);
            stickable = newPlatform;
            transform.position = new Vector2(newPlatform.transform.position.x, newPlatform.transform.position.y + 0.2f);
        }
        
        private void UpdateScoreFields(IPlatform newPlatform) {
            int newPlatformIndex = newPlatform.Index;
            float xPosDifference = 1.5f - Mathf.Abs(newPlatform.transform.position.x - transform.position.x);
            int platformDifferential = newPlatformIndex - previousPlatformIndex;
            float newScore = score.Value;
            
            if (platformDifferential > 0) {
                float bounceMultiplier = Mathf.Pow(Player.BASE_POWER_FOR_BOUNCES, cachedBounces);
                float positionDifferenceMultiplier = Mathf.Pow(xPosDifference, Player.EXPONENT_FOR_PLATFORM_DIFFERENCE);
                previousScore = newPlatform.ScoreMultiplier * platformDifferential * bounceMultiplier *
                                positionDifferenceMultiplier;
                newScore += previousScore;
            }
            else if (platformDifferential < 0) {
                newScore -= previousScore;
                previousScore = 0;
            }

            cachedBounces = 0;
            previousPlatformIndex = newPlatformIndex;
            
            UpdateScoreFieldsServerRpc(newScore);
        }

        [ServerRpc]
        private void UpdateScoreFieldsServerRpc(float newScore) {
            score.Value = newScore;
            level.UpdateScore();
        }

        [ClientRpc]
        public void UpdateScoreTextClientRpc() {
            if (!IsOwner) return;
            StartCoroutine(WaitToUpdateScoreFelds());
        }

        private IEnumerator WaitToUpdateScoreFelds() {
            yield return null;
            Debug.Log("Attempting to set score on client");
            PlayerMultiplayer[] players = FindObjectsByType<PlayerMultiplayer>(FindObjectsSortMode.None);
            bool player0IsMe = players[0].OwnerClientId == NetworkManager.Singleton.LocalClientId;
            float myScore = player0IsMe ? players[0].Score : players[1].Score;
            float otherScore = player0IsMe ? players[1].Score : players[0].Score;
            
            gui.ScoreText = myScore - otherScore;
        }

        private void OnCollisionEnter2D(Collision2D collision) {
            if (!IsOwner) return;

            if (Tools.TryGetInterface(collision.gameObject, out IPlatform newPlatform)) {
                if (transform.position.y <= newPlatform.transform.position.y) {
                    cachedBounces++;
                }
                else if (!isAttachedToPlatform) {
                    audioSource.PlayOneShot(stickSound);
                    UpdateScoreFields(newPlatform);
                    StickToPlatform(newPlatform);
                }
            }
        }

        public void RequestDespawn() {
            if (!IsServer) return;
            Debug.Log("Server Requesting Despawn");
            hasLost.Value = true;
            GetComponent<Rigidbody2D>().simulated = false;
            level.EndGame();
        }

        [ClientRpc]
        public void EndGameClientRpc() {
            Debug.Log("Client attempting to end game");
            SceneLoader.LoadMultiplayerEndScreen();
        }
    }
    
}