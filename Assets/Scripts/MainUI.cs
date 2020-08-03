using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainUI : MonoBehaviour
{

    public Button PauseBtn;

    public GameObject PausePanel;


    private void Start()
    {
        PauseBtn.onClick.AddListener(ShowPausePanel);
    }


    private void ShowPausePanel()
    {
        PauseBtn.gameObject.SetActive(false);
        Instantiate(PausePanel, transform, false);
    }

    public void ShowPauseBtn()
    {
        PauseBtn.gameObject.SetActive(true);
    }

}
