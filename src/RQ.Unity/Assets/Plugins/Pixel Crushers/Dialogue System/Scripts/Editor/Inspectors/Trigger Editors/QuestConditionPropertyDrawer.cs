// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using RQ;
using RQ.Model.Game_Data.Quest;
using System;

namespace PixelCrushers.DialogueSystem
{

    [CustomPropertyDrawer(typeof(QuestCondition))]
    public class QuestConditionDrawer : PropertyDrawer
    {

        // Draw the property inside the given rect
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {

            // Using BeginProperty / EndProperty on the parent property means that
            // prefab override logic works on the entire property.
            EditorGUI.BeginProperty(position, label, property);

            // Calculate rects:
            float thirdWidth = position.width / 3;
            float questStateWidth = Mathf.Min(thirdWidth, 120f);
            float questNameWidth = position.width - questStateWidth - 2;
            Rect questConfigRect = new Rect(position.x, position.y, questNameWidth, position.height);
            Rect questNameRect = new Rect(questConfigRect.x + questConfigRect.width + 2, position.y, questNameWidth, position.height);
            Rect questStateRect = new Rect(questNameRect.x + questNameRect.width + 2, position.y, questStateWidth, position.height);

            // Draw fields - pass GUIContent.none to each so they are drawn without labels
            var questConfigProp = property.FindPropertyRelative("questConfig");
            var questConfig = questConfigProp.objectReferenceValue as QuestConfig;
            var questId = property.FindPropertyRelative("Id");
            //if (EditorTools.selectedDatabase == null)
            //{
            //    EditorGUI.PropertyField(questNameRect, questName, GUIContent.none);
            //}
            //else
            //{
            EditorGUI.PropertyField(questConfigRect, questConfigProp, GUIContent.none, false);
            int questNameIndex;
                string[] questNames = GetQuestIds(questConfig, questId.intValue, out questNameIndex);
            int newQuestNameIndex = EditorGUI.Popup(questNameRect, questNameIndex, questNames);
            //int newQuestNameIndex = EditorGUILayout.Popup(questNameIndex, questNames);
            if (newQuestNameIndex != questNameIndex)
                {
                    questId.intValue = GetQuestName(questNames, newQuestNameIndex);
                }
            //}

            var questState = property.FindPropertyRelative("questState");
            //EditorGUI.PropertyField()
            EditorGUI.PropertyField(questStateRect, questState, GUIContent.none, false);
            
            //EditorGUILayout.PropertyField(questConfigProp, GUIContent.none, false);
            //EditorGUILayout.PropertyField(questState, GUIContent.none, false);


            EditorGUI.EndProperty();
        }

        private string[] GetQuestIds(QuestConfig questConfig, int currentQuestId, out int questNameIndex)
        {
            questNameIndex = -1;
            //var questConfig = GameController.Instance.GameConfig.GetAsset<QuestConfig>(uniqueId);
            //var database = EditorTools.selectedDatabase;
            if (questConfig == null || questConfig.QuestEntries == null)
            {
                return new string[0];
            }
            else
            {
                List<string> questNames = new List<string>();
                foreach (var item in questConfig.QuestEntries)
                {
                    //if (!item.IsItem)
                    //{
                        int questName = item.Id;
                        if (currentQuestId == questName)
                        {
                            questNameIndex = questNames.Count;
                        }
                        questNames.Add(questName.ToString());
                    //}
                }
                return questNames.ToArray();
            }
        }

        //private string[] GetQuestNames(string currentQuestName, out int questNameIndex)
        //{
        //    questNameIndex = -1;
        //    var database = EditorTools.selectedDatabase;
        //    if (database == null || database.items == null)
        //    {
        //        return new string[0];
        //    }
        //    else
        //    {
        //        List<string> questNames = new List<string>();
        //        foreach (var item in database.items)
        //        {
        //            if (!item.IsItem)
        //            {
        //                string questName = item.Name;
        //                if (string.Equals(currentQuestName, questName))
        //                {
        //                    questNameIndex = questNames.Count;
        //                }
        //                questNames.Add(questName);
        //            }
        //        }
        //        return questNames.ToArray();
        //    }
        //}

        private int GetQuestName(string[] questNames, int questNameIndex)
        {
            return (0 <= questNameIndex && questNameIndex < questNames.Length) ? Int32.Parse(questNames[questNameIndex]) : 0;
        }

    }

}
