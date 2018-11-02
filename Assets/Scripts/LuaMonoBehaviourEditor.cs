
#if UNITY_EDITOR
using System;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEditor;
using System.Text.RegularExpressions;

[CustomEditor(typeof(LuaMonoBehaviour))]
public class LuaMonoBehaviourEditor : Editor
{
    const string Tag = "LuaMonoBebaviourEdirot";
    LuaMonoBehaviour mLuaMono = null;
    string LuaStr = "";
    public void OnEnable()
    {
        mLuaMono = (LuaMonoBehaviour)target;
        var luaPath = AssetDatabase.GetAssetPath(mLuaMono.luaScript.Asset.GetInstanceID());
        if (File.Exists(luaPath))
            LuaStr = File.ReadAllText(luaPath);

        var tpath = luaPath.Remove(luaPath.Length - 4)
            .Replace(BuildConfig.BundleResRoot, "");
        if(tpath != mLuaMono.luaScript.path)
        {
            EditorUtility.SetDirty(mLuaMono);
        }
    }

    static bool mShowInjections = true;
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var tmpAsset = EditorGUILayout.ObjectField("Lua", mLuaMono.luaScript.Asset, typeof(UnityEngine.Object), true);
        EditorGUILayout.LabelField(mLuaMono.luaScript.path);
        var luaPath = AssetDatabase.GetAssetPath(tmpAsset.GetInstanceID());
        if (tmpAsset != null && luaPath.EndsWith(BuildConfig.LuaExtension))
        {
            // new
            if (mLuaMono.luaScript.Asset != tmpAsset)
                LuaStr = File.ReadAllText(luaPath);
            // renamed
            var tpath = luaPath.Remove(luaPath.Length - 4)
                .Replace(BuildConfig.BundleResRoot, "");
            if(tpath != mLuaMono.luaScript.path)
            {
                EditorUtility.SetDirty(mLuaMono);
                mLuaMono.luaScript.Asset = tmpAsset;
            }
        }

        if (mLuaMono.injections == null)
        {
            mLuaMono.injections = new LuaMonoBehaviour.Injection[0];
        }
        var size = mLuaMono.injections.Length;
        EditorGUILayout.BeginHorizontal();
        {
            mShowInjections = EditorGUILayout.Foldout(mShowInjections, "Injections", true);
            size = EditorGUILayout.DelayedIntField(size);
        }
        EditorGUILayout.EndHorizontal();

        if (size != mLuaMono.injections.Length)
        {
            var oldobjs = mLuaMono.injections;
            mLuaMono.injections = new LuaMonoBehaviour.Injection[size];
            var isfx = mLuaMono.injections.IsFixedSize;
            for (int i = 0; i < Math.Min(size, oldobjs.Length); ++i)
            {
                mLuaMono.injections[i] = oldobjs[i];
            }
        }

        if (mShowInjections)
        {
            for (var i = 0; i < mLuaMono.injections.Length; ++i)
            {
                var item = mLuaMono.injections[i] ?? new LuaMonoBehaviour.Injection();
                EditorGUILayout.BeginHorizontal();
                {
                    item.obj = (GameObject)EditorGUILayout.ObjectField(item.obj, typeof(GameObject), true);
                    if (item.obj)
                    {
                        var nname = EditorGUILayout.DelayedTextField(item.obj.name.RReplace(PathUtils.PunctuationRegex + "+", "_").TrimEnd('_'));
                        var coms = item.obj.GetComponents<Component>().Select(e =>
                        {
                            var name = e.GetType().ToString();
                            name = name.Substring(name.LastIndexOf('.') + 1);
                            return name;
                        }).ToArray();
                        if (item.exportComIdx == -1)
                            item.exportComIdx = coms.Length - 1;
                        item.exportComIdx = EditorGUILayout.Popup(item.exportComIdx, coms);

                        if (item.obj.name != nname)
                        {
                            //rename in lua
                            var comType = item.obj.GetComponents<Component>()[item.exportComIdx].GetType().ToString();
                            var comName = comType.Substring(comType.LastIndexOf('.') + 1);
                            var oldExp = item.obj.name + "_" + comName;
                            string newExp = nname + "_" + comName;
                            LuaStr = LuaStr.Replace("." + oldExp, "." + newExp);
                            item.obj.name = nname;
                        }
                    }
                }
                EditorGUILayout.EndHorizontal();
            }

            // gen lua code
            string luaname = "this";//mLuaMono.luaScript.path.Substring(mLuaMono.luaScript.path.LastIndexOf('/') + 1);
            string luaMemberValue = "--AutoGenInit Begin\nfunction " + luaname + ".AutoGenInit()";
            foreach (var i in mLuaMono.injections.Where(o => o != null && o.obj != null))
            {
                var comType = i.obj.GetComponents<Component>()[i.exportComIdx].GetType().ToString();
                var injectName = i.obj.name;
                var luakey = i.obj.name + "_" + comType.Substring(comType.LastIndexOf('.') + 1);
                if(i.obj == mLuaMono.gameObject)
                {
                    injectName = "mono.gameObject";
                    luakey = comType.Substring(comType.LastIndexOf('.') + 1);
                }
                luaMemberValue += "\n    " + luaname + "." + luakey
                    + " = " + injectName
                    + ":GetComponent(\""
                    + comType + "\")";
            }
            luaMemberValue += "\nend\n--AutoGenInit End";
            GUILayout.TextArea(luaMemberValue);

            var rect = EditorGUILayout.GetControlRect();
            if (GUI.Button(rect.Split(1, 3), "Wtrite to lua"))
            {
                var path = BuildConfig.BundleResRoot + mLuaMono.luaScript.path + BuildConfig.LuaExtension;
                var pattern = Regex.Match(LuaStr, "--AutoGenInit Begin(.|\r|\n)*--AutoGenInit End", RegexOptions.Multiline).ToString();
                LuaStr = LuaStr.Replace(pattern.ToString(), luaMemberValue);
                File.WriteAllText(path, LuaStr);
                AppLog.d(Tag, mLuaMono.luaScript.path + BuildConfig.LuaExtension + " write ok");
            }
        }

        // lua debug
        if (mLuaMono.luaTable != null)
        {
            mLuaMono.luaTable.Draw();
        }

    }
}
#endif
