using UnityEngine;

public class FaceCamera : MonoBehaviour
{
    private Transform mainCameraTransform;

    private void Start()
    {
        // Get a reference to the main camera's transform
        mainCameraTransform = Camera.main.transform;
    }

    private void Update()
    {
        // Calculate the direction to the camera
        Vector3 directionToCamera = mainCameraTransform.position - transform.position;

        // Adjust the rotation to face the camera
        transform.rotation = Quaternion.LookRotation(-directionToCamera);
    }
}
