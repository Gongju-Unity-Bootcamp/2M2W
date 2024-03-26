using UnityEngine.Android;
using UnityEngine;

public class CameraService : MonoBehaviour
{
    private void Start()
        => StartCameraService();

    public void StartCameraService(string permissionName = null)
    {
        if (false == Permission.HasUserAuthorizedPermission(Permission.Camera))
        {
            PermissionCallbacks callbacks = new();
            callbacks.PermissionGranted += StartCameraService;
            Permission.RequestUserPermission(Permission.Camera, callbacks);
        }
    }
}
