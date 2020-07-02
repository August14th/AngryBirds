using System.Collections;
using System.Collections.Generic;
using NUnit.Framework.Internal.Commands;
using UnityEngine;

public class Destroyer : MonoBehaviour {
	
	private void OnTriggerExit2D(Collider2D other)
	{
		string tag = other.gameObject.tag;
		if (tag == "Bird" || tag == "Pig" || tag == "Brick")
		{
			Destroy(other.gameObject);
		}
	}
}
