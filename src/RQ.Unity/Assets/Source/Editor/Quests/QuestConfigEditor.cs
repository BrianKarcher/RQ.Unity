using System;
using System.Collections.Generic;
using RQ.Controller.ManageScene;
using RQ.Editor.Common;
using RQ.Editor.GUIStyles;
using UnityEditor;
using RQ.Model.Game_Data.Quest;
using UnityEngine;
using RQ.Editor.List;
using RQ.Editor;

namespace Assets.Source.Editor.Quests
{
    [CustomEditor(typeof(QuestConfig), true)]
    public class QuestConfigEditor : RQBaseConfigEditor
    {
        QuestConfig agent;

        [MenuItem("Assets/Create/RQ/Quest/Quest Config")]
        public static void CreateNewAsset()
        {
            var questConfig = EditorBase.CreateAsset<QuestConfig>("NewQuestConfig.asset");
            questConfig.UniqueId = Guid.NewGuid().ToString();
            questConfig.QuestEntries = new List<QuestEntryConfig>();
            var gameConfig = Utils.FindAssetsByType<GameConfig>()[0];
            //if (!gameConfig.SceneConfigs.Contains(baseAgent as SceneConfig))
            //{
            //    gameConfig.SceneConfigs.Add(baseAgent as SceneConfig);
            //    EditorUtility.SetDirty(gameConfig);
            //}
            gameConfig.Assets.Add(questConfig);
            EditorUtility.SetDirty(gameConfig);
            //var actConfig = ScriptableObject.CreateInstance<StorySceneConfig>();
            //AssetDatabase.CreateAsset(actConfig, "Assets/Story/NewSceneConfig.asset");
            //EditorUtility.FocusProjectWindow();
            //Selection.activeObject = actConfig;
        }

        public void OnEnable()
        {
            agent = target as QuestConfig;
        }

        public override void OnInspectorGUI()
        {
            GUI.changed = false;
            base.OnInspectorGUI();
            if (agent.QuestEntries == null)
                agent.QuestEntries = new List<QuestEntryConfig>();


            agent.Name = EditorGUILayout.TextField("Quest", agent.Name);

            //_showSpawnPoints = GUILayout.Toggle(_showSpawnPoints, "Spawn Points", "Button");

            //var spawnPoints = agent.SpawnPoints;

            //if (_showSpawnPoints)
            //{
            if (GUILayout.Button("Add Entry", GUILayout.Width(130)))
            {
                var newConfig = new QuestEntryConfig();
                newConfig.UniqueId = Guid.NewGuid().ToString();
                newConfig.Index = agent.QuestEntries.Count;
                agent.QuestEntries.Add(newConfig);
                Dirty(false);
            }

            GUI.enabled = false;
            EditorGUILayout.TextField("UniqueId", agent.UniqueId);
            GUI.enabled = true;
            //EditorGUILayout.LabelField("UniqueId: " + agent.UniqueId);

            // Table Header
            //EditorGUILayout.BeginHorizontal();
            //EditorGUILayout.LabelField("Id", GUILayout.Width(20));
            //EditorGUILayout.LabelField("Scene");
            //EditorGUILayout.EndHorizontal();

            //int removeIndex = -1;
            //int moveDown = -1;
            //int moveUp = -1;

            ListEditor.DrawList(agent.QuestEntries, (questEntry, index) =>
            {
                GUILayout.Space(4.0f);
                GUILayout.BeginVertical(tk2dEditorSkin.GetStyle("InspectorHeaderBG"));
                //for (int i = 0; i < agent.QuestEntries.Count; i++)
                //{
                //var questEntry = agent.QuestEntries[i];
                //GUILayout.Space(4.0f);
                //GUILayout.BeginVertical(tk2dExternal.Skin.Inst.GetStyle("InspectorHeaderBG"));
                //EditorGUILayout.BeginHorizontal();
                //EditorGUILayout.LabelField("Id", GUILayout.Width(15));
                var fieldName = (questEntry == null ? "0" : questEntry.Index.ToString()) + " " + questEntry.Name;
                //EditorGUILayout.LabelField(fieldName, GUILayout.Width(140));
                questEntry.Id = EditorGUILayout.IntField("Id", questEntry.Id);
                //spawnPoint.Name = EditorGUILayout.TextField("Name", spawnPoint.Name);

                //EditorGUILayout.LabelField("Unique Id", spawnPoint.UniqueId);
                //EditorGUILayout.LabelField("Chapter", GUILayout.Width(50));

                //var oldSceneConfig = agent.QuestEntries[i];
                //agent.SceneConfigs[i] = EditorGUILayout.ObjectField(scene, typeof(StorySceneConfig), GUILayout.Width(150)) as StorySceneConfig;
                //if (oldSceneConfig != agent.SceneConfigs[i])
                //{
                //    agent.SceneConfigs[i].Id = i;
                //    EditorUtility.SetDirty(agent);
                //}

                //spawnPoint.IsInitialSpawnPoint = EditorGUILayout.Toggle("Initial Spawn Point", spawnPoint.IsInitialSpawnPoint);
                //spawnPoint.ExtraInfo = EditorGUILayout.TextField("Extra Info", spawnPoint.ExtraInfo);

                //GUI.enabled = (i != agent.QuestEntries.Count - 1);
                //if (GUILayout.Button("", Utils.SimpleButton("btn_down")))
                //{
                //    moveDown = i;
                //}

                //GUI.enabled = (i != 0);
                //if (GUILayout.Button("", Utils.SimpleButton("btn_up")))
                //{
                //    moveUp = i;
                //}

                //GUI.enabled = true;

                //if (GUILayout.Button("", Styles.CreateTilemapDeleteItemStyle()))
                //{
                //    var questName = questEntry == null ? "(null)" : questEntry.Name;
                //    if (EditorUtility.DisplayDialog("Remove Entry?", "Remove Entry " + questName + "?", "Yes", "No"))
                //    {
                //        removeIndex = i;
                //    }
                //}
                //EditorGUILayout.EndHorizontal();
                questEntry.Name = EditorGUILayout.TextField("Name", questEntry.Name);
                //var newName = EditorGUILayout.TextField("Name", questEntry.Name);
                //if (newName != questEntry.Name)
                //    Dirty(false);
                //questEntry.Name = newName;
                //GUILayout.EndVertical();
                //}

                //if (removeIndex != -1)
                //{
                //    agent.QuestEntries.RemoveAt(removeIndex);
                //    ResetIndexes();
                //    Dirty(false);
                //}
                //if (moveDown != -1)
                //{
                //    agent.QuestEntries.Swap(moveDown, moveDown + 1);
                //    ResetIndexes();
                //    Dirty(false);
                //}
                //if (moveUp != -1)
                //{
                //    agent.QuestEntries.Swap(moveUp, moveUp - 1);
                //    ResetIndexes();
                //    Dirty(false);
                //}
                GUILayout.EndVertical();
                return new ListEditor.DrawItemData<QuestEntryConfig>
                {
                    name = fieldName,
                    data = questEntry
                };
            });

            if (GUILayout.Button("Reset Indexes"))
            {
                if (EditorUtility.DisplayDialog("Reset Indexes?", "Reset all indexes for this Quest?", "Yes", "No"))
                {
                    ResetIndexes();
                }
            }
            //}

            if (GUI.changed)
            {
                Dirty(false);
            }
        }

        private void ResetIndexes()
        {
            for (int i = 0; i < agent.QuestEntries.Count; i++)
            {
                agent.QuestEntries[i].Index = i;
                EditorUtility.SetDirty(agent);
            }
        }
    }
}
