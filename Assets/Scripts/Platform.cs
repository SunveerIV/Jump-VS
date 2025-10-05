using UnityEngine;
using Game.Utility;
using Game.Prefabs;
using Game.Interfaces;

public class Platform : MonoBehaviour, IStickable {

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
    
    public float ScoreMultiplier => isMovingPlatform ? Mathf.Pow(velocityAmplifier, SCORE_MULTIPLIER_EXPONENT) : DEFAULT_SCORE_MULTIPLIER;

    public static Platform Create(Vector3 position, Quaternion rotation, int index) {
        Platform platform = Instantiate(PrefabContainer.PLATFORM, position, rotation);
        platform.index = index;
        platform.velocityAmplifier = Random.Range(MIN_VELOCITY_AMPLIFIER, MAX_VELOCITY_AMPLIFIER);
        
        platform.isMovingPlatform = Statistics.Probability(PROBABILITY_OF_MOVING_PLATFORM);
        if (platform.isMovingPlatform) {
            platform.direction = Statistics.FiftyPercentChance ? 1 : -1;
        }
        platform.Move();
        
        return platform;
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