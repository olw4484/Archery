using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class Bow : MonoBehaviour
{
    [Header("Arrow Socket")]
    public Transform arrowSocket; // ȭ�� ���� ��ġ
    private Arrow currentArrow;

    [Header("Draw Settings")]
    public Transform stringHand; // ������ ���� �� (��: ������)
    public float maxDrawDistance = 0.5f;
    public float fireMultiplier = 30f;
    [SerializeField] private Transform stringPull;     // XRGrab ���� ��
    [SerializeField] private Transform stringRestPos;  // ���� �⺻ ��ġ
    [SerializeField] private Transform bowRoot;
    private Vector3 originalPosition;
    private bool arrowFired = false;

    private void Start()
    {
        if (bowRoot == null)
            bowRoot = this.transform;

        originalPosition = bowRoot.localPosition;
    }

    public bool HasArrow() => currentArrow != null;

    private void Update()
    {
        if (currentArrow == null) return;

        if (!arrowFired)
        {
            currentArrow.transform.position = stringPull.position;
            currentArrow.transform.rotation = stringPull.rotation;
        }

        float drawDistance = Vector3.Distance(arrowSocket.position, stringHand.position);
        if (drawDistance >= maxDrawDistance)
        {
            FireArrow(drawDistance);
        }
    }

    public void FireArrow(float drawDistance)
    {
        if (currentArrow == null) return;

        // �߻� ����: ȭ���� ���� ���� ����
        Vector3 fireDirection = arrowSocket.forward;

        float force = Mathf.Clamp01(drawDistance / maxDrawDistance) * fireMultiplier;

        // ���� ����
        currentArrow.transform.SetParent(null);

        XRGrabInteractable grab = currentArrow.GetComponent<XRGrabInteractable>();
        if (grab != null)
        {
            grab.enabled = true;
        }

        Rigidbody rb = currentArrow.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.AddForce(fireDirection * force, ForceMode.Impulse);
        }

        currentArrow.SetFired(); // fired ���� ����
        arrowFired = true;
        currentArrow = null; // ���� ȭ�� ���� ���� ���·� ��ȯ



        Debug.Log("[Bow] ȭ���� �߻�Ǿ����ϴ�.");
    }


    private void OnTriggerEnter(Collider other)
    {
        if (currentArrow == null)
        {
            Arrow arrow = other.GetComponent<Arrow>();
            if (arrow != null && !arrow.IsFired())
            {
                arrow.transform.position = arrowSocket.position;
                arrow.transform.rotation = arrowSocket.rotation;
                arrow.transform.SetParent(arrowSocket);

                AttachArrow(arrow.transform);
            }
        }
    }

    public void SetDrawOffset(float drawPercent)
    {
        float recoilDistance = 0.05f; // Ȱ�� �ڷ� �̵��ϴ� �ִ� �Ÿ�
        Vector3 offset = new Vector3(0, 0, -drawPercent * recoilDistance);
        bowRoot.localPosition = originalPosition + offset;
    }

    public void ResetBowPosition()
    {
        bowRoot.localPosition = originalPosition;
    }

    public void AttachArrow(Transform arrowTransform)
    {
        currentArrow = arrowTransform.GetComponent<Arrow>();
        arrowFired = false;
    }
}
