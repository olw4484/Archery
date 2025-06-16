using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class Bow : MonoBehaviour
{
    [Header("Arrow Socket")]
    [SerializeField] private Transform arrowSocket; // ȭ�� ���� ��ġ
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
#if UNITY_EDITOR
        if (Keyboard.current.tKey.wasPressedThisFrame)
        {
            TryForceAttachNearestArrow();
        }
#endif

        if (currentArrow == null) return;

        Transform forcePoint = currentArrow.ForcePoint;
        if (forcePoint != null)
        {
            Vector3 offset = currentArrow.transform.position - forcePoint.position;
            currentArrow.transform.position = stringPull.position + offset;
            currentArrow.transform.rotation = stringPull.rotation;
        }

        float drawDistance = Vector3.Distance(arrowSocket.position, stringHand.position);
        if (drawDistance >= maxDrawDistance)
        {
            FireArrow(drawDistance);
        }
    }

    public void FireArrow(float force)
    {
        if (currentArrow == null) return;

        currentArrow.transform.SetParent(null);
        currentArrow.Fire(arrowSocket.forward * force);

        currentArrow = null;

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

                AttachArrow(arrow);
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

    public void AttachArrow(Arrow arrow)
    {
        currentArrow = arrow;

        // ������ ArrowSocket ����
        arrow.transform.rotation = arrowSocket.rotation;

        // ��ġ�� String_Pull ����
        arrow.transform.position = stringPull.position;

        arrow.transform.SetParent(transform);
    }
    private void TryForceAttachNearestArrow()
    {
        Collider[] hits = Physics.OverlapSphere(arrowSocket.position, 0.3f);
        foreach (var hit in hits)
        {
            Arrow arrow = hit.GetComponent<Arrow>();
            if (arrow != null && !arrow.IsFired())
            {
                Debug.Log($"[Debug] T Ű�� {arrow.name} ���� ����");
                arrow.transform.position = arrowSocket.position;
                arrow.transform.rotation = arrowSocket.rotation;
                arrow.transform.SetParent(arrowSocket);

                XRGrabInteractable grab = arrow.GetComponent<XRGrabInteractable>();
                if (grab != null) grab.enabled = false;

                Rigidbody rb = arrow.GetComponent<Rigidbody>();
                if (rb != null) rb.isKinematic = true;

                AttachArrow(arrow);
                break;
            }
        }
    }

}
