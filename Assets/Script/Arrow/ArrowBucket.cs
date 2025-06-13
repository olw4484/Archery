using UnityEngine;
using System.Collections;

public class ArrowBucket : MonoBehaviour
{
    [Header("Arrow Settings")]
    [SerializeField] private int totalArrowCount = 10;
    [SerializeField] private GameObject arrowPrefab;
    [SerializeField] private Transform spawnRoot;

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
