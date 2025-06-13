using UnityEngine;
using UnityEditor;

public class AlignSpawnPoints : EditorWindow
{
    private Transform bucketCenter;
    private Transform spawnParent;

    [MenuItem("Tools/Align Arrow SpawnPoints")]
    public static void ShowWindow()
    {
        GetWindow<AlignSpawnPoints>("Align SpawnPoints");
    }

    private void OnGUI()
    {
        GUILayout.Label("Align Arrow SpawnPoints to Bucket Center", EditorStyles.boldLabel);

        bucketCenter = (Transform)EditorGUILayout.ObjectField("Bucket Center", bucketCenter, typeof(Transform), true);
        spawnParent = (Transform)EditorGUILayout.ObjectField("Spawn Points Parent", spawnParent, typeof(Transform), true);

        if (GUILayout.Button("Align All SpawnPoints"))
        {
            if (bucketCenter == null || spawnParent == null)
            {
                Debug.LogError("Please assign both Bucket Center and Spawn Points Parent.");
                return;
            }

            AlignAll();
        }
    }

    private void AlignAll()
    {
        foreach (Transform child in spawnParent)
        {
            Vector3 direction = (bucketCenter.position - child.position).normalized;
            child.rotation = Quaternion.LookRotation(direction);
        }

        Debug.Log("All spawn points aligned toward bucket center.");
    }
}
