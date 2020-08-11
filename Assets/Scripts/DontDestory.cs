using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestory : GameBehaviour {

	// Use this for initialization
	private void Awake()
	{
		DontDestroyOnLoad(gameObject);
	}

}
