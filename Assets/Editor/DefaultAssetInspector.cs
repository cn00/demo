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
    public ISheet Sheet { get; set; }

    [Range(0, 99999999)]
    public uint RowIdxA = 1;

    [Range(0, 99999999)]
    public uint RowPerPage = 10;
    public override void DrawInspector(int indent = 0, GUILayoutOption[] guiOpts = null)
    {
        base.DrawInspector();
        var rect = EditorGUILayout.GetControlRect();
        var sn = 5;
        var idx = -1;
        if (GUI.Button(rect.Split(++idx, sn), "PageDown"))
        {
            RowIdxA += RowPerPage;
        }
        if (GUI.Button(rect.Split(++idx, sn), "PageUp"))
        {
            RowIdxA -= RowPerPage;
        }

        var head = Sheet.Row(0);
        head.Draw(guiOpts);
        var Rows = Sheet.Rows((int)RowIdxA, (int)(RowIdxA + RowPerPage));
        foreach (var r in Rows)
        {
            r.Draw(0, head.Count(), guiOpts);
        }
    }
}

public class BookStruct : InspectorDraw
{
    public IWorkbook Book {get; set;}
    public List<SheetStruct> Sheets = new List<SheetStruct>();
    public override void DrawInspector(int indent = 0, GUILayoutOption[] guiOpts = null)
    {
        base.DrawInspector();
        var rect = EditorGUILayout.GetControlRect();
        var sn = 5;
        var idx = -1;
        if (GUI.Button(rect.Split(++idx, sn), "Save"))
        {
            Book.Write(Name);
        }

        guiOpts = new GUILayoutOption[]
        {
                GUILayout.Width(30),
                GUILayout.ExpandWidth(true),
        };
        foreach (var s in Sheets)
        {
            s.Draw(0, guiOpts);
        }

        if(GUI.changed)
        {
            if (Book is XSSFWorkbook)
            {
                XSSFFormulaEvaluator.EvaluateAllFormulaCells(Book);
            }
            else
            {
                HSSFFormulaEvaluator.EvaluateAllFormulaCells(Book);
            }
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
            var book = ExcelUtils.Open(assetPath);
            var tmp = new BookStruct(){FoldOut = true};
            tmp.Name = assetPath;
            tmp.Book = book;
            var MaxSheetPreviewLength = DefaultAssetConfig.Instance().MConfig.MaxSheetPreviewLength;
            foreach(var sheet in book.AllSheets())
            {
                var ss = new SheetStruct(){FoldOut = true };
                ss.Name = sheet.SheetName;
                ss.Sheet = sheet;
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