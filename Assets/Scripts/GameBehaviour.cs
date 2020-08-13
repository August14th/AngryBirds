using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class GameBehaviour : ShortCut
{

    public void GotoScene(string sceneName)
    {
        StartCoroutine(LoadScene(sceneName));
    }

    private IEnumerator LoadScene(string sceneName)
    {
        Assets.LoadScene(sceneName);
        var asyncOpt = SceneManager.LoadSceneAsync(sceneName);
        var loading = Assets.NewUI("Loading", Canvas.transform);
        var progress = loading.GetComponentInChildren<Text>();
        while (!asyncOpt.isDone)
        {
            progress.text = asyncOpt.progress * 100 + "%";
            yield return null;
        }
    }

    protected bool IsBehindGUI()
    {
        return EventSystem.current.IsPointerOverGameObject();
    }

    
}