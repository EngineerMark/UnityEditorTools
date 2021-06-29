using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
using System.IO;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

public class PListBuilder : MonoBehaviour
{
#if PLATFORM_IOS
    public static PropertyList list;

    public static PlistElementDict rootDict;

    [PostProcessBuild]
    public static void GeneratePList(BuildTarget buildTarget, string pathToBuiltProject)
    {
        if (list.UseCustomSettings)
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

            if (list.RequiredCapabilities.Count > 0)
            {
                PlistElementArray arr = rootDict["UIRequiredDeviceCapabilities"].AsArray();
                arr.values.Clear();

                list.RequiredCapabilities.ForEach(a =>
                {
                    arr.AddString(a);
                });
            }

            File.WriteAllText(plistPath, plist.WriteToString());
        }
    }

    private static void SetValue(string key, string value)
    {
        rootDict.SetString(key, value);
    }
#endif
}

public class PListBuildCheck : IPreprocessBuildWithReport
{
    public int callbackOrder => throw new System.NotImplementedException();

    public void OnPreprocessBuild(BuildReport report)
    {
        if (report.summary.platform == BuildTarget.iOS)
        {
            if (PListBuilder.list == null || !PListBuilder.list.UseCustomSettings)
            {
                bool _continue = EditorUtility.DisplayDialog("iOS Property List", "You are not using the custom PList settings that expand the default fields from Unity. Do you want to configure them or continue building?", "Continue", "Config");
                if (!_continue)
                {
                    PListEditor.Init();
                    throw new BuildFailedException("Build was canceled by the user.");
                }
            }
        }
    }
}

public class PListEditor : EditorWindow
{
    private static PropertyList list;
    private static Editor listEditor;

    [MenuItem("Build Tools/Property List Editor")]
    public static void Init()
    {
        list = (PropertyList)AssetDatabase.LoadAssetAtPath("Assets/PropertyList.asset", typeof(PropertyList));
        if (!list || list==null)
        {
            list = CreateInstance<PropertyList>();
            AssetDatabase.CreateAsset(list, "Assets/PropertyList.asset");
            AssetDatabase.Refresh();
        }

        Debug.Log("Debug: " + list);

        PListEditor window = (PListEditor)EditorWindow.GetWindow(typeof(PListEditor));
        window.Show();
    }

    private void OnGUI()
    {
#if PLATFORM_IOS
        if (list != null)
        {
            if(listEditor==null)
                listEditor = Editor.CreateEditor(list);

            listEditor.OnInspectorGUI();
            //GUILayout.Label("Permissions", EditorStyles.boldLabel);
            //list.PermissionsCamera = EditorGUILayout.TextField("Camera", list.PermissionsCamera);
            //list.PermissionsMicrophone = EditorGUILayout.TextField("Microphone", list.PermissionsMicrophone);
            //list.PermissionsPhotoLibrary = EditorGUILayout.TextField("Photo Library", list.PermissionsPhotoLibrary);
            //GUILayout.Label("Other", EditorStyles.boldLabel);
            //GUILayout.Label("Required Capabilities", EditorStyles.label);

            //if (list.RequiredCapabilities == null)
            //    list.RequiredCapabilities = new List<string>();
            //GUILayout.BeginHorizontal();
            //if (GUILayout.Button("+", EditorStyles.miniButton, GUILayout.Width(20)))
            //{
            //    list.RequiredCapabilities.Add("");
            //}
            //EditorGUI.BeginDisabledGroup(list.RequiredCapabilities.Count == 0);
            //if (GUILayout.Button("-", EditorStyles.miniButton, GUILayout.Width(20)))
            //{
            //    list.RequiredCapabilities.RemoveAt(list.RequiredCapabilities.Count - 1);
            //}
            //EditorGUI.EndDisabledGroup();
            //GUILayout.EndHorizontal();

            //for (int i = 0; i < list.RequiredCapabilities.Count; i++)
            //{
            //    list.RequiredCapabilities[i] = EditorGUILayout.TextField("", list.RequiredCapabilities[i]);
            //}

            EditorGUILayout.HelpBox("Build platform is correct, please fill in all required fields", MessageType.Info);
            EditorGUILayout.HelpBox("Empty fields will either use the Unity default or is not included at all", MessageType.Info);
            PListBuilder.list = list;
        }
        else
        {
            EditorGUILayout.HelpBox("Please select a PList file", MessageType.Error);
        }
#else
        EditorGUILayout.HelpBox("PList is only available on iOS platform.", MessageType.Warning);
#endif
    }
}