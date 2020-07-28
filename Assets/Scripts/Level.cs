using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Level : MonoBehaviour
{
    public Image Star1;

    public Image Star2;

    public Image Star3;

    public Image Lock;

    public Text LevelTxt;

    // Use this for initialization
    public void SetStatus(int level, bool open, int stars)
    {
        LevelTxt.text = level.ToString();
        Lock.gameObject.SetActive(!open);
        if (open)
        {
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
                SceneManager.LoadScene(1);
            });
        }
    }


    // Update is called once per frame
    void Update()
    {
    }
}