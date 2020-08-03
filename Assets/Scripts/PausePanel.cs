using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PausePanel : MonoBehaviour
{

	private Animator _animator;

	public Button ResumeBtn;

	private Image _background;

	// Use this for initialization
	void Start()
	{
		_background = GetComponent<Image>();
		_animator = GetComponent<Animator>();
		ResumeBtn.onClick.AddListener(Hide);
		_animator.SetBool("Show", true);
	}

	public void Hide()
	{
		Time.timeScale = 1;
		_animator.SetBool("Show", false);
	}

	public void OnShowEnd()
	{
		Time.timeScale = 0;
	}

	public void OnHideEnd()
	{
		_background.enabled = false;
		transform.root.GetComponent<MainUI>().ShowPauseBtn();
	}
}
