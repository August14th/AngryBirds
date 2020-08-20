using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEngine : MonoBehaviour
{
	// Use this for initialization
	IEnumerator Start()
	{
#if UNITY_EDITOR
		var loader = gameObject.AddComponent<Resources>();
#else
		var loader = gameObject.AddComponent<Bundles>();
#endif
		while (!loader.IsDone())
		{
			yield return null;
		}
		gameObject.AddComponent<LuaState>();
		var scenes = gameObject.AddComponent<Scenes>();
		
		scenes.GotoScene("Main");
	}

}
