using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Map : MonoBehaviour
{
	public int Index;

	public Image Lock;

	public int StarNeed;
	
	public Levels Levels;

	public Text StarNeedText;

	private Button _button;

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
				transform.parent.gameObject.SetActive(false);
				Levels.ShowLevels(Index, 12);
			});
		}
	}
}
