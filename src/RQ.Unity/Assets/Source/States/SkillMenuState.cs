using RQ.Common.Controllers;
using RQ.Controller.ManageScene;
using RQ.Controller.UI.Grid;
using RQ.Entity.StatesV2;
using RQ.Messaging;
using RQ.Model.Item;
using RQ2.Controller.UI.Grid;
using UnityEngine;

namespace RQ.Controller.StatesV2.UI
{
    [AddComponentMenu("RQ/States/State/UI/Skill Menu")]
    public class SkillMenuState : PanelState
    {
        [SerializeField]
        private ItemClass[] _itemClasses;
        [SerializeField]
        private InventoryGrid _inventoryGrid;
        public override void Enter()
        {
            base.Enter();
            _inventoryGrid.ClearGrid();
            var itemGridData = InventoryController.Instance.GetInventoryAsGrid(_itemClasses);
            _inventoryGrid.PopulateGrid<SkillSlot>(itemGridData);
            //InputManager.Instance.
        }

        public override void StartListening()
        {
            base.StartListening();
            _componentRepository.StartListening("ItemSelected", this.UniqueId, (data) =>
                {
                    var item = (ItemInInventoryData)data.ExtraInfo;
                    GameDataController.Instance.Data.SelectedSkill = item.ItemUniqueId;
                    MessageDispatcher2.Instance.DispatchMsg("SetHUDSkill", 0f, this.UniqueId, "UI Manager", item.ItemUniqueId);
                    
                    //UIManager.
                    Debug.Log("Selected item " + item.ItemUniqueId);
                    //item.
                    Complete();
                });
        }

        public override void StopListening()
        {
            base.StopListening();
            _componentRepository.StopListening("ItemSelected", this.UniqueId);
        }

    }
}
