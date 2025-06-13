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
        // �ڽ� ������Ʈ �� Arrow ��ũ��Ʈ ���� �͵��� ���� ã�� ���
        Arrow[] arrows = GetComponentsInChildren<Arrow>();
        arrowsRemaining = arrows.Length;

        foreach (Arrow arrow in arrows)
        {
            arrow.OnTaken += OnArrowTaken;
        }

        UIManager.Instance?.UpdateArrowCount(arrowsRemaining);
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
            newArrow.transform.localPosition = Vector3.zero + Vector3.up * (i * 0.15f);
            newArrow.transform.localRotation = Quaternion.identity;

            Arrow arrow = newArrow.GetComponent<Arrow>();
            if (arrow != null)
            {
                arrow.OnTaken += OnArrowTaken;
            }
        }

        UIManager.Instance?.UpdateArrowCount(arrowsRemaining);

        Debug.Log("[ArrowBucket] Reset complete and arrows respawned.");
    }

    public int GetArrowCount() => arrowsRemaining;
}
