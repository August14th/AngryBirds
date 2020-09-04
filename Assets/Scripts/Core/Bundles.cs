using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Object = UnityEngine.Object;

public class Bundles : AssetLoader
{
    #if UNITY_ANDROID
    private string platform = "android";
    #endif
    #if UNITY_IPHONE
        private string platform = "ios";
    #endif
    #if UNITY_STANDALONE_WIN
        private string platform = "windows";
    #endif
    private string _bundleUri;

    private readonly Dictionary<string, AssetBundle> _bundles = new Dictionary<string, AssetBundle>();

    private readonly Dictionary<string, BundleInfo> _bundleInfos = new Dictionary<string, BundleInfo>();

    private static string LocalPath
    {
        get { return Path.Combine(Application.persistentDataPath, "Bundles"); }
    }

    private bool _done;

    public void StartDownloads(string bundlesPath)
    {
        _bundleUri = bundlesPath + platform + "/";
        StartCoroutine(DownloadBundles());
    }

    private void RemoveBundle(string bundleName)
    {
        AssetBundle bundle;
        if (_bundles.TryGetValue(bundleName, out bundle)) RemoveRef(bundle);
    }

    protected override void Unload(Object asset)
    {
        var bundle = asset as AssetBundle;
        if (bundle != null)
        {
            foreach (var ab in _bundles)
            {
                if (ab.Value == bundle)
                {
                    _bundles.Remove(ab.Key);
                    bundle.Unload(true);
                    Debug.Log("Unload asset bundle:" + ab.Key);
                    break;
                }
            }
        }
    }
    
    private AssetBundle Get(string bundleName)
    {
        if (!_bundles.ContainsKey(bundleName))
        {
            LoadBundle(bundleName);
        }

        return _bundles[bundleName];
    }

    private void LoadBundle(string bundleName)
    {
        BundleInfo info;
        if (!_bundleInfos.TryGetValue(bundleName, out info))
            throw new Exception("bundle file of " + bundleName + " not found!");

        var bundle = AssetBundle.LoadFromFile(info.path);
        Debug.Log("Load asset bundle:" + bundleName);
        _bundles[bundleName] = bundle;

        var dependencies = GetDependencies(bundleName);
        foreach (var dependency in dependencies)
        {
            var dependent = Get(dependency);
            AddRef(bundle, dependent);
        }
    }

    private string[] GetDependencies(string bundle)
    {
        var bundles = Get(platform);
        var manifest = bundles.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
        return manifest.GetDirectDependencies(bundle);
    }


    private IEnumerator DownloadBundles()
    {
        var request = UnityWebRequest.Get(_bundleUri + "bundles.txt");
        yield return request.SendWebRequest();

        if (request.isNetworkError || request.isHttpError)
        {
            Debug.LogError("Download bundles.txt failed:" + request.error);
            yield break;
        }

        var text = request.downloadHandler.text;

        request.Dispose();
        var jsonList = new HashSet<string>(text.Split('\n').ToList());
        var remotes = new List<BundleInfo>();
        foreach (var json in jsonList)
        {
            var bundleInfo = JsonUtility.FromJson<BundleInfo>(json);
            remotes.Add(bundleInfo);
        }
        // 本地的已经有的bundles
        var localFolder = new DirectoryInfo(LocalPath);
        if (!localFolder.Exists) localFolder.Create();
        var locals = localFolder.GetFiles("*", SearchOption.AllDirectories).ToList();

        for (int i = locals.Count - 1; i >= 0; i--)
        {
            var relativePath = locals[i].FullName.Substring(LocalPath.Length + 1);
            var file = relativePath.Replace(Path.DirectorySeparatorChar.ToString(), "/");
            var found = remotes.Find(b => b.fileName() == file);
            if (found != null)
            {
                found.path = locals[i].FullName;
                locals.RemoveAt(i);
                remotes.Remove(found);
                _bundleInfos[found.name] = found;
            }
        }

        // 删除已经过期的bundles
        locals.ForEach(d => d.Delete());
        // 存在于streamingAssets中的bundles
        var bundlesText = Resources.Load<TextAsset>("bundles").text;
        var builtInBundles = new Dictionary<string, BundleInfo>();
        foreach (string line in bundlesText.Split('\n'))
        {
            var bundle = JsonUtility.FromJson<BundleInfo>(line);
            bundle.path = Application.streamingAssetsPath + "/" + bundle.fileName();
            builtInBundles[bundle.name] = bundle;
        }

        foreach (var remote in remotes)
        {
            BundleInfo builtInBundle;
            if (builtInBundles.TryGetValue(remote.name, out builtInBundle) &&
                builtInBundle.checksum == remote.checksum)
            {
                _bundleInfos[builtInBundle.name] = builtInBundle;
            }
            else
            {
                // 下载缺失的AssetBundles
                yield return DownloadBundle(remote);
            }
        }
        _done = true;
        Debug.Log("All Bundles are downloaded.");
    }

