using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class Bow : MonoBehaviour
{
    [Header("Arrow Socket")]
    [SerializeField] private Transform arrowSocket; // 화살 장착 위치
    private Arrow currentArrow;

    [Header("Draw Settings")]
    public Transform stringHand; // 시위를 당기는 손 (예: 오른손)
    public float maxDrawDistance = 0.5f;
    public float fireMultiplier = 30f;
    [SerializeField] private Transform stringPull;     // XRGrab 시위 끝
    [SerializeField] private Transform stringRestPos;  // 시위 기본 위치
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
        VRDebugFile.Log($"[FireArrow] currentArrow 강제 활성화 시도");
        VRDebugFile.Log("FireArrow 진입, currentArrow = " + (currentArrow != null ? currentArrow.name : "null"));

        if (currentArrow == null)
        {
            VRDebugFile.Log("FireArrow: currentArrow is NULL!");
            return;
        }


        Arrow arrowToFire = currentArrow;
        currentArrow = null;

        // stringPull 기준으로 발사 방향 결정
        Vector3 fireDirection = (stringPull.position - arrowSocket.position).normalized;
        VRDebugFile.Log($"[FireArrow] force: {force}  fireDirection: {fireDirection}");
        VRDebugFile.Log($"[FireArrow] currentArrow 활성화 상태: {currentArrow.gameObject.activeInHierarchy}");
        arrowToFire.transform.SetParent(null);
        VRDebugFile.Log("FireArrow: currentArrow.Fire() 호출 직전");
        arrowToFire.Fire(fireDirection * force);
        VRDebugFile.Log("FireArrow: currentArrow.Fire() 호출 직후");

        VRDebugFile.Log("[Bow] 화살이 발사되었습니다.");
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
            VRDebugFile.Log("[Bow] 화살이 장착되었습니다 (손에서 해제).");
        }
    }


    public void SetDrawOffset(float drawPercent)
    {
        float recoilDistance = 0.05f; // 활이 뒤로 이동하는 최대 거리
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
