using UnityEngine;
using System.Collections;
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

        if (bow != null && bow.HasArrow() && drawDistanceCache >= minReleaseDistance)
        {
            float drawPercent = Mathf.Clamp01(drawDistanceCache / maxDrawDistance);
            float force = drawPercent * firePowerMultiplier;
            VRDebugFile.Log($"[OnReleased] (딜레이 후) FireArrow 호출 예정! force: {force}, drawPercent: {drawPercent}");
            StartCoroutine(FireArrowAfterDelay(force));
        }
        else
        {
            VRDebugFile.Log("[OnReleased] FireArrow 조건 불충족 - 발사 스킵");
        }

        stringMover?.OnStringReleased();
        drawDistanceCache = 0f;
    }

    private IEnumerator FireArrowAfterDelay(float force)
    {
        yield return new WaitForSeconds(0.07f); // 상황 따라 0.1f까지 조절 가능
        bow.FireArrow(force);
    }

    private void OnStringRestored()
    {
        if (bow != null)
        {
            bow.ResetBowPosition(); 
        }
    }
}
