using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public abstract class AssetLoader : MonoBehaviour
{
    public T NewActor<T>(string prefabName, Vector3 position)
    {
        var actor = NewActor(prefabName, position);
        return actor.GetComponent<T>();
    }

    public T NewUI<T>(string prefabName, Transform parent)
    {
        var ui = NewUI(prefabName, parent);
        return ui.GetComponent<T>();
    }

    public abstract GameObject NewActor(string prefabName, Vector3 position);

    public abstract GameObject NewUI(string prefabName, Transform parent);
    
    public abstract void LoadScene(string sceneName);

    public abstract byte[] Require(ref string filepath);

    public abstract void SetSprite(Image image, string atlasPath, string spriteName);

    public abstract bool IsDone();

    private readonly Dictionary<Object, HashSet<Object>> _ins = new Dictionary<Object, HashSet<Object>>();

    private readonly Dictionary<Object, HashSet<Object>> _outs = new Dictionary<Object, HashSet<Object>>();


    protected void AddRef(Object go, Object dependOn)
    {
        Assert.IsTrue(go != null);
        Assert.IsTrue(dependOn != null);

        if (!_ins.ContainsKey(dependOn))
        {
            _ins[dependOn] = new HashSet<Object>();
        }

        _ins[dependOn].Add(go);

        if (!_outs.ContainsKey(go))
        {
            _outs[go] = new HashSet<Object>();
        }

        _outs[go].Add(dependOn);
    }


    protected void TryUnload(Object go)
    {
        HashSet<Object> ins;
        if (_ins.TryGetValue(go, out ins))
        {
            if (ins.Count == 0)
            {
                _ins.Remove(go);
                Unload(go);
            } else return;
        }

        HashSet<Object> outs;
        if (_outs.TryGetValue(go, out outs))
        {
            _outs.Remove(go);
            foreach (var @out in outs)
            {
                _ins[@out].Remove(go);
                TryUnload(@out);
            }
        }
    }

    protected abstract void Unload(Object asset);
}