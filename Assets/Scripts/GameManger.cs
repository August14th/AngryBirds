using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManger : MonoBehaviour
{
    public List<Bird> Birds;

    public List<Pig> Pigs;

    private SlingShot _slingShot;

    private State _state;

    private int _birdIndex = -1;

    private CameraFollow _cameraFollow;

    // Use this for initialization
    void Start()
    {
        _slingShot = FindObjectOfType<SlingShot>();
        _cameraFollow = GetComponent<CameraFollow>();
    }

    private void Update()
    {
        switch (_state)
        {
            case State.Idle:
                if (Input.GetMouseButtonDown(0) && Birds.Count != 0)
                {
                    _state = State.Playing;
                    TakeNextBird();
                }

                break;
            case State.Playing:
                if (_slingShot.State == SlingShotState.Flying && BricksBirdsPigsStoppedMoving())
                {
                    _slingShot.State = SlingShotState.Idle;
                    _cameraFollow.MoveToStartPos().setOnCompleteHandler(x =>
                    {
                        if (!TakeNextBird())
                            SceneManager.LoadScene(0);
                    });
                }

                break;
        }
    }

    private bool TakeNextBird()
    {
        if (_birdIndex + 1 < Birds.Count)
        {
            _birdIndex++;
            var bird = Birds[_birdIndex];
            _slingShot.SetBirdToThrow(bird.gameObject);
            return true;
        }

        return false;
    }

    private bool BricksBirdsPigsStoppedMoving()
    {
        foreach (var bird in Birds)
        {
            if (bird) // 判断是不是已经被destroy了
            {
                if (bird.Flying()) return false;
            }
        }

        foreach (var pig in Pigs)
        {
            if (pig)
            {
                if (pig.GetComponent<Rigidbody2D>().velocity.magnitude > 0.05f) return false;
            }
        }

        return true;
    }
}