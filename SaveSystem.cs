using System.IO;
using UnityEngine;

public static class SaveSystem 
{
    private static string path = Application.persistentDataPath + "/save.game";

    public static void SaveGame(SaveData data)
    {
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(path, json);
    }

    public static SaveData LoadGame()
    {
        if (!File.Exists(path))
            return null;

        string json = File.ReadAllText(path);
        return JsonUtility.FromJson<SaveData>(json);
    }

    // ✅ ADD THIS
    public static void DeleteSave()
    {
        if (File.Exists(path))
        {
            File.Delete(path);
            Debug.Log("SAVE FILE DELETED");
        }
    }

    public static bool HasSave()
    {
        return File.Exists(path);
    }
}
