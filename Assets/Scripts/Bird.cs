using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Bird : MonoBehaviour
{
    public GameManger GameManger;

    protected Rigidbody2D RigidBody;
    
    private List<AudioSource> _audios;

    private TrailRenderer _trailRenderer;

    private bool _fly;

    private void Start()
    {
        RigidBody = GetComponent<Rigidbody2D>();
        _trailRenderer = GetComponent<TrailRenderer>();
        RigidBody.isKinematic = true;
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
        GameManger.ThrowBird(this);
        _fly = true;
    }

    private void FixedUpdate()
    {
        if (_fly && RigidBody.velocity.magnitude < 0.05f)
        {
            StartCoroutine(DestroyAfter(0.5f));
        }
    }

    private void Update()
    {
        if (IsFlying())
        {
            if (Input.GetMouseButtonDown(0))
            {
                CastSkill();
            }
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
        RigidBody.isKinematic = false;
        RigidBody.velocity = speed;
    }
    
    public void MoveTo(Vector2 dest)
    {
        transform.position = dest;
    }

    public void MoveTo(Vector2 dest, Action onCompleted)
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

    protected virtual void CastSkill()
    {
        
    }
}