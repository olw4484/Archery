using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GameResultUI : MonoBehaviour
{
    [SerializeField] private CanvasGroup resultPanel;
    [SerializeField] private TextMeshProUGUI resultText;
    [SerializeField] private Button retryButton;

    private void Awake()
    {
        resultPanel.alpha = 0;
        resultPanel.interactable = false;
        resultPanel.blocksRaycasts = false;

        retryButton.onClick.AddListener(OnRetry);
    }

    public void ShowResult(int score)
    {
        resultText.text = $"Score: {score}";

        resultPanel.alpha = 1;
        resultPanel.interactable = true;
        resultPanel.blocksRaycasts = true;
    }
    public void HideResult()
    {
        resultPanel.alpha = 0;
        resultPanel.interactable = false;
        resultPanel.blocksRaycasts = false;
    }
    private void OnRetry()
    {
        GameManager gm = FindObjectOfType<GameManager>();
        if (gm != null)
        {
            gm.RestartGame();
        }
    }
}
