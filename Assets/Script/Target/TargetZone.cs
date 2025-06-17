using UnityEngine;

public class TargetZone : MonoBehaviour
{
    public int score = 0;
    public Transform centerPoint;
    public void AddScore(Vector3 hitWorldPoint)
    {
        // ���� => ���� ��ȯ
        Vector3 hitLocal = transform.InverseTransformPoint(hitWorldPoint);
        Vector3 centerLocal = transform.InverseTransformPoint(centerPoint.position);

        float distance = new Vector2(hitLocal.x - centerLocal.x, hitLocal.y - centerLocal.y).magnitude;
        ScoreManager.Instance?.AddScore(score);
        VRDebugFile.Log($"[TargetZone] {name}: {score}��! �Ÿ�: {distance:F2} (local: {centerLocal})");
    }
}
