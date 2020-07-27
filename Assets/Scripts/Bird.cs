using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody2D))]
public class Bird : MonoBehaviour
{
    public GameManger GameManger;

    private Rigidbody2D _rigidBody;
    
    private List<AudioSource> _audios;

    private TrailRenderer _trailRenderer;

    private bool _fly;

    private void Start()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
        _trailRenderer = GetComponent<TrailRenderer>();
        _rigidBody.isKinematic = true;
        _trailRenderer.enabled = false;
        _audios = GetComponents<AudioSource>().ToList();
    }

    private void OnMouseDrag()
    {
        Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        GameManger.DragBird(this, pos);
    }

    private void OnMouseUp()
    {
        _trailRenderer.enabled = true;
        _rigidBody.isKinematic = false;
        GameManger.ThrowBird(this);
        _fly = true;
    }

    private void FixedUpdate()
    {
        if (_fly && _rigidBody.velocity.magnitude < 0.05f)
        {
            StartCoroutine(DestroyAfter(0.5f));
        }
    }
    
    IEnumerator DestroyAfter(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        _fly = false;
        Destroy(gameObject);
    }

    public void SetSpeed(Vector2 speed)
    {
        _rigidBody.velocity = speed;
    }
    
    public void MoveTo(Vector2 dest)
    {
        transform.position = dest;
    }

    public void MoveTo(Vector2 dest, UnityAction onCompleted)
    {
        transform.positionTo(.1f, dest).setOnCompleteHandler(x => onCompleted());
    }

    public void PlaySound(int idx)
    {
        _audios.ForEach(f => f.enabled = false);
        _audios[idx].enabled = true;
    }

    public bool IsFlying()
    {
        return _fly;
    }
}