using RQ.Render.UI;
using RQ2.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace RQ2.Editor.UI
{
    [CustomEditor(typeof(UIButtonColorLabel), true)]
    public class UIButtonColorLabelEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            GUILayout.Space(6f);
            //NGUIEditorTools.SetLabelWidth(86f);

            serializedObject.Update();

            DrawProperty("Tween Target", serializedObject, "tweenTarget");
            DrawProperties();
            serializedObject.ApplyModifiedProperties();
        }

        protected virtual void DrawProperties()
        {
            DrawTransition();
            DrawColors();
            DrawEffectColors();
        }

        protected void DrawColors()
        {
            if (serializedObject.FindProperty("tweenTarget").objectReferenceValue == null) return;

            //if (NGUIEditorTools.DrawHeader("Colors", "Colors", false, true))
            //{
            //NGUIEditorTools.BeginContents(true);
            //NGUIEditorTools.SetLabelWidth(76f);
            UIButtonColorLabel btn = target as UIButtonColorLabel;

            if (btn.tweenTarget != null)
            {
                UIWidget widget = btn.tweenTarget.GetComponent<UIWidget>();

                if (widget != null)
                {
                    EditorGUI.BeginDisabledGroup(serializedObject.isEditingMultipleObjects);
                    {
                        SerializedObject obj = new SerializedObject(widget);
                        obj.Update();
                        DrawProperty("Normal", obj, "mColor");
                        obj.ApplyModifiedProperties();
                    }
                    EditorGUI.EndDisabledGroup();
                }
            }

            DrawProperty("Hover", serializedObject, "hover");
            DrawProperty("Pressed", serializedObject, "pressed");
            DrawProperty("Disabled", serializedObject, "disabledColor");
            if (Application.isPlaying) EditorGUILayout.ColorField("Default", (target as UIButtonColor).defaultColor);
            //NGUIEditorTools.EndContents();
            //}
        }

        protected void DrawTransition()
        {
            GUILayout.BeginHorizontal();
            DrawProperty("Transition", serializedObject, "duration", false, GUILayout.Width(120f));
            GUILayout.Label("seconds");
            GUILayout.EndHorizontal();
            GUILayout.Space(3f);
        }
        //        protected override void DrawProperties()
        //        {
        //            base.DrawProperties();
        //            DrawEffectColors();
        //        }

        protected void DrawEffectColors()
        {
            if (serializedObject.FindProperty("tweenTarget").objectReferenceValue == null) return;
            //NGUIEditorTools.BeginContents(true);
            DrawProperty("Normal Effect", serializedObject, "normalEffect");
            DrawProperty("Hover Effect", serializedObject, "hoverEffect");
            //NGUIEditorTools.EndContents();
        }

        public SerializedProperty DrawProperty(string label, SerializedObject serializedObject, string property, bool padding = false, params GUILayoutOption[] options)
        {
            SerializedProperty sp = serializedObject.FindProperty(property);

            if (sp != null)
            {
                //if (NGUISettings.minimalisticLook) padding = false;

                if (padding) EditorGUILayout.BeginHorizontal();

                //if (sp.isArray && sp.type != "string") DrawArray(serializedObject, property, label ?? property);
                if (label != null) EditorGUILayout.PropertyField(sp, new GUIContent(label), options);
                else EditorGUILayout.PropertyField(sp, options);

                if (padding)
                {
                    //NGUIEditorTools.DrawPadding();
                    EditorGUILayout.EndHorizontal();
                }
            }
            else Debug.LogWarning("Unable to find property " + property);
            return sp;
        }
    }
}
