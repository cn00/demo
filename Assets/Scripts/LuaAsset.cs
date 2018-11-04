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
    public string Path = "";

    public TextAsset LuaText;
    
#if UNITY_EDITOR
    Object mLuaSrc = null;
    public Object LuaSrc
    {
        get {
            return mLuaSrc;
        }
        set {
            mLuaSrc = value;

        }
    }
#endif
}
