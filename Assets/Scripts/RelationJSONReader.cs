using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;
using UnityEngine.UI;
using TMPro;
using System;

public class RelationJSONReader : MonoBehaviour
{
    public string sceneGraphsPath = "json/scan_relations.json";
    bool visualizeCloseTo = false;
    Dictionary<int, List<(string, string, string)>> rels = new Dictionary<int, List<(string, string, string)>>();
    Dictionary<int, List<(string, string, string)>> current_user_rels = new Dictionary<int, List<(string, string, string)>>();
    Dictionary<int, List<(string, string, string)>> previous_user_rels = new Dictionary<int, List<(string, string, string)>>();

    public int currentID = 0;
    public GameObject node, relArrow, relationObj;
    public GameObject undoBttn;
    private List<GameObject> allNodes = new List<GameObject>();
    private ObjectIndicator.LabelIcon[] allTex;

    private string takeIdx;
    private bool display_user_data = false;

    public EstablishConnection connection;

    private void Start()
    {
        sceneGraphsPath = Path.Combine(Application.persistentDataPath, sceneGraphsPath);
        allTex = FindObjectOfType<ObjectIndicator>().allTex;

        rels = ReadJSON(sceneGraphsPath);

        LoadLatestUserJSON();
    }

    private void Update()
    {
        if (connection.enableAnnotation)
        {
            if (current_user_rels.ContainsKey(currentID) && current_user_rels[currentID].Count != 0)
            {
                undoBttn.SetActive(true);
            }
            else
            {
                undoBttn.SetActive(false);
            }
        }
    }

    private void LoadLatestUserJSON()
    {
        DirectoryInfo dir = new DirectoryInfo(Application.persistentDataPath);
        FileInfo[] files = dir.GetFiles("user_annotated_data.json", SearchOption.AllDirectories);
        Array.Sort(files, (a, b) => b.CreationTime.CompareTo(a.CreationTime));
        if (files.Length > 0)
        {
            previous_user_rels = ReadJSON(files[0].FullName);
            Debug.Log("Loaded user data");
        }
    }

    private Dictionary<int, List<(string, string, string)>> ReadJSON(string dataPath)
    {
        Dictionary<int, List<(string, string, string)>> relation_dict = new Dictionary<int, List<(string, string, string)>>();

        try
        {
            string json = File.ReadAllText(dataPath);
            JObject scans = JsonConvert.DeserializeObject<JObject>(json);

            foreach (KeyValuePair<string, JToken> scan in scans)
            {
                string[] scanId = scan.Key.Split('_');
                takeIdx = scanId[0];
                string frameNumber = scanId[1];

                JArray relations = (JArray)scan.Value;
                int frameID = int.Parse(frameNumber, System.Globalization.NumberStyles.Integer);
                if (relations.Count == 0) continue;
                var tmp = new List<(string, string, string)>();
                foreach (JArray relation in relations)
                {
                    string sub = relation[0].ToString();
                    string rel = relation[1].ToString();
                    string obj = relation[2].ToString();
                    if (Configurations.NameMapping.ContainsKey(sub))
                    {
                        sub = Configurations.NameMapping[sub];
                    }
                    if (Configurations.NameMapping.ContainsKey(obj))
                    {
                        obj = Configurations.NameMapping[obj];
                    }
                    if (!visualizeCloseTo && rel == "CloseTo") continue;
                    if (sub == "object" || obj == "object") continue;

                    tmp.Add((sub, obj, rel));
                }
                relation_dict.Add(frameID, tmp);
            }
        }
        catch (Exception ex)
        {
            // Handle the exception here, e.g. log it or show an error message
            Console.WriteLine(ex.ToString());
            relation_dict = null;
        }

        return relation_dict;
    }


    public void LoadUserAnnotatedData(bool flag)
    {
        display_user_data = flag;
    }

