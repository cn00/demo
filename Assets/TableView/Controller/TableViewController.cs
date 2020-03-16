using TableView;
using UnityEngine;

namespace TableView
{
    [RequireComponent(typeof(TableView))]
    public class TableViewController : MonoBehaviour, ITableViewDataSource, ITableViewDelegate
    {
        public static string Tag = "TableViewController";
        public TableView tableView;
        public GameObject[] prefabCells;

        void Awake()
        {
            tableView.Delegate = this;
            tableView.DataSource = this;
        // }
        // void Start()
        // {
            foreach(var prefabCell in prefabCells)
                tableView.RegisterPrefabForCellReuseIdentifier(prefabCell);
        }

        public delegate int Count(TableView tableView);
        public Count GetDataCount;
        public int DataCount(TableView tableView)
        {
            return GetDataCount != null ? GetDataCount(tableView) : 1;
        }


        public delegate float CellSize(TableView tableView, int row);
        public CellSize GetCellSize = null;
        public float CellSizeAt(TableView tableView, int row)
        {
            if(GetCellSize!=null)
            {
                return GetCellSize(tableView, row);//Random.Range(50.0f, 200.0f);
            }
            else
            {
                AppLog.w(Tag, "delegate null");
                var cell = CellAt(tableView, row);
                return 100f;
            }
        }

        public delegate GameObject CellRow(TableView tableView, int row);
        public CellRow CellAtRow;
        public GameObject CellAt(TableView tableView, int row)
        {
            if(CellAtRow !=null)
            {
                return CellAtRow(tableView, row);
            }
            else
            {
                var cellController = prefabCells[0].GetComponent<TableViewCell>();
                var cell = tableView.ReusableCellForRow(cellController.ReuseIdentifier, row);
                cell.name = "Cell " + row;
                return cell;
            }
        }

        public delegate void OnHighlightRow(TableView tableView, int row);
        public event OnHighlightRow OnHighlight;
        public void OnHighlightAt(TableView tableView, int row)
        {
            if(OnHighlight != null )
                OnHighlight(tableView, row);
            return ;
        }

        public delegate void OnSelectRow(TableView tableView, int row);
        public event OnSelectRow OnSelect;
        public void OnSelectAt(TableView tableView, int row)
        {
            print("TableViewDidSelectCellForRow : " + row);
            if (OnSelect != null)
                OnSelect(tableView, row);
        }
    }
}