using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UIElements;
using TMPro;

public class DefineBox : MonoBehaviour
{
    private LineRenderer linebox;
    public DatasetReceiver datasetReceiver;

    private Vector3[] pointsSet1; // Array to store the first set of 4 points in Vector3 format
    private Vector3[] pointsSet2; // Array to store the second set of 4 points in Vector3 format

    private Vector3[] vertices;
    private string labelName;

    private void Start()
    {
        pointsSet1 = new Vector3[4];
        pointsSet2 = new Vector3[4];
        vertices = new Vector3[8];
        linebox = GetComponent<LineRenderer>();
    }

    // Function to collect points for the first set
    public void CollectPointsSet1()
    {
        linebox.GetPositions(pointsSet1);
            
    }

    // Function to collect points for the second set
    public void CollectPointsSet2()
    {
        linebox.GetPositions(pointsSet2);
        linebox.positionCount = 0;
    }

    public void LabelName(TMP_InputField text)
    {
        labelName = text.text;
        Debug.Log(labelName);
        GenerateBox();

        string frame_id = Settings.Instance?.CurrentFrameID();
        UserData.Instance?.SaveBoundingBoxesWithTag("5", frame_id);
    }

    public void GenerateBox()
    {
        if (pointsSet1 == null || pointsSet1.Length != 4 || pointsSet2 == null || pointsSet2.Length != 4)
        {
            Debug.LogError("Both sets of points must be collected before generating the cube!");
            return;
        }

        for (int i = 0; i < 4; i++)
        {
            vertices[i] = new Vector3(pointsSet1[i].x, pointsSet2[0].y, pointsSet1[i].z);
            Debug.Log(vertices[i]);
        }

        for (int i = 4; i < 8; i++)
        {
            vertices[i] = new Vector3(pointsSet1[i-4].x, pointsSet2[1].y, pointsSet1[i-4].z);
            Debug.Log(vertices[i]);
        }

        Vector3 min = vertices[0];
        Vector3 max = vertices[0];

        // Find the min and max values of the vertices
        foreach (Vector3 vertex in vertices)
        {
            min = Vector3.Min(min, vertex);
            max = Vector3.Max(max, vertex);
        }

        // Calculate the center and size of the cube
        Vector3 center = (min + max) / 2f;
        Vector3 size = max - min;

        Debug.Log(center);


        LabelType _label = LabelType.otherobject;
        GameObject bbox = new GameObject(labelName);
        bbox.tag = "5";
        //bbox.transform.parent = transform;
        var vis = bbox.AddComponent<BoundingBoxVisualizer>();
        vis.MaterialColor = Configurations.AssignColor(_label);

        bbox.transform.position = center;
        bbox.transform.localScale = size;
        bbox.transform.parent = Settings.Instance?.RetriveCurrentPlayer().transform;

        datasetReceiver.objects.Add(labelName, bbox);

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