using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLua;

[LuaCallCSharp]
public static class GameObjectX
{

    public static Component com(this GameObject go, string name)
    {
        return go.GetComponent(name);
    }

    public static GameObject child(this GameObject go, string path)
    {
        var children = go.children(path);
        if (children.Count == 0) return null;

        return children[0];
    }

    public static List<GameObject> children(this GameObject go, string path)
    {
        var levels = path.Split('.');
        var children = new List<GameObject> {go};
        foreach (var level in levels)
        {
            var tmp = children;
            children = new List<GameObject>();
            foreach (var child in tmp)
            {
                children.AddRange(child.children0(level));
            }
        }

        return children;
    }

    private static List<GameObject> children0(this GameObject go, string childName)
    {
        var children = new List<GameObject>();
        foreach (Transform child in go.transform)
        {
            if (child.name == childName)
                children.Add(child.gameObject);
        }

        return children;
    }

}
