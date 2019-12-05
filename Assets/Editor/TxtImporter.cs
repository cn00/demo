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
        
        // var text = File.ReadAllText(ctx.assetPath);
        // var text1 = AssetDatabase.LoadAssetAtPath<TextAsset>(ctx.assetPath);
        // var asset = ScriptableObject.CreateInstance<LuaAsset>();
        // asset.Value = text;
        //
        // ctx.AddObjectToAsset("main obj", asset, LoadIconTexture());
        // ctx.SetMainObject(asset);

    }
    
    private const string k_IconName = "lua_icon";
    private Texture2D m_IconTexture;
    private Texture2D LoadIconTexture()
    {
        if (m_IconTexture == null)
        {
            var allCandidates = AssetDatabase.FindAssets(k_IconName);

            if (allCandidates.Length > 0)
            {
                m_IconTexture = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(allCandidates[0]), typeof(Texture2D)) as Texture2D;
            }
        }
        return m_IconTexture;
    }

}