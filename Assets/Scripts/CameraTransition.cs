using UnityEngine;

public class CameraTransition : MonoBehaviour
{
    public Transform targetCamera;          // The camera to transition towards
    public float transitionDuration = 2.0f;  // The duration of the transition in seconds

    private Transform originalCamera;       // The original camera position
    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private Vector3 targetlPosition;
    private Quaternion targetRotation;
    public float transitionTime = 0.0f;     // The current time of the transition

    public Camera _camera, _targetCamera;
    private float targetOrthographicSize;

    public bool advanced = true;
    public bool move = false;
    public bool moveback = false;

    private bool disabeAfter = false;

    void Start()
    {

        _targetCamera = targetCamera.GetComponent<Camera>();
        _camera = GetComponent<Camera>();
        targetOrthographicSize = _targetCamera.orthographicSize;
    }

    void Update()
    {

        if (move)
        {
            // Check if the transition time has reached the duration
            if (transitionTime >= transitionDuration || Vector3.Distance(transform.position, targetCamera.position) < 0.0001f)
            {
                targetCamera.gameObject.SetActive(true);
                gameObject.SetActive(false);
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
                gameObject.SetActive(false);
                if (disabeAfter)
                {
                    disabeAfter = false;
                    targetCamera.gameObject.SetActive(false);
                }
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

    public void MoveCamera()
    {
        transform.position = targetCamera.position;
        transform.rotation = targetCamera.rotation;
        moveback = false;
        transitionTime = 0;
        advanced = false;
        move = true;
    }

    public void MoveCameraBack()
    {
        originalPosition = targetCamera.position;
        originalRotation = targetCamera.rotation;
        move = false;
        transitionTime = 0;
        advanced = false;
        moveback = true;
    }

    public void MoveCameraAdvanced()
    {
        transform.position = Camera.main.transform.position;
        transform.rotation = Camera.main.transform.rotation;
        targetlPosition = targetCamera.position;
        targetRotation = targetCamera.rotation;
        moveback = false;
        transitionTime = 0;
        targetOrthographicSize = _targetCamera.orthographicSize;
        move = true;
    }

    public void MoveCameraBackAdvanced()
    {
        transform.position = targetCamera.position;
        transform.rotation = targetCamera.rotation;
        originalPosition = Camera.main.transform.position;
        originalRotation = Camera.main.transform.rotation;
        move = false;
        transitionTime = 0;
        moveback = true;
    }

    public void DisableAfterMoveBack(bool flag)
    {
        disabeAfter = flag;
    }
}