using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;

#if UNITY_EDITOR
using UnityEditor;
static class RectExtension
{
    public static Rect Split(this Rect rect, int index, int count)
    {
        int r = (int)rect.width % count; // Remainder used to compensate width and position.
        int width = (int)(rect.width / count);
        rect.width = width + (index < r ? 1 : 0) + (index + 1 == count ? (rect.width - (int)rect.width) : 0f);
        if(index > 0)
        { rect.x += width * index + (r - (count - 1 - index)); }

        return rect;
    }
}
#endif //UNITY_EDITOR

[Serializable]
public class InspectorDraw : object
{
    [SerializeField]
    string mName;
    public string Name { get { return mName; } set { mName = value; } }

    #if UNITY_EDITOR
     protected bool mFoldOut = false;
    public virtual void DrawInspector(int indent = 0, GUILayoutOption[] guiOpts = null)
    {
        var type = this.GetType();
        var fields = type.GetFields();
        foreach (var i in fields)
        {
            var v = i.GetValue(this);
            if(v is bool)
            {
                var tmp = EditorGUILayout.Toggle(i.Name, (bool)v);
                i.SetValue(this, tmp);
            }
            else if (v is int || v is uint || v is long)
            {
                if(i.Name.ToLower().Contains("time"))
                {
                    EditorGUILayout.LabelField(i.Name, DateTime.FromFileTime((long)v).ToString());
                }
                else
                {
                    var tmp = EditorGUILayout.IntField(i.Name, (int)v);
                    i.SetValue(this, tmp);
                }
            }
            else if(v is double || v is float || v is long)
            {
                var tmp = EditorGUILayout.FloatField(i.Name, (float)v);
                i.SetValue(this, tmp);
            }
            else if(v is string)
            {
                var tmp = EditorGUILayout.TextField(i.Name, (string)v);
                i.SetValue(this, tmp);
            }
        }
    }

    public virtual void Draw(int indent = 0, GUILayoutOption[] guiOpts = null)
    {
        EditorGUI.indentLevel += indent;
        mFoldOut = EditorGUILayout.Foldout(mFoldOut, Name, true);
        if (mFoldOut) using (var verticalScope = new EditorGUILayout.VerticalScope("box"))
        {
            // ++EditorGUI.indentLevel;
            DrawInspector(indent, guiOpts);
            // --EditorGUI.indentLevel;
        }
        EditorGUI.indentLevel -= indent;
    }

    #endif
}
