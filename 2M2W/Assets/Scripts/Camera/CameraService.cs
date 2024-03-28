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
    //private XRReferenceImageLibrary referenceImages;
    //private MutableRuntimeReferenceImageLibrary library;
    public bool isUpdateCamera;

    private void Awake()
    {
        SubCamera = GetComponentInChildren<Camera>();
        imageManager = GetComponent<ARTrackedImageManager>();
        imageManager.enabled = false;
        tracker = GetComponent<MultipleImageTracker>();
        //referenceImages = (XRReferenceImageLibrary)imageManager.referenceLibrary;
        //library = imageManager.CreateRuntimeLibrary(referenceImages) as MutableRuntimeReferenceImageLibrary;
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

            //Texture2D texture = Managers.Resource.LoadTexture2D(docents[index].Ref);
            //AddImageToLibrary(texture, docents[index].Ref);
        }

        //imageManager.referenceLibrary = library;
        imageManager.requestedMaxNumberOfMovingImages = docents.Count;
        imageManager.enabled = true;
        tracker.AddPlacablePrefab();
        StartCameraService();
    }

    //private void AddImageToLibrary(Texture2D texture, string name)
    //{
    //    NativeArray<byte> imageData = new NativeArray<byte>(texture.GetRawTextureData<byte>(), Allocator.Persistent);
        
    //    int originWidth = texture.width;
    //    int originHeight = texture.height;
    //    int newWidth = 1;
    //    int newHeight = Mathf.RoundToInt((float)newWidth / originWidth * originHeight);

    //    Vector2Int size = new Vector2Int(newWidth, newHeight);
    //    XRReferenceImage referenceImage = new XRReferenceImage(new SerializableGuid(), new SerializableGuid(), size, name, texture);
    //    AddReferenceImageJobState state = library.ScheduleAddImageWithValidationJob(imageData, size, texture.format, referenceImage);

    //    state.jobHandle.Complete();
    //    imageData.Dispose();
    //}

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
