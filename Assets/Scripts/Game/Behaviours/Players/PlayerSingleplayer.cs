using System;
using UnityEngine;
using Game.Utility;
using Game.Settings;
using Game.Constants;
using Game.Interfaces;
using Game.Behaviours.Directors;
using Game.Behaviours.Managers;
using JumpVS.Core.Scoring;


namespace Game.Behaviours.Players {
    public class PlayerSingleplayer : MonoBehaviour, IPlayer, ILaunchable {

        [Header("Prefabs")] 
        [SerializeField] private LineDirector lineDirectorPrefab;
        [SerializeField] private BorderSpriteManager borderSpriteManagerPrefab;

        [Header("Audio")] 
        [SerializeField] private AudioClip stickSound;
        [SerializeField] private AudioClip bounceSound;

        [Header("Components")] 
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private Rigidbody2D rb;

        //Cached References
        private Camera mainCamera;
        private PlayerScore score;
        private ILevel level;
        private IStickable stickable;

        private PlayerState playerState;
        private float cameraVelocityY;

        public float Score => score.Value;

        public static PlayerSingleplayer Create(PlayerSingleplayer prefab, Vector3 position, Quaternion rotation, ILevel level) {
            var player = Instantiate(prefab, position, rotation);
            BorderSpriteManager.Create(player.borderSpriteManagerPrefab, player.transform);
            player.level = level;
            player.score = new PlayerScore();
            player.mainCamera = Camera.main;
            player.audioSource.volume = UserSettings.SoundEffectsVolume;
            return player;
        }

        private void Update() {
            MoveCamera();
            
            switch (playerState) {
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
            Vector3 currentCameraPos = mainCamera.transform.position;
            float targetY = transform.position.y + 3.5f;
            float newY = Mathf.SmoothDamp(currentCameraPos.y, targetY, ref cameraVelocityY, Player.CAMERA_SMOOTH_TIME);
            mainCamera.transform.position = new Vector3(currentCameraPos.x, newY, currentCameraPos.z);
        }

        private void InstantiateDirector() {
            if (!Input.GetMouseButtonDown(0)) return;
            
            LineDirector.Create(lineDirectorPrefab, this);
        }

        private void RemainStuckToPlatform() {
            Vector3 playerPos = transform.position;
            playerPos.x = stickable.transform.position.x;
            transform.position = playerPos;
        }

        public void Launch(Vector3 directorPosition) {
            playerState = PlayerState.Airborne;
            stickable = null;
            rb.linearVelocity = (directorPosition - transform.position) * Player.VELOCITY_AMPLIFIER;
        }

        private void StickToPlatform(IPlatform newPlatform) {
            rb.linearVelocity = Vector2.zero;
            transform.position = new Vector2(newPlatform.transform.position.x, newPlatform.transform.position.y + 0.2f);
            rb.angularVelocity = 0f;
            playerState = PlayerState.Attached;
            stickable = newPlatform;
        }

        private void UpdateScoreFields(IPlatform newPlatform) {
            var distanceFromCenter = MathF.Abs(transform.position.x - newPlatform.transform.position.x);
            var landEvent = new LandEvent(newPlatform.Index, distanceFromCenter, newPlatform.ScoreMultiplier);
            score.LandOnPlatform(landEvent);
            
            level.UpdateScore();
        }

        public void RequestDespawn() {
            level.EndGame();
        }

        private void OnCollisionEnter2D(Collision2D collision) {
            if (Tools.TryGetInterface(collision.gameObject, out IPlatform newPlatform)) {
                if (transform.position.y <= newPlatform.transform.position.y) {
                    score.Bounce();
                }
                else if (playerState == PlayerState.Airborne) {
                    audioSource.PlayOneShot(stickSound);
                    UpdateScoreFields(newPlatform);
                    StickToPlatform(newPlatform);
                }
            }
            
            switch (collision.gameObject.tag) {
                case "Border": {
                    audioSource.PlayOneShot(bounceSound);
                    score.Bounce();
                    break;
                }
            }
        }
    }
}