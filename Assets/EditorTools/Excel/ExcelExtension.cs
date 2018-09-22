using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif // UNITY_EDITOR

public static class ExcelUtils
{
    public static string SValue(this ICell cell, CellType? FormulaResultType = null)
    {
        string svalue = "";
        var cellType = FormulaResultType ?? cell.CellType;
        switch (cellType)
        {
            case CellType.Unknown:
                svalue = "Unknown";
                break;
            case CellType.Numeric:
                svalue = cell.NumericCellValue.ToString();
                break;
            case CellType.String:
                svalue = cell.StringCellValue;
                break;
            case CellType.Formula:
                svalue = cell.SValue(cell.CachedFormulaResultType);
                break;
            case CellType.Blank:
                svalue = "";
                break;
            case CellType.Boolean:
                svalue = cell.BooleanCellValue.ToString();
                break;
            case CellType.Error:
                svalue = "Error";
                break;
            default:
                break;
        }
        return svalue;
    }
    public static string SafeSValue(this ICell self, CellType? FormulaResultType = null)
    {
        return self.SValue()
                .Replace("\n", "\\n")
                .Replace("\t", "\\t")
                .Replace("\"", "\\\"");
    }
    public static List<ISheet> AllSheets(this IWorkbook self)
    {
        List<ISheet> sheets = new List<ISheet>();
        if (self is HSSFWorkbook)
        {
            HSSFWorkbook book = self as HSSFWorkbook;
            for (int i = 0; i < book.NumberOfSheets; ++i)
            {
                sheets.Add(book.GetSheetAt(i));
            }
        }
        else if (self is XSSFWorkbook)
        {
            XSSFWorkbook book = self as XSSFWorkbook;
            for (int i = 0; i < book.NumberOfSheets; ++i)
            {
                sheets.Add(book.GetSheetAt(i));
            }
        }
        return sheets;
    }

    public static IWorkbook Open(string path)
    {
        // AppLog.d(path);
        var stream = new FileStream(path, FileMode.Open);
        stream.Position = 0;
        IWorkbook inbook = null;
        if (path.EndsWith("xls"))
        {
            inbook = new HSSFWorkbook(stream);
        }
        else if (path.EndsWith(".xlsx"))
        {
            inbook = new XSSFWorkbook(stream);
        };
        stream.Dispose();
        return inbook;
    }

    public static void Write(this IWorkbook self, string path)
    {
        var stream = new FileStream(path, FileMode.Create);
        stream.Position = 0;
        self.Write(stream);
        stream.Close();
    }

    public static ISheet Sheet(this IWorkbook self, string name)
    {
        var t = self.GetSheet(name);
        if (null == t)
            t = self.CreateSheet(name);
        return t;
    }

    public static List<IRow> Select(this ISheet self, Func<IRow, bool> condition)
    {
        var rows = new List<IRow>();
        for (var i = 0; i <= self.LastRowNum; ++i)
        {
            var r = self.GetRow(i);
            if (r != null && condition(r))
            {
                rows.Add(r);
            }
        }
        return rows;
    }

    // Summary:
    //     return row index.
    //
    // Parameters:
    public static int Contain(this ISheet self, Func<IRow, bool> condition)
    {
        for (var i = 0; i <= self.LastRowNum; ++i)
        {
            var r = self.GetRow(i);
            if (r != null && condition(r))
            {
                return i;
            }
        }
        return -1;
    }

    public static void Draw(this IRow self, GUILayoutOption[] guiOpts = null)
    {
        self.Draw(0, self.Count(), guiOpts);
    }
    public static void Draw(this IRow self, int begin = 0, int end = 0x7fffffff, GUILayoutOption[] guiOpts = null, bool newRow = true)
    {
        #if UNITY_EDITOR
        if (newRow) EditorGUILayout.BeginHorizontal();
        {
            var width = 30;
            if (newRow) EditorGUILayout.LabelField((self.Cell(0).RowIndex + 1).ToString(), new GUILayoutOption[]
            {
                GUILayout.Width(width),
                GUILayout.ExpandWidth(false),
            });
            for (var i = begin; i < end; ++i)
            {
                self.Cell(i).Draw(null, guiOpts);
            }
        }
        if (newRow) EditorGUILayout.EndHorizontal();
        #endif
    }

    public static IRow Row(this ISheet self, int i)
    {
        var t = self.GetRow(i);
        if (null == t)
            t = self.CreateRow(i);
        return t;
    }

    public static List<IRow> Rows(this ISheet self, int begin, int end)
    {
        var t = new List<IRow>();
        for(var i = begin; i < end; ++i)
        {
            t.Add(self.Row(i));
        }
        return t;
    }

    public static ICell Cell(this IRow self, int i)
    {
        var t = self.GetCell(i);
        if (null == t)
            t = self.CreateCell(i);
        return t;
    }
    public static ICell Cell(this ISheet sheet, int i, int j)
    {
        return sheet.Row(i).Cell(j);
    }

    public static void Draw(this ICell self, CellType? FormulaResultType = null, GUILayoutOption[] guiOpts = null)
    {
        #if UNITY_EDITOR
        var cellType = FormulaResultType ?? self.CellType;
        switch (cellType)
        {
            case CellType.Unknown:
                self.SetCellValue(EditorGUILayout.DelayedTextField(self.SafeSValue(), guiOpts));
                break;
            case CellType.Numeric:
                self.SetCellValue(EditorGUILayout.DelayedDoubleField(self.NumericCellValue, guiOpts));
                break;
            case CellType.String:
                self.SetCellValue(EditorGUILayout.DelayedTextField(self.StringCellValue, guiOpts));
                break;
            case CellType.Formula:
                EditorGUILayout.LabelField(self.SValue() + "=" + self.CellFormula, guiOpts);
                break;
            case CellType.Blank:
                self.SetCellValue(EditorGUILayout.DelayedTextField(self.StringCellValue, guiOpts));
                break;
            case CellType.Boolean:
                self.SetCellValue(EditorGUILayout.Toggle(self.BooleanCellValue, guiOpts));
                break;
            case CellType.Error:
                self.SetCellValue(EditorGUILayout.DelayedTextField(self.SafeSValue(), guiOpts));
                break;
            default:
                break;
        }
        #endif
    }

}
