using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class Package
{
    private static readonly string BuildFolder = "Build";

    private static readonly string AssetsFolder = "Assets/Assets";

    private static readonly string TargetFolder = "Assets/Target";

    private static readonly string LuaFolder = "Assets/Lua";

    private static readonly string BundlesFolder = "Bundles";

    private static readonly string PackageFolder = "Package";

    private static uint Version = 1;

    private static readonly List<string> BuiltinBundles = new List<string> {"Loading.ab"};

    [MenuItem("Package/Encrypt && Mark", false, 10)]
    static void Prepare()
    {
        Encrypt();
        Mark();
    }

    private static void Encrypt()
    {
        var targetFolder = new DirectoryInfo(TargetFolder);
        if (targetFolder.Exists) targetFolder.Delete(true);
        targetFolder.Create();
        EncryptLuaFiles();
        AssetDatabase.Refresh();
        Debug.Log("Encryption ok, target folder:" + targetFolder.Name);
    }

    private static void Mark()
    {
        MarkAssets();
        MarkLuaCodes();
    }

    [MenuItem("Package/BuildBundles/Windows", false, 20)]
    static void BuildWindows()
    {
        BuildBundles(BuildTarget.StandaloneWindows64);
    }

    [MenuItem("Package/BuildBundles/Android", false, 21)]
    static void BuildAndroid()
    {
        BuildBundles(BuildTarget.Android);
    }

    [MenuItem("Package/BuildPackage/Windows", false, 30)]
    static void PackageWindows()
    {
        BuildPackage(BuildTarget.StandaloneWindows64);
    }

    [MenuItem("Package/BuildPackage/Android", false, 31)]
    static void PackageAndroid()
    {
        BuildPackage(BuildTarget.Android);
    }

    private static void BuildBundles(BuildTarget target)
    {
        string platform = Platform(target);

        var buildPath = BuildFolder + "/" + platform;
        var buildFolder = new DirectoryInfo(buildPath);
        if (!buildFolder.Exists) buildFolder.Create();
        var bundlesPath = BundlesFolder + "/" + platform;
        var bundlesFolder = new DirectoryInfo(bundlesPath);
        if (bundlesFolder.Exists) bundlesFolder.Delete(true);
        bundlesFolder.Create();
        var manifest = BuildPipeline.BuildAssetBundles(buildPath, BuildAssetBundleOptions.None, target);
        var bundles = manifest.GetAllAssetBundles().ToList();
        bundles.Add(platform);
        var lines = new List<string>();
        foreach (var bundleName in bundles)
        {
            var bundle = new FileInfo(buildPath + "/" + bundleName);
            var checksum = CRC32.GetCRC32(File.ReadAllBytes(bundle.FullName)).ToString("X").ToLower();
            var bundleWithChecksum = bundleName + "." + checksum;
            var targetFile = new FileInfo(bundlesPath + "/" + bundleWithChecksum);
            if (!targetFile.Directory.Exists) targetFile.Directory.Create();
            bundle.CopyTo(targetFile.FullName);
            lines.Add(JsonUtility.ToJson(new BundleInfo(bundleName, checksum)));
        }

        File.WriteAllText(bundlesFolder + "/bundles.txt", string.Join("\n", lines.ToArray()));
        Debug.Log("Build " + target + " completed.");
    }

    private static string Platform(BuildTarget target)
    {
        switch (target)
        {
            case BuildTarget.Android:
                return "android";
            case BuildTarget.StandaloneWindows64:
                return "windows";
            case BuildTarget.iOS:
                return "ios";
            default:
                throw new Exception("Not support:" + target);
        }
    }

    private static void BuildPackage(BuildTarget target)
    {
        var streamingAssetPath = Application.dataPath + "/StreamingAssets";
        var streamingAssetFolder = new DirectoryInfo(streamingAssetPath);
        if (streamingAssetFolder.Exists) streamingAssetFolder.Delete(true);
        streamingAssetFolder.Create();
        var bundlesPath = BundlesFolder + "/" + Platform(target) + "/";
        var lines = File.ReadAllLines(bundlesPath + "bundles.txt");
        var bundles = new List<BundleInfo>();
        foreach (var line in lines)
        {
            bundles.Add(JsonUtility.FromJson<BundleInfo>(line));
        }

        var builtin = new List<string>();
        if (BuiltinBundles.Contains("*"))
            foreach (var bundle in bundles)
            {
                var bundleFile = new FileInfo(bundlesPath + bundle.fileName());
                bundleFile.CopyTo(streamingAssetPath + "/" + bundle.fileName());
                builtin.Add(JsonUtility.ToJson(bundle));
            }
        else
            foreach (var bundle in BuiltinBundles)
            {
                var found = bundles.Find(f => f.name == bundle.ToLower());
                var bundleFile = new FileInfo(bundlesPath + found.fileName());
                bundleFile.CopyTo(streamingAssetPath + "/" + found.fileName());
                builtin.Add(JsonUtility.ToJson(found));
            }
        File.WriteAllText("Assets/Resources/bundles.txt", string.Join("\n", builtin.ToArray()));
        string[] levels = {"Assets/Start.unity"};
        var targetPath = PackageFolder + "/" + Platform(target) + "/AngryBirds." + Suffix(target);
        var targetFile = new FileInfo(targetPath);
        if (targetFile.Exists) targetFile.Delete();
        if (!targetFile.Directory.Exists) targetFile.Directory.Create();
        BuildPipeline.BuildPlayer(levels, targetPath, target, BuildOptions.None);
        Debug.Log("Package created, path:" + targetPath);
    }

    private static string Suffix(BuildTarget target)
    {
        switch (target)
        {
            case BuildTarget.Android:
                return "apk";
            case BuildTarget.StandaloneWindows64:
                return "exe";
            default:
                throw new Exception("Not support:" + target);
        }
    }

    private static void MarkAssets()
    {
        var assets = AssetDatabase.FindAssets(null, new[] {AssetsFolder});
        foreach (var asset in assets)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(asset);
            if (!AssetDatabase.IsValidFolder(assetPath))
            {
                var relativePath = assetPath.Substring(AssetsFolder.Length + 1);
                string assetBundleName = relativePath.Substring(0, relativePath.LastIndexOf("."));
                var importer = AssetImporter.GetAtPath(assetPath);
                if (importer.assetBundleName != assetBundleName) importer.assetBundleName = assetBundleName;
                if (importer.assetBundleVariant != "ab") importer.assetBundleVariant = "ab";
            }
        }

        AssetDatabase.RemoveUnusedAssetBundleNames();
        Debug.Log("Assets at:" + AssetsFolder + " are marked.");
    }

    private static void MarkLuaCodes()
    {
        var assets = AssetDatabase.FindAssets(null, new[] {TargetFolder});
        foreach (var asset in assets)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(asset);

            if (!AssetDatabase.IsValidFolder(assetPath))
            {
                var importer = AssetImporter.GetAtPath(assetPath);
                if (importer.assetBundleName != "lua") importer.assetBundleName = "lua";
                if (importer.assetBundleVariant != "ab") importer.assetBundleVariant = "ab";
            }
        }

        Debug.Log("lua codes at:" + TargetFolder + " are marked.");
        AssetDatabase.RemoveUnusedAssetBundleNames();
    }

    private static void EncryptLuaFiles(DirectoryInfo sourceFolder = null, string package = null)
    {
        if (sourceFolder == null) sourceFolder = new DirectoryInfo(LuaFolder);
        var files = sourceFolder.GetFiles("*.lua");
        foreach (var file in files)
        {
            var filePath = package == null ? file.Name : package + "." + file.Name;
            file.CopyTo(Path.Combine(TargetFolder, filePath + ".txt"));
        }

        var folders = sourceFolder.GetDirectories();
        foreach (var folder in folders)
        {
            if (!folder.Name.StartsWith("."))
            {
                var folderPath = package == null ? folder.Name : package + "." + folder.Name;
                EncryptLuaFiles(folder, folderPath);
            }
        }
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

    public string fileName()
    {
        return name + "." + checksum;
    }
}