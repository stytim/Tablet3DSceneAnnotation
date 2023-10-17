using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[Serializable]
public enum ConnectionType
{
    None = 0,
    Line,
    Pin,
    Hull,
    Rect
};

[Serializable]
public class Connection
{
    public string UniqueID = Guid.NewGuid().ToString();
    public string Description;
    public ConnectionType ConnectionType;
    public string StickyTo = "";
    public Color Color;
    public float Size;
    public List<Vector3> Points = new List<Vector3>();
    public List<Transform> Objects = new List<Transform>();
    [NonSerialized]
    public List<Vector3> HelpingPoints = new List<Vector3>();
    

}