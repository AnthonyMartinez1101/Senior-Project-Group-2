using UnityEngine;
using System.IO;

public static class SaveScript
{
    public static void SaveGame(GameObject player)
    {
        string path = Application.persistentDataPath + "/gameData.json";
        GameData data = new GameData(player);
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(path, json);
    }

    public static GameData LoadGame()
    {
        string path = Application.persistentDataPath + "/gameData.json";
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            return JsonUtility.FromJson<GameData>(json);
        }
        return null;
    }

    public static void DeleteSave()
    {
        string path = Application.persistentDataPath + "/gameData.json";
        if (File.Exists(path))
        {
            File.Delete(path);
        }
    }
}
