#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

using UnityEditor;

public class ExcelTranslate : SingletonAsset<ExcelTranslate>
{
    const string JPRegular = "[\u3021-\u3126]";
    public string ExcelDataDir = "Assets/Application/Resource/ExcelData";
    public string OutJPDir = "Assets/Application/Resource/ExcelData_jp";
    public string TranslatedDir = "Assets/Application/Resource/ExcelData_cn";

    [Serializable]
    public class SubConfig : InspectorDraw
    {
        public long CountWordUniq = 0L;
        public long CountWord = 0L;
        public List<string> Pathes = new List<string>();

        public void CollectJp(string outRoot = null)
        {
            if (string.IsNullOrWhiteSpace(outRoot))
            {
                outRoot = Instance().OutJPDir;
            }

            try
            {
                var outExcelPath = outRoot + "/" + Name.Substring(Name.LastIndexOf('/') + 1) + ".xlsx";
                var outTemplatePath = outRoot + "/template.xlsx";
                // 从模板创建输出文件
                var outBook = ExcelUtils.Open(outTemplatePath);
                var outSheet = outBook.Sheet("jp") as XSSFSheet;//new XSSFSheet();//

                var infoSheet = outBook.Sheet("info") as XSSFSheet;

                var hrow = outSheet.GetRow(0);
                var head = new ExcelHead(hrow);

                //遍历文件列表输出到输出文件
                // 每 MaxRowPerSheet 条分一张表
                int MaxrowPerSheet = 50000;
                int outSheetRowIdx = 0;
                int outSheetIdx = 0;
                int totalCellCount = 0;
                int inFileIdx = 0;
                int inSheetIdx = 0;
                int MaxRowAndColumnNum = 90000;
                long wordCount = 0;
                long totalWordCount = 0;
                foreach (var path in Pathes)
                {
                    ++inFileIdx;
                    IWorkbook inbook = ExcelUtils.Open(path);
                    inSheetIdx = 0;

                    var infoCell = infoSheet.Cell(inFileIdx, inSheetIdx);
                    infoCell.SetCellValue(path);

                    foreach (var sheet in inbook.AllSheets())
                    {

                        ++inSheetIdx;
                        for (var ir = 1; ir <= sheet.LastRowNum && ir <= MaxRowAndColumnNum; ++ir)
                        {
                            var row = sheet.GetRow(ir);
                            if(row == null)
                                continue;
                            if((ir % 100) == 0)
                            {
                                EditorUtility.DisplayCancelableProgressBar(
                                    "CollectJp ..." + path.RReplace(".*ExcelData/", "")
                                    , sheet.SheetName + ": " + ir + "/" + sheet.LastRowNum
                                    , (float)(inFileIdx) / Pathes.Count());
                            }

                            for (int ic = 0; ic < row.LastCellNum && ic <= MaxRowAndColumnNum; ++ic)
                            {
                                // // 分表
                                // if (outSheetRowIdx > MaxrowPerSheet)
                                // {
                                //     ++outSheetIdx;
                                //     outSheetRowIdx = 0;
                                //     outSheet = outBook.CreateSheet("jp_" + outSheetIdx) as XSSFSheet;
                                //     // add head
                                //     var h = outSheet.Row(0);
                                //     h.RowStyle = hrow.RowStyle;
                                //     for (var hi = 0; hi < hrow.LastCellNum; ++hi)
                                //     {
                                //         h.Cell(hi).SetCellValue(hrow.Cell(hi).SafeSValue());
                                //         outSheet.SetColumnWidth(hi, 1500);
                                //     }
                                // }

                                var v = row.Cell(ic).SafeSValue();
                                // AppLog.d("{0}: {1}", v, v.Length);
                                var matches = Regex.Matches(v, JPRegular + "+.*");
                                if (matches.Count > 0)
                                {
                                    ++totalCellCount;
                                    ++outSheetRowIdx;

                                    var c = outSheet.Cell(outSheetRowIdx, head[HeadIdx.jp]);
                                    // 去重引用
                                    var iorow = outSheet.Contain(r => r.Cell(head[HeadIdx.jp]).SafeSValue() == v);
                                    if(iorow > 0)
                                    {
                                        c.SetCellValue(string.Format("${0}", iorow));
                                    }
                                    else
                                    {
                                        wordCount += v.Length;
                                        c.SetCellValue(v);
                                    }
                                    //c.CellStyle.IsLocked = cellLock;
                                    totalWordCount += v.Length;

                                    c = outSheet.Cell(outSheetRowIdx, head[HeadIdx.trans]);
                                    c.SetCellValue("译文");
                                    //c.CellStyle.IsLocked = false;

                                    c = outSheet.Cell(outSheetRowIdx, head[HeadIdx.trans_jd]);
                                    c.SetCellValue("校对");
                                    //c.CellStyle.IsLocked = false;

                                    c = outSheet.Cell(outSheetRowIdx, head[HeadIdx.i]);
                                    c.SetCellValue(ir);
                                    //c.CellStyle.IsLocked = cellLock;

                                    c = outSheet.Cell(outSheetRowIdx, head[HeadIdx.j]);
                                    c.SetCellValue(ic);
                                    //c.CellStyle.IsLocked = cellLock;

                                    c = outSheet.Cell(outSheetRowIdx, head[HeadIdx.SheetName]);
                                    c.SetCellValue(sheet.SheetName);
                                    //c.CellStyle.IsLocked = cellLock;

                                    c = outSheet.Cell(outSheetRowIdx, head[HeadIdx.FilePath]);
                                    c.SetCellValue(path);
                                }

                            } // cell
                        } // row

                        // add file sheet info to info sheet
                        // one line per file
                        infoCell = infoSheet.Cell(inFileIdx, inSheetIdx);
                        infoCell.SetCellValue(sheet.SheetName);
                    } // sheet
                } // path

                infoSheet.Cell(inFileIdx + 1, 0).SetCellValue("wordCount");
                infoSheet.Cell(inFileIdx + 1, 1).SetCellValue(wordCount);

                var outStream = new FileStream(outExcelPath, FileMode.Create);
                outBook.Write(outStream);
                AppLog.d("CollectJp: " + outExcelPath);

                CountWordUniq = wordCount;
                CountWord = totalWordCount;
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }
        }

