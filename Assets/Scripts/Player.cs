using UnityEngine;
using Game.Prefabs;
using Game.Settings;
using Game.Interfaces;

public class Player : MonoBehaviour, ILaunchable {
    
    private const float VELOCITY_AMPLIFIER = 4f;
    private const float BASE_POWER_FOR_BOUNCES = 1.3f;
    private const float EXPONENT_FOR_PLATFORM_DIFFERENCE = 12f;
    
    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip stickSound;
    [SerializeField] private AudioClip bounceSound;
    [SerializeField] private AudioClip failSong;
    
    [Header("Components")]
    [SerializeField] private Rigidbody2D RB;
    
    //Cached References
    private Level level;
    private Camera mainCamera;
    private IStickable stickable;
    
    //Scoring
    private float score;
    private float previousScore;
    private int previousPlatformIndex;
    private int cachedBounces;
    
    private bool isAttachedToPlatform;
    private float minYToRaiseCamera;

    public float Score => score;
    
    public static Player Create(Vector3 position, Quaternion rotation, Level level) {
        Player player = Instantiate(PrefabContainer.PLAYER, position, rotation);
        player.level = level;
        player.mainCamera = Camera.main;
        player.minYToRaiseCamera = player.mainCamera.transform.position.y;
        player.isAttachedToPlatform = false;
        player.audioSource.volume = UserSettings.SoundEffectsVolume;
        return player;
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
            Director.Create(this);
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
        RB.linearVelocity = (directorPosition - transform.position) * VELOCITY_AMPLIFIER;
    }

    private void StickToPlatform(Platform newPlatform) {
        RB.linearVelocity = Vector2.zero;
        transform.position = new Vector2(newPlatform.transform.position.x, newPlatform.transform.position.y + 0.2f);
        isAttachedToPlatform = true;
        stickable = newPlatform;
    }
    
    private void UpdateScoreFields(Platform newPlatform) {
        int newPlatformIndex = newPlatform.Index;
        float xPosDifference = 1.5f - Mathf.Abs(newPlatform.transform.position.x - transform.position.x);
        int platformDifferential = newPlatformIndex - previousPlatformIndex;
        
        if (platformDifferential > 0) {
            float bounceMultiplier = Mathf.Pow(BASE_POWER_FOR_BOUNCES, cachedBounces);
            float positionDifferenceMultiplier = Mathf.Pow(xPosDifference, EXPONENT_FOR_PLATFORM_DIFFERENCE);
            previousScore = newPlatform.ScoreMultiplier * platformDifferential * bounceMultiplier * positionDifferenceMultiplier;
            score += previousScore;
        } else if(platformDifferential < 0) {
            score -= previousScore;
            previousScore = 0;
        }
        
        cachedBounces = 0;
        previousPlatformIndex = newPlatformIndex;

        level.UpdateScore();
    }
    
    private void OnCollisionEnter2D(Collision2D collision) {
        switch (collision.gameObject.tag) {
            case "Platform": {
                Platform newPlatform = collision.gameObject.GetComponent<Platform>();
                if (transform.position.y <= newPlatform.transform.position.y) {
                    cachedBounces++;
                } else if (!isAttachedToPlatform) {
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

            case "BottomCollider": {
                level.EndGame();
                break;
            }
        }
    }
}
