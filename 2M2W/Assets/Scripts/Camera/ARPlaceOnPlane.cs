using System.Collections.Generic;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.ARFoundation;
using UnityEngine;

public class ARPlaceOnPlane : MonoBehaviour
{
    private ARRaycastManager raycastManager;
    private GameObject milestone;

    private void Start()
    {
        raycastManager = GetComponent<ARRaycastManager>();
        GameObject gameObject = Managers.Resource.Instantiate("Milestone");
        Managers.App.Milestone = gameObject;
        milestone = Managers.App.Milestone;
        milestone.SetActive(false);
    }

    private void Update()
    {
        if (false == Managers.App.updateLatLon)
        {
            return;
        }

        Camera sub = Managers.App.CameraService.SubCamera;
        Vector3 screenCenter = sub.ViewportToScreenPoint(new Vector2(0.5f, 0.5f));
        List<ARRaycastHit> hits = new List<ARRaycastHit>();
        raycastManager.Raycast(screenCenter, hits, TrackableType.Planes);

        if (hits.Count > 0)
        {
            Pose pose = hits[0].pose;
            milestone.SetActive(true);
            milestone.transform.SetPositionAndRotation(pose.position, pose.rotation);
            milestone.transform.LookAt(sub.transform.position);
        }
        else
        {
            milestone.SetActive(false);
        }
    }
}
