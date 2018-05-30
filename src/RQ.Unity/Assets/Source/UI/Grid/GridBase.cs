using RQ.FSM.V2.Conditionals;
using RQ.Messaging;
using RQ2.Common.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RQ2.Controller.UI.Grid
{
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
