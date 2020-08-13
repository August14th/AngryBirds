using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapPanel : MonoBehaviour
{
	public List<Map> Maps;

	void Start()
	{
		var stars = PlayerPrefs.GetInt("Stars");
		Maps.ForEach(map =>
		{
			map.Init(stars, gameObject);
		});
		
	}
}
