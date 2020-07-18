using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Bird : MonoBehaviour
{
    public SlingShot SlingShot;

    private Rigidbody2D _rigidBody;

    private TrailRenderer _trailRenderer;

    private bool _flying;
    
    private void Start()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
        _trailRenderer = GetComponent<TrailRenderer>();
        _rigidBody.isKinematic = true;
        _trailRenderer.enabled = false;
    }

    private void OnMouseDown()
    {
        SlingShot.State = SlingShotState.Pulling;
    }

    private void OnMouseDrag()
    {
        Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        SlingShot.PullBird(pos);
    }

    private void OnMouseUp()
    {
        _trailRenderer.enabled = true;
        _rigidBody.isKinematic = false;
        // GetComponent<AudioSource>().Play();
        _flying = true;
        SlingShot.ThrowBird();
    }

    private void FixedUpdate()
    {
        if (_flying && _rigidBody.velocity.magnitude < 0.05f)
        {
            StartCoroutine(DestroyAfter(1.5f));
        }
    }
    
    IEnumerator DestroyAfter(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        Destroy(gameObject);
    }

    public bool Flying()
    {
        return _flying;
    }
}