using UnityEngine;

public class WindManager : MonoBehaviour
{
    public static WindManager Instance { get; private set; }

    [Header("Wind Settings")]
    [SerializeField] private float minStrength = 0f;
    [SerializeField] private float maxStrength = 5f;
    [SerializeField] private bool randomizeOnStart = true;

    [Header("Current Wind")]
    [SerializeField] private Vector3 windDirection = Vector3.zero;
    [SerializeField] private float windStrength = 0f;

    public Vector3 WindForce => windDirection * windStrength;
    public Vector3 WindDirection => windDirection;
    public float WindStrength => windStrength;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        if (randomizeOnStart)
            RandomizeWind();
    }

    public void RandomizeWind()
    {
        float angle = Random.Range(0f, 360f);
        windDirection = Quaternion.Euler(0, angle, 0) * Vector3.forward;
        windStrength = Random.Range(minStrength, maxStrength);

        Debug.Log($"[WindManager] Wind set to {windDirection} with strength {windStrength:F1} m/s");
    }

    public void SetWind(Vector3 direction, float strength)
    {
        windDirection = direction.normalized;
        windStrength = strength;
    }
}
