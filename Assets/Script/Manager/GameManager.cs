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
                Debug.Log("[GameManager] ��� ȭ�� ���� - ���� ī��Ʈ�ٿ� ����");
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
                Debug.Log("[GameManager] ȭ�� �߰� �߰� - ī��Ʈ�ٿ� ���");
            }
        }
    }

    private bool IsAllArrowsConsumed()
    {
        // 1. ArrowBucket ����
        bool bucketEmpty = arrowBucket == null || arrowBucket.IsDepleted();

        // 2. Ȱ�� ������ ȭ��
        Bow bow = FindObjectOfType<Bow>();
        bool bowHasArrow = bow != null && bow.HasArrow();

        // 3. �� �� Grab ���� ȭ�� üũ
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
        Debug.Log($"[GameManager] ���� ����. ���� ����: {finalScore}");

        if (resultUI != null)
            resultUI.ShowResult(finalScore);
        else
            Debug.Log("���� ���� UI�� �����Ǿ� ���� �ʽ��ϴ�.");
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
