using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace RQ.Controller.UI
{
    [ExecuteInEditMode]
    [AddComponentMenu("RQ/UI/Toggle Animation")]
    public class UIToggleAnimation : MonoBehaviour
    {
        //[SerializeField]
        public UIToggle _toggle;
        public Animator _animator;
        //public Animation _animation;
        //[SerializeField]
        //public tk2dSpriteAnimator _anim;
        //[SerializeField]
        public string _activeAnim;
        //[SerializeField]
        public string _inactiveAnim;
        private int _activeAnimHash;
        private int _inactiveAnimHash;

        public void Awake()
        {
            if (!Application.isPlaying)
                return;
            _activeAnimHash = Animator.StringToHash(_activeAnim);
            _inactiveAnimHash = Animator.StringToHash(_inactiveAnim);

            UIToggle toggle = GetComponent<UIToggle>();
            EventDelegate.Add(toggle.onChange, Toggle);
        }

        public void Toggle()
        {
            Debug.LogError("Toggle called");
            if (_toggle == null)
                return;
            var value = _toggle.value;

            //var anim = value ? _activeAnimHash : _inactiveAnimHash;
            
            //_animator.SetTrigger(anim);
            var anim = value ? _activeAnim : _inactiveAnim;
            _animator.Play(anim);
            //_anim.Play(anim);
        }

        //protected override void OnUpdate()
        //{
        //    _anim.Sprite.color = mColor;
        //}

        public void OnHover(bool isOver)
        {
            Debug.LogError("OnHover called");
            _toggle.value = isOver;
            //Toggle();
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
