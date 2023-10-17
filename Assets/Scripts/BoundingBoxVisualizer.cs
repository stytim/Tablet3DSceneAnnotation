using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoundingBoxVisualizer : MonoBehaviour
{
    private Material boundMat;
    private GameObject boxObject;
    public bool visualize = true;

    public Color MaterialColor = new Color(0, 0.56f, 1f, 1f);
    private MaterialPropertyBlock propertyBlock;
    // Start is called before the first frame update
    void Start()
    {
        CreateCube(MaterialColor);
    }

    private void Update()
    {
        
    }

    void CreateCube(Color MaterialColor)
    {
        boxObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
        boxObject.transform.parent = transform;
        boxObject.transform.localRotation = Quaternion.identity;
        boxObject.transform.localPosition = Vector3.zero;
        boxObject.transform.localScale = Vector3.one;
        boundMat = Resources.Load("BoundingBoxGrabbed") as Material;

        boxObject.GetComponent<Renderer>().material = boundMat;
        ChangeColor(MaterialColor);

    }

    public void SetSize(Vector3 position, Vector3 scale)
    {
        boxObject.transform.position = position;
        boxObject.transform.localScale = scale;
    }

    // OnValidate is called in the editor after the component is edited
    public void ChangeColor(Color _MaterialColor)
    {
        //create propertyblock only if none exists
        if (propertyBlock == null)
            propertyBlock = new MaterialPropertyBlock();
        //Get a renderer component either of the own gameobject or of a child
        Renderer renderer = boxObject.GetComponent<Renderer>();
        //set the color property
        propertyBlock.SetColor("_HoverColorOverride", _MaterialColor);
        //apply propertyBlock to renderer
        renderer.SetPropertyBlock(propertyBlock);
        renderer.enabled = visualize;
    }

    public void ChangeTransparency(Color _MaterialColor)
    {
        //create propertyblock only if none exists
        if (propertyBlock == null)
            propertyBlock = new MaterialPropertyBlock();
        //Get a renderer component either of the own gameobject or of a child
        Renderer renderer = boxObject.GetComponent<Renderer>();
        //set the color property
        propertyBlock.SetColor("_Color", _MaterialColor);
        //apply propertyBlock to renderer
        renderer.SetPropertyBlock(propertyBlock);
        renderer.enabled = visualize;
    }
}
