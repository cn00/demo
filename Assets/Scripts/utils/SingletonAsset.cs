using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class SingletonAssetBase<T> : ScriptableObject
{
    protected static string AssetPath
    {
        // set { mAssetPath = value; }
        get
        {
            // switch(typeof(T).ToString())
            // {
            // default:
            //     break;
            // }
            return "Assets/config/" + typeof(T) + ".asset";
        }
    }

    public virtual bool Init()
    {
        return true;
    }

    public virtual void Save()
    {
#if UNITY_EDITOR
        EditorUtility.SetDirty(this);
        AssetDatabase.WriteImportSettingsIfDirty(AssetPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
#endif
    }

#if UNITY_EDITOR
    public void DrawSaveButton()
    {
        var rect = EditorGUILayout.GetControlRect();
        if (GUI.Button(rect.Split(1, 3), "Save Asset"))
        {
            Save();
        }
    }
#endif
}

public class SingletonAsset<T> : SingletonAssetBase<T> where T : SingletonAssetBase<T>
{
    protected static T mInstance = null;

    public static T InstanceRuntime()
    {
        if (mInstance == null)
        {
            var assetSubPath = AssetPath;
            AppLog.d("BundleConfig InstanceRuntime 0({0})", assetSubPath);
            var cachePath = AssetSys.CacheRoot + AssetSys.Instance.GetBundlePath(assetSubPath);
            if (File.Exists(cachePath))
            {
                AppLog.d("BundleConfig InstanceRuntime(1)");
                var bundle = AssetBundle.LoadFromFile(cachePath);
                mInstance = bundle.LoadAsset<T>(BundleConfig.BundleResRoot + assetSubPath);
                AppLog.d("BundleConfig GetAssetSync(2)");
            }
            else
            {
                var respath = (assetSubPath.Replace(".asset", ""));
                mInstance = Resources.Load<T>(respath);
            }

            if (mInstance == null)
            {
                mInstance = CreateInstance<T>();
            }
        }
        return mInstance;
    }

    public static T Instance()
    {
        AppLog.isEditor = Application.isEditor;
        T tmp = null;
#if UNITY_EDITOR
        tmp = InstanceEditor();
#else
        tmp = InstanceRuntime();
#endif
        tmp.Init();
        return tmp;
    }

#if UNITY_EDITOR
    public static T InstanceEditor()
    {
        if (mInstance == null)
        {
            mInstance = AssetDatabase.LoadAssetAtPath<T>(AssetPath);
            if (mInstance == null)
            {
                var dir = Path.GetDirectoryName(AssetPath);
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }

                mInstance = CreateInstance<T>();
                AssetDatabase.CreateAsset(mInstance, AssetPath);

            }
        }
        return mInstance;
    }

#endif //UNITY_EDITOR
}
