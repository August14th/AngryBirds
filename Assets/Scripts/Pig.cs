using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pig : Enemy
{
    private const float MaxSpeed = 13;

    private const float MinSpeed = 3;

    public Sprite HurtSprite;

    private SpriteRenderer _renderer;

    private string Boom = "Prefab/Boom";

    private string PigScore = "Prefab/Score3000";

    private void Start()
    {
        _renderer = GetComponent<SpriteRenderer>();
    }

    private void OnCollisionEnter2D(Collision2D bird)
    {
        if (!bird.gameObject.CompareTag("Bird")) return;
        bird.gameObject.GetComponent<Bird>().PlaySound(2);
        var speed = bird.relativeVelocity.magnitude;
        if (speed > MaxSpeed)
            Dead();
        else if (speed > MinSpeed)
            _renderer.sprite = HurtSprite;
    }

    public bool IsMoving()
    {
        return GetComponent<Rigidbody2D>().velocity.magnitude > 0.05f;
    }

    public override void Dead()
    {
        Destroy(gameObject);
        Assets.NewActor(Boom, transform.position);
        GameObject go = Assets.NewActor(PigScore, transform.position + new Vector3(0, 0.5f, 0));
        Destroy(go, 1f);
    }
}