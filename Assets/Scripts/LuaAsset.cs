using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
public class LuaAsset
{
    [SerializeField]
    public string path = "";

    [SerializeField]
    private Object textAsset = null;
    public Object Asset
    {
        get
        {
            return textAsset;
        }
        set
        {
            if(value == textAsset)
                return;
            textAsset = value;
#if UNITY_EDITOR
            path = AssetDatabase.GetAssetPath(textAsset)
                .Replace(BuildConfig.BundleResRoot, "")
                .Replace(BuildConfig.LuaExtension, "");
#endif
        }
    }
}
