using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnblockCamera : MonoBehaviour
{

    Camera dummyCam;
    // Start is called before the first frame update
    void Start()
    {
        dummyCam = GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        ;
    }

    public void Unblock()
    {
        dummyCam.targetTexture = null;
    }

    

}
