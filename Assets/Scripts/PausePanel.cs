using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PausePanel : MonoBehaviour
{

	private Animator _animator;

	public Button ResumeBtn;

	private Image _background;

	public MainUI MainUI;


	// Use this for initialization
	void Start()
	{
		_background = GetComponent<Image>();
		_animator = GetComponent<Animator>();
		ResumeBtn.onClick.AddListener(Hide);
	}

	public void Show()
	{
		_background.enabled = true;
		_animator.SetBool("Show", true);
	}

	public void Hide()
	{
		_animator.SetBool("Show", false);
	}

	public void OnHideEnd()
	{
		_background.enabled = false;
		MainUI.ShowPauseBtn();
	}
}
