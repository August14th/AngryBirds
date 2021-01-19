using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditorClient : PlatformClient {

	public override int Add(int i, int j)
	{
		return i + j;
	}
}
