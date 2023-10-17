using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Michsky.MUIP;
using UnityEngine.UI;

public class BoundingBoxControlManager : MonoBehaviour
{
    [SerializeField] private SwitchManager mySwitch; // Switch variable
    [SerializeField] private Button topButton;
    [SerializeField] private Button fovButton;
    [SerializeField] private TopViewControl topViewControl;


    private LineRenderer lineRenderer;

    public GameObject DrawNext;
    public CameraTransitionVariant cameraTransitionVariant;

    private VertexClusterBoundingBoxes VertexClusterBoundingBoxes;
    private BoxScaleAdjustTouch adjustTouch; 
    private DrawRect drawRect;
    private DefineBox DefineBox;

    public GameObject adjustScale;

    // Start is called before the first frame update
    void Start()
    {
        drawRect = GetComponent<DrawRect>();
        VertexClusterBoundingBoxes = GetComponent<VertexClusterBoundingBoxes>();
        adjustTouch = GetComponent<BoxScaleAdjustTouch>();
        DefineBox = GetComponent<DefineBox>();
        lineRenderer = GetComponent<LineRenderer>();

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ExitContainer()
    {
        if (mySwitch.isOn)
        {
            VertexClusterBoundingBoxes.StopDefineBox();
            VertexClusterBoundingBoxes.enabled = false;

            adjustScale.SetActive(false);
            adjustTouch.enabled = false;

        

        }
        else
        {
            if (drawRect.enabled)
            {
                fovButton.onClick.Invoke();
            }

            drawRect.enabled = false;
            DefineBox.enabled = false;
            lineRenderer.enabled = false;

        }
    }

    public void EnableContainer()
    {
        if (mySwitch.isOn)
        {
            VertexClusterBoundingBoxes.enabled = true;
            VertexClusterBoundingBoxes.InitilizeKDTree();

            adjustScale.SetActive(true);
            adjustTouch.enabled = true;
        }
        else
        {
            topButton.onClick.Invoke();

            drawRect.enabled = true;
            DefineBox.enabled = true;
            lineRenderer.enabled = true;
            topViewControl.EnableDisableMove(false);

            DrawNext.SetActive(true);

        }
    }

    public void ConfirmBox(TMP_InputField text)
    {
        if (mySwitch.isOn)
        {
            VertexClusterBoundingBoxes.LabelName(text);
        }
        else
        {
            DefineBox.LabelName(text);
            DrawNext.SetActive(true);
            cameraTransitionVariant.MoveCameraBackAdvanced();
            drawRect.SecondView(false);

        }

    }

    public void DeleteLastBox()
    {
        if (mySwitch.isOn)
        {
            VertexClusterBoundingBoxes.DeleteLastBox();
        }
        else
        {
            DefineBox.DeleteLastBox();
        }
    }
}
