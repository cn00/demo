using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Collections.Generic;
using UnityEngine;

using System.Reflection;
using XLua;

#if UNITY_EDITOR
using UnityEditor;
#endif // UNITY_EDITOR

public static class CollectionExtensions
{
    public static IList<T> Clone<T>(this IList<T> self) where T : ICloneable
    {
        return self.Select(item => (T)item.Clone()).ToList();
    }

    // public static List<T> Clone<T>(this List<T> self) where T : ICloneable
    // {
    //     return self.Select(item => (T)item.Clone()).ToList();
    // }

    public static string Dump(this object self)
    {
        return JsonUtility.ToJson(self);
    }

    public static T Clone<T>(this T self)
    {
        T n = default(T);

        return n;
    }
}

public static class RectExtension
{
    public static Rect Split(this Rect rect, int index, int count)
    {
        int r = (int)rect.width % count; // Remainder used to compensate width and position.
        int width = (int)(rect.width / count);
        rect.width = width + (index < r ? 1 : 0) + (index + 1 == count ? (rect.width - (int)rect.width) : 0f);
        if (index > 0)
        { rect.x += width * index + (r - (count - 1 - index)); }

        return rect;
    }
    public static Rect SubRect(this Rect rect, Vector2 offset, Vector2 size)
    {
        rect.position += offset;
        rect.size = size;
        return rect;
    }
}

public static class GameObjectExtension
{
    public static T GetOrAddComponent<T>(this GameObject self) where T : Component
    {
        T com = self.GetComponent<T>();
        if(com == null)
        {
            com = self.AddComponent<T>();
        }
        return com;
    }

    // public static Component GetOrAddComponent(this GameObject self, string comname)
    // {
    //     Component com = self.GetComponent(comname);
    //     if (com == null)
    //     {
    //         com = self.AddComponent(comname);
    //     }
    //     return com;
    // }

}

public static class LuaTableExtension
{
    public static LuaTable GetMetaTable(this XLua.LuaTable self)
    {
        var meta = LuaSys.Instance.CallLuaHelp("BridgingClass.GetMetaTable", self) as LuaTable;
        return meta;
    }

