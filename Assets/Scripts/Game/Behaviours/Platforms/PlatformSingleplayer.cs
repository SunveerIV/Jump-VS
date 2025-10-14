using UnityEngine;
using Game.Utility;
using Game.Constants;
using Game.Interfaces;

namespace Game.Behaviours.Platforms {
    public class PlatformSingleplayer : MonoBehaviour, IPlatform {

        [SerializeField] private Rigidbody2D RB;

        private float velocityAmplifier;
        private bool isMovingPlatform;
        private int direction;
        private ushort index;

        public ushort Index => index;

        public float ScoreMultiplier {
            get {
                if (index == 0) return Platform.DEFAULT_SCORE_MULTIPLIER;
                float velocityBonus = isMovingPlatform ? Mathf.Pow(velocityAmplifier, Platform.SCORE_MULTIPLIER_EXPONENT) : Platform.DEFAULT_SCORE_MULTIPLIER;
                return velocityBonus;
            }
        }
        
        public static PlatformSingleplayer Create(PlatformSingleplayer prefab, Vector3 position, Quaternion rotation, ushort index) {
            PlatformSingleplayer platform = Instantiate(prefab, position, rotation);
            platform.index = index;
            platform.velocityAmplifier = Random.Range(Platform.MIN_VELOCITY_AMPLIFIER, Platform.MAX_VELOCITY_AMPLIFIER);

            platform.isMovingPlatform = Statistics.Probability(Platform.PROBABILITY_OF_MOVING_PLATFORM);
            if (platform.isMovingPlatform) {
                platform.direction = Statistics.FiftyPercentChance ? 1 : -1;
            }

            platform.Move();
            return platform;
        }

        private void Move() {
            if (index == 0) return;
            if (!isMovingPlatform) return;
            
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