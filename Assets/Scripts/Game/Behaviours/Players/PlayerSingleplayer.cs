using UnityEngine;
using Game.Settings;
using Game.Interfaces;
using Game.Behaviours.Platforms;
using Game.Behaviours.Directors;


namespace Game.Behaviours.Players {
    public class PlayerSingleplayer : MonoBehaviour, IPlayer, ILaunchable {
        
        public const float VELOCITY_AMPLIFIER = 4f;
        private const float BASE_POWER_FOR_BOUNCES = 1.3f;
        private const float EXPONENT_FOR_PLATFORM_DIFFERENCE = 12f;

        [Header("Prefabs")] 
        [SerializeField] private LineDirector lineDirectorPrefab;

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
        private float minYToRaiseCamera;

        public float Score => score;

        public static PlayerSingleplayer Create(PlayerSingleplayer prefab, Vector3 position, Quaternion rotation, ILevel level) {
            PlayerSingleplayer playerSingleplayer = Instantiate(prefab, position, rotation);
            playerSingleplayer.Initialize(level);
            return playerSingleplayer;
        }
        
        protected void Initialize(ILevel level) {
            this.level = level;
            mainCamera = Camera.main;
            minYToRaiseCamera = mainCamera.transform.position.y;
            isAttachedToPlatform = false;
            audioSource.volume = UserSettings.SoundEffectsVolume;
        }

        private void Update() {
            RaiseCamera();
            InstantiateDirector();
            RemainStuckToPlatform();
        }

        private void RaiseCamera() {
            if (transform.position.y >= minYToRaiseCamera) {
                minYToRaiseCamera = transform.position.y;
                Vector3 currentCameraPos = mainCamera.transform.position;
                currentCameraPos.y = transform.position.y;
                mainCamera.transform.position = currentCameraPos;
            }
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
            rb.linearVelocity = (directorPosition - transform.position) * VELOCITY_AMPLIFIER;
        }

        private void StickToPlatform(PlatformSingleplayer newPlatform) {
            rb.linearVelocity = Vector2.zero;
            transform.position = new Vector2(newPlatform.transform.position.x, newPlatform.transform.position.y + 0.2f);
            isAttachedToPlatform = true;
            stickable = newPlatform;
        }

        private void UpdateScoreFields(PlatformSingleplayer newPlatform) {
            int newPlatformIndex = newPlatform.Index;
            float xPosDifference = 1.5f - Mathf.Abs(newPlatform.transform.position.x - transform.position.x);
            int platformDifferential = newPlatformIndex - previousPlatformIndex;

            if (platformDifferential > 0) {
                float bounceMultiplier = Mathf.Pow(BASE_POWER_FOR_BOUNCES, cachedBounces);
                float positionDifferenceMultiplier = Mathf.Pow(xPosDifference, EXPONENT_FOR_PLATFORM_DIFFERENCE);
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
            switch (collision.gameObject.tag) {
                case "Platform": {
                    PlatformSingleplayer newPlatform = collision.gameObject.GetComponent<PlatformSingleplayer>();
                    if (transform.position.y <= newPlatform.transform.position.y) {
                        cachedBounces++;
                    }
                    else if (!isAttachedToPlatform) {
                        audioSource.PlayOneShot(stickSound);
                        UpdateScoreFields(newPlatform);
                        StickToPlatform(newPlatform);
                    }

                    break;
                }

                case "Border": {
                    audioSource.PlayOneShot(bounceSound);
                    cachedBounces++;
                    break;
                }
            }
        }
    }
}