using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class ArrowBucket : MonoBehaviour
{
    [Header("Arrow Settings")]
    [SerializeField] private int totalArrowCount = 10;
    [SerializeField] private ArrowPool arrowPool; 
    [SerializeField] private Transform spawnRoot;
    [SerializeField] private Transform arrowSocket;

    private Arrow currentArrow;
    private int arrowsRemaining;

    public bool IsDepleted() => arrowsRemaining <= 0;

    private void Start()
    {
        ResetBucket();
    }

    public void ResetBucket()
    {
        arrowsRemaining = totalArrowCount;

        // 풀에 모든 화살 반환 (현재 버킷 내 화살)
        foreach (Transform child in spawnRoot)
        {
            Arrow arrow = child.GetComponent<Arrow>();
            if (arrow != null) arrowPool.ReturnArrow(arrow.gameObject);
        }

        StartCoroutine(SpawnArrowsSequentially());
    }

    private IEnumerator SpawnArrowsSequentially()
    {
        for (int i = 0; i < totalArrowCount; i++)
        {
            GameObject newArrowObj = arrowPool.GetArrow();
            if (newArrowObj == null)
            {
                Debug.LogWarning("[ArrowBucket] Not enough arrows in pool!");
                break;
            }
            newArrowObj.transform.SetParent(spawnRoot);
            Vector3 spawnPos = Vector3.up * (i * 0.08f - 0.25f);
            newArrowObj.transform.localPosition = spawnPos;
            newArrowObj.transform.localRotation = Quaternion.Euler(90, 0, 0);

            Arrow arrow = newArrowObj.GetComponent<Arrow>();
            if (arrow != null)
            {
                arrow.OnTaken += OnArrowTaken;
            }

            UIManager.Instance?.UpdateArrowCount(arrowsRemaining);

            yield return new WaitForSeconds(0.1f);
        }
        Debug.Log("[ArrowBucket] All arrows spawned from pool.");
    }

    private void OnArrowTaken()
    {
        arrowsRemaining = Mathf.Max(0, arrowsRemaining - 1);
        UIManager.Instance?.UpdateArrowCount(arrowsRemaining);
    }

    public int GetArrowCount() => arrowsRemaining;
}

