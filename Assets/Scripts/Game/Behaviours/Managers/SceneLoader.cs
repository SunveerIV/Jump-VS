using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.Behaviours.Managers {
    public class SceneLoader : MonoBehaviour {
        private const string GAME_SCENE_NAME = "Core Game";
        private const string SINGLEPLAYER_END_SCREEN_NAME = "Singleplayer End Screen";

        private void Awake() {
            Application.targetFrameRate = 60;
        }

        public static void LoadGame() {
            SceneManager.LoadScene(GAME_SCENE_NAME);
        }

        public static void LoadSingleplayerEndScreen() {
            SceneManager.LoadScene(SINGLEPLAYER_END_SCREEN_NAME);
        }

        public static void LoadStartMenu() {
            SceneManager.LoadScene(0);
        }

        public static void QuitGame() {
            Application.Quit();
        }
    }
}