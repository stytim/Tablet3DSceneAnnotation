using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using static UnityEngine.GraphicsBuffer;

public class JointAdjustmentAR : MonoBehaviour
{
    // The AR raycast manager
    private ARRaycastManager raycastManager;

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

    private Camera mainCamera;
    // The joint that this script is attached to
    private Transform jointTransform;

    private Transform jointParent;

    // The position and rotation of the joint before adjustment
    private Vector3 originalPosition;
    private Vector3 currentPosition;
    private Vector3 offset;
    private float initalYAngle;
    private float initalXAngle;

    // A flag to indicate whether the joint is currently being adjusted
    private bool isAdjusting = false;

    void Start()
    {
        mainCamera = Camera.main;
        // Deactivate the camera views
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

                if (Physics.Raycast(ray, out hit) && hit.transform.CompareTag("keypoint") && !isAdjusting)
                {
                    // Get the transform of the joint this script is attached to
                    jointTransform = hit.transform;
                    poseVisualizer.JointSelectVisual(jointTransform.gameObject);
                    jointParent = jointTransform.parent;

                    keypointMat = jointTransform.gameObject.GetComponent<Renderer>().material;
                    jointTransform.gameObject.GetComponent<Renderer>().material = selectedMat;

                    // Save the original position and rotation of the joint
                    originalPosition = jointTransform.position;

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



                    //offset = mainCamera.transform.position - jointTransform.position;
                    //initalXAngle = mainCamera.transform.rotation.eulerAngles.x;
                    //initalYAngle = mainCamera.transform.rotation.eulerAngles.y;

                    jointTransform.parent = mainCamera.transform;

                    // Set the flag to indicate that the joint is being adjusted
                    isAdjusting = true;
                }
            }
        }

        // Check for device movement when the joint is being adjusted
        if (isAdjusting)
        {
            //// Get the current position of the joint
            //currentPosition = jointTransform.position;


            //float deltaY = mainCamera.transform.rotation.eulerAngles.y - initalYAngle;
            //float deltaX = mainCamera.transform.rotation.eulerAngles.x - initalXAngle;

            //// Scale the displacement by a factor
            //float movementScale = 0.1f;
            //displacement *= movementScale;

            //// Calculate the new position of the joint
            //Vector3 newPosition = currentPosition + displacement;

            // Assign the new position to the joint's transform component
            //jointTransform.position = mainCamera.transform.position - offset;

            UpdateCameraPositions();
        }
    }

    public void ExitAdjustjoints()
    {
        if (isAdjusting)
        {
            isAdjusting = false;
            jointTransform.parent = jointParent;
            jointTransform.gameObject.GetComponent<Renderer>().material = keypointMat;

            topCamera.gameObject.SetActive(false);
            frontCamera.gameObject.SetActive(false);
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

    // Reset the joint to its original position
    public void ResetJoint()
    {
        jointTransform.position = originalPosition;
    }
}
       
