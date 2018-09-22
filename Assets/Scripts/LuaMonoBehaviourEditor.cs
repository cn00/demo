
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
    LuaMonoBehaviour mLuaMono = null;
    string luaStr = "";
    public void OnEnable()
    {
        mLuaMono = (LuaMonoBehaviour)target;
        var luaPath = BuildConfig.BundleResRoot + mLuaMono.luaScript.path + BuildConfig.LuaExtension;
        if (File.Exists(luaPath))
            luaStr = File.ReadAllText(luaPath);
    }

    static bool mShowInjections = true;
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var tmpAsset = EditorGUILayout.ObjectField("Lua", mLuaMono.luaScript.Asset, typeof(UnityEngine.Object), true);
        if (tmpAsset != null && AssetDatabase.GetAssetPath(tmpAsset.GetInstanceID()).EndsWith(".lua"))
            mLuaMono.luaScript.Asset = tmpAsset;

        if (mLuaMono.injections == null)
        {
            mLuaMono.injections = new LuaMonoBehaviour.Injection[0];
        }
        var size = mLuaMono.injections.Length;
        EditorGUILayout.BeginHorizontal();
        {
            mShowInjections = EditorGUILayout.Foldout(mShowInjections, "Injections", true);
            size = EditorGUILayout.IntField(size);
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
                            luaStr = luaStr.Replace("." + oldExp, "." + newExp);
                            item.obj.name = nname;
                        }
                    }
                }
                EditorGUILayout.EndHorizontal();
            }

            // gen lua code
            string luaname = mLuaMono.luaScript.path.Substring(mLuaMono.luaScript.path.LastIndexOf('/') + 1);
            string luaMemberValue = "--AutoGenInit Begin\nfunction " + luaname + ".AutoGenInit()";
            foreach (var i in mLuaMono.injections.Where(o => o != null && o.obj != null))
            {
                var comType = i.obj.GetComponents<Component>()[i.exportComIdx].GetType().ToString();
                var injectName = i.obj.name;
                if(i.obj == mLuaMono.gameObject)
                {
                    injectName = "mono.gameObject";
                }
                luaMemberValue += "\n    " + luaname + "." + i.obj.name + "_" + comType.Substring(comType.LastIndexOf('.') + 1)
                    + " = " + injectName
                    + ":GetComponent(\""
                    + comType + "\")";
            }
            luaMemberValue += "\nend\n--AutoGenInit End";
            GUILayout.TextArea(luaMemberValue);

            var rect = EditorGUILayout.GetControlRect();
            if (GUI.Button(rect.Split(1, 3), "Wtrite to lua"))
            {
                var luaPath = BuildConfig.BundleResRoot + mLuaMono.luaScript.path + BuildConfig.LuaExtension;
                var pattern = Regex.Match(luaStr, "--AutoGenInit Begin(.|\r|\n)*--AutoGenInit End", RegexOptions.Multiline).ToString();
                luaStr = luaStr.Replace(pattern.ToString(), luaMemberValue);
                File.WriteAllText(luaPath, luaStr);
                AppLog.d(mLuaMono.luaScript.path + BuildConfig.LuaExtension + " write ok");
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
