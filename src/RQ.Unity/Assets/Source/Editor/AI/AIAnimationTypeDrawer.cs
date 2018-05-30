using BehaviorDesigner.Editor;
using RQ.Animation;
using RQ.Animation.V2;
using RQ.Editor.UnityExtensions;
using RQ.Physics.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace RQ.AI.Drawers
{
    [CustomObjectDrawer(typeof(AIAnimationTypeAttribute))]
    public class AIAnimationTypeDrawer : ObjectDrawer
    {
        public override void OnGUI(GUIContent label)
        {
            //var rangeAttribute = (RangeAttribute)attribute;
            //value = EditorGUILayout.Slider(label, (float)value, rangeAttribute.min, rangeAttribute.max);
            //var animator = task.gameObject.GetComponent<BehaviorComponent>();
            //var animator = (IAnimatedObject)task;
            //var animator = prop.serializedObject.targetObject as IAnimatedObject;
            //var entity = behaviour.GetComponent<IBaseGameEntity>();
            //if (entity == null)
            //    return;

            //var animator = entity.GetSpriteAnimator2();

            //if (animator == null)
            //{
            //    //Debug.Log("Could not locate Animator (as IAnimatedObject)");
            //    return;
            //}
            var entity = Task.Owner.GetComponent<BehaviorTreeComponent>().GetComponentRepository();
            var animComponent = entity.Components.GetComponent<AnimationComponent>();
            //var animationComponent = animator.GetAnimationComponent();
            var spriteRenderer = animComponent.GetSpriteRenderer();
            //var spriteRenderer = animator.GetSpriteRenderer();
            if (spriteRenderer == null)
            {
                //Debug.Log("Could not locate Sprite Renderer");
                return;
            }

            var animations = spriteRenderer.GetStoredSpriteAnimations();
            if (animations == null)
            {
                //Debug.Log("Could not locate Stored Sprite Animations");
                return;
            }

            var animationTypes = new List<KeyValuePair<string, string>>() { new KeyValuePair<string, string>("", "Select animation...") };
            animationTypes.AddRange(animations.Select(i => new KeyValuePair<string, string>(i.ID, i.Type)).ToList());

            //prop.stringValue = ControlExtensions.EditorGUIPopup(position, label.text, prop.stringValue, animationTypes);
            EditorGUILayout.BeginVertical();
            value = ControlExtensions.Popup(label.text, (string)value, animationTypes);
            EditorGUILayout.EndVertical();
            //EditorGUILayout.Popup()

            //EditorGUILayout.LabelField("Animator Id", prop.stringValue);
            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField("Animator Id", (string)value);
            EditorGUILayout.EndVertical();
        }
    }
}