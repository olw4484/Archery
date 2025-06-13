using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("UI Reference")]
    [SerializeField] private TextMeshProUGUI arrowCountText;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    /// <summary>
    /// 화살 수 UI 갱신
    /// </summary>
    /// <param name=\"count\">남은 화살 수</param>
    public void UpdateArrowCount(int count)
    {
        if (arrowCountText != null)
        {
            arrowCountText.text = $"Arrows: {count}";
        }
    }
}
