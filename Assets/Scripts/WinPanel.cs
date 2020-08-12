using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WinPanel : GameBehaviour
{

	public Button RetryBtn;

	public Button MainBtn;

	public GameObject Star1;

	public GameObject Star2;

	public GameObject Star3;

	private int _stars;


	// Use this for initialization
	void Start()
	{
		RetryBtn.onClick.AddListener(() => GotoScene(SceneManager.GetActiveScene().name));
		MainBtn.onClick.AddListener(() => GotoScene("Main"));
	}


	public void SetStars(int stars)
	{
		_stars = stars; 
	}
	
	public IEnumerator ShowStars()
	{
		if (_stars >= 1)
		{
			Star1.SetActive(true);
		}

		if (_stars >= 2)
		{
			yield return new WaitForSeconds(0.2f);
			Star2.SetActive(true);
		}

		if (_stars >= 3)
		{
			yield return new WaitForSeconds(0.2f);
			Star3.SetActive(true);
		}
	}
	
}
