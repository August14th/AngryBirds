using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class BaseInfoPanel : GameBehaviour
{
    // Use this for initialization

    public Image Figure;

    public Text Nickname;
    
    void Start()
    {
        StartCoroutine(GetTexture());
    }

    IEnumerator GetTexture()
    {
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(Player.Figure);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            Figure.gameObject.SetActive(true);
            Texture2D figure = DownloadHandlerTexture.GetContent(www);
            Figure.sprite = Sprite.Create(figure, new Rect(0.0f, 0.0f, figure.width, figure.height),
                new Vector2(0.5f, 0.5f));
            Nickname.gameObject.SetActive(true);
            Nickname.text = Player.Nickname;
        }
    }

}