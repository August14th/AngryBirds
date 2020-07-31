using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{

    public GameObject Loading;


    public void Load(string sceneName)
    {
        StartCoroutine(Load0(sceneName));
    }

    private IEnumerator Load0(string sceneName)
    {
        var loading = Instantiate(Loading, transform, false);
        var asyncOpt = SceneManager.LoadSceneAsync(sceneName);
        while (!asyncOpt.isDone)
        {
            var progress = loading.GetComponentInChildren<Text>();
            progress.text = asyncOpt.progress * 100 + "%";
            yield return null;
        }
    }
}
