using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
using System.IO;

public class PListBuilder : MonoBehaviour
{
#if PLATFORM_IOS
    public static Plist list;

    public static PlistElementDict rootDict;

    [PostProcessBuild]
    public static void GeneratePList(BuildTarget buildTarget, string pathToBuiltProject)
    {
        string plistPath = pathToBuiltProject + "/Info.plist";
        PlistDocument plist = new PlistDocument();
        plist.ReadFromString(File.ReadAllText(plistPath));
        rootDict = plist.root;

        if (list.PermissionsCamera != string.Empty)
            SetValue("NSCameraUsageDescription", list.PermissionsCamera);

        if (list.PermissionsMicrophone != string.Empty)
            SetValue("NSMicrophoneUsageDescription", list.PermissionsMicrophone);

        if (list.PermissionsPhotoLibrary != string.Empty)
            SetValue("NSPhotoLibraryUsageDescription", list.PermissionsPhotoLibrary);

        File.WriteAllText(plistPath, plist.WriteToString());
    }

    private static void SetValue(string key, string value)
    {
        rootDict.SetString(key, value);
    }
#endif
}

public class PListEditor : EditorWindow
{
    private static Plist list = new Plist();
    private static List<string> capabilities;

    [MenuItem("Build Tools/Property List Editor")]
    public static void Init()
    {
        capabilities = new List<string>();
        PListEditor window = (PListEditor)EditorWindow.GetWindow(typeof(PListEditor));
        window.Show();
    }

    private void OnGUI()
    {
#if PLATFORM_IOS
        GUILayout.Label("Permissions", EditorStyles.boldLabel);
        list.PermissionsCamera = EditorGUILayout.TextField("Camera", list.PermissionsCamera);
        list.PermissionsMicrophone = EditorGUILayout.TextField("Microphone", list.PermissionsMicrophone);
        list.PermissionsPhotoLibrary = EditorGUILayout.TextField("Photo Library", list.PermissionsPhotoLibrary);
        GUILayout.Label("Other", EditorStyles.boldLabel);
        GUILayout.Label("Required Capabilities", EditorStyles.label);

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("+", EditorStyles.miniButton, GUILayout.Width(20)))
        {
            capabilities.Add("");
        }
        EditorGUI.BeginDisabledGroup(capabilities.Count == 0);
        if (GUILayout.Button("-", EditorStyles.miniButton, GUILayout.Width(20)))
        {
            capabilities.RemoveAt(capabilities.Count-1);
        }
        EditorGUI.EndDisabledGroup();
        GUILayout.EndHorizontal();

        for (int i = 0; i < capabilities.Count; i++)
        {
            capabilities[i] = EditorGUILayout.TextField("", capabilities[i]);
        }

        if (GUILayout.Button("Apply"))
        {
            PListBuilder.list = list;
            ShowNotification(new GUIContent("Settings applied and ready for the build"), 3f);
        }
        EditorGUILayout.HelpBox("Build platform is correct, please fill in all required fields", MessageType.Info);
        EditorGUILayout.HelpBox("Empty fields will either use the Unity default or is not included at all", MessageType.Info);
#else
        EditorGUILayout.HelpBox("PList is only available on iOS platform.", MessageType.Warning);
#endif
    }
}


public class Plist
{
    public string PermissionsCamera;
    public string PermissionsMicrophone;
    public string PermissionsPhotoLibrary;

    public List<string> RequiredCapabilities;
}