using RQ.Editor.Common;
using RQ.Editor.GUIStyles;
using RQ.Editor.UnityExtensions;
using RQ.Extensions;
using RQ.FSM.V2;
using RQ.FSM.V2.Conditionals;
using RQ.Model.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace RQ.Editor
{
    [CustomEditor(typeof(StateTransitionTable), true)]
    public class StateTransitionTableEditor : BaseObjectEditor
    {
        private StateTransitionTable _stAgent;
        private bool conditionSelected = false;
        private List<IState> States;
        //private List<StateTransitionConditionBase> Conditions;
        private List<KeyValuePair<string, string>> StateItems;
        private List<KeyValuePair<string, string>> ConditionItems;
        //private SceneSetup _sceneSetup;

        private List<StateTransitionConditionBase> _conditions;

        //public ConditionsList Conditions;

        public void Awake()
        {
            States = new List<IState>();
        }

        public override void OnEnable()
        {
            base.OnEnable();
            _stAgent = target as StateTransitionTable;

            //States = agent.GetComponentsInChildren<IState>().ToList();
            //agent.
            //agent.get
            //States = new List<IState>();
            //foreach (Transform trans in agent.transform)
            //{
            //    var state = trans.GetComponent<IState>();
            //    if (state != null)
            //        States.Add(state);
            //}

            _stAgent.transform.GetComponentsInChildrenOneDeep<IState>(States);

            StateItems = new List<KeyValuePair<string, string>>();
            StateItems.Add(new KeyValuePair<string, string>(" ", "Any"));

            StateItems.AddRange(States.Select(i => new KeyValuePair<string, string>(i.name, i.name)));


            if (_stAgent.ConditionsList != null)
            {
                _conditions = _stAgent.ConditionsList.GetComponents<StateTransitionConditionBase>().ToList();
                _conditions.AddRange(_stAgent.ConditionsList.GetComponentsInChildren<StateTransitionConditionBase>(true).ToList());
            }
            else
            {
                _conditions = _stAgent.GetComponentsInChildren<StateTransitionConditionBase>(true).ToList();
            }

            if (_stAgent.ConditionsListTransform != null)
            {
                if (_conditions == null)
                    _conditions = new List<StateTransitionConditionBase>();
                _conditions.AddRange(_stAgent.ConditionsListTransform.GetComponents<StateTransitionConditionBase>());
                _conditions.AddRange(_stAgent.ConditionsListTransform.GetComponentsInChildren<StateTransitionConditionBase>(true));
            }

            if (_conditions != null)
                _conditions = _conditions.Distinct().ToList();

            ConditionItems = new List<KeyValuePair<string, string>>();
            ConditionItems.Add(new KeyValuePair<string, string>(" ", " "));

            ConditionItems.AddRange(_conditions.Select(i => new KeyValuePair<string, string>(i.Name, i.Name)));
            //_sceneSetup = GameObject.FindObjectOfType<SceneSetup>();
        }

        //public void OnSceneGUI()
        //{
        //    var ev = Event.current;
        //}

        public override void OnInspectorGUI()
        {
            GUI.changed = false;
            base.OnInspectorGUI();

            //var spriteAnimations = agent.GetSpriteAnimations();

            //if (agent.)

            //var clipNames = new List<KeyValuePair<string, string>>();

            //clipNames.Add(new KeyValuePair<string, string>("", "Select Animation..."));

            //var allClips = agent.GetAllClipNames();
            //if (allClips.Count() != 0)
            //{
            //    foreach (var clip in allClips)
            //    {
            //        clipNames.Add(new KeyValuePair<string, string>(clip, clip));
            //    }

            if (GUILayout.Button("Add Transition Record", EditorStyles.miniButton, GUILayout.ExpandWidth(false)))
            {
                _stAgent.Records.Add(new StateTransitionRecord());
                //spriteAnimations.Add(new Animation.Common.SpriteAnimation());
            }

            if (_stAgent.Records == null)
                _stAgent.Records = new List<StateTransitionRecord>();

            //int removeTransitionIndex = -1;

            int moveTransitionDown = -1;
            int moveTransitionUp = -1;
            int deleteTransition = -1;
            for (int i = 0; i < _stAgent.Records.Count(); i++)
            {
                var transitionRecord = _stAgent.Records[i];
                GUILayout.Space(4.0f);
                GUILayout.BeginVertical(tk2dEditorSkin.SC_InspectorHeaderBG);

                EditorGUILayout.BeginHorizontal();
                //EditorGUILayout.LabelField("Transition from", GUILayout.Width(80));
                //EditorGUILayout.LabelField("Transition");
                transitionRecord.Name = EditorGUILayout.TextField("Transition", transitionRecord.Name);

                GUI.enabled = (i != _stAgent.Records.Count() - 1);
                if (GUILayout.Button("", Utils.SimpleButton("btn_down")))
                {
                    moveTransitionDown = i;
                    //Repaint();
                }

                GUI.enabled = (i != 0);
                if (GUILayout.Button("", Utils.SimpleButton("btn_up")))
                {
                    moveTransitionUp = i;
                    //Repaint();
                }

                GUI.enabled = true; //j > 1;

                if (GUILayout.Button("", Styles.CreateTilemapDeleteItemStyle()))
                {
                    if (EditorUtility.DisplayDialog("Remove Transition?", "Remove tansition " + transitionRecord.Name + "?", "Yes", "No"))
                    {
                        deleteTransition = i;
                    }
                }
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
               
                //EditorGUILayout
                //if (StateItems != null)
                //{
                    //string currentState = transitionRecord.CurrentState == null ? string.Empty : transitionRecord.CurrentState.name;
                    //string newCurrentState = ControlExtensions.Popup(currentState, StateItems, GUILayout.Width(100));
                    //if (String.IsNullOrEmpty(newCurrentState))
                    //    transitionRecord.CurrentState = null;
                    //else
                    //    transitionRecord.CurrentState = States.FirstOrDefault(j => j.name == newCurrentState) as StateBase; //StateItems.First(j => j.Key == newCurrentState);

                transitionRecord.CurrentState = CreateStatePopup("From", transitionRecord.CurrentState);
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();

                //}
                //transitionRecord.CurrentState = EditorGUILayout.ObjectField(transitionRecord.CurrentState, typeof(StateBase), GUILayout.Width(120)) as StateBase;

                //EditorGUILayout.LabelField("to", GUILayout.Width(20));
                //EditorGUILayout.PrefixLabel("to");

                transitionRecord.TargetState = CreateStatePopup("To", transitionRecord.TargetState);
                EditorGUILayout.EndHorizontal();
                transitionRecord.TargetStateSelected = EditorGUILayout.ObjectField("To",
                    transitionRecord.TargetStateSelected, typeof(StateBase)) as StateBase;
                transitionRecord.PreviousState = EditorGUILayout.Toggle("Previous", transitionRecord.PreviousState);

                

                //string targetState = transitionRecord.TargetState == null ? string.Empty : transitionRecord.TargetState.name;
                //ControlExtensions.Popup(targetState, StateItems, GUILayout.Width(100));
                //transitionRecord.TargetState = EditorGUILayout.ObjectField(transitionRecord.TargetState, typeof(StateBase), GUILayout.Width(120)) as StateBase;
                
                transitionRecord.IsActive = EditorGUILayout.Toggle("Active?", transitionRecord.IsActive);

                //GUILayout.Toggle()
                conditionSelected = GUILayout.Toggle(conditionSelected, "Conditions", EditorStyles.toolbarDropDown, GUILayout.ExpandWidth(true));

                if (conditionSelected)
                {
                    EditorGUI.indentLevel++;                  

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Type", GUILayout.Width(50));
                    transitionRecord.Type = (ConditionType) EditorGUILayout.EnumPopup(transitionRecord.Type, GUILayout.Width(75));
                    EditorGUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button("Add Condition", EditorStyles.miniButton, GUILayout.ExpandWidth(false)))
                    {
                        if (transitionRecord.Conditions == null)
                            transitionRecord.Conditions = new List<ConditionExpression>();
                        transitionRecord.Conditions.Add(new ConditionExpression() 
                        { 
                            UniqueId = Guid.NewGuid().ToString() 
                        });
                        //spriteAnimations.Add(new Animation.Common.SpriteAnimation());
                    }
                    GUILayout.EndHorizontal();

                    if (transitionRecord.Conditions != null)
                    {
                        ConditionsInspector(transitionRecord.Conditions);
                    }

                    //EditorGUILayout.LabelField("Hello", GUILayout.Width(100));

                    EditorGUI.indentLevel--;
                }
                //spriteAnimation.type = (AnimationType)EditorGUILayout.EnumPopup(spriteAnimation.type, GUILayout.Width(75));
                //if (_sceneSetup != null && _sceneSetup.SceneConfig != null && _sceneSetup.SceneConfig.Variables != null)
                //{                
                // Get list of spawn points

                //}

                //spriteAnimation.direction = (Direction)EditorGUILayout.EnumPopup(spriteAnimation.direction, GUILayout.Width(75));

                /*agent.Variable = */
                //spriteAnimation.animationName = ControlExtensions.Popup(spriteAnimation.animationName, clipNames, GUILayout.Width(125));

                //if (String.IsNullOrEmpty(spriteAnimation.animationName))
                //{
                //    spriteAnimation.clipId = -1;
                //}
                //else
                //{
                //    spriteAnimation.clipId = agent.GetClipIdByName(spriteAnimation.animationName);
                //}


                //if (GUILayout.Button("X", GUILayout.Width(20)))
                //{
                //    if (EditorUtility.DisplayDialog("Remove animation?", "Remove animation " + spriteAnimation.animationName + " from this Area?", "Yes", "No"))
                //    {
                //        removeAnimationIndex = i;
                //    }
                //}

                GUILayout.EndVertical();
            }

            if (moveTransitionDown != -1)
            {
                _stAgent.Records.Swap(moveTransitionDown, moveTransitionDown + 1);
            }

            if (moveTransitionUp != -1)
            {
                _stAgent.Records.Swap(moveTransitionUp, moveTransitionUp - 1);
            }

            if (deleteTransition != -1)
            {
                _stAgent.Records.RemoveAt(deleteTransition);
            }

            //if (removeAnimationIndex != -1)
            //    spriteAnimations.RemoveAt(removeAnimationIndex);

            //}

            //EditorGUILayout.BeginHorizontal();
            //EditorGUILayout.LabelField("Value", GUILayout.Width(75));
            //agent.Value = EditorGUILayout.TextField(agent.Value);
            //EditorGUILayout.EndHorizontal();

            if (GUI.changed)
            {
                Dirty();
            }
        }

        private void ConditionsInspector(List<ConditionExpression> conditions)
        {
            int moveConditionDown = -1;
            int moveConditionUp = -1;
            int deleteCondition = -1;
            for (int j = 0; j < conditions.Count; j++)
            {
                var condition = conditions[j];
                //foreach (var condition in transitionRecord.Conditions)
                //{
                EditorGUILayout.BeginHorizontal();
                condition.IsNot = GUILayout.Toggle(condition.IsNot, "Not", GUILayout.Width(38));
                //EditorGUILayout.LabelField("Not", GUILayout.Width(45));                            
                //condition.IsNot = EditorGUILayout.Toggle("Not", condition.IsNot);
                //EditorGUILayout.LabelField("Condition"/*, GUILayout.Width(68)*/);
                condition.Condition = CreateConditionalPopup("Condition", condition.Condition);

                
                if (GUILayout.Button("U"))
                {
                    condition.UniqueId = Guid.NewGuid().ToString();
                }
                GUI.enabled = (j != conditions.Count - 1);
                if (GUILayout.Button("", Utils.SimpleButton("btn_down")))
                {
                    moveConditionDown = j;
                    //Repaint();
                }

                GUI.enabled = (j != 0);
                if (GUILayout.Button("", Utils.SimpleButton("btn_up")))
                {
                    moveConditionUp = j;
                    //Repaint();
                }

                GUI.enabled = true; //j > 1;
                if (GUILayout.Button("", Styles.CreateTilemapDeleteItemStyle()))
                {
                    var conditionName = condition.GetCondition() == null ? "(no name)" : condition.GetCondition().Name;
                    if (EditorUtility.DisplayDialog("Remove Condition?", "Remove condition " + conditionName + "?", "Yes", "No"))
                    {
                        deleteCondition = j;
                    }
                }

                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.TextField("UniqueId", condition.UniqueId);
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
                condition.Condition2 = EditorGUILayout.ObjectField("Condition", condition.Condition2, 
                    typeof(StateTransitionConditionBaseConfig)) as StateTransitionConditionBaseConfig;
                EditorGUILayout.EndHorizontal();
                //condition.Condition2 = EditorGUILayout.ObjectField("To",
                //    condition.Condition2, typeof(StateTransitionConditionBaseConfig)) 
                //    as StateTransitionConditionBaseConfig;
            }

            if (moveConditionDown != -1)
            {
                conditions.Swap(moveConditionDown, moveConditionDown + 1);
            }

            if (moveConditionUp != -1)
            {
                conditions.Swap(moveConditionUp, moveConditionUp - 1);
            }

            if (deleteCondition != -1)
            {
                conditions.RemoveAt(deleteCondition);
            }
        }

        private StateBase CreateStatePopup(string label, StateBase state)
        {
            string currentState = state == null ? string.Empty : state.name;
            string newCurrentState = ControlExtensions.Popup(label, currentState, StateItems/*, GUILayout.Width(100)*/);
            return States.FirstOrDefault(j => j.name == newCurrentState) as StateBase; //StateItems.First(j => j.Key == newCurrentState);
        }

        private StateTransitionConditionBase CreateConditionalPopup(string label, StateTransitionConditionBase condition)
        {
            string currentCondition = condition == null ? string.Empty : condition.Name;
            string newCurrentCondition = ControlExtensions.Popup(label, currentCondition, ConditionItems/*, GUILayout.ExpandWidth(true)*/);
            return _conditions.FirstOrDefault(j => j.Name == newCurrentCondition) as StateTransitionConditionBase; //StateItems.First(j => j.Key == newCurrentState);
        }
    }
}