    private IEnumerator DownloadBundle(BundleInfo info)
    {
        var localFile = new FileInfo(Path.Combine(LocalPath, info.fileName()));
        if (localFile.Exists) localFile.Delete();
        var folder = localFile.Directory;
        if (folder != null && !folder.Exists) folder.Create();

        var request = UnityWebRequest.Get(_bundleUri + info.fileName());
        var tmpFile = new FileInfo(localFile.FullName + ".tmp");
        request.downloadHandler = new DownloadHandlerFile(tmpFile.FullName);
        yield return request.SendWebRequest();

        if (request.isNetworkError || request.isHttpError)
        {
            if (tmpFile.Exists) tmpFile.Delete();
            Debug.LogError("Download Bundle:" + info.fileName() + " failed:" + request.error);
        }
        else
        {
            var crc32 = CRC32.GetCRC32(File.ReadAllBytes(tmpFile.FullName)).ToString("X").ToLower();
            if (crc32 == info.checksum)
            {
                tmpFile.MoveTo(localFile.FullName);
                info.path = localFile.FullName;
                _bundleInfos[info.name] = info;
                Debug.Log("Download Bundle:" + info.name + " ok");
            }
            else tmpFile.Delete();

        }

        request.Dispose();
    }

    public override byte[] Require(ref string file)
    {
        var bundleName = "lua.ab";
        var bundle = Get(bundleName);
        if (bundle == null) return null;
        var luaFile = bundle.LoadAsset<TextAsset>(file + ".lua.txt");
        if (luaFile == null)
        {
            Debug.LogWarning("lua file not found:" + file);
            return null;
        }

        return luaFile.bytes;
    }

    public override GameObject NewActor(string prefabName, Vector3 position)
    {
        AssetBundle bundle;
        var prefab = LoadPrefab(prefabName, out bundle);
        if (prefab != null)
        {
            var actor = Instantiate(prefab, position, Quaternion.identity);

            var t = actor.AddComponent<DestroyCallback>();
            t.Callback = () => RemoveRef(t.gameObject);

            AddRef(actor, bundle);
            return actor;
        }

        return null;
    }


    public override GameObject NewUI(string prefabName, Transform parent)
    {
        AssetBundle bundle;
        var prefab = LoadPrefab(prefabName, out bundle);
        if (prefab != null)
        {
            var ui = Instantiate(prefab, parent.transform, false);

            var t = ui.AddComponent<DestroyCallback>();
            t.Callback = () => RemoveRef(t.gameObject);
            AddRef(ui, bundle);
            return ui;
        }

        return null;
    }

    public override void LoadScene(string sceneName)
    {
        var currentBundleName = SceneBundleName(SceneManager.GetActiveScene().name);
        RemoveBundle(currentBundleName);

        var bundleName = SceneBundleName(sceneName);
        Get(bundleName);
    }

    private string SceneBundleName(string sceneName)
    {
        return "scenes/" + sceneName.ToLower() + ".ab";
    }

    private GameObject LoadPrefab(string prefabName, out AssetBundle bundle)
    {
        var bundleName = prefabName.ToLower() + ".ab";
        bundle = Get(bundleName);
        if (bundle == null) return null;
        var assetName = prefabName.Substring(prefabName.LastIndexOf("/") + 1);
        var prefab = bundle.LoadAsset<GameObject>(assetName);
        return prefab;
    }

    public override void SetSprite(Image image, string atlasPath, string spriteName)
    {
        var bundleName = atlasPath.ToLower() + ".ab";
        var bundle = Get(bundleName);
        if (bundle != null)
        {
            foreach (var sprite in bundle.LoadAllAssets<Sprite>())
            {
                if (sprite.name == spriteName)
                {
                    AddRef(bundle, image.gameObject);
                    var t = image.gameObject.AddComponent<DestroyCallback>();
                    t.Callback = () => RemoveRef(t.gameObject);
                    image.sprite = sprite;
                    break;
                }
            }
        }
    }

    public override bool IsDone()
    {
        return _done;
    }

}

class BundleInfo
{
    public BundleInfo(string name, string checksum)
    {
        this.name = name;
        this.checksum = checksum;
    }

    public string name;

    public string checksum;

    public string path;

    public string fileName()
    {
        return name + "." + checksum;
    }
}