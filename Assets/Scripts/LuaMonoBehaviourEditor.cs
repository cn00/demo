
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
    LuaMonoBehaviour mTarget = null;
    string LuaText = "";

    UnityEngine.Object mSourceLua = null;
    public void OnEnable()
    {
        mTarget = (LuaMonoBehaviour)target;
        if(mTarget.mAsset == null)
            return;

        var luaPath = AssetDatabase.GetAssetPath(mTarget.mAsset.GetInstanceID());
        luaPath = luaPath.Remove(luaPath.Length - 4);
        if (File.Exists(luaPath))
        {
            LuaText = File.ReadAllText(luaPath);
            mTarget.mAsset = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(luaPath);
        }

        refreshAutoGen();
    }

    static bool mShowInjections = true;
    static bool mShowLuaAutogens = true;
    string luaMemberValue;
    void refreshAutoGen()
    {
        string luaname = "this";//mLuaMono.luaScript.path.Substring(mLuaMono.luaScript.path.LastIndexOf('/') + 1);
        luaMemberValue = "--AutoGenInit Begin\n--DO NOT EDIT THIS FUNCTION MANUALLY.\nfunction " + luaname + ".AutoGenInit()";
        foreach (var i in mTarget.injections.Where(o => o != null && o.obj != null))
        {
            var comType = i.obj.GetComponents<Component>()[i.exportComIdx].GetType().ToString();
            var injectName = i.obj.name;
            var luakey = i.obj.name + "_" + comType.Substring(comType.LastIndexOf('.') + 1);
            if(i.obj == mTarget.gameObject)
            {
                injectName = "gameObject";
                luakey = comType.Substring(comType.LastIndexOf('.') + 1);
            }
            luaMemberValue += "\n    " + luaname + "." + luakey
                + " = " + injectName
                + ":GetComponent(typeof(CS."
                + comType + "))";
        }
        luaMemberValue += "\nend\n--AutoGenInit End";
    }
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var luaAsset = EditorGUILayout.ObjectField("LuaSrc", mTarget.mAsset, typeof(UnityEngine.Object), true);
        EditorGUILayout.LabelField(mTarget.luaPath);
        if (luaAsset != null)
        {
            var luaPath = AssetDatabase.GetAssetPath(luaAsset.GetInstanceID());
            var txtPath = luaPath + ".txt";
            if(luaPath.EndsWith(BuildConfig.LuaExtension))
            {
                var txtAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(txtPath);
                if (txtAsset != mTarget.mAsset)
                {
                    // new
                    LuaText = File.ReadAllText(luaPath);

                    mTarget.mAsset = luaAsset;
                    mTarget.luaPath = luaPath.Remove(luaPath.Length - 4)
                        .Replace(BuildConfig.BundleResRoot, "");
                    EditorUtility.SetDirty(mTarget);
                }
            }
        }

        if (mTarget.injections == null)
        {
            mTarget.injections = new LuaMonoBehaviour.Injection[0];
        }
        var size = mTarget.injections.Length;
        EditorGUILayout.BeginHorizontal();
        {
            mShowInjections = EditorGUILayout.Foldout(mShowInjections, "Injections", true);
            size = EditorGUILayout.DelayedIntField(size);
        }
        EditorGUILayout.EndHorizontal();

        if (size != mTarget.injections.Length)
        {
            var oldobjs = mTarget.injections;
            mTarget.injections = new LuaMonoBehaviour.Injection[size];
            for (int i = 0; i < Math.Min(size, oldobjs.Length); ++i)
            {
                mTarget.injections[i] = oldobjs[i];
            }
        }

        if (mShowInjections)
        {
            for (var i = 0; i < mTarget.injections.Length; ++i)
            {
                var item = mTarget.injections[i] ?? new LuaMonoBehaviour.Injection();
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
                            LuaText = LuaText.Replace("." + oldExp, "." + newExp);
                            item.obj.name = nname;
                        }
                    }
                }
                EditorGUILayout.EndHorizontal();
            }
        }

        // gen lua code
        if (GUI.changed)
            refreshAutoGen();

        var rect = EditorGUILayout.GetControlRect();
        if (GUI.Button(rect.Split(0, 3), "Wtrite to lua"))
        {
            var path = BuildConfig.BundleResRoot + mTarget.luaPath + BuildConfig.LuaExtension;
            var pattern = Regex.Match(LuaText, "--AutoGenInit Begin(.|\r|\n)*--AutoGenInit End", RegexOptions.Multiline).ToString();
            LuaText = LuaText.Replace(pattern.ToString(), luaMemberValue);
            File.WriteAllText(path, LuaText);
            AppLog.d(Tag, mTarget.luaPath + BuildConfig.LuaExtension + " write ok");
        }
        mShowLuaAutogens = EditorGUILayout.Foldout(mShowLuaAutogens, "LuaAutogen", true);
        if(mShowLuaAutogens)
            GUILayout.TextArea(luaMemberValue);

        // lua debug
        if (mTarget.luaTable != null)
        {
            mTarget.luaTable.Draw();
        }

    }
}
#endif
