using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AndroidClient : PlatformClient
{
    private AndroidJavaClass _unityPlayer;

    private AndroidJavaObject _mainActivity;

    // Use this for initialization
    void Start()
    {
        _unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        _mainActivity = _unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
    }

    public override int Add(int i, int j)
    {
        return _mainActivity.Call<int>("Add", i, j);
    }

    private void OnDestroy()
    {
        _mainActivity.Dispose();
        _unityPlayer.Dispose();
    }
}