using UnityEngine;

public class TargetZone : MonoBehaviour
{
    public int score = 0;
    public void AddScore(Vector3 hitWorldPoint)
    {
        // ���� => ���� ��ȯ
        Vector3 localPoint = transform.InverseTransformPoint(hitWorldPoint);

        float distance = new Vector2(localPoint.x, localPoint.y).magnitude;

        ScoreManager.Instance?.AddScore(score);
        VRDebugFile.Log($"[TargetZone] {name}: {score}��! �Ÿ�: {distance:F2} (local: {localPoint})");
    }
}
