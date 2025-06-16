using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class Arrow : MonoBehaviour
{
    private Rigidbody rb;
    private XRGrabInteractable grabInteractable;

    private bool isFired = false;
    private bool grabbed = false;
    private Transform forcePoint;
    public bool IsFired() => isFired;
    public void SetFired() => isFired = true;
    public Transform ForcePoint => forcePoint;
    public bool IsGrabbed() => grabbed;

    public event System.Action OnTaken;

    [SerializeField] private float fireForce = 20f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        grabInteractable = GetComponent<XRGrabInteractable>();

        forcePoint = transform.Find("ForcePoint"); 
        if (forcePoint == null)
        {
            VRDebugFile.Log($"[Arrow] ForcePoint가 누락되어 있습니다: {name}");
        }
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
        VRDebugFile.Log("[Arrow.Fire] called! (isFired: " + isFired + ")");
        if (isFired) return;
        isFired = true;

        transform.SetParent(null);

        rb.isKinematic = false;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        Vector3 windForce = WindManager.Instance != null ? WindManager.Instance.WindForce : Vector3.zero;
        Vector3 finalForce = direction.normalized * fireForce + windForce;
        VRDebugFile.Log("[Arrow] Fire() force: " + finalForce);

        rb.AddForce(finalForce, ForceMode.Impulse);
        VRDebugFile.Log("[Arrow.Fire] AddForce 후 velocity: " + rb.velocity);

        XRGrabInteractable grab = GetComponent<XRGrabInteractable>();
        if (grab != null) grab.enabled = false;
    }



    public void ForceTaken()
    {
        if (grabbed || isFired) return;
        OnTaken?.Invoke();
        OnTaken = null;
    }
}
