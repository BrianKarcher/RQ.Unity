using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace RQ.Controller.UI
{
    [ExecuteInEditMode]
    //[AddComponentMenu("RQ/UI/Toggle Animation")]
    public class UIToggleAnimationSecond : UIWidget
    {
        [SerializeField]
        public UIToggle _toggle;
        [SerializeField]
        public tk2dSpriteAnimator _anim;
        [SerializeField]
        public string _activeAnim;
        [SerializeField]
        public string _inactiveAnim;

        public void Toggle()
        {
            Debug.LogError("Toggle called");
            var value = _toggle.value;
            var anim = value ? _activeAnim : _inactiveAnim;
            _anim.Play(anim);
        }

        //protected override void OnUpdate()
        //{
        //    _anim.Sprite.color = mColor;
        //}

        public void OnHover(bool isOver)
        {
            _toggle.value = isOver;
        }

        //public new Color color
        //{
        //    get
        //    {
        //        return mColor;
        //    }
        //    set
        //    {
        //        if (mColor != value)
        //        {
        //            bool alphaChange = (mColor.a != value.a);
        //            mColor = value;
        //            _anim.Sprite.color = value;
        //            Invalidate(alphaChange);
        //        }
        //    }
        //}
    }
}
