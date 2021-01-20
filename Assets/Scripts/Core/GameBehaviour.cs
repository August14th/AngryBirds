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

	protected static Player Player
	{
		get { return GetEngine().Player; }
	}

	protected static Servlet Servlet
	{
		get { return GetEngine().Servlet; }
	}

	protected static GameObject Canvas
	{
		get { return GameObject.Find("Canvas"); }
	}

	protected static LuaState LuaState
	{
		get { return GetEngine().LuaState; }
	}

	protected static Scenes Scenes
	{
		get { return GetEngine().Scenes; }
	}

	protected static bool IsBehindGUI()
	{
		return EventSystem.current.IsPointerOverGameObject();
	}

	protected static void GotoScene(string sceneName)
	{
		Scenes.GotoScene(sceneName);
	}
}
