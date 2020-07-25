using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainUI : MonoBehaviour
{

    public Button PauseBtn;

    public PausePanel PausePanel;


    private void Start()
    {
        PauseBtn.onClick.AddListener(ShowPausePanel);
    }


    private void ShowPausePanel()
    {
        PauseBtn.gameObject.SetActive(false);
        PausePanel.Show();
    }

    public void ShowPauseBtn()
    {
        PauseBtn.gameObject.SetActive(true);
    }

}
