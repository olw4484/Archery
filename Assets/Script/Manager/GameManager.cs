using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private ArrowBucket arrowBucket;
    [SerializeField] private GameResultUI resultUI; // 결과 출력용 UI

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

        Debug.Log($"[GameManager] 게임 종료. 최종 점수: {finalScore}");

        if (resultUI != null)
        {
            resultUI.ShowResult(finalScore);
        }
        else
        {
            Debug.Log("게임 종료 UI가 설정되어 있지 않습니다.");
        }
    }

    public void RestartGame()
    {
        Debug.Log("[GameManager] Restarting game (soft reset)");

        // 점수 초기화
        ScoreManager.Instance?.ResetScore();

        // 화살 리셋
        arrowBucket?.ResetBucket();

        // UI 숨기기
        resultUI?.HideResult();

        // 바람 랜덤화
        WindManager.Instance?.RandomizeWind();

        // 게임 상태 리셋
        gameEnded = false;
    }
}
