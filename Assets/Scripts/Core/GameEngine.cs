﻿using System.Collections;
using UnityEngine;

public class GameEngine : MonoBehaviour
{
	public string BundlesUri;

	private AssetLoader _assetLoader;

	private LuaState _luaState;

	private Scenes _scenes;

	private PlatformClient _client;

	private IEnumerator Start()
	{
#if UNITY_EDITOR
		_assetLoader = gameObject.AddComponent<LocalAsset>();
		_client = gameObject.AddComponent<EditorClient>();
#elif UNITY_ANDROID
		var bundles = gameObject.AddComponent<Bundles>();
		bundles.StartDownloads(BundlesUri);
		_assetLoader = bundles;
		_client = gameObject.AddComponent<AndroidClient>(); 
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

	public PlatformClient Client
	{
		get { return _client; }
	}
}
