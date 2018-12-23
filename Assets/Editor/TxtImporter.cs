using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Experimental.AssetImporters;
using System.IO;

[ScriptedImporter(2, new[]{"lua", "sql", "bfbs"})]
public class TxtImporter : ScriptedImporter
{
    const string Tag = "LuaImporter";
    public override void OnImportAsset(AssetImportContext ctx)
    {
        File.Copy(ctx.assetPath, ctx.assetPath + ".txt", true);
        AppLog.d(Tag, "OnImportAsset: {0} => {1}", ctx.assetPath, ctx.assetPath + ".txt");
        // TODO: clean comments, compress, encode, base64 .lua.txt

        AssetDatabase.ImportAsset(ctx.assetPath + ".txt");
    }
}