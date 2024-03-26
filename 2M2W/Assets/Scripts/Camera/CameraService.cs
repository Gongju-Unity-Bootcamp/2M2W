using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.Android;
using UnityEngine;
using System.Collections.Generic;
using UnityEditor.XR.ARSubsystems;

public class CameraService : MonoBehaviour
{
    [HideInInspector] public List<MarkerData> docents;

    private Camera subCamera;
    private ARTrackedImageManager imageManager;
    private MultipleImageTracker tracker;
    private XRReferenceImageLibrary library;
    public bool isUpdateCamera;

    private void Awake()
    {
        subCamera = GameObject.FindGameObjectWithTag("SubCamera").GetComponent<Camera>();
        imageManager = GetComponent<ARTrackedImageManager>();
        tracker = GetComponent<MultipleImageTracker>();
        library = new XRReferenceImageLibrary();
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
            Texture2D texture = Managers.Resource.LoadTexture2D(docents[index].Ref);
            library.SetTexture(index, texture, false);
            library.SetName(index, docents[index].Ref);
            library.SetSpecifySize(index, true);
            tracker.placeablePrefabs[index] = Managers.Resource.LoadDocent(docents[index].Ref);
        }

        imageManager.referenceLibrary = library;

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
