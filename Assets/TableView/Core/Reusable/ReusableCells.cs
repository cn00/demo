using System;
using System.Collections.Generic;
using UnityEngine;

namespace TableView
{
    public class ReusableCells
    {
        #region Property

        private ReusableContainer reusableContainer;
        private readonly Dictionary<string, LinkedList<GameObject>> reusableCells;

        #endregion

        #region Public

        public ReusableCells()
        {
            reusableCells = new Dictionary<string, LinkedList<GameObject>>();
        }

        public void AddToParentTransform(RectTransform transform)
        {
            reusableContainer = new ReusableContainer(transform);
        }

        public void AddReusableCell(GameObject co)
        {
            var cell = co.GetComponentInChildren<TableViewCell>();
            if (!IsValidContainer()) return;
            if (string.IsNullOrEmpty(cell.ReuseIdentifier)) return;

            if (!reusableCells.ContainsKey(cell.ReuseIdentifier))
            {
                reusableCells.Add(cell.ReuseIdentifier, new LinkedList<GameObject>());
            }

            reusableCells[cell.ReuseIdentifier].AddLast(co);
            cell.transform.SetParent(reusableContainer.Container, false);
        }

        public GameObject GetReusableCell(string reuseIdentifier)
        {
            LinkedList<GameObject> reusableCellsList;

            if (!reusableCells.TryGetValue(reuseIdentifier, out reusableCellsList)) return null;
            if (reusableCellsList.Count == 0) return null;

            var reusableCell = reusableCellsList.First.Value;
            reusableCellsList.RemoveFirst();

            return reusableCell;
        }

        #endregion

        #region Private

        private bool IsValidContainer()
        {
            if (reusableContainer == null)
            {
                throw new Exception("Reusable Container can not be null !!!");
            }

            return true;
        }

        #endregion
    }
}