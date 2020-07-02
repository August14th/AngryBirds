using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManger : MonoBehaviour
{
    private List<GameObject> _birds;

    private SlingShot _slingShot;

    private State _state;

    private int _birdIndex = -1;

    private CameraFollow _cameraFollow;

    // Use this for initialization
    void Start()
    {
        _birds = new List<GameObject>(GameObject.FindGameObjectsWithTag("Bird"));
        _slingShot = FindObjectOfType<SlingShot>();
        _cameraFollow = GetComponent<CameraFollow>();
    }

    private void Update()
    {
        switch (_state)
        {
            case State.Idle:
                if (Input.GetMouseButtonDown(0) && _birds.Count != 0)
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
                            SceneManager.LoadScene(1);
                    });
                }

                break;
        }
    }

    private bool TakeNextBird()
    {
        if (_birdIndex + 1 < _birds.Count)
        {
            _birdIndex++;
            var bird = _birds[_birdIndex];
            _slingShot.SetBirdToThrow(bird);
            return true;
        }
        return false;
    }

    private bool BricksBirdsPigsStoppedMoving()
    {
        foreach (var bird in _birds)
        {
            if (bird) // 判断是不是已经被destroy了
            {
                if (bird.GetComponent<Bird>().Flying()) return false;
            }
        }

        return true;
    }
}