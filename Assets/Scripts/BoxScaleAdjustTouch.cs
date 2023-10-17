using UnityEngine;
using UnityEngine.UIElements;

public class BoxScaleAdjustTouch : MonoBehaviour
{
    public RectTransform[] touchArea;
    // The cube game object to scale
    public GameObject cube;

    // The speed at which to scale the cube
    public float scaleSpeed = 0.1f;

    // The last position of the touch input
    private Vector2 lastTouchPos;
    private Vector3 initialPosition;
    private Vector3 initialScale;

    private Vector3 normal;

    bool boxflag = false;

    // Update is called once per frame
    void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (RectTransformUtility.RectangleContainsScreenPoint(touchArea[0], touch.position) || RectTransformUtility.RectangleContainsScreenPoint(touchArea[1], touch.position))
            {

                if (touch.phase == TouchPhase.Began)
                {
                    // Cast a ray from the touch position
                    Ray ray = Camera.main.ScreenPointToRay(touch.position);
                    RaycastHit hit;
                    if (Physics.Raycast(ray, out hit))
                    {
                        // Check if the raycast hit the cube
                        if (hit.collider != null && hit.collider is BoxCollider && hit.collider.transform.parent.name == "custom")
                        {
                            cube = hit.collider.transform.parent.gameObject;
                            // Save the initial touch position
                            lastTouchPos = touch.position;

                            // Determine which face of the cube was hit
                            normal = hit.normal;

                            initialPosition = cube.transform.position;
                            initialScale = cube.transform.localScale;
                            boxflag = true;
                        }
                        else
                        {
                            boxflag = false;
                        }
                    }

                }
                else if (touch.phase == TouchPhase.Moved && boxflag)
                {
                    // Calculate the difference in touch position
                    Vector2 touchDelta = touch.position - lastTouchPos;

                    // Scale the cube based on the touchDelta and time elapsed since the last frame
                    float scaleFactor = scaleSpeed * Time.deltaTime;

                    if (Mathf.Abs(normal.y) > 0.9f)
                    {
                        // Scale the cube in the Y direction
                        Vector3 newScale = cube.transform.localScale;

                        if (Camera.main.transform.position.y > cube.transform.position.y)
                            newScale.y += scaleFactor * touchDelta.y;
                        else
                            newScale.y -= scaleFactor * touchDelta.y;
                        cube.transform.localScale = newScale;
                        cube.transform.position = new Vector3(initialPosition.x, initialPosition.y + Mathf.Sign(Camera.main.transform.position.y - cube.transform.position.y) * (newScale.y - initialScale.y) * 0.5f, initialPosition.z);
                    }
                    else if (Mathf.Abs(normal.x) > 0.9f)
                    {
                        float angle = Vector2.Angle(touchDelta, Vector2.up);
                        // Scale the cube in the X direction
                        Vector3 newScale = cube.transform.localScale;

                        if (angle < 20 || angle > 160)
                        {
                            if (Camera.main.transform.position.y > cube.transform.position.y)
                                newScale.y -= scaleFactor * touchDelta.y;
                            else
                                newScale.y += scaleFactor * touchDelta.y;
                            cube.transform.localScale = newScale;
                            cube.transform.position = new Vector3(initialPosition.x, initialPosition.y - Mathf.Sign(Camera.main.transform.position.y - cube.transform.position.y) * (newScale.y - initialScale.y) * 0.5f, initialPosition.z);
                        }
                        else
                        {

                            if (Camera.main.transform.position.z > cube.transform.position.z)
                                newScale.x -= scaleFactor * Mathf.Sign(normal.x) * touchDelta.x;
                            else
                                newScale.x += scaleFactor * Mathf.Sign(normal.x) * touchDelta.x;
                            cube.transform.localScale = newScale;
                            cube.transform.position = new Vector3(initialPosition.x + Mathf.Sign(Camera.main.transform.position.x - cube.transform.position.x) * (newScale.x - initialScale.x) * 0.5f, initialPosition.y, initialPosition.z);

                        }
                    }
                    else if (Mathf.Abs(normal.z) > 0.9f)
                    {

                        float angle = Vector2.Angle(touchDelta, Vector2.up);
                        // Scale the cube in the Z direction
                        Vector3 newScale = cube.transform.localScale;

                        if (angle < 20 || angle > 160)
                        {
                            if (Camera.main.transform.position.y > cube.transform.position.y)
                                newScale.y -= scaleFactor * touchDelta.y;
                            else
                                newScale.y += scaleFactor * touchDelta.y;
                            cube.transform.localScale = newScale;
                            cube.transform.position = new Vector3(initialPosition.x, initialPosition.y - Mathf.Sign(Camera.main.transform.position.y - cube.transform.position.y) * (newScale.y - initialScale.y) * 0.5f, initialPosition.z);
                        }
                        else
                        {
                            if (Camera.main.transform.position.x > cube.transform.position.x)
                                newScale.z += scaleFactor * Mathf.Sign(normal.z) * touchDelta.x;
                            else
                                newScale.z -= scaleFactor * Mathf.Sign(normal.z) * touchDelta.x;
                            cube.transform.localScale = newScale;
                            cube.transform.position = new Vector3(initialPosition.x, initialPosition.y, initialPosition.z + Mathf.Sign(Camera.main.transform.position.z - cube.transform.position.z) * (newScale.z - initialScale.z) * 0.5f);
                        }

                    }


                    // Save the current touch position
                    lastTouchPos = touch.position;

                    initialPosition = cube.transform.position;
                    initialScale = cube.transform.localScale;
                }
                else if (touch.phase == TouchPhase.Ended && boxflag)
                {
                    initialPosition = cube.transform.position;
                    initialScale = cube.transform.localScale;
                    cube = null;
                }
            }
        }
        else
        {
            boxflag = false;
        }
    }
}
