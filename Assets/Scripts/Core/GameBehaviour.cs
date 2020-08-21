using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GameBehaviour : MonoBehaviour
{

	private static GameEngine _engine;

	private static GameEngine GetEngine()
	{
		if (_engine == null)
		{
			_engine = GameObject.FindWithTag("GameEngine").GetComponent<GameEngine>();
		}

		return _engine;
	}

	public static AssetLoader Assets
	{
		get { return GetEngine().Assets; }
	}

	public static GameObject Canvas
	{
		get { return GameObject.Find("Canvas"); }
	}

	public static LuaState LuaState
	{
		get { return GetEngine().LuaState; }
	}

	public static Scenes Scenes
	{
		get { return GetEngine().Scenes; }
	}

	public static bool IsBehindGUI()
	{
		return EventSystem.current.IsPointerOverGameObject();
	}

	public static void GotoScene(string sceneName)
	{
		Scenes.GotoScene(sceneName);
	}
}
