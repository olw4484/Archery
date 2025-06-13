using UnityEngine;

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

        arrowsRemaining = totalArrowCount;


        for (int i = 0; i < totalArrowCount; i++)
        {
            GameObject newArrow = Instantiate(arrowPrefab, spawnRoot);

            
            Vector3 spawnPos = Vector3.up * (i * 0.12f); 
            newArrow.transform.localPosition = spawnPos;
            newArrow.transform.localRotation = Quaternion.identity;

            Arrow arrow = newArrow.GetComponent<Arrow>();
            if (arrow != null)
            {
                arrow.OnTaken += OnArrowTaken;
            }
        }

        UIManager.Instance?.UpdateArrowCount(arrowsRemaining);
    }


    public int GetArrowCount() => arrowsRemaining;
}
