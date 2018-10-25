using RQ.Controller.ManageScene;
using RQ.Controller.UI.Grid.Data;
using RQ.FSM.V2.Conditionals;
using RQ.Messaging;
using RQ.Model.Item;
using RQ.Serialization;
using RQ2.Common.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace RQ2.Controller.UI.Grid
{
    [AddComponentMenu("RQ/Item/Inventory Grid")]
    public class InventoryGrid : GridBase
    {   
        public ItemSlot Slot;
        //public List<UIButtonPressedCondition> onClick = new List<UIButtonPressedCondition>();
        [SerializeField]
        private UIButton _cancelButton;

        public void PopulateGrid<T>(IEnumerable<ItemGridData> itemGridData) where T : ItemSlot
        {
            bool isFirst = true;
            foreach (var item in itemGridData)
            {
                var gridItem = AddSlotToGrid<T>(item);
                if (isFirst)
                {
                    SetFirstItemSettings(gridItem);
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

        private ItemSlot AddSlotToGrid<T>(ItemGridData itemGridData) where T : ItemSlot
        {
            var saveSlotGO = GridUtilities.AddItemToGrid(Grid, Slot.gameObject);
            var slot = saveSlotGO.GetComponent<T>();

            // Loads the onClick events
            //slot.AddButtonEvents(onClick);
            //slot.SetItemSelectedCondition(InventoryController.Instance.ItemClickedCondition);
            slot.Texture = itemGridData.Config.GridTexture;
            slot.Quantity = itemGridData.Data.Quantity;
            slot.UniqueId = itemGridData.Data.UniqueId;
            slot.ItemUniqueId = itemGridData.Data.ItemUniqueId;
            slot.Item = itemGridData.Data;
            slot.LabelText = itemGridData.Config.Title;
            slot.Value = itemGridData.Config.Value;
            slot.ItemConfig = itemGridData.Config as ItemConfig;
            slot.SetScrollView(ScrollView);
            return slot;
        }
    }
}
