using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.Networking;

public class Bundles : MonoBehaviour
{
    private static readonly string HomeURI = "http://localhost:7070/D%3A/Projects/AngryBirds/";

    private static readonly string BundleURI = HomeURI + "Bundles/";

    private readonly Dictionary<object, HashSet<object>> _ins = new Dictionary<object, HashSet<object>>();

    private readonly Dictionary<object, HashSet<AssetBundle>> _outs = new Dictionary<object, HashSet<AssetBundle>>();

    private readonly Dictionary<string, AssetBundle> _bundles = new Dictionary<string, AssetBundle>();

    private readonly Dictionary<string, string> _bundleFiles = new Dictionary<string, string>();

    private static string LocalPath
    {
        get { return Path.Combine(Application.persistentDataPath, "Bundles"); }
    }

    private void Start()
    {
        StartCoroutine(DownloadBundles());
    }

    public void AddRef(AssetBundle bundle, GameObject go)
    {
        if (bundle == null) return;
        _ins[bundle].Add(go);

        if (!_outs.ContainsKey(go))
        {
            _outs[go] = new HashSet<AssetBundle>();
        }

        _outs[go].Add(bundle);
    }

    public void RemoveBundle(string bundleName)
    {
        AssetBundle bundle;
        if(_bundles.TryGetValue(bundleName, out bundle)) RemoveRef(bundle);
    }

    public void RemoveRef(object go)
    {
        if (_ins.Remove(go))
        {
            var bundle = go as AssetBundle;
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

        HashSet<AssetBundle> outs;
        if (_outs.TryGetValue(go, out outs))
        {
            _outs.Remove(go);
            foreach (var @out in outs)
            {
                _ins[@out].Remove(go);
                if (_ins[@out].Count == 0) RemoveRef(@out);
            }
        }
    }

    public AssetBundle Get(string bundleName)
    {
        if (!_bundles.ContainsKey(bundleName))
        {
            LoadBundle(bundleName);
        }

        return _bundles[bundleName];
    }

    private AssetBundle LoadBundle(string bundleName)
    {
        string fileName;
        if(!_bundleFiles.TryGetValue(bundleName, out fileName))
            throw new Exception("bundle file of " + bundleName + " not found!");

        var bundle = AssetBundle.LoadFromFile(Path.Combine(LocalPath, fileName));
        Debug.Log("Load asset bundle:" + bundleName);
        _bundles[bundleName] = bundle;
        _ins[bundle] = new HashSet<object>();
        _outs[bundle] = new HashSet<AssetBundle>();

        var dependencies = GetDependencies(bundleName);
        foreach (var dependency in dependencies)
        {
            var dependent = Get(dependency);
            _ins[dependent].Add(bundle);
            _outs[bundle].Add(dependent);
        }

        return bundle;
    }

    private string[] GetDependencies(string bundle)
    {
        var bundles = Get("Bundles");
        var manifest = bundles.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
        return manifest.GetDirectDependencies(bundle);
    }


    private IEnumerator DownloadBundles()
    {
        var request = UnityWebRequest.Get(HomeURI + "bundles.txt");
        yield return request.SendWebRequest();

        if (request.isNetworkError || request.isHttpError)
        {
            Debug.LogError("Download bundles.txt failed:" + request.error);
        }

        var text = request.downloadHandler.text;

        request.Dispose();

        var remotes = new HashSet<string>(text.Split('\n').ToList());
        var localFolder = new DirectoryInfo(LocalPath);
        var locals = localFolder.GetFiles("*", SearchOption.AllDirectories).ToList();

        for (int i = locals.Count - 1; i >= 0; i--)
        {
            var relativePath = locals[i].FullName.Substring(LocalPath.Length + 1);
            var file = relativePath.Replace(Path.DirectorySeparatorChar.ToString(), "/");
            if (remotes.Contains(file))
            {
                locals.RemoveAt(i);
                remotes.Remove(file);
                string bundle, crc32;
                Parse(file, out bundle, out crc32);
                _bundleFiles[bundle] = file;
            }
        }

        locals.ForEach(d => d.Delete());

        foreach (var remote in remotes)
        {
            yield return DownloadBundle(remote);
        }

        Debug.Log("All Bundles are downloaded.");
    }

    private void Parse(string file, out string bundle, out string crc32)
    {
        var sep = file.LastIndexOf('.');
        bundle = file.Substring(0, sep);
        crc32 = file.Substring(sep + 1);
    }


    private IEnumerator DownloadBundle(string file)
    {
        string bundle, crc32String;
        Parse(file, out bundle, out crc32String);

        var localFile = new FileInfo(Path.Combine(LocalPath, file));
        if (localFile.Exists) localFile.Delete();
        var folder = localFile.Directory;
        if (folder != null && !folder.Exists) folder.Create();

        var request = UnityWebRequest.Get(BundleURI + file);
        var tmpFile = new FileInfo(localFile.FullName + ".tmp");
        request.downloadHandler = new DownloadHandlerFile(tmpFile.FullName);
        yield return request.SendWebRequest();

        if (request.isNetworkError || request.isHttpError)
        {
            if (tmpFile.Exists) tmpFile.Delete();
            Debug.LogError("Download Bundle:" + file + " failed:" + request.error);
        }
        else
        {
            var crc32 = CRC32.GetCRC32(File.ReadAllBytes(tmpFile.FullName)).ToString("X").ToLower();
            if (crc32 == crc32String)
            {
                tmpFile.MoveTo(localFile.FullName);
                _bundleFiles[bundle] = file;
                Debug.Log("Download Bundle:" + bundle + " ok");
            }
            else tmpFile.Delete();

        }

        request.Dispose();
    }

}