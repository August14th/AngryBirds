using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelPanel : MonoBehaviour
{
	public GameObject LevelPrefab;

	public GameObject Grid;

	public Button Return;

	private GameObject _parent;

	private void Start()
	{
		Return.onClick.AddListener(() =>
		{
			Destroy(gameObject);
			_parent.SetActive(true);
		});
	}

	public void ShowLevels(GameObject parent, int map, int levels)
	{
		_parent = parent;
		var closed = false;

		for (var i = 1; i <= levels; i++)
		{
			var levelName = map + "-" + i;
			var levelGo = Instantiate(LevelPrefab, Grid.transform, false);
			levelGo.name = levelName;
			var level = levelGo.GetComponent<Level>();
			if (!closed)
			{
				var stars = PlayerPrefs.GetInt(levelName, 0);
				level.SetStatus(map, i, true, stars);
				if (stars == 0) closed = true;
			}
			else
			{
				level.SetStatus(map, i, false, 0);
			}
		}
	}
	
	
}
