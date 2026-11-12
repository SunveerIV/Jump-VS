using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;

namespace Game.Behaviours.Managers {
    public class MonoNetworkManager : MonoBehaviour {

        private static MonoNetworkManager Singleton;
        
        private NetworkManager networkManager;

        private void Awake() {
            networkManager = GetComponent<NetworkManager>();
        }
        
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
            if (Singleton != this && Singleton != null) {
                Destroy(gameObject);
                return;
            }
            Singleton = this;
            if (!scene.name.Equals("Core Game")) networkManager.Shutdown();
        }
        
        private void OnEnable() {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDisable() {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }
}