    public static string ToString(this XLua.LuaTable self, string fmt = null, int indent = 0, bool strfun = false)
    {
        var indents0 = "";
        var indents = "";
        if(fmt != null)
        {
            indents0 = "\n" + new String(' ', 4*indent);
            indents = "\n" + new String(' ', 4*(indent+1));
        }
        
        StringBuilder sb = new StringBuilder(20480);
        sb.Append(indents0 + "{");
        self.ForEach<int, object>((k, v) =>
        {
            if (v is XLua.LuaTable)
            {
                var t = (v as XLua.LuaTable);
                sb.Append(indents + "[" + k.ToString() + "] = ");
                sb.Append(t.ToString(fmt, 1+indent));
                sb.Append(",");
            }
            else
            {
                if((v is bool)||(v is long)||(v is double))
                    sb.Append(indents + "[" + k + "] = " + (v != null ? v.ToString() + "," : "nil,"));
                // else if (v is LuaFunction && strfun)
                // {
                //     sb.Append(indents + "[\"" + k + "\"] = \"" + v.ToString() + "\",");
                // }
                else if (v is string) //|| (v is Component))
                {
                    sb.Append(indents + "[\"" + k + "\"] = \"" + v.ToString() + "\",");
                }
            }
        });
        self.ForEach<string, object>((k, v) =>
        {
            if (v is XLua.LuaTable)
            {
                var t = (v as XLua.LuaTable);
                sb.Append(indents + "[\"" + k.ToString() + "\"] = ");
                sb.Append(t.ToString(fmt,  1+indent));
                sb.Append(",");
            }
            else
            {
                if((v is bool)||(v is long)||(v is double))
                    sb.Append(indents + "[\"" + k + "\"] = " + (v != null ? v.ToString() + "," : "nil,"));
                else //if (v is string || (v is Component))
                {
                    sb.Append(indents + "[\"" + k + "\"] = \"" + v.ToString() + "\",");
                }
            }
        });

        sb.Append(indents0 + "}");
        return sb.ToString();
    }
#if UNITY_EDITOR
    public delegate object ValueDelegate(object k, object o);
    public static void Draw(this XLua.LuaTable self, int indent = 0, GUILayoutOption[] guiOpts = null)
    {
        EditorGUI.indentLevel += indent;
        var name = self.ContainsKey("Name") ? self.Get<string>("Name"):"";
        var Foldout = self.ContainsKey("Foldout") ? self.Get<bool>("Foldout"):false;
        Foldout = EditorGUILayout.Foldout(Foldout, name + ": LuaTable", true);
        self.Set("Foldout", Foldout);
        if (Foldout)
        {
            ValueDelegate drawv = (k, v)=>{
                if (v is bool)
                {
                    var tmp = EditorGUILayout.Toggle((bool)v);
                    self.Set(k, tmp);
                }
                else if (v is Enum)
                {
                    {
                        var tmp = EditorGUILayout.EnumPopup((Enum)v);
                        self.Set(k, tmp);
                    }
                }
                else if (v is long)
                {
                    {
                        var tmp = EditorGUILayout.LongField((long)v);
                        self.Set(k, tmp);
                    }
                }
                else if (v is double)
                {
                    var tmp = EditorGUILayout.DoubleField((double)v);
                    self.Set(k, tmp);
                }
                else if (v is string)
                {
                    var tmp = EditorGUILayout.TextField((string)v);
                    self.Set(k, tmp);
                }
                else if (v is Component)
                {
                    EditorGUILayout.ObjectField((v as Component).gameObject, typeof(UnityEngine.Object), true);
                }
                return v;
            };
            using (var verticalScope = new EditorGUILayout.VerticalScope("box"))
            {
                self.ForEach<int, object>((k, v) =>
                {
                    if (v is XLua.LuaTable)
                    {
                        var t = (v as XLua.LuaTable);
                        t.Set("Name", "[" + k.ToString() + "]");
                        t.Draw(indent);
                    }
                    else
                    {
                        EditorGUILayout.BeginHorizontal();
                        var tname = (v != null ? v.GetType().ToString() : "nil");
                        var dotidx = tname.LastIndexOf('.');
                        if(dotidx > 0)tname = tname.Substring(dotidx+1);
                        EditorGUILayout.LabelField("[" + k + "]: " + tname);
                        drawv(k,v);
                        EditorGUILayout.EndHorizontal();
                        if (v is LuaMonoBehaviour)
                        {
                            (v as LuaMonoBehaviour).luaTable.Draw(1+indent);
                        }
                    }
                });

                self.ForEach<string, object>((k, v) =>
                {
                    if (v is XLua.LuaTable)
                    {
                        var t = (v as XLua.LuaTable);
                        t.Set("Name", k);
                        t.Draw(indent);
                    }
                    else
                    {
                        if(k == "Foldout" || k == "Name")
                        {
                            return;
                        }
                        EditorGUILayout.BeginHorizontal();
                        var tname = (v != null ? v.GetType().ToString() : "nil");
                        var dotidx = tname.LastIndexOf('.');
                        if(dotidx > 0)tname = tname.Substring(dotidx+1);
                        EditorGUILayout.LabelField(k + ": " + tname);
                        drawv(k,v);
                        EditorGUILayout.EndHorizontal();
                        if (v is LuaMonoBehaviour)
                        {
                            (v as LuaMonoBehaviour).luaTable.Draw(1+indent);
                        }
                    }
                });

                // metatable
                var meta = self.GetMetaTable();
                if(meta != null)
                {
                    meta.Set("Name", "__meta");
                    meta.Draw(indent+1);
                }
            }
        }
        EditorGUI.indentLevel -= indent;
    }
#endif

}
