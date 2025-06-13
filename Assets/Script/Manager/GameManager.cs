using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private ArrowBucket arrowBucket;
    [SerializeField] private GameResultUI resultUI; // ��� ��¿� UI

    private bool gameEnded = false;

    private void Update()
    {
        if (gameEnded) return;

        if (arrowBucket != null && arrowBucket.IsDepleted())
        {
            gameEnded = true;
            EndGame();
        }
    }

    private void EndGame()
    {
        int finalScore = ScoreManager.Instance?.GetScore() ?? 0;

        Debug.Log($"[GameManager] ���� ����. ���� ����: {finalScore}");

        if (resultUI != null)
        {
            resultUI.ShowResult(finalScore);
        }
        else
        {
            Debug.Log("���� ���� UI�� �����Ǿ� ���� �ʽ��ϴ�.");
        }
    }

    public void RestartGame()
    {
        Debug.Log("[GameManager] Restarting game (soft reset)");

        // ���� �ʱ�ȭ
        ScoreManager.Instance?.ResetScore();

        // ȭ�� ����
        arrowBucket?.ResetBucket();

        // UI �����
        resultUI?.HideResult();

        // �ٶ� ����ȭ
        WindManager.Instance?.RandomizeWind();

        // ���� ���� ����
        gameEnded = false;
    }
}
