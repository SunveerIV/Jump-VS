using UnityEngine;

public class Platform : MonoBehaviour {

    private const float MAX_DIFFERENCE = 6f;
    
    private bool hasPassed = false;
    private int velocityAmplifier;
    public Circle player;
    public Level level;
    
    int Direction;
    private void Start() {
        if(Random.Range(1, 5) == 1) {
            velocityAmplifier = 2;
        }
        player = FindFirstObjectByType<Circle>();

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
/*
    public void checkDelete(float lowestCircle) {
        if (lowestCircle - transform.position.y > MAX_DIFFERENCE) {
            Destroy(gameObject);
        }
    }
    */
}