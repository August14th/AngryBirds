﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    private readonly float _minX = 0;

    private readonly float _maxX = 13;

    public SlingShot SlingShot;

    private Vector2 _previousPos;

    public float DragSpeed = 0.1f;

    private float _timeDragStart;

    // Update is called once per frame
    void Update()
    {
        if (SlingShot.State < SlingShotState.Pulling)
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
                float newX = Mathf.Clamp(transform.position.x + delta.x, _minX, _maxX);
                transform.position = new Vector3(newX, transform.position.y, transform.position.z);
                _previousPos = pos;
            }
        }
    }

}