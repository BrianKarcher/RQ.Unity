using RQ.FSM.V2.Conditionals;
using RQ.Messaging;
using RQ2.Common.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace RQ2.Controller.UI.Grid
{
    public abstract class GridBase<T> : GridBase where T : GridItem
    {
        public T Slot;
        [SerializeField]
        private UIButton _cancelButton;

        protected void PopulateGrid<TData>(IEnumerable<TData> itemGridData, Action<T, TData> populate)
        {
            bool isFirst = true;
            foreach (var item in itemGridData)
            {
                //var gridItem = AddSlotToGrid<T>(item);
                var saveSlotGO = GridUtilities.AddItemToGrid(Grid, Slot.gameObject);
                var slot = saveSlotGO.GetComponent<T>();
                populate(slot, item);

                if (isFirst)
                {
                    SetFirstItemSettings(slot);
                    isFirst = false;
                }
            }
            Grid.Reposition();
            ScrollView.ResetPosition();
            if (!itemGridData.Any())
            {
                UICamera.hoveredObject = _cancelButton.gameObject;
            }
        }

        private void SetFirstSaveButtonSettings(SaveSlot button)
        {
            var keyNavigator = button.GetComponent<UIKeyNavigation>();
            keyNavigator.startsSelected = true;
        }
    }

    public abstract class GridBase : MessagingObject
    {
        public UIScrollView ScrollView;
        public UIGrid Grid;

        public void ClearGrid()
        {
            GridUtilities.ClearGrid(ScrollView, Grid);
        }

        protected void SetFirstItemSettings(GridItem item)
        {
            var keyNavigator = item.GetComponent<UIKeyNavigation>();
            UICamera.hoveredObject = item.gameObject;
            keyNavigator.startsSelected = true;
        }
    }
}
