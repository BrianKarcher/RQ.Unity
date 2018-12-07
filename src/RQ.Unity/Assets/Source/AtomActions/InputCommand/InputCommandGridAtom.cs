using RQ;
using RQ.AI;
using RQ.Entity.AtomAction;
using RQ.Entity.Components;
using RQ.Model.Serialization.Input;
using RQ2.Controller.UI.Grid;
using RQ2.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Source.Actions
{
    public class InputCommandGridAtom : AtomActionBase
    {
        private InputCommandGrid _inputGrid;
        private UIManager _entity;

        public override void Start(IComponentRepository entity)
        {
            base.Start(entity);
            _entity = entity.GetComponent<UIManager>();
            _inputGrid = _entity.InputGrid;
            //_inventoryGrid = GameObject.FindObjectOfType<InventoryGrid>();
            _inputGrid.ClearGrid();
            var gamePrefs = GameStateController.Instance.GetGamePrefs();
            var slotDatas = new List<InputCommandSlotData>();
            foreach (var inputActions in gamePrefs.InputCommands)
            {
                var slotData = new InputCommandSlotData();
                InputCommand controllerCommand;
                inputActions.Value.TryGetValue(InputType.Controller, out controllerCommand);
                slotData.ControllerInputCommand = controllerCommand;
                InputCommand keyboardCommand;
                inputActions.Value.TryGetValue(InputType.Keyboard, out keyboardCommand);
                slotData.KeyboardInputCommand = keyboardCommand;
                // TODO Put this somewhere else
                switch(inputActions.Key)
                {
                    case InputAction.MoveDown:
                        slotData.InputAction = "Move Down";
                        break;
                    case InputAction.MoveLeft:
                        slotData.InputAction = "Move Left";
                        break;
                    case InputAction.MoveRight:
                        slotData.InputAction = "Move Right";
                        break;
                    case InputAction.MoveUp:
                        slotData.InputAction = "Move Up";
                        break;
                    case InputAction.SkillMenu:
                        slotData.InputAction = "Skill Menu";
                        break;
                    default:
                        slotData.InputAction = inputActions.Key.ToString();
                        break;
                }
                //inputActions.Key
                slotDatas.Add(slotData);
            }
            //var itemGridData = InventoryController.Instance.GetInventoryAsGrid(_itemClasses);
            _inputGrid.PopulateGrid(slotDatas);
        }

        public override void StartListening(IComponentRepository entity)
        {
            base.StartListening(entity);
            entity.StartListening("ItemSelected", entity.UniqueId, (data) =>
            {
                //var item = (ItemInInventoryData)data.ExtraInfo;
                //GameDataController.Instance.Data.SelectedSkill = item.ItemUniqueId;
                //MessageDispatcher2.Instance.DispatchMsg("SetHUDSkill", 0f, entity.UniqueId, "UI Manager", item.ItemUniqueId);

                //UIManager.
                //Debug.Log("Selected item " + item.ItemUniqueId);
                //item.
                //Complete();
                _isRunning = false;
            });
        }

        public override void StopListening(IComponentRepository entity)
        {
            base.StopListening(entity);
            entity.StopListening("ItemSelected", entity.UniqueId);
        }

        public override AtomActionResults OnUpdate()
        {
            return _isRunning ? AtomActionResults.Running : AtomActionResults.Success;
        }
    }
}
