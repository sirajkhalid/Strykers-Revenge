using UnityEngine;
using System.IO;

public static class SaveSystem
{
    public static void Save(string fileName, CharacterData data)
    {
        try
        {
            var path = Path.Combine(Application.persistentDataPath, fileName);
            var json = JsonUtility.ToJson(data, true);
            File.WriteAllText(path, json);
#if UNITY_EDITOR
            Debug.Log($"Saved character to: {path}\n{json}");
#endif
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Save failed: {ex}");
        }
    }

    public static CharacterData Load(string fileName)
    {
        try
        {
            var path = Path.Combine(Application.persistentDataPath, fileName);
            if (!File.Exists(path)) return null;
            var json = File.ReadAllText(path);
            var data = JsonUtility.FromJson<CharacterData>(json);
#if UNITY_EDITOR
            Debug.Log($"Loaded character from: {path}\n{json}");
#endif
            return data;
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Load failed: {ex}");
            return null;
        }
    }

    public static bool Exists(string fileName)
    {
        var path = Path.Combine(Application.persistentDataPath, fileName);
        return File.Exists(path);
    }
}
