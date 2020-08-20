using UnityEngine;
using UnityEngine.UI;

public static class ImageX
{
    public static void SetSprite(this Image image, string atlasPath, string spriteName)
    {
        var Assets = GameBehaviour.Assets;
        Assets.SetSprite(image, atlasPath, spriteName);
    }

}