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
    /// ȭ�� �� UI ����
    /// </summary>
    /// <param name=\"count\">���� ȭ�� ��</param>
    public void UpdateArrowCount(int count)
    {
        if (arrowCountText != null)
        {
            arrowCountText.text = $"Arrows: {count}";
        }
    }
}
