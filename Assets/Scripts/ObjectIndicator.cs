using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectIndicator : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject indicator;
    public float factor = 205f;

    [Serializable]
    public struct LabelIcon
    {
        public LabelType label;
        public Sprite icon;
    }

    public LabelIcon[] allTex;

    private BoxCollider[] objects;
    private List<GameObject> allNodes = new List<GameObject>();
    private Transform user;
    private Camera pov;
    private bool updated = false;


    void Start()
    {
        user = Camera.main.transform;
        pov = Camera.main;
        FindAllObjetcs();
    }

    // Update is called once per frame
    void Update()
    {
        if (updated)
        {
            for (int i = 0; i < objects.Length; i++)
            {
                //Vector3 objectDirectionLocal = user.InverseTransformPoint(objects[i].transform.position);
                if (objects[i] != null)
                {
                    Vector3 screenPoint = pov.WorldToViewportPoint(objects[i].transform.position);

                    if (screenPoint.z < 0)
                    {
                        if (screenPoint.x <= 0.5f)
                        {
                            screenPoint.x = 2f;
                        }
                        else
                        {
                            screenPoint.x = -1f;

                        }
                    }
                    else
                    {
                        if (screenPoint.x < -1f)
                        {
                            screenPoint.x = -1f;
                        }
                        else if (screenPoint.x > 2f)
                        {
                            screenPoint.x = 2f;

                        }
                    }
                    if (objects[i].transform.parent!= null && int.TryParse(objects[i].transform.parent.tag, out int objTagNumber))
                    {
                        allNodes[i].transform.localPosition = new Vector3((screenPoint.x - 0.5f) * factor, indicator.transform.localPosition.y, 0);
                    }
                }

            }
        }
    }

    public void DeleteAllIcons()
    {
        foreach (var node in allNodes)
        {
            Destroy(node);
        }
        allNodes.Clear();
        updated = false;
    }

    public void FindAllObjetcs()
    {
        objects = (BoxCollider[])GameObject.FindObjectsOfType(typeof(BoxCollider));
        foreach (var node in allNodes)
        {
            Destroy(node);
        }
        allNodes.Clear();
        updated = false;
        foreach (var obj in objects)
        {
            int objTagNumber = -1;
            if (obj.transform.parent != null && int.TryParse(obj.transform.parent.tag, out objTagNumber))
            {
                GameObject _newNode = Instantiate(indicator, indicator.transform.parent);
                switch (objTagNumber)
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
                _newNode.SetActive(true);
                allNodes.Add(_newNode);
            }

        }
        updated = true;
    }
}
