using UnityEngine;
using Game.Interfaces;
using Game.Settings;
using Game.Utility.Prefab;

public class Player : MonoBehaviour, ILaunchable {
    private const float VELOCITY_AMPLIFIER = 4f;
    
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
    }
    
    private void RaiseCamera() {
        if (transform.position.y >= minYToRaiseCamera) {
            minYToRaiseCamera = transform.position.y;
            mainCamera.transform.position = new Vector3(mainCamera.transform.position.x, transform.position.y, -1f);
        }
    }
    
    private void InstantiateDirector() {
        if (isAttachedToPlatform && Input.GetMouseButtonDown(0)) {
            Director.Create(transform.position, transform.rotation, this);
        }
    }
    public void Launch(Vector3 directorPosition) {
        isAttachedToPlatform = false;
        transform.SetParent(null);
        RB.linearVelocity = (directorPosition - transform.position) * VELOCITY_AMPLIFIER;
    }

    private void UpdateScoreFields(float newPlatformXPos, int newPlatformIndex) {
        float xPosDifference = 1.5f - Mathf.Abs(newPlatformXPos - transform.position.x);
        int platformDifferential = newPlatformIndex - previousPlatformIndex;
        
        if (platformDifferential > 0) {
            previousScore = platformDifferential * Mathf.Pow(1.3f, cachedBounces) * Mathf.Pow(xPosDifference, 12);
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
                    
                    UpdateScoreFields(newPlatform.transform.position.x, newPlatform.Index);
                    
                    RB.linearVelocity = Vector2.zero;
                    transform.position = new Vector2(newPlatform.transform.position.x, newPlatform.transform.position.y + 0.2f);
                    isAttachedToPlatform = true;
                    transform.SetParent(newPlatform.transform);
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
