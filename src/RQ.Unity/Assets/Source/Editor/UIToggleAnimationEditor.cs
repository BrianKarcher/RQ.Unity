//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using UnityEngine;
//using UnityEditor;
//using RQ.Entity.Common;
//using RQ.Entity.UI;
//using RQ.Controller.ManageScene;
//using RQ.Controller.Actions;
//using RQ.Editor.UnityExtensions;
//using RQ.UI;
//using RQ.Controller.UI;

//namespace RQ.Editor
//{
//    [CustomEditor(typeof(UIToggleAnimation), true)]
//    public class UIToggleAnimationEditor : UIWidgetInspector
//    {
//        private UIToggleAnimation agent;

//        protected override void OnEnable()
//        {
//            base.OnEnable();
//            agent = target as UIToggleAnimation;
//        }

//        //public void OnSceneGUI()
//        //{
//        //    var ev = Event.current;
//        //}

//        //public override void OnInspectorGUI()
//        //{
//        //    GUI.changed = false;
//        //    base.OnInspectorGUI();

//        //    NGUIEditorTools.DrawEvents("On Click Save", agent, agent.onClickSave, false);
//        //    NGUIEditorTools.DrawEvents("On Click New Save", agent, agent.onClickNewSave, false);
//        //    NGUIEditorTools.DrawEvents("On Click Load", agent, agent.onClickLoad, false);

//        //    if (GUI.changed)
//        //    {
//        //        Dirty();
//        //    }
//        //}

//        protected override void DrawCustomProperties()
//        {
//            base.DrawCustomProperties();
//            //public UIToggle _toggle;
//            //public tk2dSpriteAnimator _anim;
//            //public string _activeAnim;
//            //public string _inactiveAnim;
//            //spawnPoint.SceneCameFrom = EditorGUILayout.ObjectField("Scene came from", spawnPoint.SceneCameFrom, typeof(SceneConfig)) as SceneConfig;
//            agent._toggle = EditorGUILayout.ObjectField("Toggle", agent._toggle, typeof(UIToggle)) as UIToggle;
//            agent._animator = EditorGUILayout.ObjectField("Toggle", agent._animator, typeof(Animator)) as Animator;
//            //agent._anim = EditorGUILayout.ObjectField("Anim", agent._anim, typeof(tk2dSpriteAnimator)) as tk2dSpriteAnimator;
//            agent._activeAnim = EditorGUILayout.TextField("Active Anim", agent._activeAnim);
//            agent._inactiveAnim = EditorGUILayout.TextField("Inactive Anim", agent._inactiveAnim);
//            //GUILayout.BeginHorizontal();
//            //if (NGUIEditorTools.DrawPrefixButton("Atlas"))
//            //    ComponentSelector.Show<UIAtlas>(OnSelectAtlas);
//            //SerializedProperty atlas = NGUIEditorTools.DrawProperty("", serializedObject, "mAtlas", GUILayout.MinWidth(20f));

//            //if (GUILayout.Button("Edit", GUILayout.Width(40f)))
//            //{
//            //    if (atlas != null)
//            //    {
//            //        UIAtlas atl = atlas.objectReferenceValue as UIAtlas;
//            //        NGUISettings.atlas = atl;
//            //        if (atl != null) NGUIEditorTools.Select(atl.gameObject);
//            //    }
//            //}
//            //GUILayout.EndHorizontal();

//            //SerializedProperty sp = serializedObject.FindProperty("mSpriteName");
//            //NGUIEditorTools.DrawAdvancedSpriteField(atlas.objectReferenceValue as UIAtlas, sp.stringValue, SelectSprite, false);
//        }
//    }
//}
