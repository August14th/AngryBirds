using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class GameBehaviour : MonoBehaviour
{

    private Bundles _bundles;

    private Bundles GetBundles()
    {
        if (_bundles == null)
            _bundles = FindObjectOfType<Bundles>();
        return _bundles;
    }

    public void GotoScene(string sceneName)
    {
        StartCoroutine(LoadScene(sceneName));
    }

    private IEnumerator LoadScene(string sceneName)
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

    protected bool IsBehindGUI()
    {
        return EventSystem.current.IsPointerOverGameObject();
    }

    protected T NewGameObject<T>(string prefabName, GameObject parent) where T : GameBehaviour
    {
        var t = CreateFromBundle<T>(prefabName, parent);
        return t != null ? t : CreateFromResources<T>(prefabName, parent);
    }

    private T CreateFromBundle<T>(string prefabName, GameObject parent) where T : GameBehaviour
    {
        var ab = prefabName.ToLower() + ".ab";
        var bundle = GetBundles().Get(ab);
        if (bundle == null) return null;
        var prefab = bundle.LoadAsset<GameObject>(prefabName);
        var go = Instantiate(prefab, parent.transform, false);
        GetBundles().AddRef(bundle, go);
        return go.GetComponent<T>();
    }

    private T CreateFromResources<T>(string prefabName, GameObject parent) where T : GameBehaviour
    {
        var prefab = Resources.Load<T>(prefabName);
        var go = Instantiate(prefab, parent.transform, false);
        return go.GetComponent<T>();
    }


    private void OnDestroy()
    {
        if (_bundles) _bundles.RemoveRef(gameObject);
    }
}