using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Level : GameBehaviour
{
    public Image Star1;

    public Image Star2;

    public Image Star3;

    public Text LevelTxt;

    public Sprite Lock;

    // Use this for initialization
    public void SetStatus(int map, int level, bool open, int stars)
    {
        var sceneName = map + "-" + level;
        if (open)
        {
            LevelTxt.text = level.ToString();
            if (stars >= 1)
            {
                Star1.gameObject.SetActive(true);
            }

            if (stars >= 2)
            {
                Star2.gameObject.SetActive(true);
            }

            if (stars >= 3)
            {
                Star3.gameObject.SetActive(true);
            }
            GetComponent<Button>().onClick.AddListener(() =>
            {
                GotoScene(sceneName);
            });
        }
        else
        {
            LevelTxt.gameObject.SetActive(false);
            GetComponent<Image>().overrideSprite = Lock;
        }
    }
}