
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ToggleEx))]
public class ToggleEx_Editor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUI.changed)
        {
            EditorUtility.SetDirty(target);
        }
    }
}
