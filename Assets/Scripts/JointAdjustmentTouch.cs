using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class JointAdjustmentTouch : MonoBehaviour
{
    // The cameras that will show the joint from different angles
    public Camera topCamera;
    public Camera frontCamera;
    public Camera rightCamera;

    // The UI panels that will show the camera views
    public GameObject topPanel;
    public GameObject frontPanel;
    public GameObject rightPanel;

    public Material selectedMat;

    public PoseVisualizer poseVisualizer;
    private Material keypointMat;

    // The joint that this script is attached to
    private Transform jointTransform;

    // The position and rotation of the joint before adjustment
    private Vector3 originalPosition;
    private Vector3 currentPosition;
    private Quaternion originalRotation;

    // A flag to indicate whether the joint is currently being adjusted
    private bool isAdjusting = false;

    private Vector2 initialPosition;

    void Start()
    {

        // Deactivate the camera views and UI panels
        topCamera.gameObject.SetActive(false);
        frontCamera.gameObject.SetActive(false);
        rightCamera.gameObject.SetActive(false);
        topPanel.SetActive(false);
        frontPanel.SetActive(false);
        rightPanel.SetActive(false);
    }

    void Update()
    {
        // Check for touch events on the joint
        if (Input.touchCount > 0 && !isAdjusting)
        {
            Touch touch = Input.GetTouch(0);
            // Check if the touch is on the joint
            if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Began)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit) && hit.transform.CompareTag("keypoint"))
                {
                    // Get the transform of the joint this script is attached to
                    jointTransform = hit.transform;

                    poseVisualizer.JointSelectVisual(jointTransform.gameObject);

                    keypointMat = jointTransform.gameObject.GetComponent<Renderer>().material;
                    jointTransform.gameObject.GetComponent<Renderer>().material = selectedMat;

                    // Save the original position and rotation of the joint
                    originalPosition = jointTransform.position;
                    originalRotation = jointTransform.rotation;

                    // Activate the camera views and UI panels
                    topCamera.gameObject.SetActive(true);
                    frontCamera.gameObject.SetActive(true);
                    rightCamera.gameObject.SetActive(true);
                    topPanel.SetActive(true);
                    frontPanel.SetActive(true);
                    rightPanel.SetActive(true);

                    // Set the camera positions and orientations
                    topCamera.transform.position = jointTransform.position + Vector3.up * 5f;
                    topCamera.transform.rotation = Quaternion.LookRotation(Vector3.down, Vector3.forward);

                    frontCamera.transform.position = jointTransform.position + Vector3.forward * 5f;
                    frontCamera.transform.rotation = Quaternion.LookRotation(Vector3.back, Vector3.up);

                    rightCamera.transform.position = jointTransform.position + Vector3.right * 5f;
                    rightCamera.transform.rotation = Quaternion.LookRotation(Vector3.left, Vector3.up);

                    // Set the flag to indicate that the joint is being adjusted
                    isAdjusting = true;
                }
            }
        }

        // Check for touch events when the joint is being adjusted
        if (isAdjusting && Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began &&
                (RectTransformUtility.RectangleContainsScreenPoint(topPanel.GetComponent<RectTransform>(), touch.position) ||
                 RectTransformUtility.RectangleContainsScreenPoint(frontPanel.GetComponent<RectTransform>(), touch.position) ||
                 RectTransformUtility.RectangleContainsScreenPoint(rightPanel.GetComponent<RectTransform>(), touch.position)))

            {
                initialPosition = touch.position;
                currentPosition = jointTransform.position;
            }

            // Check if the touch is on one of the UI panels
            if (touch.phase == TouchPhase.Moved &&
                (RectTransformUtility.RectangleContainsScreenPoint(topPanel.GetComponent<RectTransform>(), touch.position) ||
                 RectTransformUtility.RectangleContainsScreenPoint(frontPanel.GetComponent<RectTransform>(), touch.position) ||
                 RectTransformUtility.RectangleContainsScreenPoint(rightPanel.GetComponent<RectTransform>(), touch.position)))
            {
                // Calculate how much the finger has moved on the panel
                Vector2 deltaPosition = touch.position - initialPosition;

                // Scale the movement by a factor
                float movementScale = 0.005f;
                deltaPosition *= movementScale;


                Vector3 newPosition = currentPosition;
                // Calculate the new position of the joint on the x-z plane
                //Vector3 newPosition = currentPosition + new Vector3(deltaPosition.x, 0f, deltaPosition.y);


                if (RectTransformUtility.RectangleContainsScreenPoint(frontPanel.GetComponent<RectTransform>(), touch.position))
                {
                    // Move the joint on the x-y plane when touching the front panel
                    newPosition += new Vector3(-deltaPosition.x, deltaPosition.y, 0f);
                }
                else if (RectTransformUtility.RectangleContainsScreenPoint(rightPanel.GetComponent<RectTransform>(), touch.position))
                {
                    // Move the joint on the y-z plane when touching the side panel
                    newPosition += new Vector3(0f, deltaPosition.y, deltaPosition.x);
                }
                else if (RectTransformUtility.RectangleContainsScreenPoint(topPanel.GetComponent<RectTransform>(), touch.position))
                {
                    // Move the joint on the y-z plane when touching the side panel
                    newPosition += new Vector3(deltaPosition.x, 0f, deltaPosition.y);
                }

                // Assign the new position to the joint's transform component
                jointTransform.position = newPosition;

                UpdateCameraPositions();
            }

            if (touch.phase == TouchPhase.Ended &&
                (RectTransformUtility.RectangleContainsScreenPoint(topPanel.GetComponent<RectTransform>(), touch.position) ||
                 RectTransformUtility.RectangleContainsScreenPoint(frontPanel.GetComponent<RectTransform>(), touch.position) ||
                 RectTransformUtility.RectangleContainsScreenPoint(rightPanel.GetComponent<RectTransform>(), touch.position)))
            {
                currentPosition = jointTransform.position;
            }
        }


    }

    public void ExitAdjustjoints()
    {
        if (isAdjusting)
        {
            isAdjusting = false;
            jointTransform.gameObject.GetComponent<Renderer>().material = keypointMat;

          
            frontCamera.gameObject.SetActive(false);
            topCamera.gameObject.SetActive(false);
            rightCamera.gameObject.SetActive(false);

            topPanel.SetActive(false);
            frontPanel.SetActive(false);
            rightPanel.SetActive(false);

            poseVisualizer.JointDeselectVisual();

        }
    }

    private void UpdateCameraPositions()
    {
        // Set the camera positions and orientations based on the current position of the joint
        topCamera.transform.position = jointTransform.position + Vector3.up * 5f;
        topCamera.transform.rotation = Quaternion.LookRotation(Vector3.down, Vector3.forward);

        frontCamera.transform.position = jointTransform.position + Vector3.forward * 5f;
        frontCamera.transform.rotation = Quaternion.LookRotation(Vector3.back, Vector3.up);

        rightCamera.transform.position = jointTransform.position + Vector3.right * 5f;
        rightCamera.transform.rotation = Quaternion.LookRotation(Vector3.left, Vector3.up);
    }

    // Reset the joint to its original position and rotation
    public void ResetJoint()
    {
        jointTransform.position = originalPosition;
        jointTransform.rotation = originalRotation;
    }
}