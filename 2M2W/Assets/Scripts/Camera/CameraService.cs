using System.Collections.Generic;
using Unity.Jobs;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.Android;
using UnityEngine;

public class CameraService : MonoBehaviour
{
    [HideInInspector] public List<MarkerData> docents;

    private Camera subCamera;
    private ARTrackedImageManager imageManager;
    private MultipleImageTracker tracker;
    private MutableRuntimeReferenceImageLibrary library;
    public bool isUpdateCamera;

    private void Awake()
    {
        subCamera = GameObject.FindGameObjectWithTag("SubCamera").GetComponent<Camera>();
        imageManager = GetComponent<ARTrackedImageManager>();
        imageManager.enabled = false;
        tracker = GetComponent<MultipleImageTracker>();
        library = imageManager.CreateRuntimeLibrary() as MutableRuntimeReferenceImageLibrary;
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
            Texture2D texture = Managers.Resource.LoadTexture2D(docents[index].Ref);

            AddImageToLibrary(texture, docents[index].Name);
        }

        imageManager.referenceLibrary = library;
        imageManager.enabled = true;

        StartCameraService();
    }

    private void AddImageToLibrary(Texture2D texture, string name)
    {
        AddReferenceImageJobState jobState = library.ScheduleAddImageWithValidationJob(texture, name, null);
        JobHandle handle = jobState.jobHandle;
        handle.Complete();
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
