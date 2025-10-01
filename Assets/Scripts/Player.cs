using UnityEngine;
using Game.Interfaces;
using Game.Settings;

public class Player : MonoBehaviour, ILaunchable {
    private const float VELOCITY_AMPLIFIER = 4f;
    
    [Header("Prefabs")]
    [SerializeField] private Director director;
    
    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip stickSound;
    [SerializeField] private AudioClip bounceSound;
    [SerializeField] private AudioClip failSong;
    
    [Header("Components")]
    [SerializeField] private Rigidbody2D RB;
    
    private Level level;
    private Camera mainCamera;
    
    private bool hasStuck;
    private int previousPlatformIndex;
    private int cachedBorderBounces;
    private float minY;
    private float score;
    private float lastScore;

    public float Score => score;
    
    public static Player Create(Player prefab, Vector3 position, Quaternion rotation, Level level) {
        Player player = Instantiate(prefab, position, rotation);
        player.level = level;
        player.mainCamera = Camera.main;
        player.minY = player.mainCamera.transform.position.y;
        player.hasStuck = false;
        player.audioSource.volume = UserSettings.SoundEffectsVolume;
        return player;
    }

    private void Update() {
        if (hasStuck && Input.GetMouseButtonDown(0)) {
            Director.Create(director, transform.position, transform.rotation, this);
        }

        if (transform.position.y >= minY) {
            minY = transform.position.y;
            mainCamera.transform.position = new Vector3(mainCamera.transform.position.x, transform.position.y, -1f);
        }
    }

    private void UpdateScore(float xPosDifference, int platformDifferential) {
        
        if (platformDifferential > 0) {
            lastScore = platformDifferential * Mathf.Pow(1.3f, cachedBorderBounces) * Mathf.Pow(xPosDifference, 12);
            score += lastScore;
        } else if(platformDifferential < 0) {
            score -= lastScore;
            lastScore = 0;
        }

        level.UpdateScore();
    }
    
    private void OnCollisionEnter2D(Collision2D collision) {
        switch (collision.gameObject.tag) {
            case "Platform": {
                Platform platform = collision.gameObject.GetComponent<Platform>();
                if (transform.position.y <= platform.transform.position.y) {
                    cachedBorderBounces++;
                } else if (!hasStuck) {
                    audioSource.PlayOneShot(stickSound);
                    
                    int platformIndex = platform.Index;
                    UpdateScore(1.5f - Mathf.Abs(platform.transform.position.x - transform.position.x), platformIndex - previousPlatformIndex);
                    previousPlatformIndex = platformIndex;
                    cachedBorderBounces = 0;
                    
                    RB.linearVelocity = Vector2.zero;
                    transform.position = new Vector2(platform.transform.position.x, platform.transform.position.y + 0.2f);
                    hasStuck = true;
                    transform.SetParent(platform.transform);
                }
                break;
            }

            case "Border": {
                audioSource.PlayOneShot(bounceSound);
                cachedBorderBounces++;
                break;
            }

            case "BottomCollider": {
                level.EndGame();
                break;
            }
        }
    }

    public void Launch(Vector3 directorPosition) {
        hasStuck = false;
        transform.SetParent(null);
        RB.linearVelocity = (directorPosition - transform.position) * VELOCITY_AMPLIFIER;
    }
}
