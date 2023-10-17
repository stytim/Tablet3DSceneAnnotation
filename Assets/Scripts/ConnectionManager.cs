using System.Collections;
using System.Collections.Generic;
using Dalak.LineRenderer3D;
using UnityEngine;
using System.Linq;

public class ConnectionManager : MonoBehaviour
{
    public static ConnectionManager Instance;

    public List<Connection> Connections = new List<Connection>();

    private Connection _ActiveConnection = null;

    private Dictionary<string, GameObject> _ConnectionGameObjects;

    private Color _ConnectionColor = Color.green;

    public GameObject _ConnectionPrefab;

    private bool displayConnection = true;

    private float _radius = 0.01f;
    // Start is called before the first frame update
    void Start()
    {
        Instance = this;

        _ConnectionGameObjects = new Dictionary<string, GameObject>();

        _ActiveConnection = null;
    }

    // Update is called once per frame
    void Update()
    {


        HashSet<string> touchedConnection = new HashSet<string>();
        foreach (var connection in Connections)
        {
            GenerateConnectionObject(connection);
            touchedConnection.Add(connection.UniqueID);
        }

        var KeyCollection = _ConnectionGameObjects.Keys;
        List<string> keys = new List<string>();
        foreach (var key in KeyCollection)
        {
            keys.Add(key);
        }

        for (int i = 0; i < keys.Count; i++)
        {
            var key = keys[i];
            if (!touchedConnection.Contains(key))
            {
                Destroy(_ConnectionGameObjects[key]);
                _ConnectionGameObjects.Remove(key);
            }
        }
    }

    public void AddConnection(ConnectionType connectionType, Transform pose, Vector3 point, bool isSticky = true, float? size = null, string annotationName = null, string description = null, string materialName = null)
    {


        switch (connectionType)
        {
            case ConnectionType.Line:
                {
                    // Create new container for a line annotation
                    if (_ActiveConnection == null || _ActiveConnection.ConnectionType == ConnectionType.None)
                    {
                        _ActiveConnection = new Connection()
                        {
                            Points = new List<Vector3>(),
                            Objects = new List<Transform>(),
                            ConnectionType = ConnectionType.Line,
                            Size = size.GetValueOrDefault(0.001f),
                            Color = _ConnectionColor,
                            Description = string.IsNullOrEmpty(description) ? "" : description
                        };

                        Connections.Add(_ActiveConnection);
                        Debug.Log("new Add connection");
                    }

                    if (_ActiveConnection.ConnectionType == ConnectionType.Line)
                    {
                        _ActiveConnection.Objects.Add(pose);
                        _ActiveConnection.Points.Add(point);
                    }
                        

                    if (_ActiveConnection.Objects.Count > 1)
                    {
                        _ActiveConnection = null;
                    }


                    break;
                }
            default: break;
        }
    }

    public void DeleteLastObject()
    {
        if (_ActiveConnection != null)
        {
            if (_ActiveConnection.Objects.Count > 0)
            {
                _ActiveConnection.Objects.RemoveAt(_ActiveConnection.Objects.Count - 1);
                _ActiveConnection.Points.RemoveAt(_ActiveConnection.Points.Count - 1);
            }
        }
    }

    public void DeleteLastConnection()
    {
        if (Connections.Count > 0)
        {
            var _LastConnection = Connections.Last();
            foreach (var selectedObject in _LastConnection.Objects)
            {
                selectedObject.parent.GetComponent<BoundingBoxVisualizer>().ChangeTransparency(new Color(0f, 0f, 0f, 0f));
            }
            Connections.RemoveAt(Connections.Count - 1);
        }

    }

    public void DeleteAllLocalConnection()
    {
        Connections.Clear();
    }

    public void DisplayConection(bool flag)
    {
        displayConnection = flag;
    }

    public void ChangeLineRadius(float radius)
    {
        _radius = radius;
    }

    public void AddRelation(string relation)
    {
        if (Connections.Count != 0 )
        {
            var _LastConnection = Connections.Last();
            Settings.Instance?.AddRelation(_LastConnection.Objects[0].parent.name, _LastConnection.Objects[1].parent.name, relation);
            _LastConnection.Objects[0].parent.GetComponent<BoundingBoxVisualizer>().ChangeTransparency(new Color(0f, 0f, 0f, 0f));
            _LastConnection.Objects[1].parent.GetComponent<BoundingBoxVisualizer>().ChangeTransparency(new Color(0f, 0f, 0f, 0f));
            EstablishConnection.Instance?.ResumeAnnotation();
        }
    }

    public void GenerateConnectionObject(Connection connection)
    {

        GameObject _CurrentConnection = null;

        //############################### Line Visualization #################################
        if (connection.ConnectionType == ConnectionType.Line)
        {

            LineRenderer3D _TubeRenderer;
            if (_ConnectionGameObjects.ContainsKey(connection.UniqueID))
            {
                _CurrentConnection = _ConnectionGameObjects[connection.UniqueID];
                _TubeRenderer = _CurrentConnection.GetComponentInChildren<LineRenderer3D>();
            }
            else
            {
                _CurrentConnection = Instantiate(_ConnectionPrefab, transform);
                //_ConnectionPrefab.transform.parent = transform;

                _TubeRenderer = _CurrentConnection.GetComponentInChildren<LineRenderer3D>();


                //_CurrentConnection = new GameObject("connection");
                //_CurrentConnection.transform.parent = transform;
                //GameObject _LineConnection = new GameObject("LineModel");
                //_LineConnection.transform.parent = _CurrentConnection.transform;
                //_LineConnection.transform.localPosition = Vector3.zero;
                //_LineConnection.transform.localRotation = Quaternion.identity;
                //_LineConnection.transform.localScale = Vector3.one;
                //_LineConnection.layer = LayerMask.NameToLayer("Content");

                _ConnectionGameObjects[connection.UniqueID] = _CurrentConnection;

                //MeshFilter _MeshFilter = _LineConnection.AddComponent<MeshFilter>();
                //_TubeRenderer = _LineConnection.AddComponent<LineRenderer3D>();
                //_TubeRenderer.meshFilter = _MeshFilter;
                //_TubeRenderer.generateOnAwake = false;
                //_TubeRenderer.pipeMeshSettings.radius = 0.25f;
                //_TubeRenderer.markDynamic = true;

            }

            if (connection.Objects.Count > 1)
            {
                _TubeRenderer.pathData.positions.Clear();
                _TubeRenderer.pathData.positions.Add(connection.Objects[0].position);
                _TubeRenderer.pathData.positions.Add(connection.Objects[1].position);
                _TubeRenderer.pipeMeshSettings.radius = _radius;
                _TubeRenderer.UpdateMesh();
            }


        }

        _CurrentConnection.GetComponentInChildren<MeshRenderer>().enabled = displayConnection;
    }

}
