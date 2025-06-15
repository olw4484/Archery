using UnityEngine;

public class BowStringMover : MonoBehaviour
{
    [SerializeField] private Transform stringPull;
    [SerializeField] private Transform stringMid;
    [SerializeField] private Transform stringRestPosition;

    [Header("Spring Settings")]
    [SerializeField] private float springStrength = 100f;
    [SerializeField] private float damping = 5f;

    private Vector3 velocity = Vector3.zero;
    private bool isReleased = false;

    public System.Action OnRestored;

    public void OnStringReleased()
    {
        isReleased = true;
        velocity = Vector3.zero;
        Debug.Log("[BowStringMover] OnStringReleased() CALLED!");
    }

    private void Update()
    {
        if (stringPull == null || stringMid == null || stringRestPosition == null)
            return;

        if (!isReleased)
        {
            // 평소엔 stringMid가 stringPull을 따라감
            stringMid.position = stringPull.position;
        }
        else
        {
            // 탄성 복원
            Vector3 direction = stringRestPosition.position - stringMid.position;
            velocity += direction * springStrength * Time.deltaTime;
            velocity *= Mathf.Exp(-damping * Time.deltaTime);
            stringMid.position += velocity * Time.deltaTime;

            if (direction.magnitude < 0.001f && velocity.magnitude < 0.01f)
            {
                stringMid.position = stringRestPosition.position;
                stringPull.position = stringRestPosition.position;
                isReleased = false;

                Debug.Log("[BowStringMover] 복원 완료. 위치 고정.");
                OnRestored?.Invoke(); // 복원 완료 알림
            }
        }
    }
}
