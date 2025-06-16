using UnityEngine;
using System.Collections;
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

    //public void FireArrow(float force)
    //{
    //    currentArrow.gameObject.SetActive(true);
    //    VRDebugFile.Log($"[FireArrow] currentArrow ���� Ȱ��ȭ �õ�");
    //    VRDebugFile.Log("FireArrow ����, currentArrow = " + (currentArrow != null ? currentArrow.name : "null"));
    //
    //    if (currentArrow == null)
    //    {
    //        VRDebugFile.Log("FireArrow: currentArrow is NULL!");
    //        return;
    //    }
    //
    //
    //    Arrow arrowToFire = currentArrow;
    //    currentArrow = null;
    //
    //    // stringPull �������� �߻� ���� ����
    //    Vector3 fireDirection = (arrowSocket.position - stringPull.position).normalized;
    //    VRDebugFile.Log($"[FireArrow] force: {force}  fireDirection: {fireDirection}");
    //    VRDebugFile.Log("FireArrow: arrowToFire = " + (arrowToFire != null ? "OK" : "NULL"));
    //    VRDebugFile.Log($"[FireArrow] currentArrow Ȱ��ȭ ����: {currentArrow.gameObject.activeInHierarchy}");
    //
    //
    //    try
    //    {
    //        arrowToFire.transform.SetParent(null);
    //        VRDebugFile.Log("FireArrow: SetParent �Ϸ�");
    //        arrowToFire.Fire(fireDirection * force);
    //        VRDebugFile.Log("FireArrow: Fire() ȣ�� �Ϸ�");
    //    }
    //    catch (System.Exception e)
    //    {
    //        VRDebugFile.Log("[FireArrow] Exception: " + e.Message + "\n" + e.StackTrace);
    //    }
    //    arrowToFire.transform.SetParent(null);
    //    VRDebugFile.Log("FireArrow: currentArrow.Fire() ȣ�� ����");
    //    arrowToFire.Fire(fireDirection * force);
    //    VRDebugFile.Log("FireArrow: currentArrow.Fire() ȣ�� ����");
    //
    //    VRDebugFile.Log("[Bow] ȭ���� �߻�Ǿ����ϴ�.");
    //}

    public void FireArrow(float force)
    {
        Arrow arrowToFire = currentArrow;

        currentArrow = null;

        Vector3 fireDirection = (arrowSocket.position - stringPull.position).normalized;

        StartCoroutine(FireArrowCoroutine(arrowToFire, fireDirection * force));
    }

    private IEnumerator FireArrowCoroutine(Arrow arrow, Vector3 finalForce)
    {
        yield return new WaitForFixedUpdate();

        arrow.transform.SetParent(null);

        arrow.Fire(finalForce);

        VRDebugFile.Log("[FireArrowCoroutine] ȭ�� �߻� �Ϸ�, force: " + finalForce);
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
                VRDebugFile.Log("[AttachArrow] isKinematic = true");
                rb.WakeUp();
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
        arrow.transform.position = arrowSocket.position;

        Vector3 dir = arrowSocket.position - stringPull.position;
        VRDebugFile.Log("AttachArrow dir: " + dir);
        Debug.DrawLine(arrowSocket.position, stringPull.position, Color.red, 3f);
        arrow.transform.rotation = Quaternion.LookRotation(dir, Vector3.up);

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

        Vector3 dir = arrowSocket.position - stringPull.position;
        currentArrow.transform.rotation = Quaternion.LookRotation(dir, bowRoot.up);
    }

}
