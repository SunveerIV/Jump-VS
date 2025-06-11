using UnityEngine;
using UnityEngine.SceneManagement;

public class Circle : MonoBehaviour {
    public Rigidbody2D RB;
    [SerializeField] private float minY;
    [SerializeField] private Director director;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip stickSound;
    [SerializeField] private AudioClip bounceSound;
    [SerializeField] private AudioClip failSong;

    private Camera mainCamera;
    
    public Level level;
    
    public bool hasStuck = false;


    private void Start() {
        mainCamera = Camera.main;
        minY = mainCamera.transform.position.y;
    }

    private void Update() {

        if (hasStuck && Input.GetMouseButtonDown(0)) {
            Director tempDirector = Instantiate(director, transform.position, Quaternion.identity);
            tempDirector.transform.SetParent(transform);
            tempDirector.player = this;
        } 
        
        if (transform.position.y >= minY) {
            minY = transform.position.y;
            mainCamera.transform.position = new Vector3(mainCamera.transform.position.x, transform.position.y, -1f);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.CompareTag("Platform")) {
            if (transform.position.y > collision.transform.position.y && hasStuck == false) {
                audioSource.PlayOneShot(stickSound);
                level.UpdateScore(1.5f - Mathf.Abs(collision.transform.position.x - transform.position.x));
                RB.linearVelocity = Vector2.zero;
                transform.position = new Vector2(collision.transform.position.x, collision.transform.position.y + 0.2f);
                hasStuck = true;
                LockToPlatform(collision.gameObject);
            }
        } else if (collision.gameObject.CompareTag("Border")) {
            audioSource.PlayOneShot(bounceSound);
            level.CachedBounces += 1;
        } else if (collision.gameObject.CompareTag("BottomCollider")) {
            level.EndSingleplayer();
        }
    }

    public void LockToPlatform(GameObject platform) {
        transform.SetParent(platform.transform);
    }
}
