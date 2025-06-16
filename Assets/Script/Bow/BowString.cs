using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class BowString : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Bow bow; // 활 본체 참조
    [SerializeField] private Transform stringRestPosition; // 시위 기본 위치 (고정점)
    [SerializeField] private BowStringMover stringMover;

    [Header("Draw Settings")]
    [SerializeField] private float maxDrawDistance = 0.5f;
    [SerializeField] private float minReleaseDistance = 0.1f;
    [SerializeField] private float firePowerMultiplier = 30f;

    private XRGrabInteractable grabInteractable;
    private float drawDistanceCache = 0f;

    private void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
    }

    private void OnEnable()
    {
        grabInteractable.selectExited.AddListener(OnReleased);

        if (stringMover != null)
            stringMover.OnRestored += OnStringRestored;
    }

    private void OnDisable()
    {
        grabInteractable.selectExited.RemoveListener(OnReleased);

        if (stringMover != null)
            stringMover.OnRestored -= OnStringRestored;
    }

    private void OnReleased(SelectExitEventArgs args)
    {
        drawDistanceCache = Vector3.Distance(stringRestPosition.position, transform.position);

        if (bow != null)
        {
            float drawPercent = Mathf.Clamp01(drawDistanceCache / maxDrawDistance);
            bow.SetDrawOffset(drawPercent);
        }

        stringMover?.OnStringReleased();
    }

    private void OnStringRestored()
    {
        if (bow != null)
        {
            bow.ResetBowPosition(); // 활 원위치 복구
        }

        if (bow != null && bow.HasArrow() && drawDistanceCache >= minReleaseDistance)
        {
            float drawPercent = Mathf.Clamp01(drawDistanceCache / maxDrawDistance);
            float force = drawPercent * firePowerMultiplier;
            bow.FireArrow(force);
        }

        drawDistanceCache = 0f;
    }
}
