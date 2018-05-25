#if UNITY_EDITOR

using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

using UnityEditor;
using UnityEngine;

class Excel2Lua : SingletonAsset<Excel2Lua>
{

    [Serializable]
    public class Config : InspectorDraw
    {
        public string InPath = "test";
        public string OutPath = "out";
        public long LastBuildTime = 0;
        public bool Rebuild = false;
    }

    [MenuItem("Tools/Create Excel2Lua.asset")]
    public static void Create()
    {
        mInstance = null;
        Instance();
    }

    [SerializeField, HideInInspector]
    Config mConfig = new Config() { Name = "Config" };

    public static bool writeLua(string sheetname, ISheet sheet, string path)
    {
        IRow headerRow = sheet.GetRow(1);
        int columnCount = headerRow.LastCellNum;
        int rowCount = sheet.LastRowNum;
        string columnIdxs = "\nlocal ColumnIdx={";

        for (int i = 0; i < columnCount; ++i)
        {
            string head = headerRow.GetCell(i).StringCellValue.RReplace(PathUtils.PunctuationRegex + "+", "_");
            columnIdxs += "\n\t" + head + "=" + (i + 1) + ",";
        }
        columnIdxs += "\n}\n";

        string body = "";
        try
        {
            for (int i = 2; i < rowCount; ++i)
            {
                IRow row = sheet.GetRow(i);
                if (row == null)
                    continue;
                var cell0 = row.GetCell(0);
                if (cell0.CellType == CellType.Blank
                    || (cell0.CellType == CellType.String && cell0.StringCellValue == ""))
                    continue;

                body += "\n\t[" + cell0.ToString() + "]" + "={";
                for (int j = 0; j < row.LastCellNum; ++j)
                {
                    var cell = row.GetCell(j) ?? row.CreateCell(j);
                    body += cell.SValue() + ",\t";
                }
                body += "},";
            }
        }
        catch (Exception e)
        {
            AppLog.d(e);
        }

        string tail = "\nfor k,v in pairs(" + sheetname + ") do"
            + "\n\tif type(v) == \"table\" then"
            + "\n\t\tsetmetatable(v,{"
            + "\n\t\t__newindex=function(t,kk) print(\"warning: attempte to change a readonly table\") end,"
            + "\n\t\t__index=function(t,kk)"
            + "\n\t\t\tif ColumnIdx[kk] ~= nil then"
            + "\n\t\t\t\treturn t[ColumnIdx[kk]]"
            + "\n\t\t\telse"
            + "\n\t\t\t\tprint(\"err: \\\"" + sheetname + "\\\" have no field [\"..kk..\"]\")"
            + "\n\t\t\t\treturn nil"
            + "\n\t\t\tend"
            + "\n\t\tend})"
            + "\n\tend"
            + "\nend";

        string strLua = "-- usage: \n--\t" + sheetname + "[id][ColumnName] \n--\t" + sheetname + "[id].ColumnName\n";

        strLua += columnIdxs;

        strLua += "\nlocal " + sheetname + "={";
        strLua += body;
        strLua += "\n}\n";

        strLua += tail;
        strLua += "\nreturn " + sheetname + "\n";

        path += "/" + sheetname + ".lua";
        UTF8Encoding utf8 = new UTF8Encoding(false);
        StreamWriter sw;
        using (sw = new StreamWriter(path, false, utf8))
        {
            sw.Write(strLua);
        }
        sw.Close();
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
                AppLog.d("building [" + file.Name + "] ...");

                var fstream = new FileStream(file.FullName, FileMode.Open);
                IWorkbook workbook = null;
                if (file.Name.EndsWith(".xlsx"))
                    workbook = new XSSFWorkbook(fstream);
                else
                    workbook = new HSSFWorkbook(fstream);

                for (int i = 0; i < workbook.NumberOfSheets; ++i)
                {
                    var sheet = workbook.GetSheetAt(i);
                    var sheetname = sheet.SheetName;

                    if (writeLua(sheetname.Replace("$", ""), sheet, mConfig.OutPath))
                    {
                        AppLog.d(file.Name + ": " + sheetname + " ok.");
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
            AppLog.e("{0} errors", errno);
            AppLog.e(errmsg);
        }

    }

    private void Build()
    {
        AppLog.d("{0} => {1}", mConfig.InPath, mConfig.OutPath);

        long lastWriteTime = mConfig.LastBuildTime;
        AppLog.d("上次转换时间：" + DateTime.FromFileTimeUtc(lastWriteTime));

        DirectoryInfo dir = new DirectoryInfo(mConfig.InPath);
        try
        {
            Build(dir.GetFiles("*.xls"));
            Build(dir.GetFiles("*.xlsx"));
        }
        catch (Exception e)
        {
            Console.Error.Write("error：" + e.Message);
        }

        mConfig.LastBuildTime = DateTime.Now.ToFileTimeUtc();
    }

    public static void DrawBuildButton()
    {
        var rect = EditorGUILayout.GetControlRect();
        if (GUI.Button(rect.Split(1, 3), "Excel2Lua"))
        {
            Instance().Build();
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
            mTarget.mConfig.Draw();

            Excel2Lua.DrawBuildButton();

            mTarget.DrawSaveButton();
        }
    }
}
#endif