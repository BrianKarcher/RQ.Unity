using RQ;
using RQ2.Common.Utilities;
using RQ.Entity.StatesV2;
using RQ.FSM.V2.Conditionals;
using RQ.Messaging;
using RQ.Model.UI;
using RQ.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using RQ.Model.Enums;

namespace RQ2.Controller.UI.Grid
{
    [AddComponentMenu("RQ/Persistence/Persistence Grid")]
    public class PersistenceGrid : GridBase
    {   
        public SaveSlot SaveSlot;
        public GridItem NewSaveSlot;
        public List<UIButtonPressedCondition> onClickSave = new List<UIButtonPressedCondition>();
        public List<UIButtonPressedCondition> onClickNewSave = new List<UIButtonPressedCondition>();
        /// <summary>
        /// Click event listener.
        /// </summary>
        public List<UIButtonPressedCondition> onClickLoad = new List<UIButtonPressedCondition>();

        public bool AddNewSaveSlot { get; set; }
        public SaveOrLoad SavePanelState { get; set; }

        public void PopulateGrid()
        {
            bool isFirstSlot = true;
            SaveSlot saveSlot;
            int saveCount = 1;
            if (AddNewSaveSlot)
            {
                var saveSlotData = new SaveSlotData()
                {
                    IsNewSlot = true
                };
                saveSlot = AddSaveSlotToGrid(NewSaveSlot.gameObject, saveSlotData, onClickNewSave);
                if (isFirstSlot)
                {
                    SetFirstSaveButtonSettings(saveSlot);
                    //saveSlot.IsNewSlot = true;
                    isFirstSlot = false;
                }
            }
            if (SavePanelState == SaveOrLoad.Load)
            {
                var autoSaveFileName = GameController.Instance.AutoSaveFileName;
                var autoSaveExists = Persistence.CheckFileExists(autoSaveFileName);
                if (autoSaveExists)
                {
                    int saveCountDump = 0;
                    var slotData = GetSaveData(autoSaveFileName, ref saveCountDump);
                    slotData.IsAutoSave = true;
                    saveSlot = AddSaveSlotToGrid(slotData, onClickLoad);
                }
            }
            var saveFiles = Persistence.GetSaveFiles();
            var saveSlotDatas = new List<SaveSlotData>();
            foreach (var saveFile in saveFiles)
            {
                FileInfo fileInfo = new FileInfo(saveFile);
                var saveFileName = fileInfo.Name;
                //for (int i = 0; i < 10; i++)
                //{
                //var saveSlot = GameObject.Instantiate(SaveSlot, new Vector3(0, 0, 0), Quaternion.identity) as SaveSlot;
                //
                var saveSlotData = GetSaveData(saveFileName, ref saveCount);
                saveSlotDatas.Add(saveSlotData);
            }

            foreach (var saveSlotData in saveSlotDatas.OrderByDescending(i => i.Date))
            {

                //saveSlot = AddSaveSlotToGrid(saveFileName.Replace(".sav", string.Empty), saveFileName, AddNewSaveSlot, AddNewSaveSlot ? onClickSave : onClickLoad);
                saveSlot = AddSaveSlotToGrid(saveSlotData, AddNewSaveSlot ? onClickSave : onClickLoad);
                if (isFirstSlot)
                {
                    SetFirstSaveButtonSettings(saveSlot);
                    isFirstSlot = false;
                }
                //saveSlot.transform.SetParent(PersistenceGrid.transform);
                //PersistenceGrid.AddChild(SaveSlot.transform);
            }
            //PersistenceGrid.

            Grid.Reposition();
            //PersistenceScrollView.ResetPosition();
            ScrollView.ResetPosition();
        }

        private SaveSlotData GetSaveData(string fileName, ref int saveCount)
        {
            var gameData = Persistence.LoadGame(fileName);
            var saveSlotData = new SaveSlotData()
            {
                FileName = fileName,
                Chapter = "Forest Town",
                Gold = gameData.SceneSnapshot.Inventory.Gold,
                Level = gameData.SceneSnapshot.EntityStats.Level,
                SaveCount = saveCount,
                Date = gameData.SaveDate
            };
            saveCount++;
            return saveSlotData;
        }

        private void SetFirstSaveButtonSettings(SaveSlot button)
        {
            var keyNavigator = button.GetComponent<UIKeyNavigation>();
            keyNavigator.startsSelected = true;
        }

        private SaveSlot AddSaveSlotToGrid(SaveSlotData saveSlotData, List<UIButtonPressedCondition> events)
        {
            return AddSaveSlotToGrid(SaveSlot.gameObject, saveSlotData, events);
        }

        private SaveSlot AddSaveSlotToGrid(GameObject saveSlotGO, SaveSlotData saveSlotData, List<UIButtonPressedCondition> events)
        {
            var saveSlot = AddItemToGrid<SaveSlot>(saveSlotGO, events);
            saveSlot.SetSaveSlotData(saveSlotData);
            saveSlot.SetScrollView(ScrollView);
            return saveSlot;
        }

        private T AddItemToGrid<T>(GameObject go, List<UIButtonPressedCondition> events) where T : GridItem
        {
            var slotGO = GridUtilities.AddItemToGrid(Grid, go);
            var slot = slotGO.GetComponent<T>();
            // Loads the onClick events
            slot.AddButtonEvents(events);
            return slot;
        }
    }
}
