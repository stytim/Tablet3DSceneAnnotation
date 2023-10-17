using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;

public class PoseVisualizer : MonoBehaviour
{
    //[SerializeField] private string jsonFilePath;
    [SerializeField] private GameObject jointPrefab;
    [SerializeField] private float scaleFactor = 1.0f;
    [SerializeField] private Material lineMaterial;

    private int CurrentIndex = 0;
    private string dataPath;

    private string poseName;

    Dictionary<string, GameObject> humanParents = new Dictionary<string, GameObject>();
    Dictionary<string, Dictionary<string, GameObject>> jointObjectsByHuman = new Dictionary<string, Dictionary<string, GameObject>>();

    private string[] allFiles;

    private bool showPose = true;

    private void Awake()
    {
        dataPath = Path.Combine(Application.persistentDataPath, "annotations");
        allFiles = Directory.GetFiles(dataPath, "*.json");
    }
    private void Update()
    {
        LoadPoses(CurrentIndex);
    }

    public void LoadPoses(int frame)
    {
        if (CurrentIndex != frame && showPose)
        {

            CurrentIndex = frame;
            foreach (var jointObjs in jointObjectsByHuman.Values)
            {
                foreach (var gameObject in jointObjs.Values)
                {
                    GameObject.Destroy(gameObject);
                }
                jointObjs.Clear();
            }
            //Debug.Log(allFiles[frame]);
            string jsonString = File.ReadAllText(allFiles[frame]);
            PoseData poseData = JsonConvert.DeserializeObject<PoseData>(jsonString);
            poseName = poseData.name;

            Dictionary<string, GameObject> jointObjects = new Dictionary<string, GameObject>();

            foreach (Label label in poseData.labels)
            {
                Vector3 position = new Vector3(
                    (float)label.point3d.location.x * scaleFactor,
                    ((float)label.point3d.location.z + 10) * scaleFactor,
                    (float)label.point3d.location.y * scaleFactor
                );
                GameObject jointInstance = Instantiate(jointPrefab, position, Quaternion.identity);
                TMPro.TextMeshPro jointName = jointInstance.GetComponentInChildren<TMPro.TextMeshPro>();
                if(jointName)
                {
                    jointName.text = label.jointName;
                }
                jointInstance.name = $"{label.humanName}_{label.jointName}";

                // Parent joint instance to a GameObject with the same humanName
                if (!jointObjectsByHuman.ContainsKey(label.humanName))
                {
                    GameObject humanParent = new GameObject(label.humanName);
                    humanParent.tag = "pose";
                    humanParents[label.humanName] = humanParent;
                    jointObjectsByHuman[label.humanName] = new Dictionary<string, GameObject>();
                }
                jointInstance.transform.SetParent(humanParents[label.humanName].transform);

                jointObjectsByHuman[label.humanName][label.jointName] = jointInstance;


                // Store joint instance in dictionary for future reference
                jointObjects[label.jointName] = jointInstance;
            }
        }


        foreach (string humanName in jointObjectsByHuman.Keys)
        {
            // Connect joints with LineRenderer
            ConnectJoints(jointObjectsByHuman[humanName], "head", "neck", humanName);

            ConnectTwoJoints(jointObjectsByHuman[humanName], "neck", "leftshoulder", "rightshoulder", humanName);

            ConnectTwoJoints(jointObjectsByHuman[humanName], "leftshoulder", "leftelbow", "lefthip", humanName);

            ConnectTwoJoints(jointObjectsByHuman[humanName], "rightshoulder", "rightelbow", "righthip", humanName);

            ConnectJoints(jointObjectsByHuman[humanName], "leftelbow", "leftwrist", humanName);
            ConnectJoints(jointObjectsByHuman[humanName], "rightelbow", "rightwrist", humanName);

            ConnectTwoJoints(jointObjectsByHuman[humanName], "lefthip", "righthip", "leftknee", humanName);

            ConnectJoints(jointObjectsByHuman[humanName], "righthip", "rightknee", humanName);
            ConnectJoints(jointObjectsByHuman[humanName], "leftknee", "leftfoot", humanName);
            ConnectJoints(jointObjectsByHuman[humanName], "rightknee", "rightfoot", humanName);
        }


    }

    public void JointSelectVisual(GameObject selectedJoint)
    {
        // Find the human name that the selected joint belongs to
        string selectedHumanName = "";
        foreach (var humanPair in jointObjectsByHuman)
        {
            if (humanPair.Value.ContainsValue(selectedJoint))
            {
                selectedHumanName = humanPair.Key;
                break;
            }
        }

        if (selectedHumanName == "")
        {
            // The selected joint does not belong to any known human, do nothing
            return;
        }

        // Deactivate all joints that are not from the same human
        foreach (var humanPair in jointObjectsByHuman)
        {
            string humanName = humanPair.Key;
            Dictionary<string, GameObject> jointObjs = humanPair.Value;

            foreach (var jointObjPair in jointObjs)
            {
                GameObject jointObj = jointObjPair.Value;

                // Check if the joint belongs to the selected human or not
                bool isSameHuman = (humanName == selectedHumanName);

                // Activate or deactivate the joint based on whether it belongs to the same human or not
                jointObj.SetActive(isSameHuman);
            }
        }
    }

