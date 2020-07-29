using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Map : MonoBehaviour
{
	public int Index;

	public Image Lock;

	public int StarNeed;

	public Text StarNeedText;

	private Button _button;

	public GameObject LevelPanel;

	// Use this for initialization
	void Start()
	{
		var stars = PlayerPrefs.GetInt("Stars");
		if (stars < StarNeed)
		{
			StarNeedText.text = stars + "/" + StarNeed;
		}
		else
		{
			Lock.gameObject.SetActive(false);
			StarNeedText.gameObject.SetActive(false);
			
			_button = GetComponent<Button>();
			_button.onClick.AddListener(() =>
			{
				var levelPanel = Instantiate(LevelPanel, transform.root, false);
				levelPanel.GetComponent<LevelPanel>().ShowLevels(Index, 12);
			});
		}
	}
}
