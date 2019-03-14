using RQ.AI;
using RQ.Common.Controllers;
using RQ.Controller.ManageScene;
using RQ.Controller.UI;
using RQ.Entity.AtomAction;
using RQ.Entity.Components;
using RQ.FSM.V2.Conditionals;
using RQ.Logging;
using RQ.Messaging;
using RQ.Model.GameData.StoryProgress;
using RQ.Model.Item;
using RQ2.Controller.UI.Grid;
using RQ2.UI;
using System;
using UnityEngine;

namespace RQ.Animation.BasicAction.Action
{
    [Serializable]
    public class SkillMenuAtom : AtomActionBase
    {
        public ItemClass[] _itemClasses;
        public InventoryGrid _inventoryGrid;
        private UIManager _entity;
        private long _itemSelectedId;
        private Action<Telegram2> _itemSelectedDelegate;

        public override void Start(IComponentRepository entity)
        {
            base.Start(entity);
            if (_itemSelectedDelegate == null)
            {
                _itemSelectedDelegate = (data) =>
                {
                    var item = (ItemInInventoryData)data.ExtraInfo;
                    GameDataController.Instance.Data.SelectedSkill = item.ItemUniqueId;
                    MessageDispatcher2.Instance.DispatchMsg("SetHUDSkill", 0f, entity.UniqueId, "UI Manager", item.ItemUniqueId);

                    //UIManager.
                    Debug.Log("Selected item " + item.ItemUniqueId);
                    //item.
                    //Complete();
                    _isRunning = false;
                };
            }
            _entity = entity.GetComponent<UIManager>();
            _inventoryGrid = _entity.SkillGrid;
            //_inventoryGrid = GameObject.FindObjectOfType<InventoryGrid>();
            _inventoryGrid.ClearGrid();
            var itemGridData = InventoryController.Instance.GetInventoryAsGrid(_itemClasses);
            _inventoryGrid.PopulateGrid(itemGridData);
        }

        public override void StartListening(IComponentRepository entity)
        {
            base.StartListening(entity);
            _itemSelectedId = MessageDispatcher2.Instance.StartListening("ItemSelected", entity.UniqueId, _itemSelectedDelegate);
            //entity.StartListening("ItemSelected", entity.UniqueId, );
        }

        public override void StopListening(IComponentRepository entity)
        {
            base.StopListening(entity);
            MessageDispatcher2.Instance.StopListening("ItemSelected", entity.UniqueId, _itemSelectedId);
            //entity.StopListening("ItemSelected", entity.UniqueId);
        }

        public override AtomActionResults OnUpdate()
        {
            return _isRunning ? AtomActionResults.Running : AtomActionResults.Success;
        }
    }
}
