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

    private int _birdIndex = -1;

    private CameraMove _cameraMove;

    private CameraFollow _cameraFollow;

    public String Level;

    private bool _playing;

    // Use this for initialization
    void Start()
    {
        _cameraMove = GetComponent<CameraMove>();
        _cameraFollow = GetComponent<CameraFollow>();
    }

    private void Update()
    {
        if (!_playing && Input.GetMouseButtonDown(0) && !IsBehindGUI())
        {
            _playing = true;
            TakeNextBird();
        }
    }

    private void TakeNextBird()
    {
        _birdIndex++;
        var bird = Birds[_birdIndex];
        _cameraFollow.MoveToStartPos(() =>
        {
            SlingShot.Take(bird);
            _cameraMove.enabled = true;
        });
    }

    public void DragBird(Bird bird, Vector2 newPos)
    {
        if (bird != Birds[_birdIndex]) return;
        if (_cameraMove.enabled) _cameraMove.enabled = false;
        SlingShot.DragBird(newPos);
    }

    public void ThrowBird(Bird bird)
    {
        StartCoroutine(DoThrowBird(bird));
    }

    private IEnumerator DoThrowBird(Bird bird)
    {
        if (bird != Birds[_birdIndex]) yield break;
        if (_cameraMove.enabled) _cameraMove.enabled = false;
        _cameraFollow.StartFollow(bird.gameObject);
        SlingShot.ThrowBird();
        while (!AllStopMoving())
        {
            yield return null;
        }

        if (!IsOver()) TakeNextBird();
        else Settle();
    }

    private bool AllStopMoving()
    {
        foreach (var bird in Birds)
        {
            // 判断是不是已经被destroy了
            if (bird && bird.IsFlying) return false;
        }

        foreach (var pig in Pigs)
        {
            if (pig && pig.IsMoving()) return false;
        }

        return true;
    }

    private void Settle()
    {
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
            LuaState.DoString("local panel = assets:NewUI('Prefab/LosePanel', canvas().transform);" +
                              "panel:lua('lose_panel')");
        }
    }

    private bool IsOver()
    {
        return Birds.Count(b => b) == 0 || Pigs.Count(b => b) == 0;
    }
}