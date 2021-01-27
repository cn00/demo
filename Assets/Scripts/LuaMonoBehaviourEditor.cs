
using System.Collections.Generic;
using System.Text;

#if UNITY_EDITOR
namespace UnityEditor
{
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
        string mLuaText = "";
        
        public void OnEnable()
        {
            mTarget = (LuaMonoBehaviour) target;

            if (mTarget.LuaAsset && !Application.isPlaying)
            {
                mLuaText = mTarget.LuaAsset.text;
                refreshAutoGen();
            }

        }

        static bool mShowInjections = true;
        static bool mShowInjectionValues = true;
        static bool mShowLuaAutogens = false;
        StringBuilder luaMember = new StringBuilder(1024);

        void refreshAutoGen()
        {
            string luaname = "this"; //mLuaMono.luaScript.path.Substring(mLuaMono.luaScript.path.LastIndexOf('/') + 1);
            luaMember.Clear();
            luaMember.Append("--AutoGenInit Begin\n--DO NOT EDIT THIS FUNCTION MANUALLY.\nfunction " + luaname +
                             ".AutoGenInit()");
            var btnListeners = new StringBuilder(512);
            foreach (var i in mTarget.Injections.Where(o => o != null && o.obj != null))
            {
                var comType = i.obj.GetComponents<Component>()[i.exportComIdx].GetType().ToString();
                var injectName = i.obj.name;
                var luakey = i.obj.name + "_" + comType.Substring(comType.LastIndexOf('.') + 1);
                if (i.obj == mTarget.gameObject)
                {
                    injectName = "gameObject";
                    luakey = comType.Substring(comType.LastIndexOf('.') + 1);
                }

                luaMember.Append("\n    " + luaname + "." + luakey
                                  + " = " + injectName
                                  + ":GetComponent(typeof(CS."
                                  + comType + "))");
                if (comType.EndsWith("Button"))
                {
                    var funname = i.obj.name + "_OnClick";
                    luaMember.Append("\n    " + luaname + "." + luakey + ".onClick:AddListener("+luaname + "."+ funname +")");
                    var btnListener = "function " + luaname + "."+ funname + "()";
                    if( !mLuaText.Contains(btnListener))
                    {
                        btnListeners.Append("\n\n" + btnListener + "\n    print('" + funname + "')\nend -- " + funname);
                    }
                }
            }

            luaMember.Append("\nend\n--AutoGenInit End");
            luaMember.Append(btnListeners);
        }

        public string grep;
        public override void OnInspectorGUI()
        {
            var newLuaAsset = EditorGUILayout.ObjectField("Lua", mTarget.LuaAsset, typeof(TextAsset), false) as TextAsset;
            if(newLuaAsset == null) return;
            var assetPath = AssetDatabase.GetAssetPath(newLuaAsset.GetInstanceID());
            if (Path.GetExtension(assetPath) == ".lua")
            {
                mTarget.LuaAsset = newLuaAsset;
                mTarget.LuaPath = assetPath;
            }
            else
            {
                AppLog.d(Tag, "not a lua script");
            }
            
            base.OnInspectorGUI();

            #region Injections
            {
                if (mTarget.Injections == null)
                {
                    mTarget.Injections = new List<LuaMonoBehaviour.Injection>();
                }

                var size = mTarget.Injections.Count;
                EditorGUILayout.BeginHorizontal();
                {
                    mShowInjections = EditorGUILayout.Foldout(mShowInjections, "Injections", true);
                    size = EditorGUILayout.DelayedIntField(size);
                }
                EditorGUILayout.EndHorizontal();

                if(mTarget.Injections.Count != size)
                {
                    while (mTarget.Injections.Count < size)
                    {
                        mTarget.Injections.Add(new LuaMonoBehaviour.Injection());
                    }
                    if (mTarget.Injections.Count > size)
                    {
                        mTarget.Injections.RemoveRange(size, mTarget.Injections.Count-size);
                    }
                }

                if (mShowInjections)
                {
                    for (var i = 0; i < mTarget.Injections.Count; ++i)
                    {
                        var item = mTarget.Injections[i] ?? new LuaMonoBehaviour.Injection();
                        EditorGUILayout.BeginHorizontal();
                        {
                            item.obj = (GameObject) EditorGUILayout.ObjectField(item.obj, typeof(GameObject), true);
                            if (item.obj)
                            {
                                var nname = EditorGUILayout.DelayedTextField(item.obj.name
                                    .RReplace(PathUtils.PunctuationRegex + "+", "_").TrimEnd('_'));
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
                                    var comType = item.obj.GetComponents<Component>()[item.exportComIdx].GetType()
                                        .ToString();
                                    var comName = comType.Substring(comType.LastIndexOf('.') + 1);
                                    var oldExp = item.obj.name + "_" + comName;
                                    string newExp = nname + "_" + comName;
                                    mLuaText = mLuaText.Replace("." + oldExp, "." + newExp);
                                    item.obj.name = nname;
                                }
                            }
                        }
                        EditorGUILayout.EndHorizontal();
                    }
                }
            }
            #endregion

