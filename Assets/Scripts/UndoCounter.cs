using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using Michsky.MUIP;

[Serializable]
public class UndoData
{
    public int undoCount;
}


public class UndoCounter : MonoBehaviour
{
    private string actionName;
    private int undoCount = 0;
    private ButtonManager undoButton;

    private void Start()
    {
        actionName = gameObject.name;
        undoButton = GetComponent<ButtonManager>();
        undoButton.onClick.AddListener(IncrementUndoCount);
    }

    private void IncrementUndoCount()
    {
        undoCount++;
    }

    public void QuitMode()
    {
        SaveUndoCountToJson();
    }

    private void SaveUndoCountToJson()
    {
        string timeStamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        string fileName = timeStamp + "_undo_count_" + actionName + ".json";

        string filePath = Path.Combine(Application.persistentDataPath, fileName);

        UndoData undoData = new UndoData
        {
            undoCount = undoCount
        };

        string jsonData = JsonUtility.ToJson(undoData);
        Debug.Log(jsonData);

        File.WriteAllText(filePath, jsonData);

        undoCount = 0;

        Debug.Log("Undo count saved to: " + filePath);
    }
}
