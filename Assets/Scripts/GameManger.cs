using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManger : MonoBehaviour
{
    private List<Bird> _birds;

    private SlingShot _slingShot;

    private State _state;

    // Use this for initialization
    void Start()
    {
        _birds = new List<Bird>(FindObjectsOfType<Bird>());
        _slingShot = FindObjectOfType<SlingShot>();
    }

    private void Update()
    {
        switch (_state)
        {
            case State.Idle:
                if (Input.GetMouseButtonDown(0) && _birds.Count != 0)
                {
                    _state = State.Playing;
                    var bird = _birds[0];
                    _birds.Remove(bird);
                    _slingShot.SetBirdToThrow(bird.gameObject);
                }

                break;
            case State.Playing:
                if (_slingShot.State == SlingShotState.Flying && BricksBirdsPigsStoppedMoving())
                {
                    _state = State.Idle;
                    _slingShot.State = SlingShotState.Idle;
                }

                break;
        }
    }

    private bool BricksBirdsPigsStoppedMoving()
    {
        foreach (var bird in _birds)
        {
            var velocity = bird.GetComponent<Rigidbody2D>().velocity.sqrMagnitude;
            if (velocity > 0.05f) return false;
        }

        return true;
    }
}