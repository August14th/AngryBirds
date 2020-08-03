using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class GameBehaviour : MonoBehaviour
{
    public void GotoScene(string sceneName)
    {
        StartCoroutine(Load(sceneName));
    }

    private IEnumerator Load(string sceneName)
    {
        GameObject prefab = Resources.Load<GameObject>("Loading");
        var loading = Instantiate(prefab, gameObject.transform.root, false);
        var asyncOpt = SceneManager.LoadSceneAsync(sceneName);
        while (!asyncOpt.isDone)
        {
            var progress = loading.GetComponentInChildren<Text>();
            progress.text = asyncOpt.progress * 100 + "%";
            yield return null;
        }
    }

    public bool IsOnGUI()
    {
        return EventSystem.current.IsPointerOverGameObject();
    }
}