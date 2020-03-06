

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public class BundleInfo //: InspectorDraw
{
    public string Name;

    public ulong Size;

    public string Hash;

    public string Version;

    public bool Rebuild;
}

public class AppBundleManifest : SingletonAsset<AppBundleManifest> 
{
    public List<BundleInfo> BundleInfos;
}