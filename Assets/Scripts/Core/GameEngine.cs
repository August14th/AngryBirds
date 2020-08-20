using System.Collections;
using UnityEngine;

public class GameEngine : MonoBehaviour
{
	public string ResourceUri = "http://localhost:7070/D%3A/Projects/AngryBirds/";

	// Use this for initialization
	private IEnumerator Start()
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


	public AssetLoader Assets
	{
#if UNITY_EDITOR
		get { return gameObject.GetComponent<Resources>(); }
#else
		get { return gameObject.GetComponent<Bundles>(); }
#endif
	}
}
