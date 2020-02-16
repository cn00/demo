using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Serialization;
#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
public class LuaAsset : ScriptableObject
{
    public string Path = "";
    public string Value;
    public TextAsset TextAsset;

//#if UNITY_EDITOR
    public UnityEngine.Object mAsset;
//#endif
}
