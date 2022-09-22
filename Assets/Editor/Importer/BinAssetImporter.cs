using System.IO;
using UnityEditor.AssetImporters;
using UnityEngine;

namespace App
{
    [UnityEditor.AssetImporters.ScriptedImporter(2, new[]{"bin"})]
    public class BinAssetImporter : UnityEditor.AssetImporters.ScriptedImporter
    {
        public override void OnImportAsset(AssetImportContext ctx)
        {
            var asset = ScriptableObject.CreateInstance<BinAsset>();
            asset.data = File.ReadAllBytes(ctx.assetPath);

            ctx.AddObjectToAsset("main obj", asset);
            ctx.SetMainObject(asset);
        }
    }
}