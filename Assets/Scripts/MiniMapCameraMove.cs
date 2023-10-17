using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMapCameraMove : MonoBehaviour
{
    public Transform player;
    public Transform scene;
    private Camera _camera;
    // Start is called before the first frame update
    void Start()
    {
        _camera = GetComponent<Camera>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        //Vector3 newPosition = player.position;
        //newPosition.y = transform.position.y;
        //transform.position = newPosition;
        transform.position = new Vector3(scene.position.x, 8.61f + scene.position.y, scene.position.z);
        transform.rotation = Quaternion.Euler(90f, player.eulerAngles.y, 0f);
        _camera.orthographicSize = 3 * scene.localScale.x;
    }
}
