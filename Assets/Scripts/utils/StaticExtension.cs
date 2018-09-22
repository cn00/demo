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
}


public class FoldAbleDictionary<TK, TV> : Dictionary<TK, TV>
{
    public bool Foldout = false;
    public bool FoldoutAll = false;
}

public static class DictionaryExtension
{
    public static void Draw<TK, TV>(this FoldAbleDictionary<string, TV> self) where TV : FoldAble//, where TV : object
    {
#if UNITY_EDITOR
        ++EditorGUI.indentLevel;

        var FoldoutAll = EditorGUILayout.Toggle("FoldoutAll", self.FoldoutAll);
        if(FoldoutAll != self.FoldoutAll)
        {
            foreach (var item in self)
            {
                item.Value.Foldout = FoldoutAll;
            }
            self.FoldoutAll = FoldoutAll;
        }

        var keyToDelete = "";
        var keyToAdd = "";
        TV valueToAdd = null;
        foreach (var item in self)
        {
            // EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.BeginHorizontal();
                {
                    item.Value.Foldout = EditorGUILayout.Foldout(item.Value.Foldout, item.Key.ToString(), true);
                    var rect = EditorGUILayout.GetControlRect();
                    if (GUI.Button(rect.Split(0, 5), "-"))
                    {
                        // self.Remove(item.Key);
                        keyToDelete = item.Key;
                    }
                }
                EditorGUILayout.EndHorizontal();

                if(item.Value.Foldout)
                {
                    // ++EditorGUI.indentLevel;
                    // item.Key = EditorGUILayout.TextField("Key", item.Key.ToString());
                    var nkey = EditorGUILayout.TextField("Key", item.Key);
                    if(nkey != item.Key)
                    {
                        keyToDelete = item.Key;
                        keyToAdd = nkey;
                        valueToAdd = item.Value;
                    }
                    InspectorDraw.DrawObj(item.Value);
                    // --EditorGUI.indentLevel;
                }
            }
            // EditorGUILayout.EndHorizontal();

        }// for
        if(!string.IsNullOrEmpty(keyToDelete))
        {
            self.Remove(keyToDelete);
        }
        if (!string.IsNullOrEmpty(keyToAdd))
        {
            self.Add(keyToAdd, valueToAdd);
        }

        --EditorGUI.indentLevel;
#endif //#if UNITY_EDITOR
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