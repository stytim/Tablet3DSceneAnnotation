using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

[Serializable]
public class TimeRecord
{
    public string TaskName;
    public string StartTime;
    public string EndTime;
    public string TimeInterval;
}

public class UserStudySetup : MonoBehaviour
{
    public GLTFPlayer player;
    public PoseVisualizer poseVisualizer;
    public DatasetReceiver datasetReceiver;




    private DateTime startTime;
    private DateTime endTime;
    private string taskName = "annotate";

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.LeftAlt) && Input.GetKeyDown(KeyCode.Q))
            Task1A();
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.LeftAlt) && Input.GetKeyDown(KeyCode.W))
            Task1B();
 
        if (Input.GetKeyDown(KeyCode.F1))
            Task1C();
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.LeftAlt) && Input.GetKeyDown(KeyCode.R))
            Task1D();
 
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.LeftAlt) && Input.GetKeyDown(KeyCode.Alpha5))
            Task2A();
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.LeftAlt) && Input.GetKeyDown(KeyCode.Alpha6))
            Task2B();


        if (Input.GetKeyDown(KeyCode.F2))
            Task2C();
        //if (Input.GetKeyDown(KeyCode.F2))
        //    Task2D();


        if (Input.GetKeyDown(KeyCode.F3))
            Task3A();
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.LeftAlt) && Input.GetKeyDown(KeyCode.Alpha0))
            Task3B();

        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.LeftAlt) && Input.GetKeyDown(KeyCode.Minus))
            Task3C();
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.LeftAlt) && Input.GetKeyDown(KeyCode.Equals))
            Task3D();
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ScreenCapture.CaptureScreenshot(taskName+".png");
        }

    }


    // Box
    public void Task1A()
    {
        player.LoadFrameByNumber(3);
    }

    public void Task1B()
    {
        player.LoadFrameByNumber(37);
    }

    public void Task1C()
    {
        player.LoadFrameByNumber(151);
    }

    public void Task1D()
    {
        player.LoadFrameByNumber(162);
    }


    //Pose
    public void Task2A()
    {
        player.LoadFrameByNumber(7);
        poseVisualizer.LoadPoses(3);

    }

    public void Task2B()
    {
        player.LoadFrameByNumber(71);
        poseVisualizer.LoadPoses(70/2);

    }

    public void Task2C()
    {
        player.LoadFrameByNumber(131);
        poseVisualizer.LoadPoses(130/2);

    }

    public void Task2D()
    {
        player.LoadFrameByNumber(145);
        poseVisualizer.LoadPoses(144 / 2);

    }


    // relation
    public void Task3A()
    {
        player.LoadFrameByNumber(147);

        datasetReceiver.PlayFrame(146/2);
    }

    public void Task3B()
    {
        player.LoadFrameByNumber(165);

        datasetReceiver.PlayFrame(164/2);
    }



    public void Task3C()
    {
        player.LoadFrameByNumberInterval(245);
        //poseVisualizer.LoadPoses(1);
        datasetReceiver.PlayFrame(244/2);
    }

    public void Task3D()
    {
        player.LoadFrameByNumberInterval(179);
        //poseVisualizer.LoadPoses(178/2);
        datasetReceiver.PlayFrame(178/2);
    }


    public void StartRecording(string name)
    {
        taskName = name;
        startTime = DateTime.Now;
    }


    public void StopRecording()
    {
        endTime = DateTime.Now;
        TimeSpan timeInterval = endTime - startTime;

        TimeRecord record = new TimeRecord
        {
            TaskName = taskName,
            StartTime = startTime.ToString(),
            EndTime = endTime.ToString(),
            TimeInterval = timeInterval.ToString()
        };

        // Convert the dictionary to JSON
        string jsonData = JsonUtility.ToJson(record, true);
        Debug.Log(taskName);
        Debug.Log(timeInterval.ToString());

        Debug.Log(jsonData);

        // Save the JSON data to a file

        string time = System.DateTime.Now.ToString("yyyy_MM_dd__HH_mm_ss");

        string filePath = Path.Combine(Application.persistentDataPath, time + "time.json");
        File.WriteAllText(filePath, jsonData);

        Debug.Log("Recording saved to: " + filePath);
    }


}
