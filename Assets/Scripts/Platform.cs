using UnityEngine;

public class Platform : MonoBehaviour {

    private const float MAX_DIFFERENCE = 6f;
    
    private bool hasPassed = false;
    private int velocityAmplifier;
    private Player player;
    private Level level;
    
    int Direction;
    private void Start() {
        if(Random.Range(1, 5) == 1) {
            velocityAmplifier = 2;
        }
        player = FindFirstObjectByType<Player>();

    }
    private void Update() {
        if (player.transform.position.y > transform.position.y && hasPassed == false) {
            level.CachedScore++;
            hasPassed = true;
        }
        else if (player.transform.position.y < transform.position.y && hasPassed == true) {
            level.CachedScore--;
            hasPassed = false;
        }
    }

    public static Platform Create(Platform prefab, Vector3 position, Quaternion rotation, Level level) {
        Platform platform = Instantiate(prefab, position, rotation);
        platform.level = level;
        return platform;
    }
/*
    public void checkDelete(float lowestCircle) {
        if (lowestCircle - transform.position.y > MAX_DIFFERENCE) {
            Destroy(gameObject);
        }
    }
    */
}