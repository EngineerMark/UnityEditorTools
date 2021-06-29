using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "BuildTools/PropertyList", order = 1)]
public class PropertyList : ScriptableObject
{
    [Tooltip("If ticked, Unity will use the fields here that are populated")]
    public bool UseCustomSettings;

    [Tooltip("NSCameraUsageDescription: Explain why the camera is required")]
    public string PermissionsCamera;
    [Tooltip("NSMicrophoneUsageDescription: Explain why the microphone is required")]
    public string PermissionsMicrophone;
    [Tooltip("NSPhotoLibraryUsageDescription: Explain why the photo library is required")]
    public string PermissionsPhotoLibrary;

    [Tooltip("UIRequiredDeviceCapabilities: List the required capabilities here")]
    public List<string> RequiredCapabilities;
}