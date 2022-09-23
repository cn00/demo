﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using System.Reflection;

#if UNITY_EDITOR
using UnityEditor;
#endif //UNITY_EDITOR

[Serializable, HideInInspector]
public class Inspector
{

#if UNITY_EDITOR
    const string Tag = "Inspector";
    public class SafeDictionary<TK, TV>
    {
        Dictionary<TK, TV> mContainer = new Dictionary<TK, TV>();
        public TV this[TK k]
        {
            get
            {
                TV tmp = default(TV);
                if(!mContainer.TryGetValue(k, out tmp))
                    tmp = (mContainer[k] = default(TV));
                return tmp;
            }
            set
            {
                mContainer[k] = value;
            }
        }

    }

    public static void DrawList(string name, IList ls, ref bool foldout, bool showidx = false,Action<object> foreach_item = null, GUILayoutOption[] options = null)
    {
        var size = ls.Count;
        EditorGUILayout.BeginHorizontal();
        {
            foldout = EditorGUILayout.Foldout(foldout, name, true);
            size = EditorGUILayout.DelayedIntField(size);
        }
        EditorGUILayout.EndHorizontal();

        if (size < ls.Count)
        {
            // ls.RemoveRange(size - 1, ls.Count - size);
            while(ls.Count > size)
            {
                ls.RemoveAt(ls.Count-1);
            }
        }
        else if (size > ls.Count)
        {
            var it = ls.GetType().GetGenericArguments().Single();
            for (var i = ls.Count; i < size; ++i)
                ls.Add(Default(it));
        }
        if (foldout)
        {
            try//(ls)
            {
                ++EditorGUI.indentLevel;
                for (var i = 0; i < ls.Count; ++i)
                {
                    var iobj = ls[i];
                    if(showidx)
                        DrawObj(i.ToString(), ref iobj);
                    else
                    {
                        DrawObj("", ref iobj);
                    }
                    ls[i] = iobj;
                    if(foreach_item != null)
                        foreach_item(iobj);
                }// foreach
                // --EditorGUI.indentLevel;
            }
            catch(Exception e)
            {
                Debug.LogError(e);
            }
            finally
            {
                --EditorGUI.indentLevel;
            }
        }
    }

    public static void DrwaDic(string name, IDictionary dic, ref bool foldout, GUILayoutOption[] options = null)
    {
        var size = dic.Count;
        EditorGUILayout.BeginHorizontal();
        {
            foldout = EditorGUILayout.Foldout(foldout, name, true);
            size = EditorGUILayout.DelayedIntField(size);
        }
        EditorGUILayout.EndHorizontal();

        if (foldout)
        {
            ++EditorGUI.indentLevel;
            foreach (DictionaryEntry item in dic)
            {
                var vv = item.Value  as object;
                bool del = false;
                DrawObj(item.Key.ToString(), ref vv, null, ()=>{
                    if(tmpFoldout[item.Key.ToString()])
                    {
                        int idx = -1;
                        int count = 6;
                        var rect = EditorGUILayout.GetControlRect();
                        rect.position += new Vector2(14f * EditorGUI.indentLevel, 0f);
                        if(GUI.Button(rect.Split(++idx, count), "-"))
                        {
                            dic.Remove(item.Key);
                            del = true;;
                        }
                    }
                });
                if(del)return;
                // dic[item.Key] = vv;
            }// for
            --EditorGUI.indentLevel;
        }
    }
    protected static object Default(Type t)
    {
        if (t.IsValueType )
            return default(int);
        else if(t == typeof(string)) {
            return "";
        } else {
            return Activator.CreateInstance(t);
        }
    }
    public static void DrawObj(string name, ref object obj, Action begin = null, Action end = null)
    {
        if(begin != null)begin();// once only
        if (obj is bool)
        {
            obj = EditorGUILayout.Toggle(name, (bool)obj);
        }
        else if (obj is Enum)
        {
            if (name.ToLower().Contains("flag") || name.ToLower().Contains("mask"))
            {
                obj = EditorGUILayout.EnumFlagsField(name, (Enum)obj);
            }
            else
            {
                obj = EditorGUILayout.EnumPopup(name, (Enum)obj);
            }
        }
        else if (obj is int)
        {
            obj = EditorGUILayout.IntField(name, (int)obj);
        }
        else if(obj is uint)
        {
            uint vv = (uint)obj;
            var ivv = EditorGUILayout.IntField(name, (int)vv);
            ivv = ivv < 0 ? 0 : ivv;
            obj = (uint)ivv;// & 0xefffffff;
        }
        else if ( obj is long)
        {
            if (name.ToLower().Contains("time"))
            {
                EditorGUILayout.LabelField(name, DateTime.FromFileTime((long)obj).ToString());
            }
            else
            {
                obj = EditorGUILayout.LongField(name, (long)obj);
            }
        }
        else if (obj is double || obj is float)
        {
            obj = EditorGUILayout.FloatField(name, (float)obj);
        }
        else if (obj is string)
        {
            if (name.ToLower().EndsWith("s"))
            {
                EditorGUILayout.LabelField(name);
                obj = EditorGUILayout.TextArea((string)obj)
                    .Replace("\r", "")
                    .Replace("\n\n", "\n")
                    .TrimStart(' ', '\n', '\t')
                    .TrimEnd(' ', '\n', '\t');
            }
            else
            {
                obj = EditorGUILayout.TextField(name, (string)obj);
            }
        }
        else if (obj is IList)
        {
            var il = obj as IList;
            var foldouti = tmpFoldout[name];
            DrawList(name, il, ref foldouti);
            tmpFoldout[name] = foldouti;
        }
        else if (obj is IDictionary)
        {
            var f = tmpFoldout[name];
            DrwaDic(name, obj as IDictionary, ref f);
            tmpFoldout[name] = f;
        }
        else if (obj is XLua.LuaTable)
        {
            (obj as XLua.LuaTable).Draw(name);
        }
        else if (obj is UnityEngine.Object)
        {
            obj = EditorGUILayout.ObjectField(name, obj as UnityEngine.Object, typeof(UnityEngine.Object), true);
        }
        else
        {
            DrawComObj(name, obj);
        }
        if(end!= null)end();
    }

