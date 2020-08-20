using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEngine : MonoBehaviour
{
	public string ResourceUri = "http://localhost:7070/D%3A/Projects/AngryBirds/";
	
	// Use this for initialization
	IEnumerator Start()
	{
#if UNITY_EDITOR
		var loader = gameObject.AddComponent<Resources>();
#else
		var loader = gameObject.AddComponent<Bundles>();
		loader.StartDownloads(ResourceUri);
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
