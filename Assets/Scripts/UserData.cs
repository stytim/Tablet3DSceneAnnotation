using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
public class UserData : MonoBehaviour
{

    public static UserData Instance;

    public DatasetReceiver datasetReceiver;

    string bbox_file = "user_bbox.txt";
    string pose_file = "user_pose.json";
    string relation_file = "user_relation.json";

    string previous_frame = "-9";

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        string time = System.DateTime.Now.ToString("yyyy_MM_dd__HH_mm_ss");
        string fileName = Path.Combine(Application.persistentDataPath, time);
        Directory.CreateDirectory(fileName);
        JsonWriter.Instance?.InitilizeRelationWriter(Path.Combine(fileName, relation_file));

        bbox_file = Path.Combine(fileName, bbox_file);
        pose_file = fileName;

    }

    public string GetPoseSavingPath()
    {
        return pose_file;
    }


    public void SaveBoundingBoxesWithTag(string tag, string frame_id)
    {
        bool fileExists = File.Exists(bbox_file);
        using (StreamWriter writer = new StreamWriter(bbox_file, true))
        {
            if (fileExists)
            {
                writer.WriteLine(); // Start a new line before writing the data
            }
            previous_frame = frame_id;
            bool isFirstBBox = true;
            foreach (var kvp in datasetReceiver.objects)
            {
                GameObject bbox = kvp.Value;
                if (bbox.tag == tag)
                {
                    if (isFirstBBox)
                    {
                        writer.Write($"{frame_id} ");
                    }

                    string line = $"{bbox.name} {bbox.tag} {bbox.transform.position.x} {bbox.transform.position.y} {bbox.transform.position.z} " +
                                  $"{bbox.transform.localScale.x} {bbox.transform.localScale.y} {bbox.transform.localScale.z} " +
                                  $"{bbox.transform.rotation.eulerAngles.y}";

                    if (!isFirstBBox)
                    {
                        writer.Write(",");
                    }

                    writer.Write(line);

                    isFirstBBox = false;
                }
            }
        }
    }

    public void SaveAdjustedPose()
    {

    }

}
