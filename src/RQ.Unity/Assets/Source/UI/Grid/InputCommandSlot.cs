using RQ.Common.UI;
using UnityEngine;

namespace RQ2.Controller.UI.Grid
{
    [AddComponentMenu("RQ/UI/Input Command Slot")]
    public class InputCommandSlot : GridItem
    {
        public OnClickHandler KeyboardClickHandler;
        public OnClickHandler ControllerClickHandler;
        public InputCommandSlotData inputCommandSlotData;
        //public InputCommand KeyboardInputCommand { get; set; }
        //public InputCommand ControllerInputCommand { get; set; }

        //public string InputAction;
        //public string UniqueId { get; set; }
        //public string ItemUniqueId { get; set; }
        //public string LabelText { get; set; }
        //public Texture Texture { get; set; }
        //public int Quantity { get; set; }
        //public int Value { get; set; }
        //public ItemInInventoryData Item { get; set; }
        //public ItemConfig ItemConfig { get; set; }
        public UILabel ActionLabel;
        public UILabel KeyboardLabel;
        public UILabel ControllerLabel;
        //public UILabel Qty;
        //public UITexture NGUIImage;
        public string UIMessage;
        //[SerializeField]
        //private ManualCondition _itemSelected;

        public virtual void Start()
        {
            ActionLabel.text = inputCommandSlotData.InputAction;
            KeyboardLabel.text = inputCommandSlotData.KeyboardInputCommand == null ? string.Empty :
                inputCommandSlotData.KeyboardInputCommand.KeyCode.ToString();
            var controllerLabel = inputCommandSlotData.ControllerInputCommand.IsAxis ? inputCommandSlotData.ControllerInputCommand.AxisName : inputCommandSlotData.ControllerInputCommand.KeyCode.ToString();
            ControllerLabel.text = controllerLabel;
            KeyboardClickHandler.SetClickAction(() =>
            {
                Debug.LogError("Keyboard button clicked " + inputCommandSlotData.KeyboardInputCommand.KeyCode.ToString());
            });
            ControllerClickHandler.SetClickAction(() =>
            {
                Debug.LogError("Controller button clicked " + controllerLabel);
            });
            //Label.text = LabelText;
            //Qty.text = Quantity.ToString();
            //NGUIImage.mainTexture = Texture;
        }

        //public void OnClick()
        //{
        //    Debug.Log("Item Slot clicked.");
        //    InventoryController.Instance.SelectedItem = Item;
        //    MessageDispatcher2.Instance.DispatchMsg(UIMessage, 0f, this.UniqueId, "UI Manager", Item);
        //    //if (_itemSelected != null)
        //    //    _itemSelected.SetComplete(true);
        //    //GameController._instance.SaveGame(FileName);
        //    //GameController._instance.UIManager.ClickedSaveSlot = this;
        //}

        //public void SetItemSelectedCondition(ManualCondition condition)
        //{
        //    _itemSelected = condition;
        //}
    }
}
