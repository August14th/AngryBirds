using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlingShot : MonoBehaviour
{

    public Transform Left, Right;

    private Vector2 _middle;

    private GameObject _birdToThrow;

    public Transform BirdPosition;

    private LineRenderer _trajectoryLineRenderer;

    public float Speed;

    public LineRenderer LeftLineRenderer;

    public LineRenderer RightLineRenderer;

    public SlingShotState State = SlingShotState.Idle;

    private void Start()
    {
        _trajectoryLineRenderer = GetComponent<LineRenderer>();
        _middle = (Left.position + Right.position) / 2;
        LeftLineRenderer.SetPosition(0, Left.position);
        RightLineRenderer.SetPosition(0, Right.position);
    }

    private Vector2 ThrowSpeed()
    {
        Vector2 pos = _birdToThrow.transform.position;
        return Speed * (_middle - pos); // 指向中心点
    }


    public void DrawSlingShotLines()
    {
        var pos = _birdToThrow.transform.position;
        LeftLineRenderer.SetPosition(1, pos);
        RightLineRenderer.SetPosition(1, pos);
    }

    public void PullBird(Vector2 newPos)
    {
        State = SlingShotState.Pulling;
        var vector = newPos - _middle;
        var distance = Mathf.Clamp(vector.sqrMagnitude, 0, 1.5f);
        _birdToThrow.transform.position = distance * vector.normalized + _middle;
        DrawSlingShotLines();
        DrawTrajectoryLine();
    }


    public void SetBirdToThrow(GameObject bird)
    {
        State = SlingShotState.Waiting;
        _birdToThrow = bird;
        bird.transform.positionTo(.1f, BirdPosition.position).setOnCompleteHandler(x =>
        {
            LeftLineRenderer.enabled = true;
            RightLineRenderer.enabled = true;
            _trajectoryLineRenderer.enabled = true;
            DrawSlingShotLines();
        });
    }

    public void ThrowBird()
    {
        State = SlingShotState.Flying;
        LeftLineRenderer.enabled = false;
        RightLineRenderer.enabled = false;
        _trajectoryLineRenderer.enabled = false;
        _trajectoryLineRenderer.positionCount = 0;
        _birdToThrow.GetComponent<Rigidbody2D>().velocity = ThrowSpeed();
        Camera.main.GetComponent<CameraFollow>().StartFollow(_birdToThrow);
        _birdToThrow = null;
    }


    public void DrawTrajectoryLine()
    {
        var gravity = _birdToThrow.GetComponent<Rigidbody2D>().gravityScale * Physics2D.gravity; // 重力
        var segmentCount = 15;
        Vector2[] segments = new Vector2[segmentCount];
        segments[0] = _birdToThrow.transform.position;
        for (var i = 1; i < segmentCount; i++)
        {
            var time = i * Time.fixedDeltaTime * 5;
            segments[i] = segments[0] + ThrowSpeed() * time + 0.5f * gravity * Mathf.Pow(time, 2); // vt + 1/2at2
        }

        _trajectoryLineRenderer.positionCount = segmentCount;
        for (var i = 0; i < segmentCount; i++)
        {
            _trajectoryLineRenderer.SetPosition(i, segments[i]);
        }
    }
}
