using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.UI;
using Game.Interfaces;
using Game.Behaviours.Players;
using Game.Behaviours.Platforms;

namespace Game.Behaviours.Managers {
    public class LevelSingleplayer : MonoBehaviour, ILevel {

        private const float MAX_DIFFERENCE = 6f;
        private const float PLAYER_START_Y = 1.6f;

        [SerializeField] private PlatformSingleplayer platformSingleplayerPrefab;
        [SerializeField] private PlayerSingleplayer playerSingleplayerPrefab;
        [SerializeField] private SingleplayerCanvas singleplayerCanvasPrefab;

        private SingleplayerCanvas gui;
        
        private List<PlayerSingleplayer> players;
        private List<PlatformSingleplayer> platforms;

        private float highestPlatform;

        private int platformIndex = 0;

        public static LevelSingleplayer Create(LevelSingleplayer prefab) {
            LevelSingleplayer level = Instantiate(prefab);
            return level;
        }

        private void Start() {
            gui = SingleplayerCanvas.Create(singleplayerCanvasPrefab);
            platforms = new List<PlatformSingleplayer>();
            highestPlatform = -1f;
            InstantiatePlatform();
            float playerStartPosX = platforms[0].transform.position.x;
            players = new List<PlayerSingleplayer>();
            PlayerSingleplayer player = PlayerSingleplayer.Create(playerSingleplayerPrefab, new Vector2(playerStartPosX, PLAYER_START_Y), Quaternion.identity, this);
            players.Add(player);
            StartCoroutine(UpdateEverySecond());
        }

        private IEnumerator UpdateEverySecond() {
            while (true) {
                UpdatePlatforms();
                yield return new WaitForSeconds(1);
            }
        }

        private void UpdatePlatforms() {
            float highestCircle = float.MinValue;
            float lowestCircle = float.MaxValue;
            foreach (PlayerSingleplayer player in players) {
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
                PlatformSingleplayer platform = platforms[i];
                if (lowestCircle - platform.transform.position.y > MAX_DIFFERENCE) {
                    Destroy(platform.gameObject);
                    platforms.RemoveAt(i);
                }
            }
        }

        private void InstantiatePlatform() {
            highestPlatform += 2f;
            PlatformSingleplayer platform = PlatformSingleplayer.Create(platformSingleplayerPrefab, new Vector2(Random.Range(-2f, 2f), highestPlatform),
                Quaternion.identity, platformIndex);
            platformIndex++;
            platforms.Add(platform);
        }

        public void UpdateScore() {
            gui.ScoreText = players[0].Score;
        }

        public void EndGame() {
            if (players[0].Score > PlayerPrefs.GetFloat("High Score")) {
                PlayerPrefs.SetFloat("High Score", players[0].Score);
            }

            SceneLoader.LoadSingleplayerEndScreen();
        }
    }
}