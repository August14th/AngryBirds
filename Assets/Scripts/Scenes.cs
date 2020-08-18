using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using UnityEngine.SceneManagement;

public class Scenes : ShortCut
{

	public void GotoScene(string sceneName)
	{
		StartCoroutine(LoadScene(sceneName));
	}

	private IEnumerator LoadScene(string sceneName)
	{
		Assets.LoadScene(sceneName);
		var asyncOpt = SceneManager.LoadSceneAsync(sceneName);
		var loading = Assets.NewUI("Loading", Canvas.transform);
		var progress = loading.GetComponentInChildren<Text>();
		while (!asyncOpt.isDone)
		{
			progress.text = asyncOpt.progress * 100 + "%";
			yield return null;
		}
	}
}