    public void DisableVisualization()
    {
        foreach (var pair in humanParents)
        {
            pair.Value.SetActive(false);
        }
        showPose = false;
    }

    public void EnableVisualization()
    {
        foreach (var pair in humanParents)
        {
            pair.Value.SetActive(true);
        }
        showPose = true;
    }


    public void JointDeselectVisual()
    {
        // Activate all joints in the jointObjectsByHuman dictionary
        foreach (var humanPair in jointObjectsByHuman)
        {
            Dictionary<string, GameObject> jointObjs = humanPair.Value;

            foreach (var jointObjPair in jointObjs)
            {
                GameObject jointObj = jointObjPair.Value;
                jointObj.SetActive(true);
            }
        }
    }

    public void SaveAdjustedPose(string pathName)
    {
        PoseData poseData = new PoseData();
        poseData.name = poseName;
        poseData.timestamp = 0; // Set the appropriate timestamp
        poseData.index = 0; // Set the appropriate index

        poseData.labels = new List<Label>();

        foreach (var jointObjs in jointObjectsByHuman.Values)
        {
            foreach (var jointObj in jointObjs.Values)
            {
                Label label = new Label();
                string[] nameParts = jointObj.name.Split('_');
                label.humanName = nameParts[0];
                label.jointName = nameParts[1];
                label.point3d = new Point3D();
                label.point3d.location = new Location();
                label.point3d.location.x = jointObj.transform.position.x / scaleFactor;
                label.point3d.location.y = jointObj.transform.position.z / scaleFactor;
                label.point3d.location.z = jointObj.transform.position.y / scaleFactor -10;
                label.unsure = false;
                label.track_idx = 0;
                label.is_interpolated = false;

                poseData.labels.Add(label);
            }
        }

        pathName = Path.Combine(pathName, poseName + "_user_pose.json");
        string jsonString = JsonConvert.SerializeObject(poseData, Formatting.Indented);
        File.WriteAllText(pathName, jsonString);
    }

    private void ConnectJoints(Dictionary<string, GameObject> jointObjects, string jointNameA, string jointNameB, string humanName)
    {
        if (jointObjects.ContainsKey(jointNameA) && jointObjects.ContainsKey(jointNameB))
        {
            GameObject jointA = jointObjects[jointNameA];
            GameObject jointB = jointObjects[jointNameB];

            // Check if jointA already has a LineRenderer component
            LineRenderer lineRenderer = jointA.GetComponent<LineRenderer>();
            if (lineRenderer == null)
            {
                // Create new LineRenderer component and set material and color
                lineRenderer = jointA.AddComponent<LineRenderer>();
                lineRenderer.material = lineMaterial;
                lineRenderer.startColor = Color.white;
                lineRenderer.endColor = Color.white;
                lineRenderer.startWidth = 0.01f;
                lineRenderer.endWidth = 0.01f;
                lineRenderer.positionCount = 2;

            }

            // Set LineRenderer positions to the positions of the two joints
            lineRenderer.SetPosition(0, jointA.transform.position);
            lineRenderer.SetPosition(1, jointB.transform.position);
        }
    }

    private void ConnectTwoJoints(Dictionary<string, GameObject> jointObjects, string jointNameA, string jointNameB,  string jointNameC,  string humanName)
    {
        if (jointObjects.ContainsKey(jointNameA) && jointObjects.ContainsKey(jointNameB) && jointObjects.ContainsKey(jointNameC))
        {
            GameObject jointA = jointObjects[jointNameA];
            GameObject jointB = jointObjects[jointNameB];
            GameObject jointC = jointObjects[jointNameC];

            // Check if jointA already has a LineRenderer component
            LineRenderer lineRenderer = jointA.GetComponent<LineRenderer>();
            if (lineRenderer == null)
            {
                // Create new LineRenderer component and set material and color
                lineRenderer = jointA.AddComponent<LineRenderer>();
                lineRenderer.material = lineMaterial;
                lineRenderer.startColor = Color.white;
                lineRenderer.endColor = Color.white;
                lineRenderer.startWidth = 0.01f;
                lineRenderer.endWidth = 0.01f;
                lineRenderer.positionCount = 4;
            }

            // Set LineRenderer positions to the positions of the two joints
            lineRenderer.SetPosition(0, jointA.transform.position);
            lineRenderer.SetPosition(1, jointB.transform.position);
            lineRenderer.SetPosition(2, jointA.transform.position);
            lineRenderer.SetPosition(3, jointC.transform.position);

        }
    }

    public class PoseData
    {
        public string name;
        public int timestamp;
        public int index;
        public List<Label> labels;
    }

    public class Label
    {
        public string humanName;
        public string jointName;
        public Point3D point3d;
        public bool unsure;
        public int track_idx;
        public bool is_interpolated;
    }

    public class Point3D
    {
        public Location location;
    }

    public class Location
    {
        public double x;
        public double y;
        public double z;
    }
}
