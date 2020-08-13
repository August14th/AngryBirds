using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Assets : MonoBehaviour
{
	private Bundles _bundles;

	// Use this for initialization
	private void Start()
	{
#if UNITY_EDITOR
#else
	_bundles = gameObject.AddComponent<Bundles>();
#endif
	}

	public T NewActor<T>(string prefabName, Vector3 position)
	{
		var actor = NewActor(prefabName, position);
		return actor.GetComponent<T>();
	}

	public GameObject NewActor(string prefabName, Vector3 position)
	{
		if (_bundles) return _bundles.NewActor(prefabName, position);
		var prefab = Resources.Load<GameObject>(prefabName);
		return Instantiate(prefab, position, Quaternion.identity);
	}

	public T NewUI<T>(string prefabName, Transform parent)
	{
		var ui = NewUI(prefabName, parent);
		return ui.GetComponent<T>();
	}

	public GameObject NewUI(string prefabName, Transform parent)
	{
		if (_bundles) return _bundles.NewUI(prefabName, parent);
		var prefab = Resources.Load<GameObject>(prefabName);
		return Instantiate(prefab, parent, false);
	}

	public void LoadScene(string sceneName)
	{
		if (_bundles) _bundles.LoadScene(sceneName);
	}
}
