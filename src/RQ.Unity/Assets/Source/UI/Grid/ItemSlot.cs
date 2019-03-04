using RQ.Controller.ManageScene;
using RQ.Messaging;
using RQ.Model.Item;
using UnityEngine;

namespace RQ2.Controller.UI.Grid
{
    [AddComponentMenu("RQ/Item/Item Slot")]
    public class ItemSlot : GridItem
    {
        public string UniqueId { get; set; }
        public string ItemUniqueId { get; set; }
        public string LabelText { get; set; }
        public Texture Texture { get; set; }
        public int Quantity { get; set; }
        public int Value { get; set; }
        public ItemInInventoryData Item { get; set; }
        public ItemConfig ItemConfig { get; set; }
        public UILabel Label;
        public UILabel Qty;
        public UITexture NGUIImage;
        public string UIMessage;
        //[SerializeField]
        //private ManualCondition _itemSelected;

        public virtual void Start()
        {
            Label.text = LabelText;
            Qty.text = Quantity.ToString();
            NGUIImage.mainTexture = Texture;
        }

        public void OnClick()
        {
            Debug.Log("Item Slot clicked.");
            InventoryController.Instance.SelectedItem = Item;
            MessageDispatcher2.Instance.DispatchMsg(UIMessage, 0f, this.UniqueId, "UI Manager", Item);
            //if (_itemSelected != null)
            //    _itemSelected.SetComplete(true);
            //GameController._instance.SaveGame(FileName);
            //GameController._instance.UIManager.ClickedSaveSlot = this;
        }

        //public void SetItemSelectedCondition(ManualCondition condition)
        //{
        //    _itemSelected = condition;
        //}
    }
}
