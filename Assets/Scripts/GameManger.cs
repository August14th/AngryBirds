using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManger : GameBehaviour
{
    public List<Bird> Birds;

    public List<Pig> Pigs;

    public SlingShot SlingShot;

    private string LosePanel = "Prefab/LosePanel";

    private string WinPanel = "Prefab/WinPanel";

    private State _state;

    private int _birdIndex = -1;

    private CameraMove _cameraMove;

    private CameraFollow _cameraFollow;

    public String Level;

    // Use this for initialization
    void Start()
    {
        _cameraMove = GetComponent<CameraMove>();
        _cameraFollow = GetComponent<CameraFollow>();
    }

    private void Update()
    {
        switch (_state)
        {
            case State.Idle:
                if (Input.GetMouseButtonDown(0)) TakeNextBird();
                break;
            case State.Flying:
                if (BricksBirdsPigsStoppedMoving())
                {
                    if (!IsOver()) TakeNextBird();
                    else Settle();
                }

                break;
        }
    }

    private void TakeNextBird()
    {
        _birdIndex++;
        var bird = Birds[_birdIndex];
        _cameraFollow.MoveToStartPos(() =>
        {
            bird.PutOnSlingShot(SlingShot);
            _cameraMove.enabled = true;
            _state = State.Waiting;
        });
        _state = State.Taking;
    }

    public void DragBird(Bird bird, Vector2 newPos)
    {
        if (bird != Birds[_birdIndex]) return;
        if (_cameraMove.enabled) _cameraMove.enabled = false;
        SlingShot.DragBird(newPos);
        _state = State.Pulling;
    }

    public void ThrowBird(Bird bird)
    {
        if (bird != Birds[_birdIndex]) return;
        if (_cameraMove.enabled) _cameraMove.enabled = false;
        _cameraFollow.StartFollow(bird.gameObject);
        SlingShot.ThrowBird();
        _state = State.Flying;
    }

    private bool BricksBirdsPigsStoppedMoving()
    {
        foreach (var bird in Birds)
        {
            // 判断是不是已经被destroy了
            if (bird && bird.IsFlying()) return false;
        }

        foreach (var pig in Pigs)
        {
            if (pig && pig.IsMoving()) return false;
        }

        return true;
    }

    private void Settle()
    {
        _state = State.Over;
        var isWin = Pigs.Count(b => b) == 0;
        if (isWin)
        {
            var stars = Birds.Count(b => b) + 1;
            var winPanel = Assets.NewUI<WinPanel>(WinPanel, Canvas.transform);
            winPanel.SetStars(stars);
            var lastStars = PlayerPrefs.GetInt(Level);
            if (lastStars < stars)
            {
                PlayerPrefs.SetInt(Level, stars);
                var totalStars = PlayerPrefs.GetInt("Stars");
                PlayerPrefs.SetInt("Stars", totalStars + stars - lastStars);
            }
        }
        else
        {
            LuaState.DoString("local panel = require 'lose_panel'; panel();");
        }
    }

    private bool IsOver()
    {
        return Birds.Count(b => b) == 0 || Pigs.Count(b => b) == 0;
    }
}