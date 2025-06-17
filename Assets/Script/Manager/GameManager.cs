using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private ArrowBucket arrowBucket;
    [SerializeField] private GameResultUI resultUI;
    [SerializeField] private float endCountdownTime = 10f;

    private bool gameEnded = false;
    private bool endCountdownStarted = false;
    private float endCountdown = 0f;

    private void Update()
    {
        if (gameEnded) return;

        if (IsAllArrowsConsumed())
        {
            if (!endCountdownStarted)
            {
                endCountdownStarted = true;
                endCountdown = endCountdownTime;
                Debug.Log("[GameManager] 모든 화살 소진 - 종료 카운트다운 시작");
            }
            else
            {
                endCountdown -= Time.deltaTime;
                if (endCountdown <= 0f)
                {
                    gameEnded = true;
                    EndGame();
                }
            }
        }
        else
        {
            if (endCountdownStarted)
            {
                endCountdownStarted = false;
                Debug.Log("[GameManager] 화살 추가 발견 - 카운트다운 취소");
            }
        }
    }

    private bool IsAllArrowsConsumed()
    {
        // 1. ArrowBucket 소진
        bool bucketEmpty = arrowBucket == null || arrowBucket.IsDepleted();

        // 2. 활에 장착된 화살
        Bow bow = FindObjectOfType<Bow>();
        bool bowHasArrow = bow != null && bow.HasArrow();

        // 3. 씬 내 Grab 중인 화살 체크
        bool handHasArrow = false;
        foreach (Arrow arrow in FindObjectsOfType<Arrow>())
        {
            var grab = arrow.GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
            if (grab != null && grab.isSelected)
            {
                handHasArrow = true;
                break;
            }
        }

        return bucketEmpty && !bowHasArrow && !handHasArrow;
    }

    private void EndGame()
    {
        int finalScore = ScoreManager.Instance?.GetScore() ?? 0;
        Debug.Log($"[GameManager] 게임 종료. 최종 점수: {finalScore}");

        if (resultUI != null)
            resultUI.ShowResult(finalScore);
        else
            Debug.Log("게임 종료 UI가 설정되어 있지 않습니다.");
    }

    public void RestartGame()
    {
        Debug.Log("[GameManager] Restarting game (soft reset)");
        ScoreManager.Instance?.ResetScore();
        arrowBucket?.ResetBucket();
        resultUI?.HideResult();
        WindManager.Instance?.RandomizeWind();
        gameEnded = false;
        endCountdownStarted = false;
    }
}
