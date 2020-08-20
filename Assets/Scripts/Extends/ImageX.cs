using UnityEngine;
using UnityEngine.UI;

public static class ImageX
{
    public static void SetSprite(this Image image, string atlasPath, string spriteName)
    {
        var loader = GameBehaviour.Assets;
        loader.SetSprite(image, atlasPath, spriteName);
    }

}