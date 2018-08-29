using RQ;
using RQ.FSM.V2.Conditionals;
using RQ.Messaging;
using RQ.Model.UI;
using RQ2.UI;
//using RQ.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace RQ2.Controller.UI.Grid
{
    [AddComponentMenu("RQ/Persistence/Save Slot")]
    public class SaveSlot : GridItem
    {
        public UILabel Chapter;
        public UILabel Level;
        public UILabel Gold;
        public UILabel Time;
        public UILabel SaveIndex;
        public UILabel AutoSave;
        public UILabel DateTimeLabel;
        private UIButton _button { get; set; }
        private SaveSlotData _saveSlotData { get; set; }

        public override void Awake()
        {
            base.Awake();
            _button = GetComponent<UIButton>();            
        }

        public void SetSaveSlotData(SaveSlotData data)
        {
            _saveSlotData = data;
            if (Chapter == null)
                return;
            Chapter.text = _saveSlotData.Chapter;
            Level.text = _saveSlotData.Level.ToString();
            if (Gold != null)
                Gold.text = _saveSlotData.Gold.ToString();
            if (Time != null)
                Time.text = _saveSlotData.Time;
            SaveIndex.text = _saveSlotData.SaveCount.ToString();
            if (AutoSave != null)
                NGUITools.SetActive(AutoSave.gameObject, _saveSlotData.IsAutoSave);
            DateTimeLabel.text = data.Date.ToString("MM/dd/yyyy hh:mm tt");
        }

        public SaveSlotData GetSaveSlotData()
        {
            return _saveSlotData;
        }

        public void OnClick()
        {
            Debug.Log("Save button clicked.");
            //GameController._instance.SaveGame(FileName);
            UIManager.Instance.ClickedSaveSlotData = GetSaveSlotData();
            ProcessButtonEvents();
            MessageDispatcher2.Instance.DispatchMsg("ButtonClicked", 0f, string.Empty, null, "SaveSlot");
        }
    }
}
