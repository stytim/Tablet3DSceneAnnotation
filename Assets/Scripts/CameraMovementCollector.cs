using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;
using Michsky.MUIP;

public class CameraMovementCollector : MonoBehaviour
{
    public float dataCollectionInterval = 0.1f; // Data collection interval in seconds
    [SerializeField] private NotificationManager notificationManager;

    private bool isCollecting;
    private Transform cameraTransform;
    private Vector3 lastPosition;
    private Quaternion lastRotation;
    private DateTime lastCollectionTime;
    private List<CameraMovementData> movementDataList;
    private string taskname = "";

    private void Start()
    {
        // Get a reference to the main camera's transform
        cameraTransform = transform;
        movementDataList = new List<CameraMovementData>();
    }

    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.C))
        //{
        //    StartCollecting();
        //}

        //if (Input.GetKeyDown(KeyCode.S))
        //{
        //    StopCollecting();
        //}

        if (isCollecting && (DateTime.Now - lastCollectionTime).TotalSeconds >= dataCollectionInterval)
        {
            // Collect camera movement data
            Vector3 currentPosition = cameraTransform.position;
            Vector3 movement = currentPosition - lastPosition;
            Quaternion currentRotation = cameraTransform.rotation;
            Quaternion rotationDelta = Quaternion.Inverse(lastRotation) * currentRotation;

            // Add the movement data to the list with the current timestamp
            movementDataList.Add(new CameraMovementData(currentPosition, movement, currentRotation, rotationDelta, DateTime.Now));

            // Update last position, rotation, and collection time
            lastPosition = currentPosition;
            lastRotation = currentRotation;
            lastCollectionTime = DateTime.Now;
        }
    }

    public void StartCollecting(string task)
    {
        // Start collecting camera movement data
        isCollecting = true;
        lastPosition = cameraTransform.position;
        lastRotation = cameraTransform.rotation;
        lastCollectionTime = DateTime.Now;
        movementDataList.Clear();
        taskname = task;
    }

    public void StopCollecting()
    {
        // Stop collecting camera movement data
        isCollecting = false;

        // Create a timestamp for the JSON file name
        string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");

        // Serialize the movement data to JSON
        string jsonData = JsonUtility.ToJson(new CameraMovementDataList(movementDataList), true);

        // Save the JSON data to a file
        string filePath = Path.Combine(Application.persistentDataPath, timestamp + "_" + taskname + "_movement.json");
        File.WriteAllText(filePath, jsonData);

        notificationManager.description = "Data Saved successfully!";
        notificationManager.UpdateUI();
        notificationManager.OpenNotification();
    }
}

[System.Serializable]
public class CameraMovementData
{
    public Vector3 position;
    public Vector3 movement;
    public Quaternion rotation;
    public Quaternion rotationDelta;
    public DateTime timestamp;

    public CameraMovementData(Vector3 position, Vector3 movement, Quaternion rotation, Quaternion rotationDelta, DateTime timestamp)
    {
        this.position = position;
        this.movement = movement;
        this.rotation = rotation;
        this.rotationDelta = rotationDelta;
        this.timestamp = timestamp;
    }
}

[System.Serializable]
public class CameraMovementDataList
{
    public List<CameraMovementData> movementDataList;

    public CameraMovementDataList(List<CameraMovementData> movementDataList)
    {
        this.movementDataList = movementDataList;
    }
}
