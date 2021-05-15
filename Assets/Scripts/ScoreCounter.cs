using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreCounter : MonoBehaviour
{
    public static ScoreCounter instance;

    [SerializeField] TextMeshProUGUI[] scoreVievers;
    [SerializeField] TextMeshProUGUI[] scoreMultipliers;

    public int Score { get; private set; }

    private void Awake()
    {
        if (ScoreCounter.instance == null)
            instance = this;
        else
        {
            Debug.LogError("ScoreCounter.instance already exist");
            Destroy(this.gameObject);
        }
    }

    public void AddScore(int additionalScore, int multiplier)
    {
        Score += (additionalScore * multiplier);
        UpdateScoreUI(multiplier);
    }
    public void ResetScore()
    {
        Score = 0;
        UpdateScoreUI(1);
    }

    private void UpdateScoreUI(int combo)
    {
        foreach (TextMeshProUGUI scoreUI in scoreVievers)
        {
            scoreUI.text = $"Score: {Score}";
        }
        foreach (TextMeshProUGUI multiplicatorUI in scoreMultipliers)
        {
            multiplicatorUI.text = $"X {combo}";
        }
    }
}
