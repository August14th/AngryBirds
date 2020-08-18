using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLua;
using System.IO;

public class LuaState : MonoBehaviour
{

	private static LuaEnv LuaEnv;

	// Use this for initialization
	private void Awake()
	{
		var luaEnv = new LuaEnv();
		luaEnv.AddLoader((ref string filepath) =>
		{
			filepath = Application.dataPath + "/Lua/" + filepath.Replace('.', '/') + ".lua";
			if (File.Exists(filepath))
			{
				return File.ReadAllBytes(filepath);
			}

			return null;
		});
		luaEnv.DoString("lookup = require 'lookup'");
		LuaEnv = luaEnv;
	}

	public void DoString(string luaString)
	{
		// 为每个脚本设置一个独立的环境，可一定程度上防止脚本间全局变量、函数冲突
		var scriptEnv = LuaEnv.NewTable();
		LuaTable meta = LuaEnv.NewTable();
		meta.Set("__index", LuaEnv.Global);
		scriptEnv.SetMetaTable(meta);
		meta.Dispose();
		LuaEnv.DoString(luaString, "chunk", scriptEnv);
	}
}
