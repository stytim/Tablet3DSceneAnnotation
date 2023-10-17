using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Michsky.MUIP;
using TMPro;

public class EstablishConnection : MonoBehaviour
{
    public static EstablishConnection Instance;
    private Transform previousObject;
    private bool first = true;
    public RectTransform[] touchArea;


    [SerializeField] private SwitchManager mySwitch; 

    [SerializeField] private GameObject connectButton; 

    [SerializeField] private ContextMenuContent relationContent;
    [SerializeField] private ContextMenuManager relationMenu;

    public Image cross;
    public GameObject startPoint,endPoint;
    public FadingText text;

    public Vector3 contextMenuPos;

    private GameObject firstObj, secondObj, tmpObj;

    Vector3 startPos;
    Vector3 currentPos;
    LineRenderer lineRenderer;

    bool flag = true;

    public bool enableAnnotation = false;

    bool enableTouch = false;


    private void Awake()
    {
        Instance = this;
    }
        // Start is called before the first frame update
    void Start()
    {
        relationContent.ProcessContent();
        relationMenu.Close();
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.startWidth = 0.008f;
        PauseAnnotation();
    }

    public void PauseAnnotation()
    {
        enableAnnotation = false;
        connectButton.SetActive(false);
        cross.gameObject.SetActive(false);
        Debug.Log("Pause Annotation");
    }

    public void ResumeAnnotation()
    {
        enableAnnotation = true;

        if (mySwitch.isOn)
        {
            enableTouch = false;
            connectButton.SetActive(true);
            cross.gameObject.SetActive(true);
        }
        else
        {
            enableTouch = true;
        }
        Debug.Log("Resume Annotation");
    }


    public void EnableTouchAnnotation(bool _enableTouch)
    {
        enableTouch = _enableTouch;
    }

    // Update is called once per frame
    void Update()
    {
        relationMenu.transform.localPosition = contextMenuPos;
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit) )
        {
            if (hit.collider != null && hit.collider is not MeshCollider)
            {
                if (hit.transform.parent != null)
                {
                    text.subjectName.text = hit.transform.parent.name;
                    text.condition = true;
                }

                if (first)
                {
                    cross.color = Color.green;

                }
                else
                    cross.color = Color.yellow;
            }
        }
        else
        {
            cross.color = Color.white;
            text.condition = false;
        }

        HandleTouch();

    }

    public void LineRendererSwitch(bool enable)
    {
        flag = enable;
    }
    void UpdateLineRenderer(Vector3 pt1, Vector3 pt2)
    {
        if (flag)
        {
            lineRenderer.SetPosition(0, pt1);
            lineRenderer.SetPosition(1, pt2);
            startPoint.transform.position = Camera.main.WorldToScreenPoint(startPos);
            endPoint.transform.position = Camera.main.WorldToScreenPoint(currentPos);
        }
    }
    public void ConnectionObject()
    {
        // Construct a ray from the current touch coordinates
        //Ray ray = Camera.main.ScreenPointToRay(new Vector2());

        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));

        RaycastHit hit;
        // Create a particle if hit
        if (Physics.Raycast(ray, out hit, 10))
        {
            ConnectionManager.Instance?.AddConnection(ConnectionType.Line, hit.transform, hit.point, isSticky: false);

            if (!first)
            {
                relationMenu.transform.localPosition = contextMenuPos;
                relationMenu.Open();
            }

            first = !first;
            hit.transform.parent.GetComponent<BoundingBoxVisualizer>().ChangeTransparency(new Color(0.3f, 0.3f, 0.3f, 0f));
        }
    }

    private void HandleTouch()
    {
        if (Input.touchCount != 1 || !enableAnnotation || !enableTouch) return;

        Touch touch = Input.GetTouch(0);
        bool isTouchInArea = RectTransformUtility.RectangleContainsScreenPoint(touchArea[0], touch.position) ||
                             RectTransformUtility.RectangleContainsScreenPoint(touchArea[1], touch.position);

        if (!isTouchInArea)
        {
            ResetInteraction();
            return;
        }

        Ray touchRay = Camera.main.ScreenPointToRay(touch.position);
        bool isOverUI = UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject();

        switch (touch.phase)
        {
            case TouchPhase.Began:
                HandleTouchBegan(touchRay, isOverUI);
                break;
            case TouchPhase.Moved:
                HandleTouchMoved(touchRay);
                break;
            case TouchPhase.Ended:
                HandleTouchEnded(touchRay, isOverUI);
                break;
        }
    }

    private void HandleTouchBegan(Ray touchRay, bool isOverUI)
    {
        Vector3 screenPosDepth = Input.GetTouch(0).position;
        screenPosDepth.z = 0.5f;
        startPos = Camera.main.ScreenToWorldPoint(screenPosDepth);

        if (Physics.Raycast(touchRay, out RaycastHit hitInfo) && !isOverUI)
        {
            firstObj = hitInfo.transform.gameObject;
            ChangeObjectTransparency(firstObj, new Color(0.3f, 0.3f, 0.3f, 0f));
            ConnectionManager.Instance?.AddConnection(ConnectionType.Line, hitInfo.transform, hitInfo.point, isSticky: false);
        }
    }

    private void HandleTouchMoved(Ray touchRay)
    {
        Vector3 screenPosDepth = Input.GetTouch(0).position;
        screenPosDepth.z = 0.5f;
        currentPos = Camera.main.ScreenToWorldPoint(screenPosDepth);
        UpdateLineRenderer(startPos, currentPos);

        if (Physics.Raycast(touchRay, out RaycastHit hitInfo) && hitInfo.transform.gameObject != firstObj)
        {
            ChangeObjectTransparency(secondObj, Color.clear);
            secondObj = hitInfo.transform.gameObject;
            ChangeObjectTransparency(secondObj, new Color(0.3f, 0.3f, 0.3f, 0f));
        }
        else
        {
            ChangeObjectTransparency(secondObj, Color.clear);
            secondObj = null;
        }
    }

    private void HandleTouchEnded(Ray touchRay, bool isOverUI)
    {
        ResetLineRenderer();

        if (Physics.Raycast(touchRay, out RaycastHit hitInfo) && !isOverUI && firstObj != hitInfo.transform.gameObject && firstObj != null)
        {
            secondObj = hitInfo.transform.gameObject;
            ChangeObjectTransparency(secondObj, new Color(0.3f, 0.3f, 0.3f, 0f));
            ConnectionManager.Instance?.AddConnection(ConnectionType.Line, hitInfo.transform, hitInfo.point, isSticky: false);
            relationMenu.transform.localPosition = contextMenuPos;
            relationMenu.Open();
            PauseAnnotation();
            firstObj = null;
        }
        else
        {
            ConnectionManager.Instance?.DeleteLastObject();
            ChangeObjectTransparency(firstObj, Color.clear);
            firstObj = null;
        }
    }

    private void ResetInteraction()
    {
        ResetLineRenderer();
        ChangeObjectTransparency(firstObj, Color.clear);
        firstObj = null;
        ChangeObjectTransparency(secondObj, Color.clear);
    }

    private void ResetLineRenderer()
    {
        UpdateLineRenderer(Vector3.zero, Vector3.zero);
        startPoint.transform.position = Vector3.one * -2f;
        endPoint.transform.position = Vector3.one * -2f;
    }

    private void ChangeObjectTransparency(GameObject obj, Color color)
    {
        if (obj != null && obj.transform.parent != null && obj.transform.parent.TryGetComponent(out BoundingBoxVisualizer bbVisualizer))
        {
            bbVisualizer.ChangeTransparency(color);
        }
    }



}
