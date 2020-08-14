using UnityEngine;
using XLua;
using System;
using System.IO;
using UnityEditor;

[Serializable]
public class Injection
{
    public string Name;
    public GameObject Value;
}

[LuaCallCSharp]
public class LuaBehaviour : MonoBehaviour
{
    public TextAsset LuaScript;
    public Injection[] Injections;

    //all lua behaviour shared one luaEnv only!
    private static readonly LuaEnv LuaEnv = NewLuaEnv();
    private static float _lastGcTime;
    private const float GcInterval = 1; //1 second 

    private Action _luaStart;
    private Action _luaUpdate;
    private Action _luaOnDestroy;

    private LuaTable _scriptEnv;

    private void Awake()
    {
        _scriptEnv = LuaEnv.NewTable();

        // 为每个脚本设置一个独立的环境，可一定程度上防止脚本间全局变量、函数冲突
        LuaTable meta = LuaEnv.NewTable();
        meta.Set("__index", LuaEnv.Global);
        _scriptEnv.SetMetaTable(meta);
        meta.Dispose();

        _scriptEnv.Set("self", this);
        foreach (var injection in Injections)
        {
            _scriptEnv.Set(injection.Name, injection.Value);
        }

        LuaEnv.DoString(LuaScript.text, LuaScript.name, _scriptEnv);

        var luaAwake = _scriptEnv.Get<Action>("awake");
        _scriptEnv.Get("start", out _luaStart);
        _scriptEnv.Get("update", out _luaUpdate);
        _scriptEnv.Get("ondestroy", out _luaOnDestroy);

        if (luaAwake != null)
        {
            luaAwake();
        }
    }

    // Use this for initialization
    private void Start()
    {
        if (_luaStart != null) _luaStart();
    }

    // Update is called once per frame
    private void Update()
    {
        if (_luaUpdate != null) _luaUpdate();

        if (Time.time - _lastGcTime > GcInterval)
        {
            LuaEnv.Tick();
            _lastGcTime = Time.time;
        }
    }

    private void OnDestroy()
    {
        if (_luaOnDestroy != null) _luaOnDestroy();

        _luaOnDestroy = null;
        _luaUpdate = null;
        _luaStart = null;
        _scriptEnv.Dispose();
        Injections = null;
    }

    private static LuaEnv NewLuaEnv()
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
        return luaEnv;
    }
}

