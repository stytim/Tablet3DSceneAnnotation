using System.Collections.Generic;

using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

using Dalak.LineRenderer3D;

[RequireComponent(typeof(ARRaycastManager))]
public class ARTapToPlace : MonoBehaviour
{
    [SerializeField] private GameObject sceneRoot;
    [SerializeField] private GameObject lineRoot;

    public RectTransform touchArea;

    private GameObject spawnedObject;
    private Vector2 touchPosition;
    private ARRaycastManager _arRaycastManager;
    private List<ARRaycastHit> hits = new List<ARRaycastHit>();

    public bool flag = false;

    private void Awake()
    {
        _arRaycastManager = GetComponent<ARRaycastManager>();
    }

    public void EnableARMode()
    {
        sceneRoot = Settings.Instance?.RetriveCurrentPlayer().gameObject;
        sceneRoot.transform.localScale = Vector3.one * 0.04f;
        var rendererComponents = sceneRoot.GetComponentsInChildren<Renderer>(true);
        // Disable rendering:
        foreach (var component in rendererComponents)
            component.enabled = false;

        //var lineRenderer3Ds = lineRoot.GetComponentsInChildren<LineRenderer3D>(true);
        // Modify radius:
        //foreach (var line in lineRenderer3Ds)
        //    line.pipeMeshSettings.radius = 0.01f * 0.04f;

        ConnectionManager.Instance?.ChangeLineRadius(0.01f * 0.06f);
        flag = true;
    }

    public void DisableARMode()
    {
        sceneRoot.transform.position = new Vector3(0,-1.5f,0);
        sceneRoot.transform.localScale = Vector3.one;
        sceneRoot.transform.rotation = Quaternion.identity;
        var rendererComponents = sceneRoot.GetComponentsInChildren<Renderer>(true);
        // Enable rendering:
        foreach (var component in rendererComponents)
            component.enabled = true;

        //var lineRenderer3Ds = lineRoot.GetComponentsInChildren<LineRenderer3D>(true);
        //// Modify radius:
        //foreach (var line in lineRenderer3Ds)
        //    line.pipeMeshSettings.radius = 0.01f;
        ConnectionManager.Instance?.ChangeLineRadius(0.01f);
        flag = false;
    }



    void Update()
    {
        if (flag)
        {
            if (Input.touchCount == 1)
            {
                Touch touch = Input.GetTouch(0);

                if (touch.phase == TouchPhase.Began || touch.phase == TouchPhase.Moved)
                {
                    touchPosition = touch.position;
                    
                    if (RectTransformUtility.RectangleContainsScreenPoint(touchArea, touchPosition))
                    {
                        if (_arRaycastManager.Raycast(touchPosition, hits, TrackableType.PlaneWithinPolygon))
                        {
                            Pose hitPose = hits[0].pose;
                           

                            sceneRoot.transform.position = hitPose.position;
                            var rendererComponents = sceneRoot.GetComponentsInChildren<Renderer>(true);
                            // Enable rendering:
                            foreach (var component in rendererComponents)
                                component.enabled = true;

                            GameObject[] planes = GameObject.FindGameObjectsWithTag("plane");
                            foreach (var plane in planes)
                                plane.GetComponent<ARPlaneMeshVisualizer>().enabled = false;
                        }
                    }
                }

            }
        }
    }

    public void TaptoPlaceSwitch(bool enable)
    {
        flag = enable;
    }
}