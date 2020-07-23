using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WinPanel : MonoBehaviour
{

	public Button RetryBtn;

	public GameObject Star1;

	public GameObject Star2;

	public GameObject Star3;

	private int _stars;


	// Use this for initialization
	void Start () {
	
		RetryBtn.onClick.AddListener(Retry);
		
	}


	private void Retry()
	{
		SceneManager.LoadScene(0);
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
