using System.Diagnostics;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;
using GLTFast;
using System.Linq;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Michsky.MUIP;
using System;


public class GLTFPlayer : MonoBehaviour
{
    [SerializeField] private SliderManager mySlider;
    //[SerializeField] private ProgressBar myProgress;
    [SerializeField] private NotificationManager notificationManager;
    [SerializeField] private Transform sceneRoot;
    [SerializeField] private DatasetReceiver datasetReceiver;
    [SerializeField] private ObjectIndicator objectIndicator;
    [SerializeField] private PoseVisualizer poseVisualizer;

    public int maxLoad = 10;
    public string dataset = "take05";

    private float playSpeed = 0.3f;

    private int loadedSceneNum = 0;
    private int SceneCount;
    public int CurrentIndex = 0;
    private string dataPath;


    private string[] allFiles;
    private Dictionary<int, GameObject> loadedScenes;

    private IDeferAgent deferAgent;
    private ImportSettings importSettings;
    private InstantiationSettings instantiationSettings;
  
    private int previousVal;


    private void Awake()
    {
        dataPath = Path.Combine(Application.persistentDataPath, "glb", dataset);
        allFiles = Directory.GetFiles(dataPath, "*.glb");
        SceneCount = allFiles.Length;
        mySlider.maxValue = allFiles.Length;
        //Debug.Log(allFiles.Length);
        //Debug.Log(mySlider.maxValue);
        //mySlider.UpdateUI();
        //Debug.Log(mySlider.maxValue);
    }

    private async Task<bool> LoadNext()
    {
        
        // check if file exists
        string meshFile = Path.Combine(dataPath, string.Format("{0:D6}.glb",CurrentIndex));
        Debug.Log(meshFile);
        if (!File.Exists(meshFile))
        {
            Debug.LogErrorFormat("Mesh file does not exist. Filepath {0}", meshFile);
            return false;
        }
        CurrentIndex++;
        // load glb file 
        var gltfImport = new GltfImport();
        if (await gltfImport.Load(meshFile))
        {
            GameObject _Scene = new GameObject("Scene");
            _Scene.transform.parent = transform;
            MeshFilter _meshFilter = _Scene.AddComponent<MeshFilter>();
            MeshRenderer _meshRenderer = _Scene.AddComponent<MeshRenderer>();

            _meshFilter.mesh = gltfImport.GetMeshes()[0];
            _meshRenderer.material = gltfImport.GetMaterial();
            loadedScenes[loadedSceneNum] = _Scene;
            loadedSceneNum += 1;
            return true;
        }
        else
        {
            Debug.LogError("Failed to load gltf file");
            return false;
        }
        
    }


    // Start is called before the first frame update
    async void Start()
    {

        mySlider.mainSlider.onValueChanged.AddListener(ShowCurrentScene);

        gameObject.transform.Translate(0, -1.5f, 0.0f, Space.Self);
        
        
        importSettings = new ImportSettings();
        instantiationSettings = new InstantiationSettings();


        loadedScenes = new Dictionary<int, GameObject>();
        maxLoad = Settings.Instance.LoadMaxLoad();
        playSpeed = 1.0f / Settings.Instance.LoadPlaySpeed();
        var files = allFiles.Take(maxLoad).ToArray();
        var tasks = new List<Task>(files.Length);

        //Stopwatch sw = Stopwatch.StartNew();
        foreach (var file in files)
        {

            //loadingTask = Task.Run(() => LoadGltf(loadedSceneNum, file, deferAgent, importSettings, instantiationSettings));
            var task = LoadGltf(loadedSceneNum, deferAgent, importSettings, instantiationSettings);

            //Task.Run(() => Run(loadedSceneNum, file));

            tasks.Add(task);
            loadedSceneNum++;

        }

        await Task.WhenAll(tasks);
        //sw.Stop();
        loadNextBatch();
       // Debug.LogFormat("Loaded glb file in {0} ms", sw.ElapsedMilliseconds);
        loadedScenes[0].SetActive(true);

    }

    // Update is called once per frame
    private void Update()
    {

    }

