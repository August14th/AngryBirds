using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Levels : MonoBehaviour
{
	public GameObject Level;

	public void ShowLevels(int map, int levels)
	{
		gameObject.SetActive(true);
		
		var closed = false;

		for (var i = 1; i <= levels; i++)
		{
			var level = map + "-" + i;
			var levelGo = Instantiate(Level, transform, false);
			levelGo.name = level;
			var levelCom = levelGo.GetComponent<Level>();
			if (!closed)
			{
				var stars = PlayerPrefs.GetInt(level, 0);
				levelCom.SetStatus(i, true, stars);
				if (stars == 0) closed = true;
			}
			else
			{
				levelCom.SetStatus(i, false, 0);
			}
		}
	}
}
