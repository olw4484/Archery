using UnityEngine;

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
        //int finalScore = ScoreManager.Instance?.GetScore() ?? 0;

        //Debug.Log($"[GameManager] 게임 종료. 최종 점수: {finalScore}");

        if (resultUI != null)
        {
            //resultUI.ShowResult(finalScore);
        }
        else
        {
            Debug.Log("게임 종료 UI가 설정되어 있지 않습니다.");
        }
    }

    public void RestartGame()
    {
        // 필요 시 씬 리셋, 점수 리셋 등 구현 예정
        Debug.Log("[GameManager] RestartGame() 호출됨 (미구현)");
    }
}
