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

public static class NpoiExtension
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
}

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
