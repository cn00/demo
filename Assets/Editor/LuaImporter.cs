﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Experimental.AssetImporters;
using System.IO;

[ScriptedImporter(1, new[]{"lua"})]
public class LuaImporter : ScriptedImporter
{
    const string Tag = "LuaImporter";
    public float m_Scale = 1;

    public override void OnImportAsset(AssetImportContext ctx)
    {
        AppLog.d(Tag, "OnImportAsset: " + ctx.assetPath);
        File.Copy(ctx.assetPath, ctx.assetPath + ".txt", true);
        // TODO: clean comments, compress, encode, base64 .lua.txt

        AssetDatabase.ImportAsset(ctx.assetPath + ".txt");
    }
}