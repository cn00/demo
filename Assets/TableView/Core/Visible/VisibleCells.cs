using System.Collections.Generic;
using UnityEngine.SocialPlatforms;
using UnityEngine;

namespace TableView
{
    public class VisibleCells
    {
        #region Property

        public Range Range;
        private Dictionary<int, GameObject> cells;

        public int Count
        {
            get { return cells.Count; }
        }

        #endregion

        #region Public

        public VisibleCells()
        {
            Range = new Range(0, 0);
            cells = new Dictionary<int, GameObject>();
        }

        public GameObject GetCellAtIndex(int index)
        {
            GameObject cell = null;
            cells.TryGetValue(index, out cell);
            return cell;
        }

        public void SetCellAtIndex(int index, GameObject cell)
        {
            cells[index] = cell;
        }

        public void RemoveCellAtIndex(int index)
        {
            cells.Remove(index);
        }

        #endregion
    }
}