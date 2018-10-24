using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class EditorConfig : SingletonAsset<EditorConfig>
{
    [Serializable]
    public class Config
    {

        public int MaxTextPreviewLength = 7000;
    }

    [SerializeField]
	[HideInInspector]
    public Config MConfig = new Config();

    [MenuItem("Tools/Create/EditorConfig.asset")]
    public static EditorConfig Create()
    {
        mInstance = null;
        return Instance();
    }

    [CanEditMultipleObjects, CustomEditor(typeof(EditorConfig))]
    public partial class Editor : UnityEditor.Editor
    {
        EditorConfig mTarget = null;
        public void OnEnable()
        {
            mTarget = target as EditorConfig;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            Inspector.DrawComObj("Config", mTarget.MConfig);

			if(GUI.changed)
			{
				EditorUtility.SetDirty(mTarget);
			}
        }

    }
}
