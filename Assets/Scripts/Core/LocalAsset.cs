#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class LocalAsset : AssetLoader
{
    private string PrefabPath(string prefabName)
    {
        return "Assets/Assets/" + prefabName + ".prefab";
    }

    public override GameObject NewActor(string prefabName, Vector3 position)
    {
        var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath(prefabName));
        var actor = Instantiate(prefab, position, Quaternion.identity);

        return actor;
    }

    public override GameObject NewUI(string prefabName, Transform parent)
    {
        var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath(prefabName));
        var ui = Instantiate(prefab, parent, false);

        return ui;
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


    public override void SetSprite(Image image, string atlasPath, string spriteName)
    {
        var objects = AssetDatabase.LoadAllAssetsAtPath(atlasPath);
        foreach (var o in objects)
        {
            var sprite = o as Sprite;
            if (sprite != null && sprite.name == spriteName)
            {
                image.sprite = sprite;
                break;
            }
        }
    }

    protected override void Unload(Object asset)
    {

    }

    public override bool IsDone()
    {
        return true;
    }
}
#endif