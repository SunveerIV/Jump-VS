using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour {
    private const string GAME_SCENE_NAME = "Core Game";
    
    private void Awake() {
        Application.targetFrameRate = 60;
    }

    public static void LoadGame() {
        SceneManager.LoadScene(GAME_SCENE_NAME);
    }

    public static void LoadStartMenu() {
        SceneManager.LoadScene(0);
    }
    
    public static void QuitGame() {
        Application.Quit();
    }
}
