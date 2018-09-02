
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
    LuaMonoBehaviour mObj = null;
    string luaStr = "";
    public void OnEnable()
    {
        mObj = (LuaMonoBehaviour)target;
        var luaPath = BuildConfig.BundleResRoot + mObj.luaScript.path + BuildConfig.LuaExtension;
        if (File.Exists(luaPath))
            luaStr = File.ReadAllText(luaPath);
    }

    static bool mShowInjections = true;
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var tmpAsset = EditorGUILayout.ObjectField("Lua", mObj.luaScript.Asset, typeof(UnityEngine.Object), true);
        if (tmpAsset != null && AssetDatabase.GetAssetPath(tmpAsset.GetInstanceID()).EndsWith(".lua"))
            mObj.luaScript.Asset = tmpAsset;

        if (mObj.injections == null)
        {
            mObj.injections = new LuaMonoBehaviour.Injection[0];
        }
        var size = mObj.injections.Length;
        EditorGUILayout.BeginHorizontal();
        {
            mShowInjections = EditorGUILayout.Foldout(mShowInjections, "Injections", true);
            size = EditorGUILayout.IntField(size);
        }
        EditorGUILayout.EndHorizontal();

        if (size != mObj.injections.Length)
        {
            var oldobjs = mObj.injections;
            mObj.injections = new LuaMonoBehaviour.Injection[size];
            var isfx = mObj.injections.IsFixedSize;
            for (int i = 0; i < Math.Min(size, oldobjs.Length); ++i)
            {
                mObj.injections[i] = oldobjs[i];
            }
        }

        if (mShowInjections)
        {
            for (var i = 0; i < mObj.injections.Length; ++i)
            {
                var item = mObj.injections[i] ?? new LuaMonoBehaviour.Injection();
                EditorGUILayout.BeginHorizontal();
                {
                    item.obj = (GameObject)EditorGUILayout.ObjectField(item.obj, typeof(GameObject), true);
                    if (item.obj)
                    {
                        var nname = EditorGUILayout.TextField(item.obj.name.RReplace(PathUtils.PunctuationRegex + "+", "_"));
                        var coms = item.obj.GetComponents<Component>();
                        if (item.exportComIdx == -1)
                            item.exportComIdx = coms.Length - 1;
                        item.exportComIdx = EditorGUILayout.Popup(item.exportComIdx, coms.Select(e =>
                        {
                            var name = e.GetType().ToString();
                            name = name.Substring(name.LastIndexOf('.') + 1);
                            return name;
                        }).ToArray());

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
            string luaname = mObj.luaScript.path.Substring(mObj.luaScript.path.LastIndexOf('/') + 1);
            string luaMemberValue = "--AutoGenInit Begin\nfunction " + luaname + ".AutoGenInit()";
            foreach (var i in mObj.injections.Where(o => o != null && o.obj != null))
            {
                var comType = i.obj.GetComponents<Component>()[i.exportComIdx].GetType().ToString();
                luaMemberValue += "\n    " + luaname + "." + i.obj.name + "_" + comType.Substring(comType.LastIndexOf('.') + 1)
                    + " = " + i.obj.name
                    + ":GetComponent(\""
                    + comType + "\")";
            }
            luaMemberValue += "\nend\n--AutoGenInit End";
            GUILayout.TextArea(luaMemberValue);

            var rect = EditorGUILayout.GetControlRect();
            if (GUI.Button(rect.Split(1, 3), "Wtrite to lua"))
            {
                var luaPath = BuildConfig.BundleResRoot + mObj.luaScript.path + BuildConfig.LuaExtension;
                var pattern = Regex.Match(luaStr, "--AutoGenInit Begin(.|\r|\n)*--AutoGenInit End", RegexOptions.Multiline).ToString();
                luaStr = luaStr.Replace(pattern.ToString(), luaMemberValue);
                File.WriteAllText(luaPath, luaStr);
            }
        }

        // lua debug
        if (mObj.luaTable != null)
        {
            mObj.luaTable.Draw();
        }

    }
}
#endif
