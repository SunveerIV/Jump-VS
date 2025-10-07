using UnityEngine;
using Unity.Netcode;
using Game.Behaviours.Directors;
using Game.Behaviours.Platforms;
using Game.Interfaces;

namespace Game.Behaviours.Players {
    public class PlayerMultiplayer : NetworkBehaviour, ILaunchable {

        private const float VELOCITY_AMPLIFIER = 4f;
        private const float BASE_POWER_FOR_BOUNCES = 1.3f;
        private const float EXPONENT_FOR_PLATFORM_DIFFERENCE = 12f;
        
        [Header("Prefabs")] 
        [SerializeField] private Director directorPrefab;

        [Header("Audio")] 
        [SerializeField] private AudioClip stickSound;
        [SerializeField] private AudioClip bounceSound;

        [Header("Components")] 
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private Rigidbody2D rb;
        
        //Cached References
        private IStickable stickable;
        
        private bool isAttachedToPlatform;
        
        public static PlayerMultiplayer Create(PlayerMultiplayer prefab, Vector3 position, ulong clientID) {
            PlayerMultiplayer player = Instantiate(prefab, position, Quaternion.identity);
            player.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientID, true);
            return player;
        }

        private void Update() {
            if (!IsOwner) return;
            InstantiateDirector();
            RemainStuckToPlatform();
        }
        
        private void InstantiateDirector() {
            if (isAttachedToPlatform && Input.GetMouseButtonDown(0)) {
                Director.Create(directorPrefab, this);
            }
        }
        
        private void RemainStuckToPlatform() {
            if (stickable == null) return;
            
            Vector3 playerPos = transform.position;
            playerPos.x = stickable.transform.position.x;
            transform.position = playerPos;
        }
        
        public void Launch(Vector3 directorPosition) {
            isAttachedToPlatform = false;
            stickable = null;
            rb.linearVelocity = (directorPosition - transform.position) * VELOCITY_AMPLIFIER;
        }
        
        private void StickToPlatform(PlatformBase newPlatform) {
            rb.linearVelocity = Vector2.zero;
            transform.position = new Vector2(newPlatform.transform.position.x, newPlatform.transform.position.y + 0.2f);
            isAttachedToPlatform = true;
            stickable = newPlatform;
        }

        private void OnCollisionEnter2D(Collision2D collision) {
            if (collision.gameObject.TryGetComponent(out PlatformBase newPlatform)) {
                if (transform.position.y <= newPlatform.transform.position.y) {
                    //cachedBounces++;
                }
                else if (!isAttachedToPlatform) {
                    audioSource.PlayOneShot(stickSound);
                    //UpdateScoreFields(newPlatform);
                    StickToPlatform(newPlatform);
                }
            }
        }
    }
}