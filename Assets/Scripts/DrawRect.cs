using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DrawRect : MonoBehaviour
{
    public RectTransform[] touchArea;
    public Camera topViewCamera;
    public GameObject boxProxy;
    private LineRenderer lineRend;
    private Vector3 initialPosition, currentPosition;

    private float area;

    private bool secondView = false;

    // Start is called before the first frame update
    void Start()
    {
        lineRend = GetComponent<LineRenderer>();
        lineRend.positionCount = 0;

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);
            Vector2 screenPosition = touch.position;
            Vector3 screenCoordinates = new Vector3(screenPosition.x, screenPosition.y, topViewCamera.nearClipPlane);
            

            if (RectTransformUtility.RectangleContainsScreenPoint(touchArea[0], touch.position) || RectTransformUtility.RectangleContainsScreenPoint(touchArea[1], touch.position))
            {
                if (touch.phase == TouchPhase.Began)
                {
                    lineRend.positionCount = 4;
                    initialPosition = topViewCamera.ScreenToWorldPoint(screenCoordinates);
                    lineRend.SetPosition(0, new Vector3(initialPosition.x, 0, initialPosition.z));
                    lineRend.SetPosition(1, new Vector3(initialPosition.x, 0, initialPosition.z));
                    lineRend.SetPosition(2, new Vector3(initialPosition.x, 0, initialPosition.z));
                    lineRend.SetPosition(3, new Vector3(initialPosition.x, 0, initialPosition.z));
                }

                if (touch.phase == TouchPhase.Moved)
                {
                    currentPosition = topViewCamera.ScreenToWorldPoint(screenCoordinates);
                    lineRend.SetPosition(0, new Vector3(initialPosition.x, 0, initialPosition.z));
                    lineRend.SetPosition(1, new Vector3(initialPosition.x, 0, currentPosition.z));
                    lineRend.SetPosition(2, new Vector3(currentPosition.x, 0, currentPosition.z));
                    lineRend.SetPosition(3, new Vector3(currentPosition.x, 0, initialPosition.z));

                    //// Calculate position of the boxProxy
                    //Vector3 boxPosition = (initialPosition + currentPosition) / 2.0f;
                    //boxPosition.y = boxProxy.transform.position.y; // Preserve the Y position of the boxProxy
                    //boxProxy.transform.position = boxPosition;

                    //// Calculate scale of the boxProxy
                    //float distanceX = Mathf.Abs(initialPosition.x - currentPosition.x);
                    //float distanceZ = Mathf.Abs(initialPosition.z - currentPosition.z);
                    //Vector3 boxScale = new Vector3(distanceX, boxProxy.transform.localScale.y, distanceZ);
                    //boxProxy.transform.localScale = boxScale;

                }
            }

            if (secondView && (RectTransformUtility.RectangleContainsScreenPoint(touchArea[0], touch.position) || RectTransformUtility.RectangleContainsScreenPoint(touchArea[1], touch.position)) )
            {
                if (touch.phase == TouchPhase.Began)
                {
                    lineRend.positionCount = 4;
                    initialPosition = topViewCamera.ScreenToWorldPoint(screenCoordinates);
                    //lineRend.SetPosition(0, new Vector3(initialPosition.x, initialPosition.y));
                    //lineRend.SetPosition(1, new Vector3(initialPosition.x, initialPosition.y));
                    //lineRend.SetPosition(2, new Vector3(initialPosition.x, initialPosition.y));
                    //lineRend.SetPosition(3, new Vector3(initialPosition.x, initialPosition.y));
                    lineRend.SetPosition(0, initialPosition);
                    lineRend.SetPosition(1, initialPosition);
                    lineRend.SetPosition(2, initialPosition);
                    lineRend.SetPosition(3, initialPosition);
                }

                if (touch.phase == TouchPhase.Moved)
                {
                    currentPosition = topViewCamera.ScreenToWorldPoint(screenCoordinates);
                    //lineRend.SetPosition(0, new Vector3(initialPosition.x, initialPosition.y));
                    //lineRend.SetPosition(1, new Vector3(initialPosition.x, currentPosition.y));
                    //lineRend.SetPosition(2, new Vector3(currentPosition.x, currentPosition.y));
                    //lineRend.SetPosition(3, new Vector3(currentPosition.x, initialPosition.y));
                    lineRend.SetPosition(0, initialPosition);
                    lineRend.SetPosition(1, new Vector3(initialPosition.x, currentPosition.y, currentPosition.z));
                    lineRend.SetPosition(2, currentPosition);
                    lineRend.SetPosition(3, new Vector3(currentPosition.x, initialPosition.y, currentPosition.z));

                }
            }
        }
    }

    public void SecondView(bool flag)
    {
        secondView = flag;
    }
}
