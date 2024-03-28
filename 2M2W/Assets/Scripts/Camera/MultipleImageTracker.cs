using System.Collections.Generic;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.ARFoundation;
using UnityEngine;

public class MultipleImageTracker : MonoBehaviour
{
    public GameObject[] placeablePrefabs;

    private ARTrackedImageManager trackedImageManager;
    private Dictionary<string, GameObject> objData;

    private void Awake()
    {
        trackedImageManager = gameObject.GetComponent<ARTrackedImageManager>();
        objData = new Dictionary<string, GameObject>();
    }

    private void OnEnable()
        => trackedImageManager.trackedImagesChanged += OnTrackedImageChanged;

    public void AddPlacablePrefab()
    {
        foreach (GameObject obj in placeablePrefabs)
        {
            GameObject newObject = Instantiate(obj);
            newObject.name = obj.name;
            newObject.SetActive(false);

            objData.Add(newObject.name, newObject);
        }

        trackedImageManager.trackedImagesChanged += OnTrackedImageChanged;
    }

    private void OnTrackedImageChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        foreach (ARTrackedImage trackedImage in eventArgs.updated)
        {
            UpdateObject(trackedImage);
        }
    }

    private void UpdateObject(ARTrackedImage trackedImage)
    {
        string referenceName = trackedImage.referenceImage.name;

        if (trackedImage.trackingState == TrackingState.Tracking)
        {
            objData[referenceName].transform.position = trackedImage.transform.position;
            objData[referenceName].transform.rotation = trackedImage.transform.rotation;
            objData[referenceName].SetActive(true);
        }
        else
        {
            objData[referenceName].SetActive(false);
        }
    }

    private void OnDisable()
        => trackedImageManager.trackedImagesChanged -= OnTrackedImageChanged;
}
