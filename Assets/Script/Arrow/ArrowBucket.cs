using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class ArrowBucket : MonoBehaviour
{
    [Header("Arrow Settings")]
    [SerializeField] private int totalArrowCount = 10;
    [SerializeField] private GameObject arrowPrefab;
    [SerializeField] private Transform spawnRoot;
    [SerializeField] private Transform arrowSocket;

    private Arrow currentArrow;
    private int arrowsRemaining;

    public bool IsDepleted() => arrowsRemaining <= 0;

    private void Start()
    {
        // 자식 오브젝트 중 Arrow 스크립트 가진 것들을 전부 찾아 등록
        //Arrow[] arrows = GetComponentsInChildren<Arrow>();
        //arrowsRemaining = arrows.Length;
        //
        //foreach (Arrow arrow in arrows)
        //{
        //    arrow.OnTaken += OnArrowTaken;
        //}
        //
        //UIManager.Instance?.UpdateArrowCount(arrowsRemaining);
        ResetBucket();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (currentArrow != null) return; // 이미 장착된 화살이 있다면 무시

        Arrow arrow = other.GetComponent<Arrow>();
        if (arrow != null && !arrow.IsFired())
        {
            currentArrow = arrow;

            // 화살을 소켓에 고정
            currentArrow.transform.SetParent(arrowSocket);
            currentArrow.transform.localPosition = Vector3.zero;
            currentArrow.transform.localRotation = Quaternion.identity;

            // XRGrabInteractable 비활성화
            XRGrabInteractable grab = currentArrow.GetComponent<XRGrabInteractable>();
            if (grab != null)
            {
                grab.enabled = false;
            }

            // Rigidbody 잠금
            Rigidbody rb = currentArrow.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = true;
            }

            Debug.Log("[Bow] 화살이 소켓에 고정되었습니다.");
        }
    }


    private void OnArrowTaken()
    {
        arrowsRemaining = Mathf.Max(0, arrowsRemaining - 1);
        UIManager.Instance?.UpdateArrowCount(arrowsRemaining);
    }

    public void ResetBucket()
    {
        foreach (Arrow arrow in GetComponentsInChildren<Arrow>())
        {
            Destroy(arrow.gameObject);
        }

        StartCoroutine(SpawnArrowsSequentially());
    }

    private IEnumerator SpawnArrowsSequentially()
    {
        arrowsRemaining = totalArrowCount;

        for (int i = 0; i < totalArrowCount; i++)
        {
            GameObject newArrow = Instantiate(arrowPrefab, spawnRoot);
            Vector3 spawnPos = Vector3.up * (i * 0.12f - 0.25f); // 높이 및 보정
            newArrow.transform.localPosition = spawnPos;
            newArrow.transform.localRotation = Quaternion.Euler(90, 0, 0); // 필요 시 각도 조정

            Arrow arrow = newArrow.GetComponent<Arrow>();
            if (arrow != null)
            {
                arrow.OnTaken += OnArrowTaken;
            }

            UIManager.Instance?.UpdateArrowCount(arrowsRemaining);

            yield return new WaitForSeconds(0.05f); // 너무 느리면 0.03f 정도로
        }

        Debug.Log("[ArrowBucket] All arrows spawned sequentially.");
    }



    public int GetArrowCount() => arrowsRemaining;
}
