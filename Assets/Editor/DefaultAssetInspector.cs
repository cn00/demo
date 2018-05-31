using UnityEngine;
using UnityEditor;
using System.IO;

using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class SheetStruct : InspectorDraw
{
    public List<IRow> Rows = new List<IRow>();
    public override void DrawInspector(int indent = 0, GUILayoutOption[] guiOpts = null)
    {
        base.DrawInspector();

        var MaxSheetPreviewLength = DefaultAssetConfig.Instance().MConfig.MaxSheetPreviewLength;
        foreach (var r in Rows)
        {
            EditorGUILayout.BeginHorizontal();
            {
                for (var i = 0; i < r.LastCellNum; ++i)
                {
                    EditorGUILayout.LabelField(r.Cell(i).SafeSValue(), guiOpts);
                }
            }
            EditorGUILayout.EndHorizontal();
        }
    }
}

public class BookStruct : InspectorDraw
{
    public List<SheetStruct> Sheets = new List<SheetStruct>();
    public override void DrawInspector(int indent = 0, GUILayoutOption[] guiOpts = null)
    {
        base.DrawInspector();

        guiOpts = new GUILayoutOption[]
        {
                GUILayout.Width(30),
                GUILayout.ExpandWidth(true),
        };
        foreach (var r in Sheets)
        {
            r.Draw(0, guiOpts);
        }
    }
}

[CanEditMultipleObjects, CustomEditor(typeof(DefaultAsset))]
public partial class DefaultAssetInspector : Editor
{
    object mTarget = null;
    string assetPath = null;
    public void OnEnable()
    {
        assetPath = AssetDatabase.GetAssetPath(target);
        if (assetPath.IsExcel())
        {
            var tmp = new BookStruct(){FoldOut = true};
            tmp.Name = assetPath;
            var excel = ExcelUtils.Open(assetPath);
            var MaxSheetPreviewLength = DefaultAssetConfig.Instance().MConfig.MaxSheetPreviewLength;
            foreach(var sheet in excel.AllSheets())
            {
                var ss = new SheetStruct(){FoldOut = true };
                ss.Name = sheet.SheetName;
                for(var i = 0; i < sheet.LastRowNum && i < MaxSheetPreviewLength; ++i)
                    ss.Rows.Add(sheet.Row(i));
                tmp.Sheets.Add(ss);
            }
            mTarget = tmp;
        }
    }

    void DrawText()
    {
        string luaFile = File.ReadAllText(assetPath);
        string text;
        if (base.targets.Length > 1)
        {
            text = Path.GetFileName(assetPath);
        }
        else
        {
            var MaxTextPreviewLength = DefaultAssetConfig.Instance().MConfig.MaxTextPreviewLength;
            text = luaFile;
            if (text.Length > MaxTextPreviewLength + 3)
            {
                text = text.Substring(0, MaxTextPreviewLength) + "...";
            }
        }

        GUIStyle style = "ScriptText";
        Rect rect = GUILayoutUtility.GetRect(new GUIContent(text), style);
        rect.x = 0f;
        rect.y -= 3f;
        rect.width = EditorGUIUtility.currentViewWidth + 1f;
        GUI.Box(rect, text, style);
    }

    public override void OnInspectorGUI()
    {
        DefaultAssetConfig.Instance().MConfig.Draw();

        bool enabled = GUI.enabled;
        GUI.enabled = true;
        if(assetPath.IsExcel())
        {
            
            if (mTarget != null)
            {
                (mTarget as BookStruct).Draw();
            }
        }
        else if (assetPath.IsText())
        {
            DrawText();
        }
        GUI.enabled = enabled;
    }
}