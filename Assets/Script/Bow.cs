using UnityEngine;

public class Bow : MonoBehaviour
{
    [Header("Arrow Socket")]
    public Transform arrowSocket; // ȭ�� ���� ��ġ
    private Arrow currentArrow;

    [Header("Draw Settings")]
    public Transform stringHand; // ������ ���� �� (��: ������)
    public float maxDrawDistance = 0.5f;
    public float fireMultiplier = 30f;

    private void Update()
    {
        if (currentArrow != null)
        {
            float drawDistance = Vector3.Distance(arrowSocket.position, stringHand.position);

            if (drawDistance >= maxDrawDistance)
            {
                FireArrow(drawDistance);
            }
        }
    }

    private void FireArrow(float drawDistance)
    {
        Vector3 fireDirection = arrowSocket.forward;
        float force = Mathf.Clamp01(drawDistance / maxDrawDistance) * fireMultiplier;

        currentArrow.Fire(fireDirection * force);
        currentArrow = null;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (currentArrow == null)
        {
            Arrow arrow = other.GetComponent<Arrow>();
            if (arrow != null && !arrow.IsFired())
            {
                // ȭ�� ����
                currentArrow = arrow;
                currentArrow.transform.position = arrowSocket.position;
                currentArrow.transform.rotation = arrowSocket.rotation;
                currentArrow.transform.SetParent(arrowSocket);
            }
        }
    }
}
