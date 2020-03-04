

using System;
using UnityEngine;

[Serializable]
public class BundleInfo //: InspectorDraw
{
    [SerializeField] public string Name;

    [SerializeField] public ulong Size;

    [SerializeField] public string Md5;

    [SerializeField] public string Version;

    public bool mRebuild = false;
}