using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class CameraTransitionVariant : MonoBehaviour
{
    public LineRenderer line;
    public float transitionDuration = 2.0f;  // The duration of the transition in seconds

    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private Vector3 targetlPosition;
    private Quaternion targetRotation;
    public float transitionTime = 0.0f;     // The current time of the transition

    private Camera _camera;
    private float targetOrthographicSize;

    public bool advanced = false;
    public bool move = false;
    public bool moveback = false;
    public bool disabeAfter = false;

    void Start()
    {
        _camera = GetComponent<Camera>();
        originalPosition = transform.position;
        originalRotation = transform.rotation;
    }

    void Update()
    {

        if (move)
        {
            // Check if the transition time has reached the duration
            if (transitionTime >= transitionDuration || Vector3.Distance(transform.position, targetlPosition) < 0.0001f)
            {
                move = false;
                return; // Exit early if the transition is already complete
            }

            // Calculate the percentage of completion for the transition
            float t = transitionTime / transitionDuration;

            // Lerp the camera's position towards the target camera's position
            transform.position = Vector3.Lerp(transform.position, targetlPosition, t);
            // Lerp the camera's rotation towards the target camera's rotation
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, t);

            if (_camera.orthographic && advanced)
                _camera.orthographicSize = Mathf.Lerp(_camera.orthographicSize, targetOrthographicSize, t);

            // Increment the transition time based on the time passed since the last frame
            transitionTime += Time.deltaTime;

        }

        if (moveback)
        {
            // Check if the transition time has reached the duration
            if (transitionTime >= transitionDuration || Vector3.Distance(transform.position, originalPosition) < 0.0001f)
            {
                moveback = false;
                return; // Exit early if the transition is already complete
            }

            // Calculate the percentage of completion for the transition
            float t = transitionTime / transitionDuration;

            // Lerp the camera's position towards the target camera's position
            transform.position = Vector3.Lerp(transform.position, originalPosition, t);
            // Lerp the camera's rotation towards the target camera's rotation
            transform.rotation = Quaternion.Lerp(transform.rotation, originalRotation, t);

            if (_camera.orthographic && advanced)
                _camera.orthographicSize = Mathf.Lerp(_camera.orthographicSize, 1.8f, t);


            // Increment the transition time based on the time passed since the last frame
            transitionTime += Time.deltaTime;
        }

    }

    //public void MoveCamera()
    //{
    //    transform.position = targetCamera.position;
    //    transform.rotation = targetCamera.rotation;
    //    moveback = false;
    //    transitionTime = 0;
    //    advanced = false;
    //    move = true;
    //}

    //public void MoveCameraBack()
    //{
    //    originalPosition = targetCamera.position;
    //    originalRotation = targetCamera.rotation;
    //    move = false;
    //    transitionTime = 0;
    //    advanced = false;
    //    moveback = true;
    //}

    public void MoveCameraAdvanced()
    {
        originalPosition = transform.position;
        originalRotation = transform.rotation;
        Vector3 pt1 = line.GetPosition(1);
        Vector3 pt2 = line.GetPosition(2);
        Vector3 midpoint = 0.5f * (pt1 + pt2);
        float distanceFromMidpoint = 0.5f; // Distance from midpoint in meters

        targetlPosition = midpoint;
        targetlPosition.z -= distanceFromMidpoint;
        targetlPosition.y = -0.5f;
        targetRotation = Quaternion.identity;
        moveback = false;
        transitionTime = 0;

        move = true;
    }

    public void MoveCameraBackAdvanced()
    {
        move = false;
        transitionTime = 0;
        moveback = true;
    }

    public void DisableAfterMoveBack(bool flag)
    {
        disabeAfter = flag;
    }
}
