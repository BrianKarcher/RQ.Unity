using RQ.FSM.V2.Conditionals;
using System.Collections.Generic;
using UnityEngine;

namespace RQ2.Controller.UI.Grid
{
    [AddComponentMenu("RQ/UI/Grid Item")]
    public class GridItem : MonoBehaviour
    {
        private List<UIButtonPressedCondition> _buttonPressedEvent;

        public virtual void Awake()
        {
            _buttonPressedEvent = new List<UIButtonPressedCondition>();
        }

        public void SetScrollView(UIScrollView scrollView)
        {
            var visibleInScrollView = GetComponent<VisibleInScrollView>();
            if (visibleInScrollView == null)
                visibleInScrollView = GetComponentInChildren<VisibleInScrollView>();
            if (visibleInScrollView != null)
            {
                visibleInScrollView.SetScrollView(scrollView);
            }
        }

        protected void ProcessButtonEvents()
        {
            foreach (var buttonEvent in _buttonPressedEvent)
            {
                buttonEvent.ButtonPressed();
            }
        }

        public void AddButtonEvents(List<UIButtonPressedCondition> events)
        {
            _buttonPressedEvent.AddRange(events);
            //_button.onClick.AddRange(events);
        }
    }
}
