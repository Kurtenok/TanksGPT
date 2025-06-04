using System.IO;
using UnityEngine;

public class ApiKeyLoader
{
    public static string LoadApiKey()
    {
#if UNITY_EDITOR
        string path = Application.dataPath + "/Resources/Secrets/apiKey.txt";
#else
        string path = Path.Combine(Application.persistentDataPath, "apiKey.txt");
#endif

        if (File.Exists(path))
        {
            return File.ReadAllText(path).Trim();
        }
        else
        {
            Debug.LogWarning("API ключ не знайдено!");
            return string.Empty;
        }
    }
}
