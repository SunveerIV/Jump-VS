using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;
using Game.Utility;
using Game.Settings;

namespace Game.Behaviours.Managers {
    public class GameManager : MonoBehaviour {

        public static GameManager Singleton { get; private set; }

        [Header("Prefabs")]
        [SerializeField] private LevelSingleplayer levelSingleplayerPrefab;
        [SerializeField] private NetworkManager twoPlayerNetworkManagerPrefab;
        
        [Header("Audio")]
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip gameMusic;
        [SerializeField] private AudioClip failSong;

        private GameMode gameMode;

        public GameMode GameMode {
            set => gameMode = value;
        }

        private void Awake() {
            if (Singleton != null) {
                Destroy(gameObject);
                return;
            }
            Singleton = this;
            DontDestroyOnLoad(gameObject);
            GameState.SetOrientationLock();
        }

        private void OnEnable() {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDisable() {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
            Application.targetFrameRate = 60;
            audioSource.Stop();
            audioSource.volume = UserSettings.MusicVolume;

            switch (scene.name) {
                case "Core Game": {
                    LoadGame();
                    break;
                }

                case "Singleplayer End Screen": {
                    PlaySong(failSong);
                    break;
                }
            }
        }

        private void LoadGame() {
            PlaySong(gameMusic);
            switch (gameMode) {
                case GameMode.Singleplayer: {
                    LevelSingleplayer.Create(levelSingleplayerPrefab);
                    break;
                }
                case GameMode.Multiplayer: {
                    Instantiate(twoPlayerNetworkManagerPrefab);
                    break;
                }
            }
            
        }

        private void PlaySong(AudioClip song) {
            audioSource.PlayOneShot(song);
            audioSource.loop = true;
        }
    }
}