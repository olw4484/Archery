using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowPool : MonoBehaviour
{
    public GameObject arrowPrefab;
    public int poolSize = 10;
    private Queue<GameObject> pool = new Queue<GameObject>();

    private void Awake()
    {
        for (int i = 0; i < poolSize; i++)
        {
            var arrow = Instantiate(arrowPrefab, transform);
            arrow.SetActive(false);
            pool.Enqueue(arrow);
        }
    }

    public GameObject GetArrow()
    {
        if (pool.Count == 0)
            return null;
        var arrowObj = pool.Dequeue();
        arrowObj.SetActive(true);
        
        return arrowObj;
    }

    public void ReturnArrow(GameObject arrow)
    {
        arrow.SetActive(false);
        pool.Enqueue(arrow);
    }
}
