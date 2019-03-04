using UnityEngine;

namespace RQ2.Common.Utilities
{
    public class GridUtilities
    {
        public static void ClearGrid(UIScrollView scrollView, UIGrid grid)
        {
            //PersistenceGrid.transform.DestroyChildren();

            //PersistenceGrid.Reposition();
            //PersistenceScrollView.ResetPosition();
            //PersistenceScrollView.MoveAbsolute(new Vector3(0, 0, 0));
            //UIPanel pan;
            //pan.clipOffset
            //PersistenceScrollView.UpdatePosition();
            //SpringPanel sp;
            //sp.

            scrollView.transform.localPosition = new Vector3(0, 0, 0);
            var panel = scrollView.GetComponent<UIPanel>();
            panel.clipOffset = new Vector2(0, 0);

            scrollView.ResetPosition();
            NGUITools.DestroyChildren(grid.transform);
            scrollView.ResetPosition();

            //PersistenceScrollView.

            //var saveSlots = PersistenceGrid.GetChildList();
            //for (int i = saveSlots.Count - 1; i >= 0; i--)
            //{

            //    PersistenceGrid.RemoveChild(saveSlots[i]);
            //}
            //foreach (var saveSlot in saveSlots)
            //{
            //    PersistenceGrid.RemoveChild(saveSlot);
            //    GameObject.Destroy(saveSlot.gameObject);
            //}
            //PersistenceGrid.child
            //PersistenceGrid.Reposition();
        }

        public static GameObject AddItemToGrid(UIGrid grid, GameObject prefab)
        {
            return NGUITools.AddChild(grid.gameObject, prefab);
        }
    }
}
