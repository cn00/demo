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
        var prefax = Path.GetExtension(ctx.assetPath).Substring(1);
        var text = File.ReadAllText(ctx.assetPath);
        var asset = new TextAsset(text);
        ctx.AddObjectToAsset("main obj", asset, LoadIconTexture(prefax));
        ctx.SetMainObject(asset);
    }
    
    private Texture2D LoadIconTexture(string prefax)
    {
        return AssetDatabase.LoadAssetAtPath("Assets/Editor/fileicon/" + prefax + ".png", typeof(Texture2D)) as Texture2D;
    }

}