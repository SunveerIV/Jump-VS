using Game.Interfaces;
using Game.Utility;
using Unity.Netcode;
using UnityEngine;

namespace Game.Behaviours.Platforms {
    public class PlatformMultiplayer : NetworkBehaviour, IPlatform {

        private const float PROBABILITY_OF_MOVING_PLATFORM = 0.35f;
        private const float DEFAULT_SCORE_MULTIPLIER = 1f;
        private const float SCORE_MULTIPLIER_EXPONENT = 3f;
        private const float MIN_VELOCITY_AMPLIFIER = 1f;
        private const float MAX_VELOCITY_AMPLIFIER = 4f;

        [SerializeField] private Rigidbody2D RB;

        private NetworkVariable<int> index = new(0);
        
        private float velocityAmplifier;
        private bool isMovingPlatform;
        private int direction;

        public int Index => index.Value;

        public float ScoreMultiplier {
            get {
                if (index.Value == 0) return DEFAULT_SCORE_MULTIPLIER;
                float velocityBonus = isMovingPlatform ? Mathf.Pow(velocityAmplifier, SCORE_MULTIPLIER_EXPONENT) : DEFAULT_SCORE_MULTIPLIER;
                return velocityBonus;
            }
        }
        
        public static PlatformMultiplayer Create(PlatformMultiplayer prefab, Vector3 position, int index) {
            PlatformMultiplayer platform = Instantiate(prefab, position, Quaternion.identity);
            platform.index.Value = index;
            platform.velocityAmplifier = Random.Range(MIN_VELOCITY_AMPLIFIER, MAX_VELOCITY_AMPLIFIER);

            platform.isMovingPlatform = Statistics.Probability(PROBABILITY_OF_MOVING_PLATFORM);
            if (platform.isMovingPlatform) {
                platform.direction = Statistics.FiftyPercentChance ? 1 : -1;
            }

            platform.GetComponent<NetworkObject>().Spawn();
            platform.Move();
            return platform;
        }
        
        private void Move() {
            if (index.Value == 0 || !isMovingPlatform) return;
            RB.linearVelocityX = direction * velocityAmplifier;
        }

        private void OnCollisionEnter2D(Collision2D other) {
            switch (other.collider.name) {
                case "LeftBorder": {
                    direction = 1;
                    Move();
                    break;
                }
                case "RightBorder": {
                    direction = -1;
                    Move();
                    break;
                }
            }
        }
    }
}