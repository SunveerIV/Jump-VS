using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Game.Prefabs;
using Game.Interfaces;
using Game.Behaviours.Players;
using Game.Behaviours.Platforms;

namespace Game.Behaviours.Managers {
    public class LevelSingleplayer : MonoBehaviour, ILevel {
        [SerializeField] private TextMeshProUGUI scoreText;
        [SerializeField] private TextMeshProUGUI FPSText;
        [SerializeField] private TextMeshProUGUI highScoreText;

        private const float MAX_DIFFERENCE = 6f;
        private const float PLAYER_START_Y = 1.6f;

        private List<PlayerBase> players;
        private List<Platform> platforms;

        private float highestPlatform;

        private int platformIndex = 0;

        public static LevelSingleplayer Create() {
            LevelSingleplayer level = Instantiate(PrefabContainer.LEVEL_SINGLEPLAYER);
            level.platforms = new List<Platform>();
            level.highestPlatform = -1f;
            level.InstantiatePlatform();
            float playerStartPosX = level.platforms[0].transform.position.x;
            level.players = new List<PlayerBase>();
            PlayerBase player = PlayerSingleplayer.Create(new Vector2(playerStartPosX, PLAYER_START_Y), Quaternion.identity, level);
            level.players.Add(player);
            level.StartCoroutine(level.UpdateEverySecond());
            level.highScoreText.text = "High Score: " + PlayerPrefs.GetFloat("High Score", 0);
            return level;
        }

        private IEnumerator UpdateEverySecond() {
            while (true) {
                UpdateFPS();
                UpdatePlatforms();
                yield return new WaitForSeconds(1);
            }
        }

        private void UpdateFPS() {
            FPSText.text = Mathf.Round(1 / Time.deltaTime).ToString();
        }

        private void UpdatePlatforms() {
            float highestCircle = float.MinValue;
            float lowestCircle = float.MaxValue;
            foreach (PlayerBase player in players) {
                if (player.transform.position.y > highestCircle) {
                    highestCircle = player.transform.position.y;
                }

                if (player.transform.position.y < lowestCircle) {
                    lowestCircle = player.transform.position.y;
                }
            }

            while (highestCircle + 15f > highestPlatform) {
                InstantiatePlatform();
            }

            for (int i = platforms.Count - 1; i >= 0; i--) {
                Platform platform = platforms[i];
                if (lowestCircle - platform.transform.position.y > MAX_DIFFERENCE) {
                    Destroy(platform.gameObject);
                    platforms.RemoveAt(i);
                }
            }
        }

        private void InstantiatePlatform() {
            highestPlatform += 2f;
            Platform platform = Platform.Create(new Vector2(UnityEngine.Random.Range(-2f, 2f), highestPlatform),
                Quaternion.identity, platformIndex);
            platformIndex++;
            platforms.Add(platform);
        }

        public void UpdateScore() {
            scoreText.text = players[0].Score.ToString();
        }

        public void EndGame() {
            if (players[0].Score > PlayerPrefs.GetFloat("High Score")) {
                PlayerPrefs.SetFloat("High Score", players[0].Score);
            }

            SceneLoader.LoadSingleplayerEndScreen();
        }
    }
}