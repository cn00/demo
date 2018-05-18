using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.IO;
using System.Text;

public static class CellExtension
{
    public static string SValue(this ICell cell, CellType? FormulaResultType = null)
    {
        string svalue = "";
        var cellType = FormulaResultType ?? cell.CellType;
        switch (cellType)
        {
            case CellType.Unknown:
                svalue = "nil";
                break;
            case CellType.Numeric:
                svalue = cell.NumericCellValue.ToString();
                break;
            case CellType.String:
                svalue = "\"" + cell.StringCellValue
                    .Replace("\n", "\\n")
                    .Replace("\t", "\\t")
                    .Replace("\"", "\\\"") + "\"";
                break;
            case CellType.Formula:
                svalue = cell.SValue(cell.CachedFormulaResultType);
                break;
            case CellType.Blank:
                svalue = "nil";
                break;
            case CellType.Boolean:
                svalue = cell.BooleanCellValue.ToString();
                break;
            case CellType.Error:
                svalue = "nil";
                break;
            default:
                break;
        }
        return svalue;
    }
}
