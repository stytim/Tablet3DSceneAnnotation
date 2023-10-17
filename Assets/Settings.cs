using UnityEngine;
using System.IO;
using UnityEngine.AI;

public class Settings : MonoBehaviour
{
    public static Settings Instance;

    public GLTFPlayer defaultPlayer;
    private GLTFPlayer activePlayer;

    GameObject PoseManager;
    GameObject BoxManager;
    GameObject ConnectionManagerObj;

    // Define the settings variables
    public int maxLoad = 10;
    public float frameRate = 5f;

    private string fileName = "userSettings.json";

    private bool istrackingEanble = true;

    private class UserSettings
    {
        public int maxLoad;
        public float frameRate;

        public UserSettings(int maxLoad, float frameRate)
        {
            this.maxLoad = maxLoad;
            this.frameRate = frameRate;
        }
    }

    // Start is called before the first frame update
    private void Awake()
    {
        Instance = this;
        if (File.Exists(Application.persistentDataPath + "/" + fileName))
        {
            string jsonData = File.ReadAllText(Application.persistentDataPath + "/" + fileName);
            UserSettings savedSettings = JsonUtility.FromJson<UserSettings>(jsonData);
            maxLoad = savedSettings.maxLoad;
            frameRate = savedSettings.frameRate;
        }
    }
    void Start()
    {
        activePlayer = defaultPlayer;
        PoseManager = GameObject.Find("/HumanPoseAdjustment");
        BoxManager = GameObject.Find("/BoundingBoxManager");
        ConnectionManagerObj = GameObject.Find("/ConnectionManager");
    }

    void OnApplicationQuit()
    {
        // Save the user settings to file on application quit
        UserSettings userSettings = new UserSettings(maxLoad, frameRate);
        string jsonData = JsonUtility.ToJson(userSettings);
        File.WriteAllText(Application.persistentDataPath + "/" + fileName, jsonData);
    }

    public void LoadNextBatch()
    {
        activePlayer.loadNextBatch();
    }

    public GLTFPlayer RetriveCurrentPlayer()
    {
        return activePlayer;
    }

    public void PlayFrames()
    {
        activePlayer.PlayFrames();
    }

    public void PausePlay()
    {
        activePlayer.PauseReplay();
    }

    public void ReplayFromBeginning()
    {
        activePlayer.Replay();
        DeleteAllMetaData();

    }

    public void AddRelation(string objA, string objB, string relation)
    {
        if (activePlayer.TryGetComponent<RelationJSONReader>(out RelationJSONReader reader))
            reader.AddRelationAnnotation(objA, objB, relation);
    }

    private void DeleteAllMetaData()
    {
        ConnectionManager.Instance?.DeleteAllLocalConnection();

        if (activePlayer.TryGetComponent<ObjectIndicator>(out ObjectIndicator indicator))
            indicator.DeleteAllIcons();

        if (activePlayer.TryGetComponent<DatasetReceiver>(out DatasetReceiver receiver))
            receiver.DeleteAllBox();

        if (activePlayer.TryGetComponent<RelationJSONReader>(out RelationJSONReader reader))
            reader.DeleteAllRelation();
    }

    public void DeleteLoadedScenes()
    {
        activePlayer.DeleteAllLoadedScene();

        DeleteAllMetaData();

    }

    public void SwitchDataset(GLTFPlayer player)
    {
        DeleteLoadedScenes();
        activePlayer.enabled = false;


        player.enabled = true;
        activePlayer = player;
        //ReplayFromBeginning();
        Debug.Log("Switch Dataset");
    }

    public void EnableTouch()
    {
        MeshCollider meshCollider = activePlayer.gameObject.GetComponent<MeshCollider>();

        if (meshCollider == null)
        {
            // If MeshCollider component doesn't exist, add one
            meshCollider = activePlayer.gameObject.AddComponent<MeshCollider>();
        }

        // Set the sharedMesh property
        meshCollider.sharedMesh = GetCurrentScene().GetComponent<MeshFilter>().sharedMesh;

        if (activePlayer.TryGetComponent<DigitalRubyShared.FingersPanRotateScaleComponentScript>(out DigitalRubyShared.FingersPanRotateScaleComponentScript touchScript))
            touchScript.enabled = true;
    }

    public void DisableTouch()
    {
        Destroy(activePlayer.gameObject.GetComponent<MeshCollider>());
        if (activePlayer.TryGetComponent<DigitalRubyShared.FingersPanRotateScaleComponentScript>(out DigitalRubyShared.FingersPanRotateScaleComponentScript touchScript))
            touchScript.enabled = false;
    }


    public void SaveMaxLoad(int max)
    {
        maxLoad = max;
    }

    public int LoadMaxLoad()
    {
        return maxLoad;
    }

    public float LoadPlaySpeed()
    {
        return frameRate;
    }

    public GameObject GetCurrentScene()
    {
       return activePlayer.GetCurrentScene();
    }

    public string CurrentFrameID()
    {
        return activePlayer.CurrentIndex.ToString();
    }
    public void TrackingEnable(bool flag)
    {

        //PoseManager.GetComponent<JointAdjustmentAR>().enabled = flag;
        //PoseManager.GetComponent<JointAdjustmentTouch>().enabled = !flag;

        BoxManager.GetComponent<VertexClusterBoundingBoxes>().enabled = flag;
        BoxManager.GetComponent<DefineBox>().enabled = !flag;

        //BoxManager.GetComponent<DrawRect>().enabled = !flag;

        BoxManager.GetComponent<LineRenderer>().enabled = !flag;

        ConnectionManagerObj.GetComponent<EstablishConnection>().EnableTouchAnnotation(!flag);

        istrackingEanble = flag;

    }

    public bool TrackingStatus()
    {
        return istrackingEanble;
    }

}
