using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class BowString : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Bow bow; // Ȱ ��ü ����
    [SerializeField] private Transform stringRestPosition; // ���� �⺻ ��ġ (������)

    [Header("Draw Settings")]
    [SerializeField] private float maxDrawDistance = 0.5f;
    [SerializeField] private float minReleaseDistance = 0.1f;
    [SerializeField] private float firePowerMultiplier = 30f;

    private XRGrabInteractable grabInteractable;

    private void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
    }

    private void OnEnable()
    {
        grabInteractable.selectExited.AddListener(OnReleased);
    }

    private void OnDisable()
    {
        grabInteractable.selectExited.RemoveListener(OnReleased);
    }

    private void OnReleased(SelectExitEventArgs args)
    {
        if (bow == null || !bow.HasArrow()) return;

        float drawDistance = Vector3.Distance(stringRestPosition.position, transform.position);

        if (drawDistance >= minReleaseDistance)
        {
            float drawPercent = Mathf.Clamp01(drawDistance / maxDrawDistance);
            float force = drawPercent * firePowerMultiplier;

            bow.FireArrow(force);
        }

        // ���� ��ġ ����
        transform.position = stringRestPosition.position;
    }
}
