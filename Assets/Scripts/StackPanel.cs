using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StackPanel : MonoBehaviour
{

	private static readonly Stack<StackPanel> Panels = new Stack<StackPanel>();

	void Awake()
	{
		if (Panels.Count != 0)
		{
			Panels.Peek().gameObject.SetActive(false);
		}

		Panels.Push(this);
	}


	void OnDestroy()
	{
		Panels.Pop();
		if (Panels.Count != 0)	
		{
			Panels.Peek().gameObject.SetActive(true);
		}
	}
}
