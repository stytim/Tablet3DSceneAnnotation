using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Collections.Generic;

public class JsonWriter : MonoBehaviour
{
    public static JsonWriter Instance;
    //private string _fileName = "user_annotated_data.json";
    private string _filePath;

    private void Awake()
    {
        Instance = this;
    }

    //void Start()
    //{
    //    string time = System.DateTime.Now.ToString("yyyy_MM_dd__HH_mm_ss");
    //    _filePath = Path.Combine(Application.persistentDataPath, time);
    //    Directory.CreateDirectory(_filePath);
    //    _filePath = Path.Combine(_filePath, _fileName);
    //}


    public void InitilizeRelationWriter(string path)
    {
        _filePath = path;
    }

    private JObject LoadDataFromFile()
    {
        if (File.Exists(_filePath))
        {
            string json = File.ReadAllText(_filePath);
            return JObject.Parse(json);
        }
        else
        {
            return null;
        }
    }

    public void AddNewRelation(string frame_id, List<string> relationship)
    {

        // Load the existing data from the file, or create a new JObject if the file doesn't exist
        JObject data = LoadDataFromFile() ?? new JObject();
        List<List<string>> relationships = new List<List<string>>();

        if (data.ContainsKey(frame_id))
        {
            relationships = data[frame_id].ToObject<List<List<string>>>();
            relationships.Add(relationship);
            data[frame_id] = JToken.FromObject(relationships);
        }
        else
        {
            relationships.Add(relationship);
            data.Add(frame_id, JToken.FromObject(relationships));
        }

        // Write the updated data back to the file
        WriteDataToFile(data);
    }

    public void DeleteLastRelation(string frame_id)
    {
        JObject data = LoadDataFromFile();
        if (data.ContainsKey(frame_id))
        {
            var relationships = data[frame_id].ToObject<List<List<string>>>();
            relationships.RemoveAt(relationships.Count - 1);
        }
        // Write the updated data back to the file
        WriteDataToFile(data);
    }

    private void WriteDataToFile(JObject data)
    {
        string json = data.ToString();
        File.WriteAllText(_filePath, json);
    }
}