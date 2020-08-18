using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class GameBehaviour : ShortCut
{

    protected bool IsBehindGUI()
    {
        return EventSystem.current.IsPointerOverGameObject();
    }

    protected void GotoScene(string sceneName)
    {
        Scenes.GotoScene(sceneName);
    }

    
}