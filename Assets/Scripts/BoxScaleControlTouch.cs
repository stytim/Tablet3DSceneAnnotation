using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxScaleControlTouch : MonoBehaviour
{
    private Vector2 touchStartPos;
    private Vector3 initialScale;
    private Vector3 initialPosition;
    private Camera mainCamera;
    private bool isSideSelected = false;
    private Vector3 selectedNormal;

    void Start()
    {
        mainCamera = Camera.main;
        initialScale = transform.localScale;
        initialPosition = transform.position;
    }

    void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            Ray ray = mainCamera.ScreenPointToRay(touch.position);
            RaycastHit hit;

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    if (Physics.Raycast(ray, out hit))
                    {
                        if (hit.transform == transform)
                        {
                            touchStartPos = touch.position;
                            isSideSelected = true;
                            selectedNormal = hit.normal;

                            Debug.Log(selectedNormal);
                        }
                    }
                    break;

                case TouchPhase.Moved:
                    if (isSideSelected)
                    {
                        Vector2 touchMove = touch.position - touchStartPos;

                        float extrudeAmount = touchMove.magnitude;// * Mathf.Sign(Vector2.Dot(touchMove, mainCamera.WorldToScreenPoint(selectedNormal).normalized));

                        Vector3 extrudeDirection = selectedNormal;

                        if (Vector3.Angle(Camera.main.transform.forward, transform.forward) < 90)
                        {
                            if (Mathf.Abs(touchMove.x) > Mathf.Abs(touchMove.y))
                            {
                                if (touchMove.x > 0)
                                {

                                    transform.localScale = initialScale + extrudeDirection * extrudeAmount * 0.01f;

                                    if (Vector3.Angle(extrudeDirection, Vector3.right) < 90f)
                                    {
                                        Debug.Log("right <90");
                                        transform.position = initialPosition + extrudeDirection * extrudeAmount * 0.005f;
                                    }
                                    else if (Vector3.Angle(extrudeDirection, Vector3.right) > 90f)
                                    {
                                        Debug.Log("right >90");
                                        transform.position = initialPosition - extrudeDirection * extrudeAmount * 0.005f;
                                    }

                                    if (Vector3.Angle(extrudeDirection, -Vector3.forward) < 90f)
                                    {
                                        Debug.Log("-forward <90");
                                        transform.localScale = initialScale - extrudeDirection * extrudeAmount * 0.01f;
                                        transform.position = initialPosition + extrudeDirection * extrudeAmount * 0.005f;
                                    }
                                    else if (Vector3.Angle(extrudeDirection, -Vector3.forward) > 90f)
                                    {
                                        Debug.Log("-forward >90");
                                        transform.localScale = initialScale - extrudeDirection * extrudeAmount * 0.01f;
                                        transform.position = initialPosition - extrudeDirection * extrudeAmount * 0.005f;
                                    }
                                        
                                    // Touch movement is to the right
                                }
                                else
                                {

                                    transform.localScale = initialScale - extrudeDirection * extrudeAmount * 0.01f;

                                    if (Vector3.Angle(extrudeDirection, Vector3.left) < 90f)
                                    {
                                        Debug.Log("left <90");
                                        transform.position = initialPosition + extrudeDirection * extrudeAmount * 0.005f;
                                    }
                                        
                                    else if (Vector3.Angle(extrudeDirection, Vector3.left) > 90f)
                                    {
                                        Debug.Log("left >90");
                                        transform.position = initialPosition - extrudeDirection * extrudeAmount * 0.005f;
                                    }

                                    if (Vector3.Angle(extrudeDirection, Vector3.forward) < 90f)
                                    {
                                        Debug.Log("-forward <90");
                                        transform.localScale = initialScale + extrudeDirection * extrudeAmount * 0.01f;
                                        transform.position = initialPosition + extrudeDirection * extrudeAmount * 0.005f;
                                    }
                                    else if (Vector3.Angle(extrudeDirection, Vector3.forward) > 90f)
                                    {
                                        Debug.Log("-forward >90");
                                        transform.localScale = initialScale + extrudeDirection * extrudeAmount * 0.01f;
                                        transform.position = initialPosition - extrudeDirection * extrudeAmount * 0.005f;
                                    }

                                    // Touch movement is to the left
                                }
                            }
                            else
                            {
                                if (touchMove.y > 0 && extrudeDirection.y != 0)
                                {
                                    transform.localScale = initialScale + extrudeDirection * extrudeAmount * 0.01f;

                                    if (Vector3.Angle(extrudeDirection, Vector3.up) < 90f)
                                        transform.position = initialPosition + extrudeDirection * extrudeAmount * 0.005f;
                                    else
                                        transform.position = initialPosition - extrudeDirection * extrudeAmount * 0.005f;
                                    // Touch movement is upwards
                                }
                                else
                                {
                                    transform.localScale = initialScale - extrudeDirection * extrudeAmount * 0.01f;

                                    if (Vector3.Angle(extrudeDirection, Vector3.down) < 90f)
                                        transform.position = initialPosition + extrudeDirection * extrudeAmount * 0.005f;
                                    else
                                        transform.position = initialPosition - extrudeDirection * extrudeAmount * 0.005f;
                                    // Touch movement is downwards
                                }
                            }
                        }
                        else
                        {

                            if (Mathf.Abs(touchMove.x) > Mathf.Abs(touchMove.y))
                            {
                                if (touchMove.x > 0)
                                {
                                    transform.localScale = initialScale - extrudeDirection * extrudeAmount * 0.01f;

                                    if (Vector3.Angle(extrudeDirection, Vector3.left) < 90f)
                                        transform.position = initialPosition + extrudeDirection * extrudeAmount * 0.005f;
                                    else if (Vector3.Angle(extrudeDirection, Vector3.left) > 90f)
                                        transform.position = initialPosition - extrudeDirection * extrudeAmount * 0.005f;

                                    if (Vector3.Angle(extrudeDirection, Vector3.forward) < 90f)
                                    {
                                        Debug.Log("-forward <90");
                                        transform.localScale = initialScale - extrudeDirection * extrudeAmount * 0.01f;
                                        transform.position = initialPosition - extrudeDirection * extrudeAmount * 0.005f;
                                    }
                                    else if (Vector3.Angle(extrudeDirection, Vector3.forward) > 90f)
                                    {
                                        Debug.Log("-forward >90");
                                        transform.localScale = initialScale - extrudeDirection * extrudeAmount * 0.01f;
                                        transform.position = initialPosition + extrudeDirection * extrudeAmount * 0.005f;
                                    }

                                    // Touch movement is to the right
                                }
                                else
                                {
                                    transform.localScale = initialScale + extrudeDirection * extrudeAmount * 0.01f;

                                    if (Vector3.Angle(extrudeDirection, Vector3.right) < 90f)
                                        transform.position = initialPosition + extrudeDirection * extrudeAmount * 0.005f;
                                    else if (Vector3.Angle(extrudeDirection, Vector3.right) > 90f)
                                        transform.position = initialPosition - extrudeDirection * extrudeAmount * 0.005f;

                                    if (Vector3.Angle(extrudeDirection, -Vector3.forward) < 90f)
                                    {
                                        Debug.Log("-forward <90");
                                        transform.localScale = initialScale + extrudeDirection * extrudeAmount * 0.01f;
                                        transform.position = initialPosition - extrudeDirection * extrudeAmount * 0.005f;
                                    }
                                    else if (Vector3.Angle(extrudeDirection, -Vector3.forward) > 90f)
                                    {
                                        Debug.Log("-forward >90");
                                        transform.localScale = initialScale + extrudeDirection * extrudeAmount * 0.01f;
                                        transform.position = initialPosition + extrudeDirection * extrudeAmount * 0.005f;
                                    }

                                    // Touch movement is to the left
                                }
                            }
                            else
                            {
                                if (touchMove.y > 0 &&  extrudeDirection.y != 0)
                                {
                                    transform.localScale = initialScale + extrudeDirection * extrudeAmount * 0.01f;

                                    if (Vector3.Angle(extrudeDirection, Vector3.up) < 90f)
                                        transform.position = initialPosition + extrudeDirection * extrudeAmount * 0.005f;
                                    else
                                        transform.position = initialPosition - extrudeDirection * extrudeAmount * 0.005f;
                                    // Touch movement is upwards
                                }
                                else
                                {
                                    transform.localScale = initialScale - extrudeDirection * extrudeAmount * 0.01f;

                                    if (Vector3.Angle(extrudeDirection, Vector3.down) < 90f)
                                        transform.position = initialPosition + extrudeDirection * extrudeAmount * 0.005f;
                                    else
                                        transform.position = initialPosition - extrudeDirection * extrudeAmount * 0.005f;
                                    // Touch movement is downwards
                                }
                            }
                        }


                    }
                    break;

                case TouchPhase.Ended:
                    isSideSelected = false;
                    initialScale = transform.localScale;
                    initialPosition = transform.position;
                    break;
            }
        }
    }
}
