using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class Arrow : MonoBehaviour
{
    private Rigidbody rb;
    private XRGrabInteractable grabInteractable;

    private bool isFired = false;
    private bool grabbed = false;
    public bool IsFired() => isFired;
    public void SetFired() => isFired = true;

    public event System.Action OnTaken;

    [SerializeField] private float fireForce = 20f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        grabInteractable = GetComponent<XRGrabInteractable>();
    }

    private void OnEnable()
    {
        grabInteractable.selectEntered.AddListener(OnGrab);
    }

    private void OnDisable()
    {
        grabInteractable.selectEntered.RemoveListener(OnGrab);
    }

    private void OnGrab(SelectEnterEventArgs args)
    {
        grabbed = true;
        OnTaken?.Invoke();
        OnTaken = null; // 중복 방지
    }

    public void Fire(Vector3 direction)
    {
        if (isFired) return;

        isFired = true;
        rb.isKinematic = false;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        // 바람 영향 적용
        Vector3 windForce = WindManager.Instance != null ? WindManager.Instance.WindForce : Vector3.zero;

        // 실제 힘 계산
        Vector3 finalForce = direction * fireForce + windForce;

        rb.AddForce(finalForce, ForceMode.Impulse);
    }

    public void ForceTaken()
    {
        if (grabbed || isFired) return;
        OnTaken?.Invoke();
        OnTaken = null;
    }
}
