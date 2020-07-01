using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Bird : MonoBehaviour
{
    public SlingShot SlingShot;

    private Rigidbody2D _rigidBody;

    private TrailRenderer _trailRenderer;

    private void Start()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
        _trailRenderer = GetComponent<TrailRenderer>();
        _rigidBody.isKinematic = true;
        _trailRenderer.enabled = false;
    }

    private void OnMouseDrag()
    {
        Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position = pos;

        SlingShot.DrawSlingShotLines();
        SlingShot.DrawTrajectoryLine();
    }

    private void OnMouseUp()
    {
        _trailRenderer.enabled = true;
        _rigidBody.isKinematic = false;
        SlingShot.ThrowBird();
    }
}