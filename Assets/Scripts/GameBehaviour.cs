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
        var currentBundleName = SceneBundleName(SceneManager.GetActiveScene().name);
        Bundles.RemoveBundle(currentBundleName);

        var bundleName = SceneBundleName(sceneName);
        Bundles.Get(bundleName);
        var asyncOpt = SceneManager.LoadSceneAsync(sceneName);
        var loading = NewUI("Loading", Canvas.transform);
        var progress = loading.GetComponentInChildren<Text>();
        while (!asyncOpt.isDone)
        {
            progress.text = asyncOpt.progress * 100 + "%";
            yield return null;
        }
    }

    private string SceneBundleName(string sceneName)
    {
        return "scenes/" + sceneName.ToLower() + ".ab";
    }

    protected bool IsBehindGUI()
    {
        return EventSystem.current.IsPointerOverGameObject();
    }

    protected T NewActor<T>(string prefabName, Vector3 position)
    {
        var actor = NewActor(prefabName, position);
        return actor.GetComponent<T>();
    }

    protected GameObject NewActor(string prefabName, Vector3 position)
    {
        AssetBundle bundle;
        var prefab = LoadFromBundle(prefabName, out bundle);
        if (prefab != null)
        {
            var actor = Instantiate(prefab, position, Quaternion.identity);
            if (!actor.GetComponent<ClearOnDestroy>()) 
                actor.AddComponent<ClearOnDestroy>();
            Bundles.AddRef(bundle, actor);
            return actor;
        }

        prefab = LoadFromResources(prefabName);
        if (prefab != null)
        {
            return Instantiate(prefab, position, Quaternion.identity);
        }

        return null;
    }

    protected T NewUI<T>(string prefabName, Transform parent)
    {
        var ui = NewUI(prefabName, parent);
        return ui.GetComponent<T>();
    }
    
    protected GameObject NewUI(string prefabName, Transform parent)
    {
        AssetBundle bundle;
        var prefab = LoadFromBundle(prefabName, out bundle);
        if (prefab != null)
        {
            var ui = Instantiate(prefab, parent.transform, false);
            ui.AddComponent<ClearOnDestroy>();
            Bundles.AddRef(bundle, ui);
            return ui;
        }

        prefab = LoadFromResources(prefabName);
        if (prefab != null)
        {
            return Instantiate(prefab, parent.transform, false);
        }

        return null;
    }

    private GameObject LoadFromBundle(string prefabName, out AssetBundle bundle)
    {
        var bundleName = prefabName.ToLower() + ".ab";
        bundle = Bundles.Get(bundleName);
        if (bundle == null) return null;
        var prefab = bundle.LoadAsset<GameObject>(prefabName);
        return prefab;
    }

    private GameObject LoadFromResources(string prefabName)
    {
        var prefab = Resources.Load<GameObject>(prefabName);
        return prefab;
    }
}