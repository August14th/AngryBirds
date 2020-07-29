using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapPanel : MonoBehaviour
{
	public List<Map> Maps;

	public GameObject LevelPanel;

	void Start()
	{
		Maps.ForEach(map =>
		{
			var stars = PlayerPrefs.GetInt("Stars");
			if (stars < map.StarNeed)
			{
				map.StarNeedText.text = stars + "/" + map.StarNeed;
			}
			else
			{
				map.Lock.gameObject.SetActive(false);
				map.StarNeedText.gameObject.SetActive(false);
			
				var button = map.GetComponent<Button>();
				button.onClick.AddListener(() =>
				{
					gameObject.SetActive(false);
					var levelPanel = Instantiate(LevelPanel, transform.root, false);
					levelPanel.GetComponent<LevelPanel>().ShowLevels(gameObject, map.Index, 12);
				});
			}
		});
		
	}
}