        public void Translate(string outRoot = null)
        {
            if (string.IsNullOrWhiteSpace(outRoot))
            {
                outRoot = Instance().TranslatedDir;
            }

            try
            {
                var transExcelPath = outRoot + "/" + Name.Substring(Name.LastIndexOf('/') + 1) + ".xlsx";
                var transStream = new FileStream(transExcelPath, FileMode.Open);
                
                var transBook = new XSSFWorkbook(transStream);
                transStream.Dispose();

                var infoSheet = transBook.Sheet("info") as XSSFSheet;
                var transSheet = transBook.Sheet("jp") as XSSFSheet;

                var hrow = transSheet.GetRow(0);
                var head = new ExcelHead(hrow);

                int inFileIdx = 0;
                foreach (var path in Pathes)
                {
                    ++inFileIdx;
                    EditorUtility.DisplayCancelableProgressBar(
                        "Translate ..."
                        , path.Replace("Assets/Application/Resource/ExcelData/", "")
                        , (float)(inFileIdx) / Pathes.Count());

                    var inbook = ExcelUtils.Open(path);
                    var rows = transSheet.Select(r=>r.Cell(head[HeadIdx.FilePath]).SafeSValue() == path);
                    foreach(var trrow in rows)
                    {
                        var trans = trrow.Cell(head[HeadIdx.trans]).SafeSValue();
                        var jp = trrow.Cell(head[HeadIdx.jp]).SafeSValue();
                        if(jp[0] == '$')
                        {
                            AppLog.d("使用引用: " + jp);
                            var row = transSheet.Row(int.Parse(jp.Substring(1)));
                            jp = row.Cell(head[HeadIdx.jp]).SafeSValue();
                            trans = row.Cell(head[HeadIdx.trans]).SafeSValue();
                        }

                        var sheetName = trrow.Cell(head[HeadIdx.SheetName]).SafeSValue();
                        var sheet = inbook.Sheet(sheetName);
                        var i = (int)trrow.Cell(head[HeadIdx.i]).NumericCellValue;
                        var j = (int)trrow.Cell(head[HeadIdx.j]).NumericCellValue;
                        var cell = sheet.Cell(i, j);
                        var ojp = cell.SafeSValue();
                        if(ojp == jp)
                        {
                            cell.SetCellValue(trans
                                .Replace("\\n", "\n")
                                .Replace("\\t", "\t")
                                .Replace("\\\"", "\""));
                        }
                        else
                        {
                            AppLog.e(i, j, ojp, " != ", jp, trans);
                        }
                    }
                    inbook.Write(path);
                } // path

                AppLog.d("Translate: " + transExcelPath);
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }
        }

