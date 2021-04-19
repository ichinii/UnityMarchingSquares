using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SideScrollScript : MonoBehaviour
{
    public GameObject gridObject;

    private Vector2 prevSize = new Vector2(0, 0);

    void Start()
    {
        
    }

    void Update()
    {
        var camera = Camera.main;
        var size = new Vector2(camera.orthographicSize * camera.aspect, camera.orthographicSize);

        if (size != prevSize)
        {
            prevSize = size;

            var grid = gridObject.GetComponent<GridScript>();
            var scale = size.y / grid.size.y * 2.0f;
            transform.localScale = new Vector3(scale, scale, 1);
        }
    }
}
