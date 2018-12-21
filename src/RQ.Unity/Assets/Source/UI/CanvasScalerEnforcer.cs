using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace RQ2.Controller.UI
{
    [AddComponentMenu("RQ/UI/Canvas Scaler Enforcer")]
    public class CanvasScalerEnforcer : MonoBehaviour
    {
        public CanvasScaler canvasScaler;
        //[SerializeField]
        //private UIScrollView _scrollView;
        //[SerializeField]
        //private float _strength = 1f;

        public void OnUpdate()
        {
            canvasScaler.referenceResolution = new Vector2(800, 600);
        }

        //public void OnHover(bool selected)
        //{
        //    if (!selected)
        //        return;

        //    if (_scrollView == null)
        //        return;

        //    if (UICamera.currentScheme == UICamera.ControlScheme.Controller)
        //    {
        //        // Make sure this object is visible in the scroll view
        //        // when selected
        //        SpringPanel.Begin(_scrollView.gameObject,
        //            new Vector3(transform.localPosition.x, -transform.localPosition.y, transform.localPosition.z),
        //            _strength);
        //        //_scrollView.
        //    }
        //}

        //public void SetScrollView(UIScrollView scrollView)
        //{
        //    _scrollView = scrollView;
        //}
    }
}
