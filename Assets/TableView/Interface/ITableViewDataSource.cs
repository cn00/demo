using UnityEngine;

namespace TableView
{
    public interface ITableViewDataSource
    {
        int DataCount(TableView tableView);
        float CellSizeAt(TableView tableView, int row);
        GameObject CellAt(TableView tableView, int row);
    }
}