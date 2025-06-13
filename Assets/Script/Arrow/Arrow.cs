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
        rb.AddForce(direction * fireForce, ForceMode.Impulse);
    }

    public void ForceTaken()
    {
        if (grabbed || isFired) return;
        OnTaken?.Invoke();
        OnTaken = null;
    }
}
