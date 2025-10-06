using UnityEngine;
using UnityEngine.SceneManagement;
using Game.Settings;

namespace Game.Behaviours.Managers {
    public class GameManager : MonoBehaviour {

        public static GameManager Singleton { get; private set; }

        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip gameMusic;
        [SerializeField] private AudioClip failSong;

        private LevelSingleplayer level;

        private void Awake() {
            if (Singleton != null) {
                Destroy(gameObject);
                return;
            }
            Singleton = this;
            DontDestroyOnLoad(gameObject);
            AudioListener.volume = UserSettings.MasterVolume;
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
            level = FindFirstObjectByType<LevelSingleplayer>();

            switch (scene.name) {
                case "Core Game": {
                    PlaySong(gameMusic);
                    LevelSingleplayer.Create();
                    break;
                }

                case "Singleplayer End Screen": {
                    PlaySong(failSong);
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