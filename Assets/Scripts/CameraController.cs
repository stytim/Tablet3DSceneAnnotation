using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform dummyCamera;

    public bool trackingEnabled = false;

    private Vector3 translationInput = Vector3.zero;
    private Vector3 rotationInput = Vector3.zero;


    private Vector3 prevDummyCameraPosition;
    private Vector3 prevDummyCameraRotation;

    private void Start()
    {
        prevDummyCameraPosition = dummyCamera.position;
        prevDummyCameraRotation = dummyCamera.rotation.eulerAngles;
    }
    private void Update()
    {

        Vector3 relativeMovement = dummyCamera.position - prevDummyCameraPosition;

        Vector3 relativeRotation = dummyCamera.rotation.eulerAngles - prevDummyCameraRotation;
        if (trackingEnabled)
        {
            transform.position = transform.position + translationInput + relativeMovement;
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + rotationInput + relativeRotation);
            transform.position = new Vector3(transform.position.x, Mathf.Clamp(transform.position.y, -0.8f, 0.8f), transform.position.z);
        }
        else
        {
            transform.position = transform.position + translationInput;
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + rotationInput);
            transform.position = new Vector3(transform.position.x, Mathf.Clamp(transform.position.y, -0.8f, 0.8f), transform.position.z);
        }

        prevDummyCameraPosition = dummyCamera.position;
        prevDummyCameraRotation = dummyCamera.rotation.eulerAngles;


    }

    public void Move(Vector2 input, float Speed)
    {

        translationInput = input.y * Camera.main.transform.forward * Speed * Time.deltaTime + 
        input.x * Camera.main.transform.right * Speed * Time.deltaTime;
        //translationInput.y = 0;

    }

    public void Rotate(Vector2 input, float Speed)
    {

        rotationInput = new Vector3(-input.y, input.x , 0) * Speed * Time.deltaTime;

    }

    public void EnableDisableTracking(bool flag)
    {
        trackingEnabled = flag;
    }
}
