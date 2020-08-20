using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyCallback : MonoBehaviour
{

	public Action Callback { get; set; }

	private void OnDestroy()
	{
		if (Callback != null) Callback();
	}
}
