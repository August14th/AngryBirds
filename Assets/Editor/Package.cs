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

    private static uint Version = 1;

    private static readonly List<string> BundlesInPackage = new List<string> {"Loading"};

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

    private static void BuildBundles(BuildTarget target)
    {
        string platform;
        switch (target)
        {
            case BuildTarget.Android:
                platform = "android";
                break;
            case BuildTarget.StandaloneWindows64:
                platform = "pc";
                break;
            default: 
                throw new Exception("Not support:" + target);
        }
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

        File.WriteAllText(bundlesFolder + "/bundles.json", string.Join("\n", lines.ToArray()));
        Debug.Log("Build " + target + " completed.");
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
}