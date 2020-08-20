using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GameBehaviour : MonoBehaviour
{

	private static GameObject _engine;

	private static GameObject GetEngine()
	{
		if (_engine == null)
		{
			_engine = GameObject.FindWithTag("GameEngine");
		}

		return _engine;
	}

	protected AssetLoader Assets
	{
#if UNITY_EDITOR
		get { return GetEngine().GetComponent<Resources>(); }
#else
		get { return GetEngine().GetComponent<Bundles>(); }
#endif
	}

	protected GameObject Canvas
	{
		get { return GameObject.Find("Canvas"); }
	}

	protected LuaState LuaState
	{
		get { return GetEngine().GetComponent<LuaState>(); }
	}

	private Scenes Scenes
	{
		get { return GetEngine().GetComponent<Scenes>(); }
	}

	protected bool IsBehindGUI()
	{
		return EventSystem.current.IsPointerOverGameObject();
	}

	protected void GotoScene(string sceneName)
	{
		Scenes.GotoScene(sceneName);
	}
}
