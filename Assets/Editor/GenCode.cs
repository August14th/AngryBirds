using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLua;

namespace CSObjectWrapEditor
{
	public static class GenCode
	{

		[LuaCallCSharp] public static List<Type> list = new List<Type>
		{
			typeof(GameObject)
		};

		[GenPath]
		public static string GenPath = Application.dataPath + "/Scripts/GenCode/";
	}
	
	
}