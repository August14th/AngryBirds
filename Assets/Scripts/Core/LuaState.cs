using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLua;
using System.IO;
using UnityEngine.UI;

public class LuaState : GameBehaviour
{

	private LuaEnv LuaEnv;

	public int DebugPort = 9966;

	// Use this for initialization
	void Awake()
	{
		var luaEnv = new LuaEnv();
#if UNITY_EDITOR
		if (File.Exists(Directory.GetCurrentDirectory() + "/emmy_core.dll"))
		{
			luaEnv.DoString("local dbg = require('emmy_core'); " +
			                "dbg.tcpListen('localhost', " + DebugPort + ")");
			Debug.Log("lua starts, listen on port:" + DebugPort);
		}
#endif
		luaEnv.AddLoader((ref string filepath) => Assets.Require(ref filepath));

		luaEnv.DoString("G_index = require 'extends.index'");
		luaEnv.DoString("G_newindex = require 'extends.newindex'");
		luaEnv.DoString("require 'extends.extends'");
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
