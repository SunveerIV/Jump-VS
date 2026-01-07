using System;
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
using JumpVS.Core.Scoring;

namespace Game.Behaviours.Players {
    public class PlayerMultiplayer : NetworkBehaviour, IPlayer, ILaunchable {
        
        [Header("Prefabs")] 
        [SerializeField] private LineDirector lineDirectorPrefab;
        [SerializeField] private SingleplayerCanvas singleplayerCanvasPrefab;
        [SerializeField] private BorderSpriteManager borderSpriteManagerPrefab;

        [Header("Audio")] 
        [SerializeField] private AudioClip stickSound;
        [SerializeField] private AudioClip bounceSound;

        [Header("Components")] 
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private Rigidbody2D rb;
        
        //Network Variables
        private readonly NetworkVariable<PlayerState> playerState = new();
        private readonly NetworkVariable<bool> hasLost = new(false);
        private readonly NetworkVariable<float> networkScore = new(0);
        
        //Cached References
        private SingleplayerCanvas gui;
        private Camera mainCamera;
        private PlayerScore score;
        private IStickable stickable;
        private ILevelMultiplayer level;
        
        private float cameraVelocityY;
        private bool clientInitialized;
        
        public float Score => networkScore.Value;
        public bool HasLost => hasLost.Value;
        
        public static PlayerMultiplayer Create(PlayerMultiplayer prefab, Vector3 position, ILevelMultiplayer level, ulong clientID) {
            PlayerMultiplayer player = Instantiate(prefab, position, Quaternion.identity);
            player.level = level;
            player.score = new PlayerScore();
            player.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientID, true);
            return player;
        }

        public override void OnNetworkSpawn() {
            base.OnNetworkSpawn();
            IgnoreCollisionsWithOtherPlayers();
            SetCamera();
            InitializeGui();
            InitializeBorderSprite();
        }

        private void IgnoreCollisionsWithOtherPlayers() {
            PlayerMultiplayer[] players = FindObjectsByType<PlayerMultiplayer>(FindObjectsSortMode.None);
            for (int i = 0; i < players.Length; i++) {
                for (int j = i + 1; j < players.Length; j++) {
                    Physics2D.IgnoreCollision(players[i].GetComponent<Collider2D>(), players[j].GetComponent<Collider2D>());
                }
            }
        }

        private void SetCamera() {
            if (!IsOwner) return;
            
            mainCamera = Camera.main;
        }

        private void InitializeGui() {
            if (!IsOwner) return;
            if (clientInitialized) return;
            
            clientInitialized = true;
            gui = SingleplayerCanvas.Create(singleplayerCanvasPrefab);
        }

        private void InitializeBorderSprite() {
            if (!IsOwner) return;
            
            BorderSpriteManager.Create(borderSpriteManagerPrefab, transform);
        }

        private void Update() {
            MoveCamera();
            
            switch (playerState.Value) {
                case PlayerState.Attached: {
                    InstantiateDirector();
                    RemainStuckToPlatform();
                    break;
                }

                case PlayerState.Airborne: {
                    break;
                }
            }
        }

        private void MoveCamera() {
            if (!IsOwner) return;
            
            Vector3 currentCameraPos = mainCamera.transform.position;
            float targetY = transform.position.y + 3.5f;
            float newY = Mathf.SmoothDamp(currentCameraPos.y, targetY, ref cameraVelocityY, Player.CAMERA_SMOOTH_TIME);
            mainCamera.transform.position = new Vector3(currentCameraPos.x, newY, currentCameraPos.z);
        }
        
        private void InstantiateDirector() {
            if (!IsOwner) return;
            if (!Input.GetMouseButtonDown(0)) return;
            
            LineDirector.Create(lineDirectorPrefab, this);
        }

        private void RemainStuckToPlatform() {
            if (!IsServer) return;

            Vector3 playerPos = transform.position;
            playerPos.x = stickable.transform.position.x;
            transform.position = playerPos;
        }
        
        public void Launch(Vector3 directorPosition) => LaunchServerRpc(directorPosition);

        [ServerRpc]
        private void LaunchServerRpc(Vector3 directorPosition) {
            playerState.Value = PlayerState.Airborne;
            rb.linearVelocity = (directorPosition - transform.position) * Player.VELOCITY_AMPLIFIER;
        }

        [ClientRpc]
        public void UpdateScoreTextClientRpc() {
            if (!IsOwner) return;
            
            StartCoroutine(WaitToUpdateScoreFelds());
        }

        private IEnumerator WaitToUpdateScoreFelds() {
            yield return null;
            PlayerMultiplayer[] players = FindObjectsByType<PlayerMultiplayer>(FindObjectsSortMode.None);
            bool player0IsMe = players[0].OwnerClientId == NetworkManager.Singleton.LocalClientId;
            float myScore = player0IsMe ? players[0].Score : players[1].Score;
            float otherScore = player0IsMe ? players[1].Score : players[0].Score;
            
            gui.ScoreText = myScore - otherScore;
        }

        private void CollideWithPlatform(IPlatform newPlatform) {
            if (transform.position.y <= newPlatform.transform.position.y) {
                score.Bounce();
            }
            else if (playerState.Value == PlayerState.Airborne) {
                audioSource.PlayOneShot(stickSound);
                var distanceFromCenter = MathF.Abs(transform.position.x - newPlatform.transform.position.x);
                var landEvent = new LandEvent(newPlatform.Index, distanceFromCenter, newPlatform.ScoreMultiplier);
                score.LandOnPlatform(landEvent);
                networkScore.Value = score.Value;
            
                rb.linearVelocity = Vector2.zero;
                transform.position = new Vector2(newPlatform.transform.position.x, newPlatform.transform.position.y + 0.2f);
                rb.angularVelocity = 0f;
                playerState.Value = PlayerState.Attached;
                stickable = newPlatform;
                
                level.UpdateScore();
            }
        }

        private void OnCollisionEnter2D(Collision2D collision) {
            if (!IsServer) return;

            if (Tools.TryGetInterface(collision.gameObject, out IPlatform newPlatform)) {
                CollideWithPlatform(newPlatform);
            }

            if (Tools.TryGetInterface(collision.gameObject, out IKillCollider ignored)) {
                CollideWithKillCollider();
            }
        }

        public void CollideWithKillCollider() {
            if (!IsServer) return;
            
            hasLost.Value = true;
            GetComponent<Rigidbody2D>().simulated = false;
            level.EndGame();
        }

        [ClientRpc]
        public void EndGameClientRpc() {
            SceneLoader.LoadMultiplayerEndScreen();
        }
    }
}