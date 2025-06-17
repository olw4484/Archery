using UnityEngine;

public class Target : MonoBehaviour
{
    [SerializeField] private float maxRadius = 0.55f; // 가장 바깥 반지름
    [SerializeField] private Transform centerPoint;   // 과녁 중심 위치

    private void Reset()
    {
        centerPoint = this.transform; // 기본 중심을 자신의 위치로 설정
    }

   //public void RegisterHit(Vector3 hitPoint)
   //{
   //    if (centerPoint == null)
   //    {
   //        Debug.LogWarning("[Target] CenterPoint가 설정되지 않았습니다.");
   //        return;
   //    }
   //
   //    float distance = Vector3.Distance(hitPoint, centerPoint.position);
   //    float ratio = distance / maxRadius;
   //
   //    int score = 0;
   //    if (ratio < 0.05f) score = 11;
   //    else if (ratio < 0.15f) score = 10;
   //    else if (ratio < 0.25f) score = 9;
   //    else if (ratio < 0.35f) score = 8;
   //    else if (ratio < 0.45f) score = 7;
   //    else if (ratio < 0.55f) score = 6;
   //    else if (ratio < 0.65f) score = 5;
   //    else if (ratio < 0.75f) score = 4;
   //    else if (ratio < 0.85f) score = 3;
   //    else if (ratio < 0.95f) score = 2;
   //    else if (ratio <= 1f) score = 1;
   //
   //    VRDebugFile.Log($"[Target] Hit registered! Distance: {distance:F2}, Score: {score}");
   //    ScoreManager.Instance?.AddScore(score);
   //}
}
