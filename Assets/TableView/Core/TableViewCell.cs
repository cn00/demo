using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace TableView
{
    public abstract class TableViewCell : MonoBehaviour, ISelectHandler, ISubmitHandler, IPointerClickHandler
    {
        #region Property

        private int rowNumber;
        public int RowNumber
        {
            get { return rowNumber; }
        }

        public string ReuseIdentifier;

        public event TableViewCellDidSelectEvent DidSelectEvent;
        public event TableViewCellDidSelectEvent DidPointClickEvent;
        public event TableViewCellDidHighlightEvent DidHighlightEvent;


        #endregion

        #region Lifecycle

        void Awake()
        {
            this.gameObject.AddComponent<Selectable>();
        }

        #endregion

        #region Public

        // public abstract void SetHighlighted();
        // public abstract void SetSelected();
        // public abstract void Display();

        public void SetRowNumber(int number)
        {
            rowNumber = number;

            // Display();
        }

        public void OnSelect(BaseEventData eventData)//ISelectHandler
        {
            // SetHighlighted();

            if (DidHighlightEvent == null)
                return;

            DidHighlightEvent.Invoke(rowNumber);
        }

        public void OnSubmit(BaseEventData eventData)//ISubmitHandler
        {
            // SetSelected();

            if (DidSelectEvent == null)
                return;

            DidSelectEvent.Invoke(rowNumber);
        }

        public void OnPointerClick(PointerEventData eventData)//IPointerClickHandler
        {
            // SetSelected();

            if (DidPointClickEvent == null)
                return;

            DidPointClickEvent.Invoke(rowNumber);
        }

        #endregion
    }
}