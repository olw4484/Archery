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
        // �ڽ� ������Ʈ �� Arrow ��ũ��Ʈ ���� �͵��� ���� ã�� ���
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
        if (currentArrow != null) return; // �̹� ������ ȭ���� �ִٸ� ����

        Arrow arrow = other.GetComponent<Arrow>();
        if (arrow != null && !arrow.IsFired())
        {
            currentArrow = arrow;

            // ȭ���� ���Ͽ� ����
            currentArrow.transform.SetParent(arrowSocket);
            currentArrow.transform.localPosition = Vector3.zero;
            currentArrow.transform.localRotation = Quaternion.identity;

            // XRGrabInteractable ��Ȱ��ȭ
            XRGrabInteractable grab = currentArrow.GetComponent<XRGrabInteractable>();
            if (grab != null)
            {
                grab.enabled = false;
            }

            // Rigidbody ���
            Rigidbody rb = currentArrow.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = true;
            }

            Debug.Log("[Bow] ȭ���� ���Ͽ� �����Ǿ����ϴ�.");
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
            Vector3 spawnPos = Vector3.up * (i * 0.12f - 0.25f); // ���� �� ����
            newArrow.transform.localPosition = spawnPos;
            newArrow.transform.localRotation = Quaternion.Euler(90, 0, 0); // �ʿ� �� ���� ����

            Arrow arrow = newArrow.GetComponent<Arrow>();
            if (arrow != null)
            {
                arrow.OnTaken += OnArrowTaken;
            }

            UIManager.Instance?.UpdateArrowCount(arrowsRemaining);

            yield return new WaitForSeconds(0.05f); // �ʹ� ������ 0.03f ������
        }

        Debug.Log("[ArrowBucket] All arrows spawned sequentially.");
    }



    public int GetArrowCount() => arrowsRemaining;
}
