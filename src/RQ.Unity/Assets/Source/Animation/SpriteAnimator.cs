using RQ.Animation;
using RQ.Animation.V2;
using System;
using System.Collections.Generic;
using UnityEngine;
using RQ.AnimationV2;
using RQ;

namespace RQ2.AnimationV2
{
    [AddComponentMenu("RQ/AnimatorV2/Animated Sprite")]
    [Serializable]
    public class SpriteAnimator : SpriteRendererBase2, ISpriteAnimator
    {
        public tk2dSpriteAnimator anim;
        //public event Action AnimComplete;
        private readonly Dictionary<string, Color> _colors = new Dictionary<string, Color>();

        public void Awake(/*IBaseGameEntity sprite*/)
        {
            if (anim == null)
            {
                anim = GetComponent<tk2dSpriteAnimator>();
            }
            //return;
            base.Awake(/*sprite, */anim.transform);
            //base.SetBaseSprite(anim.Sprite);
            SetSpriteAnimation(base._spriteAnimationsConfig as ISpriteAnimationsConfig);
        }

        public void OnEnable()
        {
            if (anim == null)
                return;
            anim.AnimationCompleted = AnimationComplete;
        }

        public void OnDisable()
        {
            if (anim == null)
                return;
            anim.AnimationCompleted = null;
            //anim.AnimationCompleted -= AnimationComplete;
        }

        public void AnimationComplete(tk2dSpriteAnimator animator, tk2dSpriteAnimationClip clip)
        {
            base.InvokeAnimComplete();
        }

        public override void Pause()
        {
            anim.Pause();
        }

        public override void  StopAnim()
        {
            anim.Stop();
        }

        public override void Resume()
        {
            anim.Resume();            
        }

        public Texture2D[] GetTextures()
        {
            return anim.Sprite.Collection.textureInsts;
        }

        public void SetSpriteAnimation(ISpriteAnimationsConfig spriteAnimationConfig)
        {
            var animConfig = spriteAnimationConfig as SpriteAnimationsConfig;
            if (spriteAnimationConfig != null)
            {
                var spriteAnimation = animConfig.Gettk2dSpriteAnimation();
                Settk2dSpriteAnimation(spriteAnimation);
            }
        }

        public void Settk2dSpriteAnimation(tk2dSpriteAnimation spriteAnimation)
        {
            if (spriteAnimation != null)
                anim.Library = spriteAnimation;
        }

        public override void SetColor(string colorName, Color color)
        {
            if (anim.Sprite == null)
                Debug.LogError("Sprite not found");
            if (colorName == null)
            {
                //Debug.LogError("(SetColor) name is null");
                return;
            }
            _colors[colorName] = color;
            ProcessColor();
        }

        public override void RemoveColor(string colorName)
        {
            if (name == null)
                return;
            if (_colors.ContainsKey(colorName))
                _colors.Remove(colorName);
            ProcessColor();
        }

        private void ProcessColor()
        {
            var newColor = Color.white;
            foreach (var oneColor in _colors)
            {
                newColor = newColor * oneColor.Value;
            }
            anim.Sprite.color = newColor;
        }

        /// <summary>
        /// Returns true if the animation changed
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public override bool RenderByName(SpriteAnimation animation)
        {
            return RenderByName(animation, 0f);
        }

        public void Render(float time)
        {
            anim.PlayFrom(time);
        }

        public bool RenderByName(SpriteAnimation animation, float time, bool setOffsetDelta = true)
        {
            //Debug.Log(this.name + " playing animation '" + name + "'.");
            if (animation == null)
            {
                return false;
            }
            return RenderByName(animation.AnimationName, time, setOffsetDelta, animation.hFlip);
        }

        public bool RenderByName(string animationName, float time, bool setOffsetDelta = true, bool hFlip = false)
        {
            //Debug.Log(this.name + " playing animation '" + name + "'.");
            if (animationName == null)
            {
                return false;
            }
            bool isChanged = false;
            //if (!anim.IsPlaying(name))
            //{
            //Log.Info(_spriteName + " SpriteAnimator2.PlayAnimation " + name);
            if (anim.CurrentClip != null)
                _previousRender = anim.CurrentClip.name;
            //Debug.Log("(" + transform.parent.name + ") playing animation " + name);
            //anim.Sprite.FlipX = hFlip;
            anim.PlayFrom(animationName, time);
            isChanged = true;
            _currentRender = animationName;
            // Since we are switching animations, set the anchor for the new animation
            //SetOffset(name, setOffsetDelta);
            //}
            return isChanged;
        }

        public bool Render(string id, float time)
        {
            return Render(id, Direction, time);
        }

        public override bool Render(string id, Direction direction)
        {
            //Debug.Log("(" + transform.parent.name + ") Setting animation to " + id + " " + direction);
            return Render(id, direction, 0f);
        }

        public bool Render(string id, Direction direction, float time)
        {
            var animation = GetAnimation(id, direction);
            _currentID = id;
            if (animation == null)
                return false;

            return RenderByName(animation, time);
        }

        public float GetClipTime()
        {
            if (anim.CurrentClip == null)
                return 0f;
            return anim.ClipTimeSeconds;
        }

        public override string GetClipName()
        {
            if (anim.CurrentClip == null)
                return null;
            return anim.CurrentClip.name;
        }

        [Obsolete]
        public override string[] GetAllClipNames()
        {
            if (anim == null)
                return new string[0];

            string[] clips = new string[anim.Library.clips.Length];
            for (int i = 0; i < anim.Library.clips.Length; i++)
            {
                clips[i] = anim.Library.clips[i].name;
                //clips.Add(anim.Library.clips[i].name);
            }
            //anim.Library.clips.
            return clips;
        }

        [Obsolete]
        public override int GetClipIdByName(string name)
        {
            return anim.GetClipIdByName(name);
        }

        public float GetClipLength(int clipId)
        {
            var clip = anim.GetClipById(clipId);
            return GetClipLength(clip);
        }

        private float GetClipLength(tk2dSpriteAnimationClip clip)
        {
            if (clip == null)
                return 0;

            return clip.frames.Length / clip.fps;
        }

        public override float GetCurrentClipLength()
        {
            return GetClipLength(anim.CurrentClip);
        }
    }
}
