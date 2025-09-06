using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.Serialization;

public class Level : MonoBehaviour {
    [SerializeField] private Platform platformGameObject;
    [SerializeField] private Player playerGameObject;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI FPSText;
    [SerializeField] private TextMeshProUGUI highScoreText;
    
    

    private const float MAX_DIFFERENCE = 6f;
    private const float PLAYER_START_Y = 1.6f;
    
    private List<Player> players;
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
        players = new List<Player>();
        Player player = Player.Create(playerGameObject, new Vector2(playerStartPosX, PLAYER_START_Y), Quaternion.identity, this);
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
        foreach (Player player in players) {
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
        Platform platform = Platform.Create(platformGameObject, new Vector2(UnityEngine.Random.Range(-2f, 2f), highestPlatform), Quaternion.identity, this);
        platforms.Add(platform);
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

    public void EndGame() {
        if (score > PlayerPrefs.GetFloat("High Score")) {
            PlayerPrefs.SetFloat("High Score", score);
        }
        SceneManager.LoadScene("Singleplayer End Screen");
    }
}