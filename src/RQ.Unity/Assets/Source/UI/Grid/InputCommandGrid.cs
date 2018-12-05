using RQ.Controller.UI.Grid.Data;
using RQ.Model.Item;
using RQ.Model.Serialization.Input;
using System.Collections.Generic;
using UnityEngine;

namespace RQ2.Controller.UI.Grid
{
    [AddComponentMenu("RQ/UI/Input Command Grid")]
    public class InputCommandGrid : GridBase<InputCommandSlot>
    {
        //public ItemSlot Slot;
        //public List<UIButtonPressedCondition> onClick = new List<UIButtonPressedCondition>();

        public void PopulateGrid(IEnumerable<InputCommandSlotData> itemGridDatas)
        {
            base.PopulateGrid(itemGridDatas, (slot, data) =>
            {
                //var inputCommandSlotData = new InputCommandSlotData();
                //inputCommandSlotData.InputAction = data.InputAction;
                //inputCommandSlotData.KeyboardInputCommand = data.KeyboardInputCommand;
                //inputCommandSlotData.ControllerInputCommand = data.ControllerInputCommand;
                slot.inputCommandSlotData = data;

                //slot.Texture = data.Config.GridTexture;
                //slot.Quantity = data.Data.Quantity;
                //slot.UniqueId = data.Data.UniqueId;
                //slot.ItemUniqueId = data.Data.ItemUniqueId;
                //slot.Item = data.Data;
                //slot.LabelText = data.Config.Title;
                //slot.Value = data.Config.Value;
                //slot.ItemConfig = data.Config as ItemConfig;
                slot.SetScrollView(ScrollView);
            });
        }
    }
}