using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using System;
using System.Collections;

public class DefaultAssetConfig : SingletonAsset<DefaultAssetConfig>
{
    [Serializable]
    public class Config : InspectorDraw
    {
        public int MaxTextPreviewLength = 7000;
        public int MaxSheetPreviewLength = 10;
    }

    [SerializeField]
	[HideInInspector]
    public Config MConfig = new Config(){ Name= "Config"};

    [MenuItem("Tools/Create/DefaultAssetConfig.asset")]
    public static DefaultAssetConfig Create()
    {
        mInstance = null;
        return Instance();
    }

    [CanEditMultipleObjects, CustomEditor(typeof(DefaultAssetConfig))]
    public partial class Editor : UnityEditor.Editor
    {
        DefaultAssetConfig mTarget = null;
        public void OnEnable()
        {
            mTarget = target as DefaultAssetConfig;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            mTarget.MConfig.Draw();

			if(GUI.changed)
			{
				EditorUtility.SetDirty(mTarget);
			}
        }

    }
}
