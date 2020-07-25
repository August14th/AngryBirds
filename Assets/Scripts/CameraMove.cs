using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    private const float MinX = 0;

    private const float MaxX = 10;

    private Vector2 _previousPos;

    public float DragSpeed = 0.1f;

    private float _timeDragStart;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _previousPos = Input.mousePosition;
            _timeDragStart = Time.time;
        }
        else if (Input.GetMouseButton(0) && Time.time - _timeDragStart > 0.05f)
        {
            Vector2 pos = Input.mousePosition;
            Vector2 delta = (_previousPos - pos) * DragSpeed;
            float newX = Mathf.Clamp(transform.position.x + delta.x, MinX, MaxX);
            transform.position = new Vector3(newX, transform.position.y, transform.position.z);
            _previousPos = pos;
        }
    }

}