#if UNITY_EDITOR

using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Linq;
using System.IO;//wfExcel
using System.Text;
using System.Collections.Generic;

using UnityEditor;
using UnityEngine;

class Excel2Lua : SingletonAsset<Excel2Lua>
{
    const string Tag = "Excel2Lua";
    [Serializable]
    public class ExcelConfig
    {
        public string Path = "";
        public long LastBuildTime = 0;
    }

    [Serializable]
    public class Config
    {
        public string InPath = "test";
        public string OutPath = "out";
        public long LastBuildTime = 0;
        public bool Rebuild = false;
    }

    [MenuItem("Tools/Create/Excel2Lua.asset")]
    public static void Create()
    {
        mInstance = null;
        Instance();
    }

    public static int BookConfig(string path)
    {
        var book = ExcelUtils.Open(path);
        if(book == null)
            return -1;

        var config = book.GetSheet("config");
        if(config == null)
        {
            config = book.Sheet("config");
            var head = config.Row(0);
            int hidx = -1;
            var snaidx = ++hidx;
            var keyidx = ++hidx;
            var validx = ++hidx;
            head.Cell(snaidx).SetCellValue("sheet");
            head.Cell(keyidx).SetCellValue("key");
            head.Cell(validx).SetCellValue("value");
            var idx = 0;
            foreach(var sheet in book.AllSheets().Where(i => i.SheetName != "config"))
            {
                ++idx;
                var row = config.Row(idx);
                row.Cell(snaidx).SetCellValue(sheet.SheetName);
                row.Cell(keyidx).SetCellValue("headerRowIdx");
                row.Cell(validx).SetCellValue(0);
            }
            book.Write(path);
        }

        return 0;
    }

    [SerializeField, HideInInspector]
    Config mConfig = new Config();


    public static bool writeLua(ISheet sheet, string path, int headerRowIdx = 0)
    {
        string sheetname = sheet.SheetName.RReplace(PathUtils.PunctuationRegex + "+", "_").TrimEnd('_');
        StringBuilder luabuild = new StringBuilder(2048000);
        luabuild.Append("-- usage: \n--\t" + sheetname + "[id][ColumnName] \n--\t" + sheetname + "[id].ColumnName\n");

        //columnIdxs
        IRow headerRow = sheet.GetRow(headerRowIdx);
        int columnCount = headerRow.LastCellNum;
        int rowCount = sheet.LastRowNum;
        luabuild.Append("\nlocal ColumnIdx={");
        for (int i = 0; i < columnCount; ++i)
        {
            var cell = headerRow.Cell(i);
            var v = cell.SValue().RReplace(PathUtils.PunctuationRegex + "+", "_");
            // if(cell.CellType == CellType.String || (cell.CellType == CellType.Formula && cell.CachedFormulaResultType ==  CellType.String))
            v = "\"" + v + "\"";
            luabuild.Append("\n\t[" + v + "]=" + (i + 1) + ",");
        }
        luabuild.Append("\n}\n");

        // body
        luabuild.Append("\nlocal " + sheetname + "={");
        try
        {
            for (int i = headerRowIdx + 1; i < rowCount; ++i)
            {
                IRow row = sheet.GetRow(i);
                if (row == null)
                    continue;
                var cell0 = row.Cell(0);
                if (cell0.CellType == CellType.Blank
                    || (cell0.CellType == CellType.String && cell0.StringCellValue == ""))
                    continue;

                var v0 = cell0.SValue();
                if(cell0.CellType == CellType.String
                    || (cell0.CellType == CellType.Formula && cell0.CachedFormulaResultType ==  CellType.String))
                    v0 = "\"" + v0 + "\"";
                luabuild.Append("\n\t[" + v0 + "]" + "={" + v0);
                for (int j = 1; j < row.LastCellNum; ++j)
                {
                    var cell = row.Cell(j);
                    var v = cell.SValueOneline();
                    if(cell.CellType == CellType.Blank)
                        v = "nil";
                    else if(cell.CellType == CellType.String
                        || (cell.CellType == CellType.Formula && cell.CachedFormulaResultType ==  CellType.String))
                        v = "\"" + v + "\"";
                    luabuild.Append(",\t" + v);
                }
                luabuild.Append("},");
            }
        }
        catch (Exception e)
        {
            Debug.LogErrorFormat(Tag+ e);
        }
        luabuild.Append("\n}\n");

        //tail
        luabuild.Append("\nfor k,v in pairs(" + sheetname + ") do")
            .Append("\n\tif type(v) == \"table\" then")
            .Append("\n\t\tsetmetatable(v,{")
            .Append("\n\t\t__newindex=function(t,kk) print(\"warning: attempte to change a readonly table\") end,")
            .Append("\n\t\t__index=function(t,kk)")
            .Append("\n\t\t\tif ColumnIdx[kk] ~= nil then")
            .Append("\n\t\t\t\treturn t[ColumnIdx[kk]]")
            .Append("\n\t\t\telse")
            .Append("\n\t\t\t\tprint(\"err: \\\"" + sheetname + "\\\" have no field [\"..kk..\"]\")")
            .Append("\n\t\t\t\treturn nil")
            .Append("\n\t\t\tend")
            .Append("\n\t\tend})")
            .Append("\n\tend")
            .Append("\nend");

        luabuild.Append("\nreturn " + sheetname + "\n");

        path += "/" + sheetname + ".lua";

        var dir = path.upath().Substring(0, path.LastIndexOf('/'));
        if(!Directory.Exists(dir))
            Directory.CreateDirectory(dir);
        File.WriteAllText(path, luabuild.ToString());
        Debug.LogFormat($"{Tag}, {path}");
        return true;
    }


