using UnityEngine;

public class CameraWork : MonoBehaviour
{
    const float offsetCameraUp = 0.4f;
    const float offsetCameraBack = 0.8f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void SetupCameraFollow()
    {
        Camera.main.transform.parent = transform;
        Camera.main.transform.localPosition = transform.up * offsetCameraUp + transform.forward * -1 * offsetCameraBack;
        Camera.main.transform.LookAt(transform.position);
    }
}