using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    public GameObject BlackMask;

    public GameObject LosePanel;

    public GameObject WinPanel;

    public GameObject Canvas;

    private CameraMove CameraMove;

    // Use this for initialization
    void Start()
    {
        _slingShot = FindObjectOfType<SlingShot>();
        _cameraFollow = GetComponent<CameraFollow>();
        BlackMask.SetActive(false);
        CameraMove = GetComponent<CameraMove>();
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
                    if (!IsOver())
                    {
                        _cameraFollow.MoveToStartPos().setOnCompleteHandler(x => { TakeNextBird(); });
                    }
                    else
                    {
                        BlackMask.SetActive(true);
                        if (IsWin())
                        {
                            int counts = AliveBirds();
                            var winPanel = Instantiate(WinPanel, Canvas.transform, false);
                            winPanel.GetComponent<WinPanel>().SetStars(3 - counts + 1);
                        }
                        else
                        {
                            Instantiate(LosePanel, Canvas.transform, false);
                        }
                    }
                }

                break;
        }
    }

    private void TakeNextBird()
    {
        CameraMove.enabled = true;
        if (_birdIndex + 1 < Birds.Count)
        {
            _birdIndex++;
            var bird = Birds[_birdIndex];
            _slingShot.SetBirdToThrow(bird);
        }
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

    private bool IsOver()
    {
        return Birds.Count(b => b) == 0 || Pigs.Count(b => b) == 0;
    }

    private bool IsWin()
    {
        return Pigs.Count(b => b) == 0;
    }

    private int AliveBirds()
    {
        return Birds.Count(b => b);
    }
}