    public static SafeDictionary<string, bool> tmpFoldout = new SafeDictionary<string, bool>();
    public static void DrawComObj(string name, object obj, Action begin = null, Action end = null, Color color = default(Color))
    {
        if(obj != null)
        {
            var type = obj.GetType();
            var fields = type.GetFields(
                      BindingFlags.Default
                    // | BindingFlags.DeclaredOnly // no inherited
                    | BindingFlags.Instance
                    | BindingFlags.Public
                ).ToList();
            var fnobj = fields.Find(ifobj => ifobj.Name.ToLower() == "name");
            if(string.IsNullOrEmpty(name) && fnobj != null)
            {
                name = (string)fnobj.GetValue(obj);
            }

            bool tmpfoldout = false;
            var ffobj = fields.Find(ifobj => ifobj.Name == "Foldout");
            if(ffobj != null)
                tmpfoldout = (bool)ffobj.GetValue(obj);
            else
            {
                tmpfoldout = tmpFoldout[name];
            }
            if(color == default(Color))
                color = Color.black;
            var style = new GUIStyle(EditorStyles.foldout);
            style.normal.textColor = color;
            tmpfoldout = EditorGUILayout.Foldout(tmpfoldout, name, true, style);
            if(ffobj != null)
                ffobj.SetValue(obj, tmpfoldout);
            else
                tmpFoldout[name] = tmpfoldout;

            if(!tmpfoldout)
                return;

            // Methods
            {
                var actionfild = type.GetMethods(
                      BindingFlags.Default
                    | BindingFlags.DeclaredOnly // no inherited
                    | BindingFlags.Instance
                    | BindingFlags.Public
                    // | BindingFlags.InvokeMethod
                ).Where(i => i.Name.EndsWith("Btn")||i.Name.StartsWith("Btn")).ToList();
                var NumPerRow = 4;
                var npobj = fields.Find(ifobj => ifobj.Name.ToLower() == "btnperrow");
                if(npobj != null)
                    NumPerRow = (int)(uint)npobj.GetValue(obj);
                NumPerRow = NumPerRow < 1 ? 1 : NumPerRow;
                for(var rowi = 0; rowi < (int)(actionfild.Count + NumPerRow*0.5) / NumPerRow; ++rowi)
                {
                    var rect = EditorGUILayout.GetControlRect();
                    for (var i = 0; i < NumPerRow && (NumPerRow * rowi +i) < actionfild.Count; ++i)
                    {
                        var fi = actionfild[NumPerRow * rowi +i];
                        var v = fi.GetParameters();
                        if(GUI.Button(rect.Split(i, NumPerRow), fi.Name.Replace("Btn", "")))
                        {
                            fi.Invoke(obj, v);
                        }
                    }
                }
            }

            if(begin != null)begin();

            fields.Sort(delegate (FieldInfo i, FieldInfo j) {
                if(i.Name == "Name" && j.Name != "Name")
                {
                    return -1;
                }
                if(j.Name == "Name" && i.Name != "Name")
                {
                    return 1;
                }
                if((i.GetValue(obj) is bool) && !(j.GetValue(obj) is bool))
                {
                    return -1;
                }
                if((j.GetValue(obj) is bool) && !(i.GetValue(obj) is bool))
                {
                    return 1;
                }
                if((i.GetValue(obj) is IList) && !(j.GetValue(obj) is IList))
                {
                    return -1;
                }
                if((j.GetValue(obj) is IList) && !(i.GetValue(obj) is IList))
                {
                    return 1;
                }
                if((i.GetValue(obj) is Enum) && !(j.GetValue(obj) is Enum))
                {
                    return -1;
                }
                if((j.GetValue(obj) is Enum) && !(i.GetValue(obj) is Enum))
                {
                    return 1;
                }
                return i.Name.CompareTo(j.Name);
            });
            // ++EditorGUI.indentLevel;
            foreach (var i in fields)
            {
                bool skipflag = i.Name.EndsWith("Foldout");
                // #if UNITY_ANDROID
                // skipflag &= i.Name.StartsWith("iOS");
                // #endif
                // #if UNITY_IOS
                // skipflag &= i.Name.StartsWith("Android");
                // #endif
                if(skipflag)
                    continue;

                var v = i.GetValue(obj);
                // if ((v is bool)|| (v is Enum)|| (v is int)||(v is uint)||( v is long)|| (v is double )||( v is float)|| (v is string))
                if ((v is ValueType) || (v is string))
                {
                    DrawObj(i.Name, ref v);
                    i.SetValue(obj, v);
                }
                else if (v is IList)
                {
                    var il = v as IList;
                    var ff = fields.Find(iil => iil.Name == (i.Name + "Foldout"));
                    var foldout = true;
                    var gk = name + "." + i.Name;
                    if(ff != null)
                        foldout = (bool) ff.GetValue(obj);
                    else
                    {
                        foldout = tmpFoldout[gk];
                    }
                    ++EditorGUI.indentLevel;
                    DrawList(i.Name, il, ref foldout);
                    --EditorGUI.indentLevel;
                    if(ff!=null)
                        ff.SetValue(obj, foldout);
                    else
                    {
                        tmpFoldout[gk] = foldout;
                    }
                }
                else if (v is IDictionary)
                {
                    var f = tmpFoldout[name + "." + i.Name];
                    DrwaDic(i.Name, v as IDictionary, ref f);
                    tmpFoldout[name + "." + i.Name] = f;
                }
                else if (v is string[])
                {
                    EditorGUILayout.LabelField(i.Name);
                    ++EditorGUI.indentLevel;
                    var l = v as string[];
                    for (var idx = 0; idx < l.Length; ++idx)
                    {
                        l[idx] = EditorGUILayout.TextField(l[idx]);
                    }
                    --EditorGUI.indentLevel;
                }
                else if (v is Action)
                {
                    // skip
                }
                else if (v is XLua.LuaTable)
                {
                    (v as XLua.LuaTable).Draw(name);
                }
                else if (v is UnityEngine.Object)
                {
                    var tmp = EditorGUILayout.ObjectField(i.Name, v as UnityEngine.Object, typeof(UnityEngine.Object), true);
                    i.SetValue(obj, tmp);
                }
                else if (v is object)
                {
                    ++EditorGUI.indentLevel;
                    using (var verticalScope2 = new EditorGUILayout.VerticalScope("box"))
                    {DrawComObj(i.Name, v);}
                    --EditorGUI.indentLevel;
                }
            }
            // --EditorGUI.indentLevel;
            if(end!= null)end();
        }
    }

#endif
}