        public override void DrawInspector(int indent, GUILayoutOption[] guiOpts)
        {
            var rect = EditorGUILayout.GetControlRect();
            var sn = 5;
            var idx = -1;
            if (GUI.Button(rect.Split(++idx, sn), "CollectJp"))
            {
                CollectJp();
            }
            if (GUI.Button(rect.Split(++idx, sn), "Translate"))
            {
                Translate();
            }
            base.DrawInspector();
        }

    } // class

    [Serializable]
    public class RootConfig : InspectorDraw
    {
        public long CountWord = 0L;

        [SerializeField]
        public List<SubConfig> Groups = new List<SubConfig>();


        public override void DrawInspector(int indent, GUILayoutOption[] guiOpts)
        {
            base.DrawInspector();
            ++EditorGUI.indentLevel;
            foreach (var f in Groups)
            {
                f.Draw();
            }
            --EditorGUI.indentLevel;
        }

    }

    [SerializeField, HideInInspector]
    public RootConfig mRootconfig = new RootConfig();

    public TextAsset DiffListAsset = null;

    [MenuItem("Tools/Create/ExcelTranslate.asset")]
    public static ExcelTranslate Create()
    {
        mInstance = null;
        return Instance();
    }


    [CustomEditor(typeof(ExcelTranslate))]
    public class Editor : UnityEditor.Editor
    {
        ExcelTranslate mTarget = null;
        void OnEnable()
        {
            mTarget = target as ExcelTranslate;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var rect = EditorGUILayout.GetControlRect();
            var sn = 5;
            var idx = -1;
            if (GUI.Button(rect.Split(++idx, sn), "Refresh"))
            {
                mTarget.mRootconfig.Groups.Clear();
                var groups = Directory.GetDirectories(mTarget.ExcelDataDir, "*", SearchOption.TopDirectoryOnly);
                foreach (var i in groups)
                {
                    var subConfig = new ExcelTranslate.SubConfig();
                    subConfig.Name = i;
                    var xls = i.GetFiles("*.xls|*.xlsx", SearchOption.AllDirectories)
                        .Where(ii => mTarget.DiffListAsset.text.Contains(ii));
                    subConfig.Pathes = xls.ToList();
                    if (subConfig.Pathes.Count() > 0)
                        mTarget.mRootconfig.Groups.Add(subConfig);
                    AppLog.d(i + ": " + xls.Count());
                }

                EditorUtility.SetDirty(mTarget);
            }
            if (GUI.Button(rect.Split(++idx, sn), "CollectJp"))
            {
                long sum = 0L;
                foreach (var i in mTarget.mRootconfig.Groups)
                {
                    i.CollectJp();
                    sum += i.CountWordUniq;
                }
                mTarget.mRootconfig.CountWord = sum;
            }
            if (GUI.Button(rect.Split(++idx, sn), "Translate"))
            {
                foreach (var i in mTarget.mRootconfig.Groups)
                {
                    i.Translate();
                }
            }
            // if (GUI.Button(rect.Split(++idx, sn), "iOS.proj.sim"))
            // {
            //     BuildScript.BuildIosIL2cppProjSim();
            // }
            mTarget.mRootconfig.Draw();
        }
    }
}
#endif
