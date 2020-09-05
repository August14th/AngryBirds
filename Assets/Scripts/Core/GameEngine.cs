using System.Collections;
using UnityEngine;

public class GameEngine : MonoBehaviour
{
	public string BundlesUri;

	private AssetLoader _assetLoader;

	private LuaState _luaState;

	private Scenes _scenes;

	private IEnumerator Start()
	{
#if UNITY_EDITOR
		_assetLoader = gameObject.AddComponent<LocalAsset>();
#else
		var bundles = gameObject.AddComponent<Bundles>();
		bundles.StartDownloads(BundlesUri);
		_assetLoader = bundles;
#endif
		while (!_assetLoader.IsDone())
		{
			yield return null;
		}

		_scenes = gameObject.AddComponent<Scenes>();
		_luaState = gameObject.AddComponent<LuaState>();

		_scenes.GotoScene("Main");
	}


	public AssetLoader Assets
	{
		get { return _assetLoader; }
	}

	public LuaState LuaState
	{
		get { return _luaState; }
	}

	public Scenes Scenes
	{
		get { return _scenes; }
	}
}
