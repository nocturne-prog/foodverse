
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using UnityEditor.Build.Reporting;
using Newtonsoft.Json;
using System.Linq;
using System;

public class Builder
{
    public static string[] SCENES = FindEnabledEditorScenes();
    public static string APP_NAME = "*****";
    public static string TARGET_DIR = "*****";

    [MenuItem("Build/Android APK Build/Build APK")]
    public static void Build()
    {
        BuildAPK();
    }

    [MenuItem("Build/Android APK Build/Build AAB")]
    public static void CleanBuild()
    {
        BuildAPK(true);
    }

    public static void BuildAPK(bool _isAAB = false)
    {
        string fileName = string.Format("{0}_Debug.{1}", APP_NAME, _isAAB ? "aab" : "apk");

        PlayerSettings.keystorePass = "*****";
        PlayerSettings.keyaliasPass = "*****";
        EditorUserBuildSettings.buildAppBundle = _isAAB;

        string strOutputDir = string.Format("{0}/{1}", Directory.GetCurrentDirectory(), TARGET_DIR);
        if (Directory.Exists(strOutputDir) == false)
        {
            var di = Directory.CreateDirectory(strOutputDir);
            if (di != null)
            {
                Debug.Log($"Make output Dir =>({di.FullName})");
            }
            else
            {
                Debug.Log("Directory is null");
            }
        }
        else
        {
            Debug.Log($"Make output Dir Exists , strOutputDir is {strOutputDir}");
        }

        GenericBuild(SCENES, strOutputDir + @"\" + fileName, BuildTargetGroup.Android, BuildTarget.Android, BuildOptions.None);
    }

    static void GenericBuild(string[] scenes, string target_dir, BuildTargetGroup build_group, BuildTarget build_target, BuildOptions build_options)
    {
        EditorUserBuildSettings.SwitchActiveBuildTarget(build_group, build_target);
        BuildReport report = BuildPipeline.BuildPlayer(scenes, target_dir, build_target, build_options);
        BuildSummary summary = report.summary;

        if (summary.result == BuildResult.Succeeded)
        {
            Debug.Log("Build succeeded: " + summary.totalSize + " bytes");
            
        }
        else if (summary.result == BuildResult.Failed)
        {
            Debug.Log("Build failed: " + summary.totalErrors + " erros");
        }
    }

    private static string[] FindEnabledEditorScenes()
    {
        List<string> EditorScenes = new List<string>();
        foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
        {
            if (!scene.enabled)
                continue;
            EditorScenes.Add(scene.path);
        }
        return EditorScenes.ToArray();
    }
}