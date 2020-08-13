using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Map : GameBehaviour
{
	public int Index;

	public Image Lock;

	public int StarNeed;

	public Text StarNeedText;

	private string LevelPanel = "Prefab/LevelPanel";

	public void Init(int stars, GameObject parent)
	{
		if (stars < StarNeed)
		{
			StarNeedText.text = stars + "/" + StarNeed;
		}
		else
		{
			Lock.gameObject.SetActive(false);
			StarNeedText.gameObject.SetActive(false);
			
			var button = GetComponent<Button>();
			button.onClick.AddListener(() =>
			{
				parent.SetActive(false);
				var go = Assets.NewUI<LevelPanel>(LevelPanel, transform.root);
				go.Init(parent, Index, 12);
			});
		}
	}

}
