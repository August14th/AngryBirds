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
        var loading = New("Loading", Canvas.transform);
        
        var bundleName = sceneName.ToLower() + ".ab";
        var bundle = Bundles.Get(bundleName);
        bundle.LoadAsset<GameObject>(sceneName);
        
        var asyncOpt = SceneManager.LoadSceneAsync(sceneName);
        while (!asyncOpt.isDone)
        {
            var progress = loading.GetComponentInChildren<Text>();
            progress.text = asyncOpt.progress * 100 + "%";
            yield return null;
        }
    }

    protected bool IsBehindGUI()
    {
        return EventSystem.current.IsPointerOverGameObject();
    }

    protected GameObject New(string prefabName, Transform parent)
    {
        var go = CreateFromBundle(prefabName, parent);
        if (go == null) go = CreateFromResources(prefabName, parent);
        if (!go.GetComponent<ClearOnDestroy>()) go.AddComponent<ClearOnDestroy>();

        return go;
    }

    protected T New<T>(string prefabName, Transform parent)
    {
        var go = New(prefabName, parent);
        return go.GetComponent<T>();
    }

    private GameObject CreateFromBundle(string prefabName, Transform parent)
    {
        var bundleName = prefabName.ToLower() + ".ab";
        var bundle = Bundles.Get(bundleName);
        if (bundle == null) return null;
        var prefab = bundle.LoadAsset<GameObject>(prefabName);
        var go = Instantiate(prefab, parent.transform, false);
        Bundles.AddRef(bundle, go);
        return go;
    }

    private GameObject CreateFromResources(string prefabName, Transform parent)
    {
        var prefab = Resources.Load<GameObject>(prefabName);
        var go = Instantiate(prefab, parent.transform, false);
        return go;
    }
}