    public void loadNextBatch()
    {

        if (loadedScenes.Count() >= maxLoad * 2)
        {
            for (int sceneIndex = CurrentIndex - maxLoad; sceneIndex > CurrentIndex - maxLoad * 2 && sceneIndex >=0; sceneIndex--)
            {
                Destroy(loadedScenes[sceneIndex].GetComponentInChildren<MeshRenderer>().material.mainTexture);
                Destroy(loadedScenes[sceneIndex].GetComponentInChildren<MeshRenderer>());
                Destroy(loadedScenes[sceneIndex].GetComponentInChildren<MeshFilter>().sharedMesh);
                Destroy(loadedScenes[sceneIndex]);
                loadedScenes.Remove(sceneIndex);

            }

        }
        deferAgent = new UninterruptedDeferAgent();
        var files = allFiles.Take(maxLoad).ToArray();
        var tasks = new List<Task>(files.Length);

        foreach (var file in files)
        {
            if (!loadedScenes.ContainsKey(loadedSceneNum))
            {
                var task = LoadGltf(loadedSceneNum, deferAgent, importSettings, instantiationSettings);
                tasks.Add(task);
                loadedSceneNum++;
            }
        }
        Task.WhenAll(tasks);

        notificationManager.description = maxLoad.ToString() + " Frames of Data Loaded successfully!";
        notificationManager.UpdateUI();
        notificationManager.OpenNotification();
        
    }

    public void DeleteAllLoadedScene()
    {
        if (loadedScenes == null)
            return;
        foreach (var scene in loadedScenes)
        {
            Destroy(scene.Value.GetComponentInChildren<MeshRenderer>().material.mainTexture);
            Destroy(scene.Value.GetComponentInChildren<MeshRenderer>());
            Destroy(scene.Value.GetComponentInChildren<MeshFilter>().sharedMesh);
            Destroy(scene.Value);
        }
        loadedScenes.Clear();
    }

    public void loadPreviousBatch()
    {

        if (loadedScenes.Count() >= maxLoad * 2 && loadedScenes.ContainsKey(CurrentIndex + maxLoad * 2))
        {
            for (int sceneIndex = CurrentIndex + maxLoad * 2; sceneIndex > CurrentIndex - maxLoad; sceneIndex--)
            {
                Destroy(loadedScenes[sceneIndex]);
                loadedScenes.Remove(sceneIndex);
                Debug.Log(sceneIndex);
                loadedSceneNum--;
            }

            Debug.Log("Destroy previous");
        }

        var files = allFiles.Take(maxLoad).ToArray();
        var tasks = new List<Task>(files.Length);

        int fileid = CurrentIndex -1;
        foreach (var file in files)
        {
            if (!loadedScenes.ContainsKey(fileid))
            {
                var task = LoadGltf(fileid, deferAgent, importSettings, instantiationSettings);
                tasks.Add(task);
                fileid--;
            }
        }

        Task.WhenAll(tasks);
    }


    public void PlayFrames()
    {
        InvokeRepeating("PlayFromCurrent", 0, playSpeed);
    }

    public void PauseReplay()
    {
        CancelInvoke();
    }

    private void PlayFromCurrent()
    {
        if (CurrentIndex == loadedSceneNum - 3)
        {
            loadNextBatch();
        }
            
        mySlider.mainSlider.value = CurrentIndex + 1;

    }

    public async void Replay()
    {
        CurrentIndex = 0;

        DeleteAllLoadedScene();
        loadedSceneNum = 0;

        deferAgent = new UninterruptedDeferAgent();
        var files = allFiles.Take(maxLoad).ToArray();
        var tasks = new List<Task>(files.Length);

        foreach (var file in files)
        {
            if (!loadedScenes.ContainsKey(loadedSceneNum))
            {
                var task = LoadGltf(loadedSceneNum, deferAgent, importSettings, instantiationSettings);
                tasks.Add(task);
                loadedSceneNum++;
            }
        }

        await Task.WhenAll(tasks);

        mySlider.mainSlider.value = CurrentIndex;

    }

    public void ShowCurrentScene(float val)
    {
        CurrentIndex = (int)val;
        if( CurrentIndex %2 != 0)
        {
            //Debug.Log(CurrentIndex);
            foreach (var scene in loadedScenes)
            {
                scene.Value.SetActive(false);
            }
        
            if (loadedScenes.ContainsKey(CurrentIndex))
            {
                loadedScenes[(int)val].SetActive(true);
                int div = (int)val / 2;
                if (datasetReceiver != null)
                    datasetReceiver.PlayFrame(div);
                objectIndicator.FindAllObjetcs();

                if (poseVisualizer != null)
                    poseVisualizer.LoadPoses(div);
            }
        }

        //if ((CurrentIndex + 1) % maxLoad == 0)
        //    if (previousVal < CurrentIndex)
        //        loadNextBatch();
        //    else
        //        loadPreviousBatch();
        //previousVal = CurrentIndex;
    }

