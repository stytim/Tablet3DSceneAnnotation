using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DigitalRubyShared;

public class JoystickController : MonoBehaviour
{
    /// <summary>
    /// Fingers joystick script
    /// </summary>
    [Tooltip("Fingers Joystick Script")]
    public FingersJoystickScript JoystickScript;

    /// <summary>
    /// Object to move with the joystick
    /// </summary>
    [Tooltip("Object to move with the joystick")]
    //public GameObject mover;

    //public GameObject model;

    public CameraController cameraController;

    /// <summary>
    /// First mask for joystick #1
    /// </summary>
    [Tooltip("First mask for joystick #1")]
    public Collider2D Mask1;

    public bool rotate = false;

    /// <summary>
    /// Units per second to move the Mover object with the joystick
    /// </summary>
    [Tooltip("Units per second to move the Mover object with the joystick")]
    public float Speed = 250.0f;

    private TapGestureRecognizer tapGesture;

    private void TapGestureFired(GestureRecognizer tap)
    {
        if (tap.State == GestureRecognizerState.Ended)
        {
            Debug.LogFormat("Tap gesture executed at {0},{1}", tap.FocusX, tap.FocusY);
            if (cameraController != null)
            {
                //mover.transform.position = Vector3.zero;
            }
        }
    }

    private void Awake()
    {
        JoystickScript.JoystickExecuted = JoystickExecuted;

    }

    private void OnEnable()
    {
        tapGesture = new TapGestureRecognizer { NumberOfTapsRequired = 2 };
        tapGesture.ClearTrackedTouchesOnEndOrFail = true;
        tapGesture.StateUpdated += TapGestureFired;
        tapGesture.AllowSimultaneousExecutionWithAllGestures();
        FingersScript.Instance.AddGesture(tapGesture);
        FingersScript.Instance.ShowTouches = false;

        // add first mask if it exists
        if (Mask1 != null && JoystickScript != null)
        {
            FingersScript.Instance.AddMask(Mask1, JoystickScript.PanGesture);
            FingersScript.Instance.AddMask(Mask1, tapGesture);
        }

    }

    private void OnDisable()
    {
        if (FingersScript.HasInstance)
        {
            FingersScript.Instance.RemoveGesture(tapGesture);

            // remove first mask if it exists
            if (Mask1 != null && JoystickScript != null)
            {
                FingersScript.Instance.RemoveMask(Mask1, JoystickScript.PanGesture);
            }

        }
    }

    private void Update()
    {

#if LOG_JOYSTICK

            foreach (Touch t in Input.touches)
            {
                Debug.LogFormat("Touch: {0},{1} {2}", t.position.x, t.position.y, t.phase);
            }

#endif

    }



    private void JoystickExecuted(FingersJoystickScript script, Vector2 amount)
    {

#if LOG_JOYSTICK

            Debug.LogFormat("Joystick: {0:0.000000},{1:0.000000}", amount.x, amount.y);

#endif

        if (cameraController != null && !rotate)
        {
            //Debug.LogFormat("Joystick: {0:0.000000},{1:0.000000}", amount.x, amount.y);
            //Vector3 pos = Vector3.zero; // cameraController.transform.position;
            //float height = pos.y;
            //Vector3 forwardInObjectSpace = mover.transform.InverseTransformDirection(Camera.main.transform.forward);

            //forwardInObjectSpace.y = 0;

            //Vector3 rightInObjectSpace = mover.transform.InverseTransformDirection(Camera.main.transform.right);
            //rightInObjectSpace.y = 0;

            //pos = (amount.y * Camera.main.transform.forward * Speed * Time.deltaTime);
            //pos += (amount.x * Camera.main.transform.right * Speed * Time.deltaTime);
            cameraController.Move(amount, Speed);
            //mover.transform.position = pos;// new Vector3(pos.x, mover.transform.position.y, pos.z);
        }

        if (rotate && cameraController != null)
        {
            cameraController.Rotate(amount, Speed);
            //Vector3 pos = mover.transform.rotation.eulerAngles;
            //pos.y += (amount.x * Speed * Time.deltaTime);
            //pos.x -= (amount.y  * Speed * Time.deltaTime);
            //mover.transform.rotation = Quaternion.Euler( new Vector3(pos.x, pos.y, 0));

            //mover.transform.RotateAround(Camera.main.transform.position, mover.transform.up, -amount.x * Speed * Time.deltaTime);
            //if (360-mover.transform.rotation.eulerAngles.x < 90 && mover.transform.rotation.eulerAngles.x > -90 && mover.transform.rotation.eulerAngles.z < 90 && mover.transform.rotation.eulerAngles.z > -90)
            //mover.transform.RotateAround(Camera.main.transform.position, Camera.main.transform.right, amount.y * Speed * Time.deltaTime);
        }


    }
}

