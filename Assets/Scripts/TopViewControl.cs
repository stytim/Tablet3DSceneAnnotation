using System.Collections;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.UIElements;
using UnityEngine.XR.ARFoundation;

public class TopViewControl : MonoBehaviour
{
    public Camera minimapCamera, topViewCamera, mainCamera;
    public RenderTexture smallMap, bigMap, fov;
    private bool flag = true;
    public bool move = false;
    public Transform user;
    public RectTransform[] touchArea;

    public float zoomOutMin = 1;
    public float zoomOutMax = 8;

    private Vector3 touchStart;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.touchCount > 0 && move)
        {
            
            Touch touch = Input.GetTouch(0);

            // Move the cube if the screen has the finger moving.
            if (touch.phase == TouchPhase.Moved && Input.touchCount == 1)
            {

                Vector2 screenPosition = touch.position;
                Vector3 screenCoordinates = new Vector3(screenPosition.x, screenPosition.y, topViewCamera.nearClipPlane);
                Vector3 worldCoordinates = topViewCamera.ScreenToWorldPoint(screenCoordinates);
                
                if (RectTransformUtility.RectangleContainsScreenPoint(touchArea[0], screenPosition) || RectTransformUtility.RectangleContainsScreenPoint(touchArea[1], screenPosition))
                {
                    user.position = new Vector3(worldCoordinates.x, user.position.y, worldCoordinates.z);
                }

            }
        }


    }

    public void SwitchTopView()
    {
        if (flag)
        {
            minimapCamera.targetTexture = bigMap;
        }
        else
        {
            minimapCamera.targetTexture = smallMap;
        }
        flag = !flag;

    }

    public void SwitchView()
    {
        if (flag)
        {
            mainCamera.targetTexture = fov;
            move = true;
            //mainCamera.enabled = false;
        }
        else
        {
            //mainCamera.enabled = true;
            mainCamera.targetTexture = null;
            //arSession.Reset();
            move = false;
        }
        flag = !flag;
    }

    public void EnableDisableMove(bool _move)
    {
        move = _move;
    }
    void zoom(Camera _camera, float increment)
    {
        _camera.orthographicSize = Mathf.Clamp(_camera.orthographicSize - increment, zoomOutMin, zoomOutMax);
    }

}
