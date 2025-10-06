using UnityEngine;
using UnityEngine.SceneManagement;
using Game.Settings;

namespace Game.Behaviours.Managers {
    public class GameManager : MonoBehaviour {

        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip gameMusic;
        [SerializeField] private AudioClip failSong;

        public LevelSingleplayer level;

        private void Start() {
            DontDestroyOnLoad(gameObject);
        }

        private void OnEnable() {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDisable() {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
            audioSource.Stop();
            audioSource.volume = UserSettings.MusicVolume;
            level = FindFirstObjectByType<LevelSingleplayer>();

            switch (scene.name) {
                case "Core Game": {
                    PlaySong(gameMusic);
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