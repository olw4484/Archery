using UnityEngine;
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

    private void Start()
    {
        if (bowRoot == null)
            bowRoot = this.transform;

        originalPosition = bowRoot.localPosition;
    }

    public bool HasArrow() => currentArrow != null;

    private void Update()
    {
    }

    private void FixedUpdate()
    {
        UpdateArrowPosition();
    }

    public void FireArrow(float force)
    {
        currentArrow.gameObject.SetActive(true);
        VRDebugFile.Log($"[FireArrow] currentArrow ���� Ȱ��ȭ �õ�");
        VRDebugFile.Log("FireArrow ����, currentArrow = " + (currentArrow != null ? currentArrow.name : "null"));

        if (currentArrow == null)
        {
            VRDebugFile.Log("FireArrow: currentArrow is NULL!");
            return;
        }


        Arrow arrowToFire = currentArrow;
        currentArrow = null;

        // stringPull �������� �߻� ���� ����
        Vector3 fireDirection = (stringPull.position - arrowSocket.position).normalized;
        VRDebugFile.Log($"[FireArrow] force: {force}  fireDirection: {fireDirection}");
        VRDebugFile.Log($"[FireArrow] currentArrow Ȱ��ȭ ����: {currentArrow.gameObject.activeInHierarchy}");
        arrowToFire.transform.SetParent(null);
        VRDebugFile.Log("FireArrow: currentArrow.Fire() ȣ�� ����");
        arrowToFire.Fire(fireDirection * force);
        VRDebugFile.Log("FireArrow: currentArrow.Fire() ȣ�� ����");

        VRDebugFile.Log("[Bow] ȭ���� �߻�Ǿ����ϴ�.");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (currentArrow != null) return;

        Arrow arrow = other.GetComponent<Arrow>();
        if (arrow != null && !arrow.IsFired())
        {
            XRGrabInteractable grab = arrow.GetComponent<XRGrabInteractable>();
            if (grab != null && grab.isSelected && grab.interactorsSelecting.Count > 0)
            {
                grab.interactionManager.SelectExit(grab.interactorsSelecting[0], grab);
            }

            if (grab != null) grab.enabled = false;

            Rigidbody rb = arrow.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = true;
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }

            AttachArrow(arrow);
            VRDebugFile.Log("[Bow] ȭ���� �����Ǿ����ϴ� (�տ��� ����).");
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

        arrow.transform.SetParent(arrowSocket);

        arrow.transform.rotation = Quaternion.LookRotation(arrowSocket.forward, Vector3.up);
        arrow.transform.position = arrowSocket.position;

        Rigidbody rb = arrow.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
        XRGrabInteractable grab = arrow.GetComponent<XRGrabInteractable>();
        if (grab != null) grab.enabled = false;
    }

    private void UpdateArrowPosition()
    {
        if (currentArrow == null) return;

        Vector3 offset = currentArrow.transform.position - currentArrow.ForcePoint.position;
        currentArrow.transform.position = stringPull.position + offset;

        VRDebugFile.Log("[Check] stringPull.forward: " + stringPull.forward);
        VRDebugFile.Log("[Check] UpdateArrowPosition rotation: " + currentArrow.transform.rotation.eulerAngles);
        currentArrow.transform.rotation = Quaternion.LookRotation(stringPull.position - arrowSocket.position,bowRoot.up);
    }
}
