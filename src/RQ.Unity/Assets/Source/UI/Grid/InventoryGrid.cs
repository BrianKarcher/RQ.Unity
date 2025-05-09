﻿using RQ.Controller.UI.Grid.Data;
using RQ.Model.Item;
using System.Collections.Generic;
using UnityEngine;

namespace RQ2.Controller.UI.Grid
{
    [AddComponentMenu("RQ/Item/Inventory Grid")]
    public class InventoryGrid : GridBase<ItemSlot>
    {
        //public ItemSlot Slot;
        //public List<UIButtonPressedCondition> onClick = new List<UIButtonPressedCondition>();

        public void PopulateGrid(IEnumerable<ItemGridData> itemGridDatas)
        {
            base.PopulateGrid(itemGridDatas, (slot, data) =>
            {
                slot.Texture = data.Config.GridTexture;
                slot.Quantity = data.Data.Quantity;
                slot.UniqueId = data.Data.UniqueId;
                slot.ItemUniqueId = data.Data.ItemUniqueId;
                slot.Item = data.Data;
                slot.LabelText = data.Config.Title;
                slot.Value = data.Config.Value;
                slot.ItemConfig = data.Config as ItemConfig;
                slot.SetScrollView(ScrollView);
            });
        }
    }
}