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

    private string mText = "";
    public string text
    {
        get
        {
            if(string.IsNullOrEmpty(mText))
            {
#if UNITY_EDITOR
                mText = File.ReadAllText(path);
                var assetName = path;
                var fileUrl = "file://" + Application.dataPath + "/" + assetName;
                Debug.Log(fileUrl);

                WWW www = new WWW(fileUrl);
#else

#endif
            }
            return mText;
        }
    }

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
            path = AssetDatabase.GetAssetPath(textAsset).Replace("Assets/ABResources/", "");
#endif
        }
    }
}
