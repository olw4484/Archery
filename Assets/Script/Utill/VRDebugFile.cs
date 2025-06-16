using System.IO;
using UnityEngine;

public static class VRDebugFile
{
    private static readonly string debugFilePath = Application.persistentDataPath + "/vr_debug.txt";

    public static void Log(string message)
    {
        string time = System.DateTime.Now.ToString("HH:mm:ss.fff");
        File.AppendAllText(debugFilePath, $"[{time}] {message}\n");
    }

    public static void Clear()
    {
        File.WriteAllText(debugFilePath, "=== Log Start ===\n");
    }
}
