using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class Bow : MonoBehaviour
{
    [Header("Arrow Socket")]
    public Transform arrowSocket; // 화살 장착 위치
    private Arrow currentArrow;

    [Header("Draw Settings")]
    public Transform stringHand; // 시위를 당기는 손 (예: 오른손)
    public float maxDrawDistance = 0.5f;
    public float fireMultiplier = 30f;
    [SerializeField] private Transform stringPull;     // XRGrab 시위 끝
    [SerializeField] private Transform stringRestPos;  // 시위 기본 위치
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

        // 발사 방향: 화살이 향한 방향 기준
        Vector3 fireDirection = arrowSocket.forward;

        float force = Mathf.Clamp01(drawDistance / maxDrawDistance) * fireMultiplier;

        // 고정 해제
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

        currentArrow.SetFired(); // fired 상태 저장
        arrowFired = true;
        currentArrow = null; // 다음 화살 장착 가능 상태로 전환



        Debug.Log("[Bow] 화살이 발사되었습니다.");
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
        float recoilDistance = 0.05f; // 활이 뒤로 이동하는 최대 거리
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
