using System.Collections;
using UnityEngine;
using Game.Utility;
using Game.Settings;
using Game.Constants;
using Game.Interfaces;
using Game.Behaviours.Directors;
using Game.Behaviours.Colliders;
using Game.Behaviours.Managers;


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
        private ILevel level;
        private IStickable stickable;

        //Scoring
        private float score;
        private float previousScore;
        private int previousPlatformIndex;
        private int cachedBounces;

        private bool isAttachedToPlatform;
        private float cameraVelocityY;

        public float Score => score;

        public static PlayerSingleplayer Create(PlayerSingleplayer prefab, Vector3 position, Quaternion rotation, ILevel level) {
            var player = Instantiate(prefab, position, rotation);
            BorderSpriteManager.Create(player.borderSpriteManagerPrefab, player.transform);
            player.level = level;
            player.mainCamera = Camera.main;
            player.isAttachedToPlatform = false;
            player.audioSource.volume = UserSettings.SoundEffectsVolume;
            return player;
        }

        private void Update() {
            MoveCamera();
            InstantiateDirector();
            RemainStuckToPlatform();
        }

        private void MoveCamera() {
            Vector3 currentCameraPos = mainCamera.transform.position;
            float targetY = transform.position.y + 3.5f;
            float newY = Mathf.SmoothDamp(currentCameraPos.y, targetY, ref cameraVelocityY, Player.CAMERA_SMOOTH_TIME);
            mainCamera.transform.position = new Vector3(currentCameraPos.x, newY, currentCameraPos.z);
        }

        private void InstantiateDirector() {
            if (isAttachedToPlatform && Input.GetMouseButtonDown(0)) {
                LineDirector.Create(lineDirectorPrefab, this);
            }
        }

        private void RemainStuckToPlatform() {
            if (stickable != null) {
                Vector3 playerPos = transform.position;
                playerPos.x = stickable.transform.position.x;
                transform.position = playerPos;
            }
        }

        public void Launch(Vector3 directorPosition) {
            isAttachedToPlatform = false;
            stickable = null;
            rb.linearVelocity = (directorPosition - transform.position) * Player.VELOCITY_AMPLIFIER;
        }

        private void StickToPlatform(IPlatform newPlatform) {
            rb.linearVelocity = Vector2.zero;
            transform.position = new Vector2(newPlatform.transform.position.x, newPlatform.transform.position.y + 0.2f);
            isAttachedToPlatform = true;
            stickable = newPlatform;
        }

        private void UpdateScoreFields(IPlatform newPlatform) {
            int newPlatformIndex = newPlatform.Index;
            float xPosDifference = 1.5f - Mathf.Abs(newPlatform.transform.position.x - transform.position.x);
            int platformDifferential = newPlatformIndex - previousPlatformIndex;

            if (platformDifferential > 0) {
                float bounceMultiplier = Mathf.Pow(Player.BASE_POWER_FOR_BOUNCES, cachedBounces);
                float positionDifferenceMultiplier = Mathf.Pow(xPosDifference, Player.EXPONENT_FOR_PLATFORM_DIFFERENCE);
                previousScore = newPlatform.ScoreMultiplier * platformDifferential * bounceMultiplier *
                                positionDifferenceMultiplier;
                score += previousScore;
            }
            else if (platformDifferential < 0) {
                score -= previousScore;
                previousScore = 0;
            }

            cachedBounces = 0;
            previousPlatformIndex = newPlatformIndex;

            level.UpdateScore();
        }

        public void RequestDespawn() {
            level.EndGame();
        }

        private void OnCollisionEnter2D(Collision2D collision) {
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
            
            switch (collision.gameObject.tag) {
                case "Border": {
                    audioSource.PlayOneShot(bounceSound);
                    cachedBounces++;
                    break;
                }
            }
        }
    }
}