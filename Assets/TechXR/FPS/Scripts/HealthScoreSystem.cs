using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HealthScoreSystem : MonoBehaviour
{
    public FpsPlayerController FpsPlayerController;
    public TextMeshProUGUI Health;
    public TextMeshProUGUI Score;
    public TextMeshProUGUI HighScore;
    public int PlayerCurrentScore => m_Score;
    //
    private int m_Score;

    // Start is called before the first frame update
    void Start()
    {
        FpsPlayerController = FindObjectOfType<FpsPlayerController>();
        Health = transform.Find("Health").GetComponent<TextMeshProUGUI>();
        Score = transform.Find("Score").GetComponent<TextMeshProUGUI>();
        HighScore = transform.Find("HighScore").GetComponent<TextMeshProUGUI>();
        //
        m_Score = 0;
        UpdateScore(m_Score);
        CheckAndRegisterHighScore(m_Score);
    }

    private void UpdateHighScore(int score)
    {
        HighScore.text = "High Score - " + score.ToString();
    }

    public void UpdateHealth(float health, float initialHealth)
    {
        if (Health == null)
        {
            Health = transform.Find("Health").GetComponent<TextMeshProUGUI>();
        }
        Health.text = "Health - " + health.ToString() + "/" + initialHealth.ToString();
    }

    public void UpdateScore(int score)
    {
        m_Score += score;
        Score.text = "Score - " + m_Score.ToString();
    }

    public void CheckAndRegisterHighScore(int currentScore)
    {
        if (PlayerPrefs.HasKey("FPSHighScore"))
        {
            int highScore = PlayerPrefs.GetInt("FPSHighScore");
            if (currentScore > highScore)
            {
                PlayerPrefs.SetInt("FPSHighScore", currentScore);
                UpdateHighScore(currentScore);
            }
            else
            {
                UpdateHighScore(highScore);
            }
        }
        else
        {
            PlayerPrefs.SetInt("FPSHighScore", currentScore);
            UpdateHighScore(currentScore);
        }
    }

    public void ResetScore()
    {
        m_Score = 0;
        Score.text = "Score - " + m_Score.ToString();
    }
}
