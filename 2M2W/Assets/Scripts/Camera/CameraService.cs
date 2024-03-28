using System.Collections.Generic;
//using System.Collections;
//using Unity.Jobs;
//using Unity.Collections;
using UnityEngine.XR.ARFoundation;
//using UnityEngine.XR.ARSubsystems;
using UnityEngine.Android;
using UnityEngine;

public class CameraService : MonoBehaviour
{
    [HideInInspector] public List<MarkerData> docents;
    [HideInInspector] public Camera SubCamera;

    private ARTrackedImageManager imageManager;
    private MultipleImageTracker tracker;
    public bool isUpdateCamera;

    private void Awake()
    {
        SubCamera = GetComponentInChildren<Camera>();
        imageManager = GetComponent<ARTrackedImageManager>();
        imageManager.enabled = false;
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

        imageManager.requestedMaxNumberOfMovingImages = docents.Count;
        imageManager.enabled = true;
        tracker.AddPlacablePrefab();
        StartCameraService();
    }

    public void StartCameraService(string permissionName = null)
    {
        if (true == Permission.HasUserAuthorizedPermission(Permission.Camera))
        {
            isUpdateCamera = !isUpdateCamera;
            SubCamera.gameObject.SetActive(isUpdateCamera);
        }
        else
        {
            SubCamera.gameObject.SetActive(isUpdateCamera);

            PermissionCallbacks callbacks = Managers.App.callbacks;
            callbacks.PermissionGranted += StartCameraService;
            Permission.RequestUserPermission(Permission.Camera, callbacks);
        }
    }
}
