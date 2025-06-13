using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    private int score = 0;

    [Header("UI Reference")]
    [SerializeField] private TextMeshProUGUI scoreText;

    private void Awake()
    {
        // ΩÃ±€≈Ê ∆–≈œ
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void AddScore(int value)
    {
        score += value;
        UpdateScoreUI();
    }

    private void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = $"Score: {score}";
        }
    }

    public void ResetScore()
    {
        score = 0;
        UpdateScoreUI();
    }
}
