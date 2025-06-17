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
            VRDebugFile.Log($"[Arrow] ForcePoint�� �����Ǿ� �ֽ��ϴ�: {name}");
        }
    }

    private void OnEnable()
    {
        if (grabInteractable != null)
            grabInteractable.selectEntered.AddListener(OnGrab);
    }

    private void OnDisable()
    {
        if (grabInteractable != null)
            grabInteractable.selectEntered.RemoveListener(OnGrab);
    }

    private void OnGrab(SelectEnterEventArgs args)
    {
        grabbed = true;
        OnTaken?.Invoke();
        OnTaken = null; // �ߺ� ����
    }

    public void Fire(Vector3 force)
    {
        try
        {
            if (isFired)
            {
                VRDebugFile.Log("[Arrow] �̹� �߻��, Fire() ����");
                return;
            }
            isFired = true;
            VRDebugFile.Log("[Arrow] Fire() ����");


            transform.SetParent(null);


            rb.isKinematic = false;
            rb.WakeUp();
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            if (arrowCollider != null)
            {
                arrowCollider.enabled = false;
                Invoke(nameof(EnableCollider), 0.2f);
            }


            if (grabInteractable != null)
                grabInteractable.enabled = false;


            VRDebugFile.Log("[Arrow] AddForce: " + force);
            rb.AddForce(force, ForceMode.Impulse);

            StartCoroutine(LogVelocityAfterPhysics());

            VRDebugFile.Log("[Arrow] Fire() �Ϸ�");
        }
        catch (System.Exception e)
        {
            VRDebugFile.Log("[Arrow] Fire() ���� �߻�: " + e.Message + "\n" + e.StackTrace);
        }
    }

    private void EnableCollider()
    {
        if (arrowCollider != null) arrowCollider.enabled = true;
    }

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
