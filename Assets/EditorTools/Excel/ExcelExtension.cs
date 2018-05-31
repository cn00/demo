using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Collections.Generic;



public static class ExcelUtils
{
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
        AppLog.d(path);
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

    public static IRow Row(this ISheet self, int i)
    {
        var t = self.GetRow(i);
        if (null == t)
            t = self.CreateRow(i);
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

}
