using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class Package
{

	private static readonly string OupPutFolder = "Bundles";

	private static uint Version = 1;

	private static readonly DirectoryInfo TargetFolder = new DirectoryInfo(Application.dataPath + "/Target");

	private static readonly DirectoryInfo LuaFolder = new DirectoryInfo(Application.dataPath + "/Lua");

	[MenuItem("Package/Encrypt")]
	static void Encrypt()
	{
		if (TargetFolder.Exists) TargetFolder.Delete(true);
		TargetFolder.Create();
		EncryptLuaFiles(LuaFolder);
		AssetDatabase.Refresh();
		Debug.Log("Encryption ok, target folder:" + TargetFolder.Name);
	}

	[MenuItem("Package/Mark")]
	static void Mark()
	{
		MarkResources("Assets/Resources");
		Debug.Log("mark Resources ok.");
		MarkLuaCodes("Assets/Target");
		Debug.Log("mark Lua codes ok.");
	}

	[MenuItem("Package/Build/Windows")]
	static void BuildWindows()
	{
		Build(BuildTarget.StandaloneWindows64);
		Debug.Log("Build " + BuildTarget.StandaloneWindows64 + " completed.");
	}

	[MenuItem("Package/Clear")]
	static void Clear()
	{
		if (TargetFolder.Exists) TargetFolder.Delete(true);
		Debug.Log("clear target folder ok.");
	}


	private static void Build(BuildTarget target)
	{
		var output = new DirectoryInfo(OupPutFolder);
		if (output.Exists) output.Delete(true);
		output.Create();
		var manifest = BuildPipeline.BuildAssetBundles(OupPutFolder, BuildAssetBundleOptions.None, target);
		var bundles = manifest.GetAllAssetBundles().ToList();
		bundles.Add("Bundles");
		var lines = new List<string>();
		foreach (var bundle in bundles)
		{
			var file = new FileInfo(output.FullName + "/" + bundle);
			var crc32 = CRC32.GetCRC32(File.ReadAllBytes(file.FullName)).ToString("X").ToLower();
			lines.Add(bundle + "." + crc32);

			file.MoveTo(file.FullName + "." + crc32);
		}

		var bundlesFile = new FileInfo("bundles.txt");
		File.WriteAllText(bundlesFile.FullName, string.Join("\n", lines.ToArray()));
	}

	private static void MarkResources(string folder)
	{
		var assets = AssetDatabase.FindAssets(null, new[] {folder});
		foreach (var asset in assets)
		{
			string assetPath = AssetDatabase.GUIDToAssetPath(asset);
			if (AssetDatabase.IsValidFolder(assetPath))
			{
				MarkResources(assetPath);
			}
			else
			{
				var relativePath = assetPath.Substring("Assets/Resources/".Length);
				string assetBundleName = relativePath.Substring(0, relativePath.LastIndexOf("."));
				var importer = AssetImporter.GetAtPath(assetPath);
				if (importer.assetBundleName != assetBundleName) importer.assetBundleName = assetBundleName;
				if (importer.assetBundleVariant != "ab") importer.assetBundleVariant = "ab";
			}
		}

		AssetDatabase.RemoveUnusedAssetBundleNames();
	}

	private static void MarkLuaCodes(string folder)
	{
		var assets = AssetDatabase.FindAssets(null, new[] {folder});
		foreach (var asset in assets)
		{
			string assetPath = AssetDatabase.GUIDToAssetPath(asset);

			var importer = AssetImporter.GetAtPath(assetPath);
			if (importer.assetBundleName != "lua") importer.assetBundleName = "lua";
			if (importer.assetBundleVariant != "ab") importer.assetBundleVariant = "ab";
			if (AssetDatabase.IsValidFolder(assetPath))
			{
				MarkLuaCodes(assetPath);
			}
		}

		AssetDatabase.RemoveUnusedAssetBundleNames();
	}

	private static void EncryptLuaFiles(DirectoryInfo sourceFolder, string path = null)
	{
		var files = sourceFolder.GetFiles("*.lua");
		foreach (var file in files)
		{
			var filePath = path == null ? file.Name : path + "." + file.Name;
			file.CopyTo(Path.Combine(TargetFolder.FullName, filePath + ".txt"));
		}

		var folders = sourceFolder.GetDirectories();
		foreach (var folder in folders)
		{
			if (!folder.Name.StartsWith("."))
			{
				var folderPath = path == null ? folder.Name : path + "." + folder.Name;
				EncryptLuaFiles(folder, folderPath);
			}
		}
	}

}
