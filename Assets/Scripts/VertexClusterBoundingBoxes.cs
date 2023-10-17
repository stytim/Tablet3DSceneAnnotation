using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System.Linq;
using DataStructures.ViliWonka.KDTree;
using TMPro;
using Michsky.MUIP;

public class VertexClusterBoundingBoxes : MonoBehaviour
{
    public RectTransform[] touchArea;
    public float maxRadius = 0.5f;
    //public Material boundingBoxMaterial;
    public int maxClusterCount = 10;

    public DatasetReceiver datasetReceiver;

    public GameObject AdjustBox, ConfirmBox;

    public bool isIntelligent = false;

    [SerializeField] private ProgressBar myBar;

    private Mesh mesh;
    private Vector3[] vertices;


    private bool isTouchingScreen = false;
    private float touchDuration = 0f;
    public float requiredTouchDuration = 1f;

    // K-d tree for fast nearest-neighbor search
    private KDTree kdtree;

    bool definebox = false;
    private string labelName;

    public void InitilizeKDTree()
    {
        GameObject scene = Settings.Instance?.GetCurrentScene();
        mesh = scene.GetComponent<MeshFilter>().mesh;
        vertices = mesh.vertices;

        Vector3[] vertices_world = new Vector3[vertices.Length];

        for (int i = 0; i < vertices.Length; ++i)
        {
            vertices_world[i] = scene.transform.TransformPoint(vertices[i]);
        }

        int maxPointsPerLeafNode = 32;
        kdtree = new KDTree(vertices_world, maxPointsPerLeafNode);
        scene.GetComponent<MeshCollider>().enabled = true;

        definebox = true;
    }

    public void StopDefineBox()
    {
        definebox = false;
    }

    public void EnableDefineBox()
    {
        definebox = true;
    }

    private void Update()
    {

        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);
            if (RectTransformUtility.RectangleContainsScreenPoint(touchArea[0], touch.position) || RectTransformUtility.RectangleContainsScreenPoint(touchArea[1], touch.position))
            {

                if (Input.GetTouch(0).phase == TouchPhase.Began)
                {
                    isTouchingScreen = true;
                    touchDuration = 0f;

                }
                else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled || touch.phase == TouchPhase.Moved)
                {
                    isTouchingScreen = false;
                    touchDuration = 0f;
                }

                if (isTouchingScreen)
                {
                    if (definebox)
                    {
                        myBar.isOn = true;
                        myBar.gameObject.SetActive(true);
                        myBar.gameObject.transform.position = Input.GetTouch(0).position;
                    }

                    touchDuration += Time.deltaTime;

                    if (touchDuration >= requiredTouchDuration)
                    {
                        Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
                        RaycastHit hit;

                        if (Physics.Raycast(ray, out hit) && definebox)
                        {
                            ComputeBoudingBox(hit.point);
                        }

                        isTouchingScreen = false;
                        touchDuration = 0f;
                    }
                }
            }
        }
        else
        {
            myBar.isOn = false;
            myBar.gameObject.SetActive(false);
        }
        myBar.currentPercent = touchDuration / requiredTouchDuration * 100;
    }

    public void LabelName(TMP_InputField text)
    {
        labelName = text.text;
        if (datasetReceiver.objects.ContainsKey("custom"))
        {
            GameObject customObject = datasetReceiver.objects["custom"];
            customObject.name = labelName;
            datasetReceiver.objects.Remove("custom");
            datasetReceiver.objects.Add(labelName, customObject);

            string frame_id = Settings.Instance?.CurrentFrameID();
            UserData.Instance?.SaveBoundingBoxesWithTag("5", frame_id);
        }
    }

    public void ComputeBoudingBox(Vector3 point)
    {

        KDQuery query = new KDQuery();

        List<int> results = new List<int>();
        // spherical query using k-d tree
        query.Radius(kdtree, point, maxRadius, results);

        if (results.Count > 0)
        {
            // Generate bounding box around the cluster
            //GameObject boundingBox = GameObject.CreatePrimitive(PrimitiveType.Cube);
            //MeshRenderer renderer = boundingBox.GetComponent<MeshRenderer>();
            //renderer.material = boundingBoxMaterial;

            // Compute center of the cluster
            Vector3 center = Vector3.zero;
            for (int i = 0; i < results.Count; i++)
            {
                center += vertices[results[i]];
            }
            center /= results.Count;

            //GameObject cluster = new GameObject("cluster");

            //// Create spheres for each vertex in the cluster
            //foreach (int index in results)
            //{
            //    GameObject vertexSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            //    vertexSphere.transform.position = vertices[index];
            //    vertexSphere.transform.localScale = Vector3.one * 0.02f;
            //    vertexSphere.transform.parent = cluster.transform;
            //}


            // Find minimum and maximum X, Y, and Z coordinates among all vertices in the cluster
            Vector3 min = vertices[results[0]];
            Vector3 max = vertices[results[0]];
            for (int i = 1; i < results.Count; i++)
            {
                min.x = Mathf.Min(min.x, vertices[results[i]].x);
                min.y = Mathf.Min(min.y, vertices[results[i]].y);
                min.z = Mathf.Min(min.z, vertices[results[i]].z);
                max.x = Mathf.Max(max.x, vertices[results[i]].x);
                max.y = Mathf.Max(max.y, vertices[results[i]].y);
                max.z = Mathf.Max(max.z, vertices[results[i]].z);
            }

            // Compute size of the bounding box
            Vector3 size = max - min;

            GameObject bbox = new GameObject("custom");
            bbox.tag = "5";
            //bbox.transform.parent = transform;
            var vis = bbox.AddComponent<BoundingBoxVisualizer>();
            vis.MaterialColor = Configurations.AssignColor(LabelType.otherobject);

            bbox.transform.parent = Settings.Instance?.RetriveCurrentPlayer().transform; //cluster.transform; 
            bbox.transform.position = new Vector3(-center.x, center.y-1.5f, -center.z);
            
            if (isIntelligent)
                bbox.transform.localScale = size;
            else
                bbox.transform.localScale = Vector3.one;

            //boundingBox.transform.parent = cluster.transform;

            //cluster.transform.localScale = new Vector3(-1, 1, -1);

            //Vector3 cutserPos = cluster.transform.position;
            //cutserPos.y = -1.5f;
            //cluster.transform.position = cutserPos;

            datasetReceiver.objects.Add("custom", bbox);
            StopDefineBox();

            AdjustBox.SetActive(true);
            ConfirmBox.SetActive(true);
        }

    }

    public void DeleteLastBox()
    {
        if (datasetReceiver.objects.ContainsKey(labelName))
        {
            GameObject customObject = datasetReceiver.objects[labelName];
            datasetReceiver.objects.Remove(labelName);
            Destroy(customObject);

        }
    }

}
