using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class WindUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TextMeshProUGUI windText;
    [SerializeField] private RectTransform arrowImage;

    [Header("Rotation Settings")]
    [SerializeField] private bool rotateClockwise = false;
    private bool isFired = false;
    private void Update()
    {
        if (WindManager.Instance == null) return;

        Vector3 windDir = WindManager.Instance.WindDirection;
        float windStrength = WindManager.Instance.WindStrength;

        windText.text = $"Wind: {windStrength:F1} m/s";

        // �ٶ� ������ UI ȸ�� ������ ��ȯ (Z�� ����)
        float angle = Mathf.Atan2(windDir.x, windDir.z) * Mathf.Rad2Deg;

        angle -= 90f;

        if (!rotateClockwise)
        {
            angle *= -1f; // �ݽð� ���� ȸ��
        }

        arrowImage.rotation = Quaternion.Euler(0, 0, angle);
    }

    public bool IsFired() => isFired;

    public void SetFired() => isFired = true;
}
