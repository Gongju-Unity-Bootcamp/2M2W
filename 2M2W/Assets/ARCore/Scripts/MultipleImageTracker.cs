using System.Collections.Generic;
using System.Collections;
using UnityEngine.XR.ARFoundation;
using UnityEngine;

public class MultipleImageTracker : MonoBehaviour
{
    private ARTrackedImageManager trackedImageManager;

    public GameObject[] placeablePrefabs;
    private Dictionary<string, GameObject> objDic;

    private void Awake()
    {
        trackedImageManager = GetComponent<ARTrackedImageManager>();
        objDic = new Dictionary<string, GameObject>();

        foreach(GameObject obj in placeablePrefabs)
        {
            GameObject newObject = Instantiate(obj);
            newObject.name = obj.name;
            newObject.SetActive(false);

            objDic.Add(newObject.name, newObject);
        }
    }

    private void OnEnable()
    {
        trackedImageManager.trackedImagesChanged += OnTrackedImageChanged;
    }

    private void OnDisable()
    {
        trackedImageManager.trackedImagesChanged -= OnTrackedImageChanged;
    }

    private void OnTrackedImageChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        foreach(ARTrackedImage trackedImage in eventArgs.added)
        {
            UpdateObject(trackedImage);
        }
        foreach (ARTrackedImage trackedImage in eventArgs.updated)
        {
            UpdateObject(trackedImage);
        }
        foreach (ARTrackedImage trackedImage in eventArgs.removed)
        {
            objDic[trackedImage.name].SetActive(false);
        }
    }

    private void UpdateObject(ARTrackedImage trackedImage)
    {
        string referenceName = trackedImage.referenceImage.name;

        objDic[referenceName].transform.position = trackedImage.transform.position;
        objDic[referenceName].transform.rotation = trackedImage.transform.rotation;

        objDic[referenceName].SetActive(true);
    }

}
