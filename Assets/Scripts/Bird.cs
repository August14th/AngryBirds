using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Bird : MonoBehaviour
{
    public SlingShot SlingShot;

    private Rigidbody2D _rigidBody;

    private TrailRenderer _trailRenderer;

    private List<AudioSource> _audios;

    private bool _flying;
    
    private void Start()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
        _trailRenderer = GetComponent<TrailRenderer>();
        _rigidBody.isKinematic = true;
        _trailRenderer.enabled = false;
        _audios = GetComponents<AudioSource>().ToList();
    }

    private void OnMouseDown()
    {
        SlingShot.StartPullingBird();
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
    
    
    public void OnThrown(Vector2 speed)
    {
        _rigidBody.velocity = speed;
        _audios.ForEach(f => f.enabled = false);
        _audios[1].enabled = true;
        
    }

    public GoTween OnSelected(Vector3 position)
    {
        _audios.ForEach(f => f.enabled = false);
        _audios[0].enabled = true;

        return transform.positionTo(.1f, position);
    }


    public void OnCollision()
    {
        _audios.ForEach(f => f.enabled = false);
        _audios[2].enabled = true;
    }
}