using System;
using UnityEngine;
using Game.Utility;
using Game.Utility.Prefab;

public class Platform : MonoBehaviour {

    private const float PROBABILITY_OF_MOVING_PLATFORM = 0.5f;
    private const float VELOCITY_AMPLIFIER = 2f;
    
    [SerializeField] private Rigidbody2D RB;

    private bool isMovingPlatform;
    private int direction;
    private int index;

    public int Index => index;

    public static Platform Create(Vector3 position, Quaternion rotation, int index) {
        Platform platform = Instantiate(PrefabContainer.PLATFORM, position, rotation);
        platform.index = index;
        
        platform.isMovingPlatform = Statistics.Probability(PROBABILITY_OF_MOVING_PLATFORM);
        if (platform.isMovingPlatform) {
            platform.direction = Statistics.FiftyPercentChance ? 1 : -1;
        }
        else {
            platform.direction = 0;
        }
        platform.Move();
        
        return platform;
    }

    private void Move() {
        if (index == 0) return;
        if (isMovingPlatform) {
            RB.linearVelocityX = direction * VELOCITY_AMPLIFIER;
        }
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