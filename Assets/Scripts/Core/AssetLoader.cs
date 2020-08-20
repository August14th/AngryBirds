
using UnityEngine;
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
}
