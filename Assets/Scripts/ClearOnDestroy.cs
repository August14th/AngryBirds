using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearOnDestroy : ShortCut
{

	private void OnDestroy()
	{
		if (Bundles) Bundles.RemoveRef(gameObject);
	}
}
