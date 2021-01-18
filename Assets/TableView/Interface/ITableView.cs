using UnityEngine;
using UnityEngine.SocialPlatforms;

namespace TableView
{
    public interface ITableView
    {
        Range VisibleRange { get; }
        float ContentSize { get; }
        float Position { get; }

        GameObject ReusableCellForRow(string reuseIdentifier, int row);
        GameObject CellForRow(int row);
        float PositionForRow(int row);
        void ReloadData();

        void SetPosition(float newPosition);
        void SetPosition(float newPosition, float time);

        void RegisterPrefabForCellReuseIdentifier(GameObject prefab);
    }
}