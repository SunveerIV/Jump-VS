using UnityEngine;
using Unity.Netcode;
using Game.Utility;
using Game.Interfaces;

namespace Game.Behaviours.Platforms {
    public abstract class PlatformBase : NetworkBehaviour, IStickable {

        private const float PROBABILITY_OF_MOVING_PLATFORM = 0.35f;
        private const float DEFAULT_SCORE_MULTIPLIER = 1f;
        private const float SCORE_MULTIPLIER_EXPONENT = 3f;
        private const float MIN_VELOCITY_AMPLIFIER = 1f;
        private const float MAX_VELOCITY_AMPLIFIER = 4f;

        [SerializeField] private Rigidbody2D RB;

        private float velocityAmplifier;
        private bool isMovingPlatform;
        private int direction;
        private int index;

        public int Index => index;

        public float ScoreMultiplier {
            get {
                if (index == 0) return DEFAULT_SCORE_MULTIPLIER;
                float velocityBonus = isMovingPlatform ? Mathf.Pow(velocityAmplifier, SCORE_MULTIPLIER_EXPONENT) : DEFAULT_SCORE_MULTIPLIER;
                return velocityBonus;
            }
        }

        protected void Initialize(int index) {
            this.index = index;
            velocityAmplifier = Random.Range(MIN_VELOCITY_AMPLIFIER, MAX_VELOCITY_AMPLIFIER);

            isMovingPlatform = Statistics.Probability(PROBABILITY_OF_MOVING_PLATFORM);
            if (isMovingPlatform) {
                direction = Statistics.FiftyPercentChance ? 1 : -1;
            }

            Move();
        }

        private void Move() {
            if (index == 0 || !isMovingPlatform) return;
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