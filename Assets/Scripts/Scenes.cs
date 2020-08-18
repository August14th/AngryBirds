using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class Scenes : ShortCut
{
	private float minTime = 3;
	
	public void GotoScene(string sceneName)
	{
		StartCoroutine(LoadScene(sceneName));
	}

	private IEnumerator LoadScene(string sceneName)
	{
		Assets.LoadScene(sceneName);
		var asyncOpt = SceneManager.LoadSceneAsync(sceneName);
		asyncOpt.allowSceneActivation = false;
		var loading = Assets.NewUI("Loading", Canvas.transform);
		var progressBar = loading.GetComponentInChildren<Text>();
		float duration = 0; 
		while (duration < minTime)
		{
			float wait = Random.value;
			yield return new WaitForSeconds(wait);
			duration += wait;
			float progress = Math.Min(duration / minTime, asyncOpt.progress);
			if (progress >= 0.9f)
			{
				asyncOpt.allowSceneActivation = true;
			}
			if(progressBar) progressBar.text = Convert.ToInt16(progress * 100) + "%";
		}
	}
}
