using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShortCut : MonoBehaviour {

	private static GameObject _engine;

	private GameObject GetEngine()
	{
		if (_engine == null) _engine = GameObject.FindWithTag("GameEngine");
		return _engine;
	}

	protected Assets Assets
	{
		get { return GetEngine().GetComponent<Assets>(); }
	}
    
	protected GameObject Canvas
	{
		get { return GameObject.Find("Canvas"); }
	}
}
