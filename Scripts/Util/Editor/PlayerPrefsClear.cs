using UnityEngine;
using UnityEditor;

public class PlayerPrefsClear : ScriptableObject
{
    [MenuItem("Tools/Clear all Editor Preferences")]
    static void deleteAllExample()
    {
        if (EditorUtility.DisplayDialog("Delete all PlayerPref",
            "Are you sure you want to delete all the PlayerPref? " +
            "This action cannot be undone.", "Yes", "No"))
        {
            PlayerPrefs.DeleteAll();
            PlayerPrefs.Save();

            Debug.Log("yes");
        }
    }
}
