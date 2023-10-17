using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Globalization;
using TMPro;
using System.IO;

public class DatasetReceiver : MonoBehaviour
{

    private RelationJSONReader relationJSONReader;

    private string[] bboxData;
    private bool displayBBox = false;
    public enum DataSource // *
    {
        Local,
        Remote,
    }
    public DataSource dataSource;

    public Dictionary<string, GameObject> objects = new Dictionary<string, GameObject>();

    public int index = 4;


    // Start is called before the first frame update
    void Start()
    {
        if (dataSource == DataSource.Local)
            ReadLocalBBoxFile();
        relationJSONReader = GetComponent<RelationJSONReader>();
    }

    // Update is called once per frame
    void Update()
    {


    }

    void ReadLocalBBoxFile()
    {
        string dataPath = Path.Combine(Application.persistentDataPath, "bbox.txt");
        bboxData = File.ReadAllLines(dataPath);
    }

    public void NextFrame()
    {
        index += 1;
        CreateBBox(bboxData[index]);
    }

    public void PreviousFrame()
    {
        index -= 1;
        CreateBBox(bboxData[index]);
    }

    public void PlayFrame(int frameID)
    {
        //Debug.Log(frameID + 4);
        CreateBBox(bboxData[frameID + 4]);
        relationJSONReader.VisualizeSceneGraph(frameID + 5);
        relationJSONReader.Visualize3DSceneGraph(frameID + 5);
    }

    public void DeleteAllBox()
    {
        foreach (var item in objects)
        {
            Destroy(item.Value.GetComponentInChildren<MeshRenderer>().material.mainTexture);
            Destroy(item.Value.GetComponentInChildren<MeshRenderer>());
            Destroy(item.Value);
        }
        objects.Clear();
    }
    void CreateBBox(string data)
    {
        var instance = data.Split(',');
        foreach (var obj in instance)
        {
            var split = obj.Split(' ');
            if (split.Length >= 6)
            {
                Vector3 position = new Vector3(float.Parse(split[1], NumberStyles.Float, CultureInfo.InvariantCulture),
                       float.Parse(split[2], NumberStyles.Float, CultureInfo.InvariantCulture),
                       - float.Parse(split[3], NumberStyles.Float, CultureInfo.InvariantCulture)) * 0.002f ;

                Vector3 scale = new Vector3(float.Parse(split[4], NumberStyles.Float, CultureInfo.InvariantCulture),
                    float.Parse(split[5], NumberStyles.Float, CultureInfo.InvariantCulture),
                    float.Parse(split[6], NumberStyles.Float, CultureInfo.InvariantCulture)) * 0.0015f;
                float y_deg = float.Parse(split[7], NumberStyles.Float, CultureInfo.InvariantCulture);
                GameObject bbox = null;
                if (!objects.ContainsKey(split[0]))
                {
                    LabelType _label = (LabelType)int.Parse(split[0]);
                    bbox = new GameObject(Configurations.NameMapping[_label.ToString()]);
                    bbox.tag = split[0];
                    objects.Add(split[0], bbox);
                    bbox.transform.parent = transform;
                    var vis = bbox.AddComponent<BoundingBoxVisualizer>();
                    vis.MaterialColor = Configurations.AssignColor(_label);
                    vis.visualize = !displayBBox;
                    //Debug.Log("Succesfully received box position and scale: " + position + " " + scale);
                }
                else
                {
                    bbox = objects[split[0]];
                }

                bbox.transform.position = transform.TransformPoint(position);
                bbox.transform.localScale = scale;
                bbox.transform.rotation = Quaternion.Euler(0, y_deg + transform.rotation.eulerAngles.y, 0);
            }
            else
            {
                Debug.Log("Error parsing box position ");
            }
        }
       
    }

    public void DisplayBBox(bool flag)
    {
        foreach (var item in objects)
        {
            item.Value.GetComponentInChildren<MeshRenderer>().enabled = flag;
            item.Value.GetComponentInChildren<BoxCollider>().enabled = flag;
        }
        displayBBox = flag;
    }


   
}
