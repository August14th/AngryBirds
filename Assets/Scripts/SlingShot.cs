using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLua;

public class SlingShot : MonoBehaviour
{
    public float Speed;

    public Transform Left, Right;

    public LineRenderer LeftLineRenderer;

    public LineRenderer RightLineRenderer;

    private Bird _bird;

    private Vector2 _middle;

    public Transform BirdPosition;

    private LineRenderer _trajectoryLineRenderer;

    private void Start()
    {
        _middle = (Left.position + Right.position) / 2;
        LeftLineRenderer.enabled = false;
        RightLineRenderer.enabled = false;
        _trajectoryLineRenderer = GetComponent<LineRenderer>();
    }

    public void Take(Bird bird)
    {
        bird.PlaySound(0);
        bird.transform.positionTo(.1f, BirdPosition.position).setOnCompleteHandler(x =>
        {
            _bird = bird;
            DrawSlingShotLines();
        });
    }

    public void DragBird(Vector2 newPos)
    {
        var vector = newPos - _middle;
        var distance = Mathf.Clamp(vector.magnitude, 0, 1.5f);
        _bird.transform.position = distance * vector.normalized + _middle;
        DrawSlingShotLines();
        DrawTrajectoryLine();
    }

    private Vector2 ThrowSpeed()
    {
        Vector2 pos = _bird.transform.position;
        return Speed * (_middle - pos); // 指向中心点
    }


    public void ThrowBird()
    {
        LeftLineRenderer.enabled = false;
        RightLineRenderer.enabled = false;
        _trajectoryLineRenderer.enabled = false;
        _trajectoryLineRenderer.positionCount = 0;
        _bird.PlaySound(1);
        _bird.Fly(ThrowSpeed());
    }

    public void DrawSlingShotLines()
    {
        var pos = _bird.transform.position;
        LeftLineRenderer.enabled = true;
        RightLineRenderer.enabled = true;
        LeftLineRenderer.SetPosition(0, Left.position);
        LeftLineRenderer.SetPosition(1, pos);
        RightLineRenderer.SetPosition(0, Right.position);
        RightLineRenderer.SetPosition(1, pos);
    }

    public void DrawTrajectoryLine()
    {
        _trajectoryLineRenderer.enabled = true;
        var segmentCount = 15;
        Vector2[] segments = new Vector2[segmentCount];
        segments[0] = _bird.transform.position;
        var speed = ThrowSpeed();
        for (var i = 1; i < segmentCount; i++)
        {
            var time = i * Time.fixedDeltaTime * 5;
            segments[i] = segments[0] + speed * time + 0.5f * Physics2D.gravity * Mathf.Pow(time, 2); // vt + 1/2at2
        }

        _trajectoryLineRenderer.positionCount = segmentCount;
        for (var i = 0; i < segmentCount; i++)
        {
            _trajectoryLineRenderer.SetPosition(i, segments[i]);
        }
    }
}
