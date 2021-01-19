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
    public Text Text;

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

    private void Awake()
    {
        Text = GameObject.Find("DlText").GetComponent<Text>();
    }

    public void StartDownloads(string bundlesPath)
    {
        _bundleUri = bundlesPath + platform + "/";
        StartCoroutine(UpdateBundles());
    }

    private void RemoveBundle(string bundleName)
    {
        AssetBundle bundle;
        if (_bundles.TryGetValue(bundleName, out bundle)) TryUnload(bundle);
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

    private Dictionary<string, BundleInfo> GetLocalBundles()
    {
        // 存在于persistentDataPath的bundles
        var localFolder = new DirectoryInfo(LocalPath);
        if (!localFolder.Exists) localFolder.Create();
        var localFiles = localFolder.GetFiles("*", SearchOption.AllDirectories).ToList();
        var locals = new Dictionary<string, BundleInfo>();
        foreach (var file in localFiles)
        {
            var relativePath = file.FullName.Substring(LocalPath.Length + 1);
            relativePath = relativePath.Replace(Path.DirectorySeparatorChar.ToString(), "/");
            string name, checksum;
            Parse(relativePath, out name, out checksum);
            var bundle = new BundleInfo
            {
                name = name, checksum = checksum, path = file.FullName
            };
            locals[bundle.name] = bundle;
        }

        // 存在于streamingAssets中的bundles
        var bundlesText = Resources.Load<TextAsset>("bundles").text;
        foreach (var json in bundlesText.Split('\n'))
        {
            var bundleInfo = JsonUtility.FromJson<BundleInfo>(json);
            bundleInfo.path = Application.streamingAssetsPath + "/" + bundleInfo.fileName();
            bundleInfo.builtin = true;
            if (!locals.ContainsKey(bundleInfo.name))
            {
                locals[bundleInfo.name] = bundleInfo;
            }
        }

        return locals;
    }

    private IEnumerator UpdateBundles()
    {
        Text.text = "正在连接资源服务器...";
        var locals = GetLocalBundles();

        var request = UnityWebRequest.Get(_bundleUri + "bundles.txt");
        request.timeout = 3;
        yield return request.SendWebRequest();

        if (request.isNetworkError || request.isHttpError)
        {
            Debug.LogError("Download bundles.txt failed:" + request.error);
            request.Dispose();
            foreach (var bundle in locals)
            {
                _bundleInfos[bundle.Key] = bundle.Value;
            }

            _done = true;
            Text.text = "网络异常，使用本地资源。";
            yield return new WaitForSeconds(2f);
            yield break;
        }

        var text = request.downloadHandler.text;
        request.Dispose();
        //  最新的bundles列表
        var remotes = new List<BundleInfo>();
        foreach (var json in text.Split('\n'))
        {
            var bundleInfo = JsonUtility.FromJson<BundleInfo>(json);
            remotes.Add(bundleInfo);
        }

        // 计算本地和最新的差异，删除过期的，下载缺失的
        var misses = new List<BundleInfo>();
        foreach (var remote in remotes)
        {
            BundleInfo bundle;
            if (locals.TryGetValue(remote.name, out bundle) &&
                bundle.checksum == remote.checksum)
            {
                _bundleInfos[bundle.name] = bundle;
                locals.Remove(remote.name);
            }
            else
            {
                misses.Add(remote);
            }
        }

        foreach (var bundle in locals.Values)
        {
            if (!bundle.builtin) File.Delete(bundle.path);
        }

        var notice = "正在下载资源...";
        var total = misses.Count;
        var current = 0;
        foreach (var bundle in misses)
        {
            yield return DownloadBundle(bundle);
            Text.text = notice + ++current + "/" + total;
            yield return new WaitForSeconds(0.1f);
        }

        _done = true;
        Debug.Log("All Bundles are downloaded.");
    }

    private void Parse(string file, out string bundle, out string crc32)
    {
        var sep = file.LastIndexOf('.');
        bundle = file.Substring(0, sep);
        crc32 = file.Substring(sep + 1);
    }

    private IEnumerator DownloadBundle(BundleInfo bundle)
    {
        var localFile = new FileInfo(Path.Combine(LocalPath, bundle.fileName()));
        if (localFile.Exists) localFile.Delete();
        var folder = localFile.Directory;
        if (folder != null && !folder.Exists) folder.Create();

        var request = UnityWebRequest.Get(_bundleUri + bundle.fileName());
        var tmpFile = new FileInfo(localFile.FullName + ".tmp");
        request.downloadHandler = new DownloadHandlerFile(tmpFile.FullName);
        yield return request.SendWebRequest();

        if (request.isNetworkError || request.isHttpError)
        {
            if (tmpFile.Exists) tmpFile.Delete();
            throw new Exception("Download Bundle:" + bundle.fileName() + " failed:" + request.error);
        }
        request.Dispose();
        
        var crc32 = CRC32.GetCRC32(File.ReadAllBytes(tmpFile.FullName)).ToString("X").ToLower();
        if (crc32 == bundle.checksum)
        {
            tmpFile.MoveTo(localFile.FullName);
            bundle.path = localFile.FullName;
            _bundleInfos[bundle.name] = bundle;
            Debug.Log("Download Bundle:" + bundle.name + " ok");
        }
        else
        {
            tmpFile.Delete();
            throw new Exception("Checksum Bundle:" + bundle.fileName() + " failed, expected: " +
                                bundle.checksum + ", but got:" + crc32);
        }
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
            t.Callback = () => TryUnload(t.gameObject);

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
            t.Callback = () => TryUnload(t.gameObject);
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
                    AddRef(image.gameObject, bundle);
                    var t = image.gameObject.AddComponent<DestroyCallback>();
                    t.Callback = () => TryUnload(t.gameObject);
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
    public string name;

    public string checksum;

    public string path;

    public bool builtin;

    public string fileName()
    {
        return name + "." + checksum;
    }
}