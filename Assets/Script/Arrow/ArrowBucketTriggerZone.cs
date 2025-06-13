using UnityEngine;

public class ArrowBucketTriggerZone : MonoBehaviour
{
    private void OnTriggerExit(Collider other)
    {
        Arrow arrow = other.GetComponent<Arrow>();
        if (arrow != null && !arrow.IsFired())
        {
            arrow.ForceTaken();
        }
    }
}
