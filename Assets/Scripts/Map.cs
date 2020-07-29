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

	public void SetStatus(int stars, GameObject levelPanel, GameObject parent)
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
				var go = Instantiate(levelPanel, transform.root, false);
				go.GetComponent<LevelPanel>().ShowLevels(parent, Index, 12);
			});
		}
	}

}
