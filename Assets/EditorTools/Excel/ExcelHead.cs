using NPOI.SS.UserModel;

public enum HeadIdx
{
    jp,
    trans,
    trans_jd,
    i,
    j,
    SheetName,
    
    FilePath,

    Count
}
public class ExcelHead
{
    public int[] hidx;

    public int this[HeadIdx i]
    {
        get { return hidx[(int)i]; }
    }

    public ExcelHead(IRow hrow)
    {
        hidx = new int[(int)HeadIdx.Count];
        for(int i = 0; i < hrow.LastCellNum; ++i)
        {
            var v = hrow.Cell(i);
            for(int j = 0; j < (int)HeadIdx.Count; ++j)
            {
                if(v.StringCellValue == ((HeadIdx)j).ToString())
                {
                    hidx[j] = i;
                    break;
                }
            }
        }

    }
}
