using RQ.Animation.V2;
using RQ.AnimationV2;
using RQ.Common.Config;
using RQ.Common.UniqueId;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace RQ2.AnimationV2
{
    [Serializable]
    [ExecuteInEditMode]
    public class SpriteAnimationsConfig : RQBaseConfig, ISpriteAnimationsConfig
    {
        [SerializeField]
        private tk2dSpriteAnimation _spriteAnimation = null;

        [SerializeField]
        private List<SpriteAnimationType> _storedSpriteAnimations = new List<SpriteAnimationType>();

        public List<SpriteAnimationType> GetStoredSpriteAnimations()
        {
            return _storedSpriteAnimations;
        }

        public int GetClipIdByName(string name)
        {
            return _spriteAnimation.GetClipIdByName(name);
        }

        public IEnumerable<string> GetAllClipNames()
        {
            if (_spriteAnimation == null)
                return new List<string>();

            List<string> clips = new List<string>();
            for (int i = 0; i < _spriteAnimation.clips.Length; i++)
            {
                clips.Add(_spriteAnimation.clips[i].name);
            }
            //anim.Library.clips.
            return clips;
        }

        public tk2dSpriteAnimation Gettk2dSpriteAnimation()
        {
            return _spriteAnimation;
        }
    }
}