            #region InjectValues
            {
                if (mTarget.InjectValues == null)
                {
                    mTarget.InjectValues = new List<LuaMonoBehaviour.InjectValue>();
                }

                var size = mTarget.InjectValues.Count;
                EditorGUILayout.BeginHorizontal();
                {
                    mShowInjectionValues = EditorGUILayout.Foldout(mShowInjectionValues, "InjectionValues", true);
                    size = EditorGUILayout.DelayedIntField(size);
                }
                EditorGUILayout.EndHorizontal();

                if(mTarget.InjectValues.Count != size)
                {
                    while (mTarget.InjectValues.Count < size)
                    {
                        mTarget.InjectValues.Add(new LuaMonoBehaviour.InjectValue());
                    }
                    if (mTarget.InjectValues.Count > size)
                    {
                        mTarget.InjectValues.RemoveRange(size, mTarget.InjectValues.Count-size);
                    }
                }

                if (mShowInjectionValues)
                {
                    for (var i = 0; i < mTarget.InjectValues.Count; ++i)
                    {
                        var item = mTarget.InjectValues[i] ?? new LuaMonoBehaviour.InjectValue();
                        EditorGUILayout.BeginHorizontal();
                        {
                            item.k = EditorGUILayout.DelayedTextField(item.k);
                            item.v = EditorGUILayout.DelayedTextField(item.v);
                        }
                        EditorGUILayout.EndHorizontal();
                    }
                }
            }            
            #endregion
            
            // gen lua code
            if(!Application.isPlaying)
            {
                if (GUI.changed)
                {
                    mTarget.InjectValues.Sort((a, b) =>
                    {
                        if (a.k != null && b.k != null)
                            return a.k.CompareTo(b.k);
                        else if (a.k == null && b.k != null)
                            return 1;
                        else
                            return -1;
                    });

                    mTarget.Injections.Sort((a, b) =>
                    {
                        if (a.obj != null && b.obj != null)
                            return a.obj.name.CompareTo(b.obj.name);
                        else if (a.obj == null && b.obj != null)
                            return 1;
                        else
                            return -1;
                    });
                    refreshAutoGen();
                    EditorUtility.SetDirty(mTarget);
                }

                var rect = EditorGUILayout.GetControlRect();
                if (GUI.Button(rect.Split(0, 3), "Wtrite to lua"))
                {
                    var path = AssetDatabase.GetAssetPath(mTarget.LuaAsset);
                    var pattern = Regex.Match(mLuaText, "--AutoGenInit Begin(.|\r|\n)*--AutoGenInit End",
                        RegexOptions.Multiline).ToString();
                    mLuaText = mLuaText.Replace(pattern.ToString(), luaMember.ToString());
                    File.WriteAllText(path, mLuaText);
                    AppLog.d(Tag, path + " updated");
                }

                mShowLuaAutogens = EditorGUILayout.Foldout(mShowLuaAutogens, "LuaAutogen", true);
                if (mShowLuaAutogens && !Application.isPlaying)
                    GUILayout.TextArea(luaMember.ToString());
            }

            // lua debug
            if (mTarget.Lua != null)
            {
                grep = EditorGUILayout.TextField("grep", grep);
                mTarget.Lua.Draw("<color=red>t</color>", 0, null, grep);
            }

        }
    }
}
#endif
