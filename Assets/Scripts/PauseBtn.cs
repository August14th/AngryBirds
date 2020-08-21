using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseBtn : GameBehaviour
{

	private bool _isPause;
	
	private Image _image;

	// Use this for initialization
	void Start ()
	{
		_image = GetComponent<Image>();
		GetComponent<Button>().onClick.AddListener(() =>
		{
			if (!_isPause)
			{
				_isPause = true;
				_image.SetSprite("Image/BUTTONS", "BUTTONS_8");
				Time.timeScale = 0;
			}
			else
			{
				_isPause = false;
				_image.SetSprite("Image/BUTTONS", "BUTTONS_1");
				Time.timeScale = 1;
			}
		});			
	}

}
