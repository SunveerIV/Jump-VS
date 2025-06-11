using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip gameMusic;
    [SerializeField] private AudioClip failSong;

    public Level level;
        
    void Start() {
        DontDestroyOnLoad(gameObject);
    }

    void OnEnable() {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable() {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        audioSource.Stop();
        level = FindFirstObjectByType<Level>();
        
        switch (scene.name) {
            case "Core Game" : {
                audioSource.PlayOneShot(gameMusic);
                audioSource.loop = true;
                break;
            }

            case "Singleplayer End Screen" : {
                audioSource.PlayOneShot(failSong);
                audioSource.loop = true;
                break;
            }
        }
    }
}