using UnityEngine;
using Unity.Netcode;
using Game.Utility;
using Game.Constants;
using Game.Interfaces;

namespace Game.Behaviours.Platforms {
    public class PlatformMultiplayer : NetworkBehaviour, IPlatform {

        

        [SerializeField] private Rigidbody2D RB;

        private NetworkVariable<ushort> index = new(0);
        
        private float velocityAmplifier;
        private bool isMovingPlatform;
        private int direction;

        public ushort Index => index.Value;

        public float ScoreMultiplier {
            get {
                if (index.Value == 0) return Platform.DEFAULT_SCORE_MULTIPLIER;
                float velocityBonus = isMovingPlatform ? Mathf.Pow(velocityAmplifier, Platform.SCORE_MULTIPLIER_EXPONENT) : Platform.DEFAULT_SCORE_MULTIPLIER;
                return velocityBonus;
            }
        }
        
        public static PlatformMultiplayer Create(PlatformMultiplayer prefab, Vector3 position, ushort index) {
            PlatformMultiplayer platform = Instantiate(prefab, position, Quaternion.identity);
            platform.index.Value = index;
            platform.velocityAmplifier = Random.Range(Platform.MIN_VELOCITY_AMPLIFIER, Platform.MAX_VELOCITY_AMPLIFIER);

            platform.isMovingPlatform = Statistics.Probability(Platform.PROBABILITY_OF_MOVING_PLATFORM);
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