using UnityEngine.Android;
using UnityEngine;

public class CameraService : MonoBehaviour
{
    private Camera subCamera;
    public bool isUpdateCamera;

    private void Awake()
        => subCamera = GameObject.FindGameObjectWithTag("SubCamera").GetComponent<Camera>();

    private void Start()
        => StartCameraService();

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
