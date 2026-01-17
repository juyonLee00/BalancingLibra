#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.IO;

public class DataMenuTools
{
    [MenuItem("Tools/Clear Save Data (Delete All)")]
    public static void ClearSaveData()
    {
        string path = Path.Combine(Application.persistentDataPath, "UserGoodsData.json");
        if (File.Exists(path))
        {
            File.Delete(path); 
            Debug.Log("Delate Save Data!");
        }
        
        PlayerPrefs.DeleteKey("Setting_Sound"); 
        PlayerPrefs.Save();
        Debug.Log("Reset Setting Data");
    }

    [MenuItem("Tools/Open Save Folder")]
    public static void OpenSaveFolder()
    {
        EditorUtility.RevealInFinder(Application.persistentDataPath);
    }
}
#endif