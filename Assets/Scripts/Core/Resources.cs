using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Resources : AssetLoader
{

	public override GameObject NewActor(string prefabName, Vector3 position)
	{
		var prefab = UnityEngine.Resources.Load<GameObject>(prefabName);
		return Instantiate(prefab, position, Quaternion.identity);
	}

	public override GameObject NewUI(string prefabName, Transform parent)
	{
		var prefab = UnityEngine.Resources.Load<GameObject>(prefabName);
		return Instantiate(prefab, parent, false);
	}

	public override void LoadScene(string sceneName)
	{

	}

	public override byte[] Require(ref string filepath)
	{
		filepath = Application.dataPath + "/Lua/" + filepath.Replace('.', '/') + ".lua";
		if (File.Exists(filepath))
		{
			return File.ReadAllBytes(filepath);
		}

		return null;
	}

	public override bool IsDone()
	{
		return true;
	}
}
