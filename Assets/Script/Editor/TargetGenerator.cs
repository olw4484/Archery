using UnityEditor;
using UnityEngine;

public class TargetGenerator : EditorWindow
{
    private GameObject parentObject;
    private GameObject ringPrefab;
    private float baseRadius = 0.05f;
    private float ringWidth = 0.05f;
    private int ringCount = 11;

    [MenuItem("Tools/Generate Archery Target")]
    public static void ShowWindow()
    {
        GetWindow<TargetGenerator>("Target Generator");
    }

    private void OnGUI()
    {
        GUILayout.Label("Archery Target Settings", EditorStyles.boldLabel);

        parentObject = (GameObject)EditorGUILayout.ObjectField("Parent Object", parentObject, typeof(GameObject), true);
        ringPrefab = (GameObject)EditorGUILayout.ObjectField("Ring Prefab", ringPrefab, typeof(GameObject), false);

        baseRadius = EditorGUILayout.FloatField("Inner Radius", baseRadius);
        ringWidth = EditorGUILayout.FloatField("Ring Width", ringWidth);
        ringCount = EditorGUILayout.IntField("Ring Count", ringCount);

        if (GUILayout.Button("Generate Rings"))
        {
            if (parentObject == null || ringPrefab == null)
            {
                Debug.LogError("Parent Object and Ring Prefab must be assigned.");
                return;
            }
            GenerateRings();
        }
    }

    private void GenerateRings()
    {
        for (int i = 0; i < ringCount; i++)
        {
            float radius = baseRadius + i * ringWidth;
            GameObject ring = (GameObject)PrefabUtility.InstantiatePrefab(ringPrefab, parentObject.transform);
            ring.name = $"Ring{ringCount - i}";

            ring.transform.localScale = new Vector3(radius * 2f, 0.01f, radius * 2f);
            ring.transform.localPosition = Vector3.zero;
            ring.transform.localRotation = Quaternion.identity;

            TargetZone zone = ring.GetComponent<TargetZone>();
            if (zone == null)
                zone = ring.AddComponent<TargetZone>();

            zone.score = ringCount - i;
        }

        Debug.Log("Archery target rings generated.");
    }
}
