using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Experimental.AssetImporters;
using System.IO;

namespace App
{

public class AssetImporter : UnityEditor.AssetPostprocessor
{
    const string Tag = "AssetImporter";

    //get a notification just before any Asset is imported.
    void OnPreprocessAsset()
    {
        if (assetImporter)
        {
            ModelImporter modelImporter = assetImporter as ModelImporter;
            if (modelImporter != null)
            {
                if (!assetPath.Contains("@"))
                    modelImporter.importAnimation = false;
            }
        }
    }

    //This is called after importing of any number of assets is complete (when the Assets progress bar has reached the end).
    static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    {
        // foreach (string str in importedAssets)
        // {
        //     Debug.Log("Reimported Asset: " + str);
        // }
        foreach (string str in deletedAssets)
        {
            UnityEngine.Debug.Log("Deleted Asset: " + str);
        }

        for (int i = 0; i < movedAssets.Length; i++)
        {
            UnityEngine.Debug.Log(movedFromAssetPaths[i] + " => " + movedAssets[i]);
        }
    }
}    
}