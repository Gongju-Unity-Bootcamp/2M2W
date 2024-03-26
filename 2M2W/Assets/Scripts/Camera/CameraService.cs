using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.Android;
using UnityEngine;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor.XR.ARSubsystems;
#endif

public class CameraService : MonoBehaviour
{
    [HideInInspector] public List<MarkerData> docents;

    private Camera subCamera;
    private ARTrackedImageManager imageManager;
    private MultipleImageTracker tracker;
    public bool isUpdateCamera;

    private void Awake()
    {
        subCamera = GameObject.FindGameObjectWithTag("SubCamera").GetComponent<Camera>();
        imageManager = GetComponent<ARTrackedImageManager>();
        tracker = GetComponent<MultipleImageTracker>();
    }

    private void Start()
    {
        docents = new List<MarkerData>();

        foreach (MarkerData data in Managers.Data.Marker.Values)
        {
            if (false == string.IsNullOrEmpty(data.Ref))
            {
                docents.Add(data);
            }
        }
        
        tracker.placeablePrefabs = new GameObject[docents.Count];

        for (int index = 0; index < docents.Count; ++index)
        {
            tracker.placeablePrefabs[index] = Managers.Resource.LoadDocent(docents[index].Ref);
        }

        StartCameraService();
    }

    public void StartCameraService(string permissionName = null)
    {
        if (true == Permission.HasUserAuthorizedPermission(Permission.Camera))
        {
            isUpdateCamera = !isUpdateCamera;
            subCamera.gameObject.SetActive(isUpdateCamera);
        }
        else
        {
            subCamera.gameObject.SetActive(isUpdateCamera);

            PermissionCallbacks callbacks = Managers.App.callbacks;
            callbacks.PermissionGranted += StartCameraService;
            Permission.RequestUserPermission(Permission.Camera, callbacks);
        }
    }
}
