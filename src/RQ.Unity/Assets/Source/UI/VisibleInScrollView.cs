using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace RQ2.Controller.UI
{
    [AddComponentMenu("RQ/UI/Visible in Scroll View")]
    public class VisibleInScrollView : MonoBehaviour
    {
        [SerializeField]
        private UIScrollView _scrollView;
        [SerializeField]
        private float _strength = 1f;

        public void OnHover(bool selected)
        {
            if (!selected)
                return;

            if (_scrollView == null)
                return;

            if (UICamera.currentScheme == UICamera.ControlScheme.Controller)
            {
                // Make sure this object is visible in the scroll view
                // when selected
                SpringPanel.Begin(_scrollView.gameObject,
                    new Vector3(transform.localPosition.x, -transform.localPosition.y, transform.localPosition.z),
                    _strength);
                //_scrollView.
            }
        }

        public void SetScrollView(UIScrollView scrollView)
        {
            _scrollView = scrollView;
        }
    }
}