    public void VisualizeSceneGraph(int frameID)
    {
        if (rels.ContainsKey(frameID) || previous_user_rels.ContainsKey(frameID))
        {
            foreach (var _node in allNodes)
            {
                Destroy(_node);
            }
            allNodes.Clear();
        }
        if (display_user_data && previous_user_rels.ContainsKey(frameID))
        {
            VisualizeRelations(previous_user_rels[frameID]);
        }
        if (rels.ContainsKey(frameID))
        {
            VisualizeRelations(rels[frameID]);
           
        }
        if (current_user_rels.ContainsKey(frameID))
        {
            VisualizeRelations(current_user_rels[frameID]);
        }
        currentID = frameID;

    }

    private void VisualizeRelations(List<(string, string, string)> relations)
    {
        foreach (var relation in relations)
        {

            GameObject _newRelObj = Instantiate(relationObj, relationObj.transform.parent);
            allNodes.Add(_newRelObj);
            AssignIcon(relation.Item1, _newRelObj.transform);
            AssignRelation(relation.Item3, _newRelObj.transform);
            AssignIcon(relation.Item2, _newRelObj.transform);
        }
    }

    public void AddRelationAnnotation(string objA, string objB, string relation)
    {
        string labelA = "";
        string labelB = "";
        if (Configurations.LabelMapping.ContainsKey(objA))
        {
            labelA = Configurations.reverseMapping[objA];
        }
        else
        {
            labelA = objA;
        }
        if (Configurations.LabelMapping.ContainsKey(objB))
        {
            labelB = Configurations.reverseMapping[objB];
        }
        else
        {
            labelB = objB;
        }

        GameObject _newRelObj = Instantiate(relationObj, relationObj.transform.parent);
        allNodes.Add(_newRelObj);
        AssignIcon(objA, _newRelObj.transform);
        AssignRelation(relation, _newRelObj.transform);
        AssignIcon(objB, _newRelObj.transform);

        // Add relationships to the data
        if (!current_user_rels.ContainsKey(currentID))
            current_user_rels.Add(currentID, new List<(string, string, string)> { (objA, objB, relation) });
        else
            current_user_rels[currentID].Add((objA, objB, relation));

        List<string> relationship = new List<string>() { labelA, relation, labelB };
        string scanID = takeIdx + "_" + currentID.ToString("D6") + "_0";
        JsonWriter.Instance?.AddNewRelation(scanID,relationship);

    }

    public void DeleteLastRelation()
    {
        if (current_user_rels.ContainsKey(currentID))
        {
            current_user_rels[currentID].RemoveAt(current_user_rels[currentID].Count - 1);
            string scanID = takeIdx + "_" + currentID.ToString("D6") + "_0";
            JsonWriter.Instance?.DeleteLastRelation(scanID);

            Destroy(allNodes.Last());
            allNodes.RemoveAt(allNodes.Count - 1);

        }
    }

    public void Visualize3DSceneGraph(int frameID)
    {
        if (rels.ContainsKey(frameID) || previous_user_rels.ContainsKey(frameID))
        {
            ConnectionManager.Instance?.DeleteAllLocalConnection();
        }
        if (display_user_data && previous_user_rels.ContainsKey(frameID))
        {
            Visualize3DRelations(previous_user_rels[frameID]);
        }
        if (rels.ContainsKey(frameID))
        {
            Visualize3DRelations(rels[frameID]);
            
        }
        if (current_user_rels.ContainsKey(frameID))
        {
            Visualize3DRelations(current_user_rels[frameID]);
        }


    }
    private void Visualize3DRelations(List<(string, string, string)> relations)
    {
        foreach (var relation in relations)
        {

            GameObject objA = GameObject.Find(relation.Item1);
            GameObject objB = GameObject.Find(relation.Item2);
            if (objA != null && objB != null)
            {
                ConnectionManager.Instance?.AddConnection(ConnectionType.Line, objA.transform, objA.transform.position, isSticky: false);
                ConnectionManager.Instance?.AddConnection(ConnectionType.Line, objB.transform, objB.transform.position, isSticky: false);
            }

        }
    }

