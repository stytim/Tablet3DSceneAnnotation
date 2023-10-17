using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Michsky.MUIP;

public class JointAdjustmentManager : MonoBehaviour
{
    [SerializeField] private SwitchManager mySwitch; // Switch variable
    JointAdjustmentAR jointAR;
    JointAdjustmentTouch jointTouch;
    public PoseVisualizer poseVisualizer;


    // Start is called before the first frame update
    void Start()
    {
        jointAR = GetComponent<JointAdjustmentAR>();
        jointTouch = GetComponent<JointAdjustmentTouch>();
    }


    public void StartAlignment()
    {
        if (mySwitch.isOn)
        {
            jointAR.enabled = true;
            jointTouch.enabled = false;
        }
        else
        {
            jointAR.enabled = false;
            jointTouch.enabled = true;
        }
    }

    public void ResetJoiint()
    {
        if (mySwitch.isOn)
        {
            jointAR.ResetJoint();
        }
        else
        {
            jointTouch.ResetJoint();
        }
    }

    public void ExitAdjustjoints()
    {
        if (mySwitch.isOn)
        {
            jointAR.ExitAdjustjoints();
            jointAR.enabled = false;
        }
        else
        {
            jointTouch.ExitAdjustjoints();
            jointTouch.enabled = false;
        }
        string path = UserData.Instance?.GetPoseSavingPath();
        poseVisualizer.SaveAdjustedPose(path);
    }

    public void ConfirmAdjustJoint()
    {
        if (mySwitch.isOn)
        {
            jointAR.ExitAdjustjoints();
        }
        else
        {
            jointTouch.ExitAdjustjoints();
        }
    }

}
