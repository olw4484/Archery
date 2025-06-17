using UnityEngine;
using System.Collections;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class Arrow : MonoBehaviour
{
    [Header("Physics")]
    [SerializeField] private float fireForce = 30f;

    private Rigidbody rb;
    private Collider arrowCollider;
    private XRGrabInteractable grabInteractable;
    private Transform forcePoint;

    private bool isFired = false;
    private bool grabbed = false;
    private bool stuck = false;

    public bool IsFired() => isFired;
    public void SetFired() => isFired = true;
    public bool IsGrabbed() => grabbed;
    
    public Transform ForcePoint => forcePoint;

    public event System.Action OnTaken;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        arrowCollider = GetComponent<Collider>();
        grabInteractable = GetComponent<XRGrabInteractable>();
        forcePoint = transform.Find("ForcePoint");
        if (forcePoint == null)
        {
            VRDebugFile.Log($"[Arrow] ForcePoint가 누락되어 있습니다: {name}");
        }
    }

    private void OnEnable()
    {
        VRDebugFile.Log("[Arrow] OnEnable! 콜라이더: " + (arrowCollider != null ? arrowCollider.enabled.ToString() : "null") +
          ", isKinematic: " + (rb != null ? rb.isKinematic.ToString() : "null"));

        if (grabInteractable != null)
            grabInteractable.selectEntered.AddListener(OnGrab);

        if (arrowCollider != null)
            arrowCollider.enabled = true;
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }

    private void OnDisable()
    {
        if (grabInteractable != null)
            grabInteractable.selectEntered.RemoveListener(OnGrab);
    }
    void OnCollisionEnter(Collision collision)
    {
        if (!isFired || stuck) return;
        Target target = collision.collider.GetComponentInParent<Target>();
        if (target != null)
        {
            ContactPoint contact = collision.contacts[0];
            float depthOffset = 0.03f;
            transform.position = contact.point + (-contact.normal * depthOffset);
            transform.rotation = Quaternion.LookRotation(-contact.normal, Vector3.up);

            rb.isKinematic = true;
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.Sleep(); 
            transform.SetParent(target.transform);
            if (arrowCollider != null)
                arrowCollider.enabled = false;
            stuck = true;
        }
    }


    private void OnGrab(SelectEnterEventArgs args)
    {
        grabbed = true;
        OnTaken?.Invoke();
        OnTaken = null; // 중복 방지
    }

    public void Fire(Vector3 force)
    {
        VRDebugFile.Log("[Arrow] Fire() 호출! 콜라이더: " + (arrowCollider != null ? arrowCollider.enabled.ToString() : "null") +
              ", isKinematic: " + (rb != null ? rb.isKinematic.ToString() : "null"));
        try
        {
            if (isFired) return;
            isFired = true;

            transform.SetParent(null);
            rb.isKinematic = false;
            VRDebugFile.Log("[Arrow] Fire() 후 isKinematic: " + rb.isKinematic);
            rb.WakeUp();
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            if (arrowCollider != null)
                arrowCollider.enabled = true;

            if (grabInteractable != null)
                grabInteractable.enabled = false;

            rb.AddForce(force, ForceMode.Impulse);

            StartCoroutine(LogVelocityAfterPhysics());
        }
        catch (System.Exception e)
        {
            VRDebugFile.Log("[Arrow] Fire() 예외 발생: " + e.Message + "\n" + e.StackTrace);
        }
    }

   //private void EnableCollider()
   //{
   //    if (arrowCollider != null) arrowCollider.enabled = true;
   //}

    private IEnumerator LogVelocityAfterPhysics()
    {
        yield return new WaitForFixedUpdate();
        VRDebugFile.Log("[Arrow] velocity after physics: " + rb.velocity);
    }

    public void ForceTaken()
    {
        if (grabbed || isFired) return;
        OnTaken?.Invoke();
        OnTaken = null;
    }
}