    public async void LoadFrameByNumber(int frame_num)
    {
        DeleteAllLoadedScene();

        var tasks = new List<Task>(1);
        var task = LoadGltf(frame_num, deferAgent, importSettings, instantiationSettings);
        tasks.Add(task);
        await Task.WhenAll(tasks);
        loadedScenes[frame_num].SetActive(true);
        CurrentIndex = frame_num;

    }


    public async void LoadFrameByNumberInterval(int frame_num)
    {
        DeleteAllLoadedScene();

        var tasks = new List<Task>(19);

        int num = 0;
        for (int i = 0; i < 10; i++)
        {
            var task = LoadGltf(frame_num + i, deferAgent, importSettings, instantiationSettings);
            tasks.Add(task);
            num++;
        }

        for (int i = -1; i > -10; i--)
        {
            var task = LoadGltf(frame_num + i, deferAgent, importSettings, instantiationSettings);
            tasks.Add(task);
            num++;
        }
        Debug.Log(num);
        await Task.WhenAll(tasks);


        loadedScenes[frame_num].SetActive(true);
        CurrentIndex = frame_num;
        mySlider.mainSlider.value = CurrentIndex;

    }


    public GameObject GetCurrentScene()
    {
        return loadedScenes[CurrentIndex].transform.GetChild(0).gameObject;
    }

    //async Task CustomDeferAgentPerGltfImport()
    //{
    //    // Recommended: Use a common defer agent across multiple GltfImport instances!
    //    IDeferAgent deferAgent = new UninterruptedDeferAgent();

    //    var tasks = new List<Task>();
    //    var files = allFiles.Take(maxLoad).ToArray();

    //    foreach (var file in files)
    //    {
    //        var gltf = new GLTFast.GltfImport(null, deferAgent);
    //        var task = gltf.Load(file).ContinueWith(
    //            async t => {
    //                if (t.Result)
    //                {
    //                    await gltf.InstantiateMainSceneAsync(transform);
    //                }
    //            },
    //            TaskScheduler.FromCurrentSynchronizationContext()
    //            );
    //        tasks.Add(task);
    //    }

    //    await Task.WhenAll(tasks);
    //}


    //private async Task TestAsync(string url)
    //{
    //    await Task.Run(() =>
    //    {
    //          Debug.Log(url);

    //    });
    //}

    private async void LoadNextGltfAsync(int index)
    {
        //Stopwatch sw = Stopwatch.StartNew();
        var gltf = new GltfImport();
        // Load the glTF and pass along the settings
        var success = await gltf.Load(allFiles[index], importSettings);
        
        if (success)
        {
            var gameObject = new GameObject("Frame");
            await gltf.InstantiateMainSceneAsync(gameObject.transform);
            gameObject.SetActive(false);
            loadedScenes[index] = gameObject;
        }
        else
        {
            Debug.LogError("Loading glTF failed!");
        }
        //sw.Stop();
        //Debug.LogFormat("Loaded glb file in {0} ms", sw.ElapsedMilliseconds);


    }

    async Task LoadGltf(
     int index,
     IDeferAgent deferAgent,
     ImportSettings importSettings,
     InstantiationSettings instantiationSettings
     )
    {
       
        var gltf = new GltfImport(deferAgent: deferAgent);
        var success = await gltf.Load(allFiles[index], importSettings);
        if (success)
        {
            GameObject readyObject = new GameObject(index.ToString());
            readyObject.transform.parent = sceneRoot;
            readyObject.transform.localScale = Vector3.one;
            readyObject.transform.localPosition = Vector3.zero;
            readyObject.transform.localRotation = Quaternion.identity;
            var instantiator = new GameObjectInstantiator(gltf, readyObject.transform, settings: instantiationSettings);
            success = await gltf.InstantiateMainSceneAsync(instantiator);
            readyObject.transform.GetChild(0).localScale = new Vector3(-1f, 1f, -1f);
            readyObject.SetActive(false);
            //MeshCollider meshCollider = readyObject.transform.GetChild(0).gameObject.AddComponent<MeshCollider>();
            //meshCollider.enabled = false;
            //readyObject.transform.GetChild(0).gameObject.AddComponent<VertexClusterBoundingBoxes>();
            //Debug.Log(index);
            loadedScenes.Add(index, readyObject);

        }

    }


}
