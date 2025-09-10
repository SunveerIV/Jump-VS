using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip gameMusic;
    [SerializeField] private AudioClip failSong;

    public Level level;
        
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
        level = FindFirstObjectByType<Level>();
        
        switch (scene.name) {
            case "Core Game" : {
                PlaySong(gameMusic);
                break;
            }

            case "Singleplayer End Screen" : {
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