    private void AssignRelation(string name, Transform root)
    {
        GameObject _newRel = Instantiate(relArrow, root);
        _newRel.GetComponentInChildren<TextMeshProUGUI>().text = name;
        _newRel.SetActive(true);
        //allNodes.Add(_newRel);
    }

    public void DeleteAllRelation()
    {
        foreach (var _node in allNodes)
        {
            Destroy(_node);
        }
        allNodes.Clear();
    }

    private GameObject AssignIcon(string item, Transform root)
    {
        GameObject _newNode = Instantiate(node, root);
        _newNode.GetComponentInChildren<TextMeshProUGUI>().text = item;
        if (Configurations.LabelMapping.TryGetValue(item, out LabelType labelType))
        {
            switch ((int)labelType)
            {
                case (int)LabelType.anesthesia_equipment:
                    _newNode.GetComponent<Image>().sprite = allTex[0].icon;
                    _newNode.GetComponent<Image>().color = Configurations.AssignColor(LabelType.anesthesia_equipment);
                    break;
                case (int)LabelType.operating_table:
                    _newNode.GetComponent<Image>().sprite = allTex[1].icon;
                    _newNode.GetComponent<Image>().color = Configurations.AssignColor(LabelType.operating_table);
                    break;
                case (int)LabelType.instrument_table:
                    _newNode.GetComponent<Image>().sprite = allTex[2].icon;
                    _newNode.GetComponent<Image>().color = Configurations.AssignColor(LabelType.instrument_table);
                    break;
                case (int)LabelType.secondary_table:
                    _newNode.GetComponent<Image>().sprite = allTex[3].icon;
                    _newNode.GetComponent<Image>().color = Configurations.AssignColor(LabelType.secondary_table);
                    break;
                case (int)LabelType.otherobject:
                    _newNode.GetComponent<Image>().sprite = allTex[10].icon;
                    _newNode.GetComponent<Image>().color = Configurations.AssignColor(LabelType.otherobject);
                    break;
                case (int)LabelType.Patient:
                    _newNode.GetComponent<Image>().sprite = allTex[4].icon;
                    _newNode.GetComponent<Image>().color = Configurations.AssignColor(LabelType.Patient);
                    break;
                case (int)LabelType.human_0:
                    _newNode.GetComponent<Image>().sprite = allTex[4].icon;
                    _newNode.GetComponent<Image>().color = Configurations.AssignColor(LabelType.human_0);
                    break;
                case (int)LabelType.human_1:
                    _newNode.GetComponent<Image>().sprite = allTex[4].icon;
                    _newNode.GetComponent<Image>().color = Configurations.AssignColor(LabelType.human_1);
                    break;
                case (int)LabelType.human_2:
                    _newNode.GetComponent<Image>().sprite = allTex[4].icon;
                    _newNode.GetComponent<Image>().color = Configurations.AssignColor(LabelType.human_2);
                    break;
                case (int)LabelType.human_3:
                    _newNode.GetComponent<Image>().sprite = allTex[4].icon;
                    _newNode.GetComponent<Image>().color = Configurations.AssignColor(LabelType.human_3);
                    break;
                case (int)LabelType.human_4:
                    _newNode.GetComponent<Image>().sprite = allTex[4].icon;
                    _newNode.GetComponent<Image>().color = Configurations.AssignColor(LabelType.human_4);
                    break;
                case (int)LabelType.human_5:
                    _newNode.GetComponent<Image>().sprite = allTex[4].icon;
                    _newNode.GetComponent<Image>().color = Configurations.AssignColor(LabelType.human_5);
                    break;
                default:
                    break;
            }
        }
        else
        {
            _newNode.GetComponent<Image>().sprite = allTex[10].icon;
            _newNode.GetComponent<Image>().color = Configurations.AssignColor(LabelType.otherobject);
        }
        _newNode.SetActive(true);
        //allNodes.Add(_newNode);
        return _newNode;
    }
}
