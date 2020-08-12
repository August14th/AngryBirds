using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainUI : GameBehaviour
{

    public Button PauseBtn;

    private string PausePanel = "Prefab/PausePanel";


    private void Start()
    {
        PauseBtn.onClick.AddListener(ShowPausePanel);
    }


    private void ShowPausePanel()
    {
        PauseBtn.gameObject.SetActive(false);
        NewUI(PausePanel, transform);
    }

    public void ShowPauseBtn()
    {
        PauseBtn.gameObject.SetActive(true);
    }

}
