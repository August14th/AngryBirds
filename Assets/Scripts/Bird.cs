using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Bird : GameBehaviour
{
    public GameManger GameManger;

    protected Rigidbody2D RigidBody;

    private List<AudioSource> _audios;

    private TrailRenderer _trailRenderer;

    public bool IsFlying { private set; get; }

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
        if (IsBehindGUI()) return;
        if (IsFlying) return;
        Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        GameManger.DragBird(this, pos);
    }

    private void OnMouseUp()
    {
        if (IsBehindGUI()) return;
        if (IsFlying) return;
        GameManger.ThrowBird(this);
    }


    private void Update()
    {
        if (IsFlying)
        {
            if (Input.GetMouseButtonDown(0) && !IsBehindGUI())
            {
                CastSkill();
            }
        }
    }

    public Vector2 Speed
    {
        private get { return RigidBody.velocity; }
        set { RigidBody.velocity = value; }
    }

    public void Fly(Vector2 speed)
    {
        Speed = speed;
        IsFlying = true;
        _trailRenderer.enabled = true;
        RigidBody.isKinematic = false;
        StartCoroutine(DestroyIfStop());

    }

    private IEnumerator DestroyIfStop()
    {
        while (Speed.magnitude > 0.05f)
            yield return null;
        yield return new WaitForSeconds(0.5f);
        Destroy(gameObject);
    }

    public void PlaySound(int idx)
    {
        _audios.ForEach(f => f.enabled = false);
        _audios[idx].enabled = true;
    }

    protected virtual void CastSkill()
    {

    }
}