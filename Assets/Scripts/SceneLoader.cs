using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour 
{
    private void Awake() {
        Application.targetFrameRate = 60;
    }

    public void LoadSingleplayer() {
        SceneManager.LoadScene("Core Game");
    }

    public void LoadStartMenu() {
        SceneManager.LoadScene(0);
    }
    public void QuitGame() {
        Application.Quit();
    }
}
