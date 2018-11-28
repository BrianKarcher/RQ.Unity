using RQ.Controller.Actions;
using RQ.Controller.ManageScene;
using RQ.Controller.UI.Grid;
using RQ.Model.Item;
using RQ2.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace RQ2.Controller.Actions
{
    [AddComponentMenu("RQ/Action/Item/Populate Inventory Grid")]
    public class PopulateInventoryGrid : ActionBase
    {
        public ItemClass[] itemClasses;
        public override void Act(Component otherRigidBody)
        {
            base.Act(otherRigidBody);
            UIManager.Instance.InventoryGrid.ClearGrid();
            var itemGridData = InventoryController.Instance.GetInventoryAsGrid(itemClasses);
            UIManager.Instance.InventoryGrid.PopulateGrid(itemGridData);
        }
    }
}
