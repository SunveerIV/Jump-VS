using UnityEngine;
using Unity.Netcode;
using Game.Behaviours.Directors;
using Game.Behaviours.Managers;
using Game.Behaviours.Platforms;
using Game.Interfaces;

namespace Game.Behaviours.Players {
    public class PlayerMultiplayer : NetworkBehaviour, ILaunchable {

        private const float VELOCITY_AMPLIFIER = 4f;
        private const float BASE_POWER_FOR_BOUNCES = 1.3f;
        private const float EXPONENT_FOR_PLATFORM_DIFFERENCE = 12f;
        
        [Header("Prefabs")] 
        [SerializeField] private LineDirector lineDirectorPrefab;

        [Header("Audio")] 
        [SerializeField] private AudioClip stickSound;
        [SerializeField] private AudioClip bounceSound;

        [Header("Components")] 
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private Rigidbody2D rb;
        
        //Network Variables
        public readonly NetworkVariable<bool> hasLost = new(false);
        
        //Cached References
        private IStickable stickable;
        
        private bool isAttachedToPlatform;
        private float minYToRaiseCamera;
        
        public static PlayerMultiplayer Create(PlayerMultiplayer prefab, Vector3 position, ulong clientID) {
            PlayerMultiplayer player = Instantiate(prefab, position, Quaternion.identity);
            player.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientID, true);
            return player;
        }

        public override void OnNetworkSpawn() {
            base.OnNetworkSpawn();
            PlayerMultiplayer[] players = FindObjectsByType<PlayerMultiplayer>(FindObjectsSortMode.None);
            for (int i = 0; i < players.Length; i++) {
                for (int j = i + 1; j < players.Length; j++) {
                    Physics2D.IgnoreCollision(players[i].GetComponent<Collider2D>(), players[j].GetComponent<Collider2D>());
                }
            }
            minYToRaiseCamera = float.MinValue;
        }

        private void Update() {
            RaiseCamera();
            InstantiateDirector();
            RemainStuckToPlatform();
        }

        private void RaiseCamera() {
            if (!IsOwner) return;
            if (transform.position.y < minYToRaiseCamera) return;

            Camera mainCamera = Camera.main;
            minYToRaiseCamera = transform.position.y;
            Vector3 currentCameraPos = mainCamera.transform.position;
            currentCameraPos.y = transform.position.y;
            mainCamera.transform.position = currentCameraPos;
        }
        
        private void InstantiateDirector() {
            if (!IsOwner) return;
            if (isAttachedToPlatform && Input.GetMouseButtonDown(0)) {
                LineDirector.Create(lineDirectorPrefab, this);
            }
        }

        private void RemainStuckToPlatform() {
            if (!IsOwner) return;
            RemainStuckToPlatformServerRpc();
        }
        
        [ServerRpc]
        private void RemainStuckToPlatformServerRpc() {
            if (stickable == null) return;
            
            Vector3 playerPos = transform.position;
            playerPos.x = stickable.transform.position.x;
            transform.position = playerPos;
        }
        
        public void Launch(Vector3 directorPosition) {
            isAttachedToPlatform = false;
            LaunchServerRpc(directorPosition);
        }

        [ServerRpc]
        private void LaunchServerRpc(Vector3 directorPosition) {
            stickable = null;
            rb.linearVelocity = (directorPosition - transform.position) * VELOCITY_AMPLIFIER;
        }
        
        private void StickToPlatform(PlatformMultiplayer newPlatform) {
            isAttachedToPlatform = true;
            StickToPlatformServerRpc(newPlatform.Index);
        }

        [ServerRpc]
        private void StickToPlatformServerRpc(int platformIndex) {
            
            rb.linearVelocity = Vector2.zero;
            PlatformMultiplayer newPlatform = FindFirstObjectByType<TwoPlayerLevel>().GetPlatformAtIndex(platformIndex);
            stickable = newPlatform;
            transform.position = new Vector2(newPlatform.transform.position.x, newPlatform.transform.position.y + 0.2f);
        }

        private void OnCollisionEnter2D(Collision2D collision) {
            if (!IsOwner) return;
            
            if (collision.gameObject.TryGetComponent(out PlatformMultiplayer newPlatform)) {
                if (transform.position.y <= newPlatform.transform.position.y) {
                    //cachedBounces++;
                }
                else if (!isAttachedToPlatform) {
                    audioSource.PlayOneShot(stickSound);
                    //UpdateScoreFields(newPlatform);
                    StickToPlatform(newPlatform);
                }
            }

            if (collision.gameObject.CompareTag("BottomCollider")) {
                Debug.Log("Client Requesting Despawn");
                RequestDespawnServerRpc();
            }
        }
        
        [ServerRpc]
        private void RequestDespawnServerRpc() {
            Debug.Log("Server Requesting Despawn");
            hasLost.Value = true;
            GetComponent<Rigidbody2D>().simulated = false;
            FindFirstObjectByType<TwoPlayerLevel>().EndGame();
        }

        [ClientRpc]
        public void EndGameClientRpc() {
            Debug.Log("Client attempting to end game");
            SceneLoader.LoadMultiplayerEndScreen();
        }
    }
    
}