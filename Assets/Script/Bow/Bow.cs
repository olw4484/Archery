using UnityEngine;
using UnityEngine.InputSystem;
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

                AttachArrow(arrow);
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

    public void AttachArrow(Arrow arrow)
    {
        currentArrow = arrow;

        // 방향은 ArrowSocket 기준
        arrow.transform.rotation = arrowSocket.rotation;

        // 위치는 String_Pull 기준
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
                Debug.Log($"[Debug] T 키로 {arrow.name} 강제 장착");
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
