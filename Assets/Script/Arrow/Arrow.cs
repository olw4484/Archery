using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using static UnityEngine.GraphicsBuffer;

public class Arrow : MonoBehaviour
{
    private Rigidbody rb;
    private XRGrabInteractable grabInteractable;
    public event System.Action OnTaken;
    private bool isFired = false;
    public bool IsFired() => isFired;

    [SerializeField] private float fireForce = 20f;
    [SerializeField] private float windInfluence = 0.3f; 

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        grabInteractable = GetComponent<XRGrabInteractable>();
    }

    private void OnEnable()
    {
        grabInteractable.selectExited.AddListener(OnRelease);

        var grab = GetComponent<XRGrabInteractable>();
        if (grab != null)
            grab.selectEntered.AddListener(OnGrab);
    }

    private void OnDisable()
    {
        grabInteractable.selectExited.RemoveListener(OnRelease);
    }



    private void OnCollisionEnter(Collision collision)
    {
        if (!isFired) return;

        Target target = collision.gameObject.GetComponent<Target>();
        if (target != null)
        {
            ContactPoint contact = collision.contacts[0];
            target.RegisterHit(contact.point);

            rb.isKinematic = true;
            transform.SetParent(collision.transform);
        }
    }

    private void FixedUpdate()
    {
        if (!isFired || rb == null) return;

        // 바람 힘 적용
        Vector3 windForce = WindManager.Instance != null ? WindManager.Instance.WindForce : Vector3.zero;
        rb.AddForce(windForce * windInfluence, ForceMode.Force);
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

    private void OnGrab(SelectEnterEventArgs args)
    {
        OnTaken?.Invoke();
        OnTaken = null; // 중복 호출 방지
    }

    private void OnRelease(SelectExitEventArgs args)
    {
        if (!isFired)
        {
            Fire(transform.forward);
        }
    }


}
