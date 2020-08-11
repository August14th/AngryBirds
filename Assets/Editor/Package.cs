using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using UnityEditor;
using UnityEngine;

public class Package
{

	private static readonly string OupPutFolder = "Bundles";
	
	[MenuItem("Package/Mark")]
	static void Mark()
	{
		MarkAssetBundleLabels("Assets/Resources");
	}

	private static uint Version = 1;

	[MenuItem("Package/Build/Android")]
	static void BuildAndroid()
	{
		var output = new DirectoryInfo(OupPutFolder);
		if (output.Exists) output.Delete(true);
		output.Create();
		var manifest = BuildPipeline.BuildAssetBundles(OupPutFolder, BuildAssetBundleOptions.None, BuildTarget.Android);
		var bundles = manifest.GetAllAssetBundles().ToList();
		bundles.Add("Bundles");
		var lines = new List<string>();
		using (var md5 = MD5.Create())
		{
			foreach (var bundle in bundles)
			{
				var file = new FileInfo(output.FullName + "/" + bundle);
				string md5String;
				using (var stream = file.Open(FileMode.Open))
				{
					var md5Bytes = md5.ComputeHash(stream);
					md5String = BitConverter.ToString(md5Bytes).Replace("-", string.Empty).ToLower();
					lines.Add(bundle + "." + md5String);
				}

				file.MoveTo(file.FullName + "." + md5String);
			}
		}

		var bundlesFile = new FileInfo("bundles.txt");
		File.WriteAllText(bundlesFile.FullName, string.Join("\n", lines.ToArray()));

		Debug.Log("BuildAndroid completed.");
	}

	private static void MarkAssetBundleLabels(string folder)
	{
		var assets = AssetDatabase.FindAssets(null, new[] {folder});
		foreach (var asset in assets)
		{
			string assetPath = AssetDatabase.GUIDToAssetPath(asset);
			if (AssetDatabase.IsValidFolder(assetPath))
			{
				MarkAssetBundleLabels(assetPath);
			}
			else
			{
				var relativePath = assetPath.Substring("Assets/Resources/".Length);
				string assetBundleName = relativePath.Substring(0, relativePath.LastIndexOf("."));
				var importer = AssetImporter.GetAtPath(assetPath);
				importer.SetAssetBundleNameAndVariant(assetBundleName, "ab");
			}
		}
		AssetDatabase.RemoveUnusedAssetBundleNames();
		Debug.Log(folder + ": Mark AssetBundle Labels ok.");
	}
}
