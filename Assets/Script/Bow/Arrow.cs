using UnityEngine;
using UnityEngine.SceneManagement;
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
        if (!isFired)
        {
            Fire(transform.forward);
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
    private void OnCollisionEnter(Collision collision)
    {
        if (!isFired) return;

        if (collision.gameObject.CompareTag("Target"))
        {
            Debug.Log("[Arrow] Target hit!");

            // ���� �ý��� ȣ�� (�ӽ÷� 10�� �ο�)
            ScoreManager.Instance?.AddScore(10);

            // ȭ�� ���� (�������� �ʵ���)
            rb.isKinematic = true;
            transform.SetParent(collision.transform);
        }
    }
}
