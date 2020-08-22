using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroy : GameBehaviour {

	// Use this for initialization
	private void Awake()
	{
		DontDestroyOnLoad(gameObject);
	}

}
