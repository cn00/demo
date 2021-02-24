namespace Helper
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class IFixAsset : ScriptableObject
    {
        public byte[] data;
#if  UNITY_EDITOR
        public Int64 magic;
        public string interfaceBridgeTypeName;
        public Type[] externTypes;
#endif
    }
}