    private void Build(FileInfo[] excels)
    {
        int errno = 0;
        string errmsg = "error list:\n";
        foreach (FileInfo file in excels)
        {
            if (file.LastWriteTime.ToFileTimeUtc() > mConfig.LastBuildTime || mConfig.Rebuild)
            {
                UnityEngine.Debug.Log(Tag+": building [" + file.Name + "] ...");

                IWorkbook workbook = ExcelUtils.Open(file.FullName);

                var config = workbook.GetSheet("config");
                for (int i = 0; i < workbook.NumberOfSheets; ++i)
                {
                    var sheet = workbook.GetSheetAt(i);
                    var sheetname = sheet.SheetName;
                    if(sheetname == "config")
                        continue;

                    var headerRowIdx = 0;
                    if(config != null)
                    {
                        var cr = config.Select(r => r.Cell(0).SValue() == sheetname && r.Cell(1).SValue() == "headerRowIdx");
                        if(cr.Count > 0)
                            headerRowIdx = (int)cr.First().Cell(2).NumericCellValue;
                    }

                    var OutPath = mConfig.OutPath;
                    if(workbook.NumberOfSheets > 1)
                    {
                        OutPath += "/" + file.Name.Substring(0, file.Name.LastIndexOf('.')).RReplace(PathUtils.PunctuationRegex + "+", "_");
                    }

                    if (writeLua(sheet, OutPath, headerRowIdx))
                    {
                        Debug.LogFormat(file.Name + ": " + sheetname + " ok.");
                    }
                    else
                    {
                        errmsg += file.Name + ": " + sheetname + "\n";
                        ++errno;
                    }
                }
            }
        } // for

        if (errno > 0)
        {
            UnityEngine.Debug.LogError(Tag+$": {errno} {errmsg}");
        }

    }

    private void Build()
    {
        UnityEngine.Debug.LogFormat(Tag+": {0} => {1}", mConfig.InPath, mConfig.OutPath);

        long lastWriteTime = mConfig.LastBuildTime;
        UnityEngine.Debug.LogFormat(Tag+": 上次转换时间：" + DateTime.FromFileTimeUtc(lastWriteTime));

        DirectoryInfo dir = new DirectoryInfo(mConfig.InPath);
        // try
        // {
            Build(dir.GetFiles("*.xls", SearchOption.AllDirectories));
            Build(dir.GetFiles("*.xlsx", SearchOption.AllDirectories));
        // }
        // catch (Exception LogErrorFormat)
        // {
        //     Debug.LogError(Tag+": error：" + LogErrorFormat.Message);
        // }

        mConfig.LastBuildTime = DateTime.Now.ToFileTimeUtc();
    }

    public static void DrawBuildButton()
    {
        var rect = EditorGUILayout.GetControlRect();
        if (GUI.Button(rect.Split(1, 3), "Excel2Lua"))
        {
            Instance().Build();
        }
        if (GUI.Button(rect.Split(2, 3), "Config"))
        {
            foreach(var f in Directory.GetFiles(Instance().mConfig.InPath, "*.xlsx", SearchOption.AllDirectories))
            {
                Excel2Lua.BookConfig(f);
            }
        }
    }


    [CustomEditor(typeof(Excel2Lua))]
    public class Editor : UnityEditor.Editor
    {
        Excel2Lua mTarget = null;
        /// <summary>
        /// This function is called when the object becomes enabled and active.
        /// </summary>
        void OnEnable()
        {
            mTarget = target as Excel2Lua;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            Inspector.DrawComObj("Config", mTarget.mConfig);

            Excel2Lua.DrawBuildButton();

            mTarget.DrawSaveButton();
        }
    }
}
#endif