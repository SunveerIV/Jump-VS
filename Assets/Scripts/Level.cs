using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;

public class Level : MonoBehaviour {
    [SerializeField] private Platform platformGameObject;
    [SerializeField] private Circle circleGameObject;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI FPSText;
    [SerializeField] private TextMeshProUGUI highScoreText;
    
    

    private const float MAX_DIFFERENCE = 6f;
    private const float PLAYER_START_Y = 1.6f;
    
    private List<Circle> players;
    private List<Platform> platforms;
    
    private float highestPlatform;
    private float score = 0;
    
    public int CachedScore = 0;
    public int CachedBounces = 0;

    private float lastScore;

    private void Start() {
        platforms = new List<Platform>();
        highestPlatform = -1f;
        InstantiatePlatform();
        float playerStartPosX = platforms[0].transform.position.x;
        players = new List<Circle>();
        Circle player = (Instantiate(circleGameObject, new Vector2(playerStartPosX, PLAYER_START_Y), Quaternion.identity));
        player.level = this;
        players.Add(player);
        StartCoroutine(UpdateEverySecond());
        highScoreText.text = "High Score: " + PlayerPrefs.GetFloat("High Score", 0);
    }
    
    private IEnumerator UpdateEverySecond() {
        while(true) {
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
        foreach (Circle player in players) {
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
        Platform plat = Instantiate(platformGameObject, new Vector2(UnityEngine.Random.Range(-2f, 2f), highestPlatform), Quaternion.identity);
        plat.level = this;
        platforms.Add(plat);
    }

    public void UpdateScore(float difference) {

        if (CachedScore > 0) {
            lastScore = CachedScore * Mathf.Pow(1.3f, CachedBounces) * Mathf.Pow(difference, 12);
            score += lastScore;
        } else if(CachedScore < 0) {
            score -= lastScore;
            lastScore = 0;
        }
        scoreText.text = score.ToString();
        CachedScore = 0;
        CachedBounces = 0;
    }

    public void EndSingleplayer() {
        if (score > PlayerPrefs.GetFloat("High Score")) {
            PlayerPrefs.SetFloat("High Score", score);
        }
        SceneManager.LoadScene("Singleplayer End Screen");
    }
}