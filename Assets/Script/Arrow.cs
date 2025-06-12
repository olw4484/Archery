using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class Arrow : MonoBehaviour
{
    private Rigidbody rb;
    private XRGrabInteractable grabInteractable;

    private bool isFired = false;
    public bool IsFired() => isFired;

    [SerializeField] private float fireForce = 20f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        grabInteractable = GetComponent<XRGrabInteractable>();
    }

    private void OnEnable()
    {
        grabInteractable.selectExited.AddListener(OnRelease);
    }

    private void OnDisable()
    {
        grabInteractable.selectExited.RemoveListener(OnRelease);
    }

    private void OnRelease(SelectExitEventArgs args)
    {
        // 만약 장착 후 놓으면 발사되게 할 경우
        if (!isFired)
        {
            Fire(transform.forward); // 전방 방향으로 발사
        }
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
}
