using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pig : MonoBehaviour
{
    private const float MaxSpeed = 13;

    private const float MinSpeed = 3;

    public Sprite HurtSprite;

    private SpriteRenderer _renderer;

    public GameObject Boom;

    public GameObject PigScore;

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
        {
            Destroy(gameObject);
            Instantiate(Boom, transform.position, Quaternion.identity);
            GameObject go = Instantiate(PigScore, transform.position + new Vector3(0, 0.5f, 0), Quaternion.identity);
            Destroy(go, 1f);
        }
        else if (speed > MinSpeed)
        {
            _renderer.sprite = HurtSprite;
        }
    }

    public bool IsMoving()
    {
        return GetComponent<Rigidbody2D>().velocity.magnitude > 0.05f;
    }
}