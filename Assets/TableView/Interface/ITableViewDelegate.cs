namespace TableView
{
    public interface ITableViewDelegate
    {
        void OnHighlightAt(TableView tableView, int row);
        void OnSelectAt(TableView tableView, int row);
    }
}