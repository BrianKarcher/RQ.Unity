using PixelCrushers.DialogueSystem;
//using PixelCrushers.DialogueSystem.NGUI;
using RQ;
using RQ.Common.Container;
using RQ.Common.Controllers;
//using RQ.Controller.ManageScene;
//using RQ.Controller.UI.Grid;
using RQ.Entity.Common;
using RQ.Entity.Data;
using RQ.Messaging;
using RQ.Model.Game_Data.StoryProgress;
using RQ.Model.Item;
using RQ.Model.UI;
using RQ.Physics.Components;
using RQ.Render;
using RQ2.Controller.ManageScene;
using RQ2.Controller.UI.Grid;
//using Assets.Scripts.Enums;
using System;
using System.Collections.Generic;
using UnityEngine;
using WellFired;
using RQ.Model.Enums;
using RQ.Controller.Sequencer;
using PixelCrushers.DialogueSystem.NGUISupport;
using RQ.Model.Interfaces;
using RQ.UI;

//using RQ.Common.UI;
//using RQ.FSM.Game;

namespace RQ2.UI
{
    public class UIManager : ComponentRepository, RQ.Controller.UI.IUIManager
    {
        [SerializeField]
        private string _name;
        public string Name { get { return _name; } set { _name = value; } }
        //public Transform DialogManager;
        //public DialogueManager DialogManager; // This is a static type and a singleton, access through that
        public NGUIDialogueUI NguiDialogUI;
        //public DisplayDebugInfo DisplayDebugInfo;
        //public FPSCounter FPSCounter;
        //public SpriteBaseComponent PortraitEntity;
        //public UILabel HPLabel;
        //public UIProgressBar HPBar;
        //public UILabel MPLabel;
        //public UILabel LevelLabel;
        //public UIProgressBar MPBar;
        //public UILabel GoldLabel;
        //public UISprite LifeBar;
        public UILabel ConversationTextBox;
        public UILabel MessageBoxTextBox;
        public UILabel UsageLabel;
        public UILabel ModalText;
        //public UILabel UsableLabel;
        public UIButton ModalOkButton;
        public UIButton ModalCancelButton;
        public UIButton ContinueButton;
        public UIButton SaveButton;
        public List<PanelInfo> PanelList;
        private int _stopSaveRequestCount;
        //public Transform HudRoot;
        // TODO: Remove
        //public GameObject FloatingLabel;
        //public List<HUDText> hudTextList;
        public UIPanel MinimapPanel;

        [SerializeField]
        public PersistenceGrid PersistenceGrid;
        [SerializeField]
        public InventoryGrid InventoryGrid;
        public InventoryGrid SkillGrid;
        public InputCommandGrid InputGrid;

        //public UITexture HUDSkill;

        //[SerializeField]
        //private ItemSlot _itemSlot;
        [SerializeField]
        private UILabel _persistenceMainLabel = null;
        [SerializeField]
        private RQ.UI.ConversationLink _conversationLink = null;

        //public USSequencer StartSequenceOnConversationEnd { get; set; }
        public ISequencerLink CurrentSequence { get; set; }

        public SaveSlotData ClickedSaveSlotData { get; set; }

        public RQ.UI.ButtonManager ButtonManager;
        [SerializeField]
        //private StateMachine _stateMachine = null;


        //Dictionary<RQ.Enum.Panels, UIPanel> panels;
        Dictionary<string, UIPanel> panels;
        //[SerializeField]
        //private AudioClip _levelUp;
        //private GameManager _gameManager;
        //private GameController _gameController
        //private Action _eventDelegate;
        //private bool _isTypewriter;
        public TypewriterEffect ConversationTypewriter;
        
        //public UILabel CreationLabel;
        //public UILabel JusticeLabel;
        //public UILabel TemperanceLabel;
        //public UILabel SpiritLabel;

        //public ItemConfig CreationOrb;
        //public ItemConfig JusticeOrb;
        //public ItemConfig TemperanceOrb;
        //public ItemConfig SpiritOrb;

        public UICamera UICamera;

        //private DialogueManager _dialogManager;
        //private DialogManager _dialogManager;
        //private d
        //private NGUIDialogueUI _nguiDialogUI;
        // Use this for initialization

        public string PersistenceMainLabelText { get { return _persistenceMainLabel.text; }
            set { _persistenceMainLabel.text = value; } }

        public override string UniqueId { get { return "UI Manager"; } set { } }
        private int _conversationIndex;

        private static UIManager _instance;
        [HideInInspector]
        public static UIManager Instance
        {
            get
            {
                if (_instance == null)
                    _instance = GameObject.FindObjectOfType<UIManager>();
                return _instance;
            }
        }

        public override void Awake()
        {
            base.Awake();
            if (!Application.isPlaying)
                return;

            _stopSaveRequestCount = 0;
            if (panels == null)
                SetupPanels();
            //if (NguiDialogUI != null)
            //    _nguiDialogUI = NguiDialogUI.GetComponent<NGUIDialogueUI>();
            if (ConversationTextBox != null)
            {
                ConversationTypewriter = ConversationTextBox.GetComponent<TypewriterEffect>();
            }
            if (ButtonManager == null)
            {
                ButtonManager = GetComponent<RQ.UI.ButtonManager>();
            }

            HideAllPanels();
            ShowPanel(MinimapPanel, false);
            //_isTypewriter = false;
            //if (DialogManager != null)
            //    _dialogManager = DialogManager.GetComponent<DialogueManager>();
            //DialogueManager.Instance.DialogueUI.

        }

        public override void OnEnable()
        {
            base.OnEnable();
            Lua.RegisterFunction("GetGlobalVariable", this, typeof(UIManager).GetMethod("GetGlobalVariable"));
            Lua.RegisterFunction("GetSceneVariable", this, typeof(UIManager).GetMethod("GetSceneVariable"));
            Lua.RegisterFunction("SetGlobalVariable", this, typeof(UIManager).GetMethod("SetGlobalVariable"));
            Lua.RegisterFunction("SetSceneVariable", this, typeof(UIManager).GetMethod("SetSceneVariable"));
            Lua.RegisterFunction("CompareStoryProgress", this, typeof(UIManager).GetMethod("CompareStoryProgress"));
            Lua.RegisterFunction("CompareStoryProgressByUniqueId", this, typeof(UIManager).GetMethod("CompareStoryProgressByUniqueId"));
            Lua.RegisterFunction("SetStoryProgress", this, typeof(UIManager).GetMethod("SetStoryProgress"));
            Lua.RegisterFunction("SetStoryProgressByUniqueId", this, typeof(UIManager).GetMethod("SetStoryProgressByUniqueId"));
            Lua.RegisterFunction("GetConversationIndex", this, typeof(UIManager).GetMethod("GetConversationIndex"));
            //_conversationLink.ConversationEnd += ConversationEnd;
        }

        public override void OnDisable()
        {
            base.OnDisable();
            Lua.UnregisterFunction("GetGlobalVariable");
            Lua.UnregisterFunction("GetSceneVariable");
            Lua.UnregisterFunction("SetGlobalVariable");
            Lua.UnregisterFunction("SetSceneVariable");
            Lua.UnregisterFunction("CompareStoryProgress");
            Lua.UnregisterFunction("CompareStoryProgressByUniqueId");
            Lua.UnregisterFunction("SetStoryProgress");
            Lua.UnregisterFunction("SetStoryProgressByUniqueId");
            Lua.UnregisterFunction("GetConversationIndex");  
        }

        public override void StartListening()
        {
            base.StartListening();
            if (!MessageDispatcher.Instance.IsRegistered(this))
                MessageDispatcher.Instance.RegisterMessageHandler(this);
            MessageDispatcher2.Instance.StartListening("SetConversationIndex", this.UniqueId, (data) =>
            {
                _conversationIndex = (int)data.ExtraInfo;
            });
            MessageDispatcher2.Instance.StartListening("StopSaveRequest", this.UniqueId, (data) =>
            {
                AddStopSave();
            });
            MessageDispatcher2.Instance.StartListening("ResumeSaveRequest", this.UniqueId, (data) =>
            {
                RemoveStopSave();
            });
            MessageDispatcher2.Instance.StartListening("ClearSaveRequest", this.UniqueId, (data) =>
            {
                _stopSaveRequestCount = 0;
            });
            MessageDispatcher2.Instance.StartListening("DisplayModal", this.UniqueId, (data) =>
                {
                    DisplayModal((string)data.ExtraInfo);
                });
            MessageDispatcher2.Instance.StartListening("ShowMinimap", this.UniqueId, (data) =>
            {
                //var gold = (int)data.ExtraInfo;
                var isOn = (bool)data.ExtraInfo;
                ShowPanel(MinimapPanel, isOn);
                //var itemConfig = ConfigsContainer.Instance.GetConfig<ItemConfig>(itemUniqueId);
                //HUDSkill.mainTexture = itemConfig.GridTexture;
                //SetHUDSkillColor();
                //MessageDispatcher2.Instance.DispatchMsg("LevelUp", 0f, this.UniqueId, PortraitEntity.UniqueId, null);
            });
            //MessageDispatcher2.Instance.StartListening("EnablePanelButtons", this.UniqueId, (data) =>
            //{
            //    var panelState = data.ExtraInfo as PanelStateState;
            //    var buttons = GetAllPanelButtons(panelState.panels);
            //    foreach (var button in buttons)
            //    {
            //        if (button == null)
            //            continue;
            //        button.isEnabled = panelState.Enabled;
            //    }
            //});
            //MessageDispatcher2.Instance.StartListening("SetMinimapCamera", this.UniqueId, (data) =>
            //{
            //    var camera = data.ExtraInfo as Camera;
            //    camera.targetTexture = _renderTexture as RenderTexture;
            //});
            //MessageDispatcher2.Instance.StartListening("InventoryUpdated", this.UniqueId, (data) =>
            //{

            //    ////var gold = (int)data.ExtraInfo;
            //    //var itemUniqueId = (string)data.ExtraInfo;
            //    //var itemConfig = ConfigsContainer.Instance.GetConfig<ItemConfig>(itemUniqueId);
            //    //HUDSkill.mainTexture = itemConfig.GridTexture;
            //    ////MessageDispatcher2.Instance.DispatchMsg("LevelUp", 0f, this.UniqueId, PortraitEntity.UniqueId, null);
            //});

        }

        public void EnablePanelButtons(IEnumerable<string> panels, bool state)
        {
            var buttons = GetAllPanelButtons(panels);
            foreach (var button in buttons)
            {
                if (button == null)
                    continue;
                button.isEnabled = state;
            }
        }

        protected IEnumerable<UIButton> GetAllPanelButtons(IEnumerable<string> panels)
        {
            var buttons = new List<UIButton>();
            foreach (var panel in panels)
            {
                var panelButtons = GetPanelButtons(panel);
                if (panelButtons != null)
                    buttons.AddRange(panelButtons);
            }
            return buttons;
        }

        protected UIButton[] GetPanelButtons(string panel)
        {
            var uiPanel = GetPanel(panel);
            if (uiPanel == null)
                return null;

            return uiPanel.GetComponentsInChildren<UIButton>(true);
            //if (buttons != null)
            //    _buttons.AddRange(buttons);
        }

        public override void StopListening()
        {
            base.StopListening();
            MessageDispatcher2.Instance.StopListening("SetConversationIndex", this.UniqueId, -1);
            MessageDispatcher2.Instance.StopListening("StopSaveRequest", this.UniqueId, -1);
            MessageDispatcher2.Instance.StopListening("ResumeSaveRequest", this.UniqueId, -1);
            MessageDispatcher2.Instance.StopListening("ClearSaveRequest", this.UniqueId, -1);
            MessageDispatcher2.Instance.StopListening("DisplayModal", this.UniqueId, -1);
            MessageDispatcher2.Instance.StopListening("ShowMinimap", this.UniqueId, -1);
            //MessageDispatcher2.Instance.StopListening("EnablePanelButtons", this.UniqueId);

            //MessageDispatcher2.Instance.StopListening("SetMinimapCamera", this.UniqueId);
        }

        private void AddStopSave()
        {
            _stopSaveRequestCount++;
            if (_stopSaveRequestCount > 0)
            {
                NGUITools.SetActive(SaveButton.gameObject, false);
            }
        }

        private void RemoveStopSave()
        {
            _stopSaveRequestCount--;
            if (_stopSaveRequestCount == 0)
            {
                NGUITools.SetActive(SaveButton.gameObject, true);
            }
        }

        //private void ConversationEnd()
        //{

        //}

        public string GetGlobalVariable(string variableName)
        {
            return GameDataController.Instance.Data.GlobalVariables[variableName].Value;
            //return GameObject.Find(name) != null;
        }

        public int GetConversationIndex()
        {
            return _conversationIndex;
        }

        public string GetSceneVariable(string variableName)
        {
            return GameDataController.Instance.Data.CurrentScene.Variables[variableName].Value;
            //return GameObject.Find(name) != null;
        }

        public void SetGlobalVariable(string variableName, string value)
        {
            GameDataController.Instance.Data.GlobalVariables[variableName].Value = value;
            //return GameObject.Find(name) != null;
        }

        public void SetSceneVariable(string variableName, string value)
        {
            GameDataController.Instance.Data.CurrentScene.Variables[variableName].Value = value;
            //return GameObject.Find(name) != null;
        }

        //public StoryProgressData GetStoryProgress()
        //{
        //    return GameDataController.Instance.Data.StoryProgress;
        //}

        public void SetStoryProgress(double act, double chapter, double scene)
        {
            var storyProgress = GameDataController.Instance.Data.GetStoryProgress();
            storyProgress.Act = (int)act;
            storyProgress.Chapter = (int)chapter;
            storyProgress.Scene = (int)scene;
        }

        public void SetStoryProgressByUniqueId(string uniqueId)
        {
            var storyProgress = GameDataController.Instance.GetStorySceneConfig(uniqueId);
            if (storyProgress == null)
            {
                Debug.LogError("Story " + uniqueId + " does not exist");
                return;
            }
            GameDataController.Instance.Data.SetStoryProgress(storyProgress);
        }

        public double CompareStoryProgress(double act, double chapter, double scene)
        {
            StoryProgressData storyCompare = new StoryProgressData((int)act, (int)chapter, (int)scene);
            var currentStory = GameDataController.Instance.Data.GetStoryProgress();
            if (storyCompare < currentStory)
                return -1;
            if (storyCompare == currentStory)
                return 0;
            else
                return 1;
        }

        public double CompareStoryProgressByUniqueId(string uniqueId)
        {
            var storySceneConfigCompare = GameDataController.Instance.GetStorySceneConfig(uniqueId);
            //StoryProgressData storyCompare = new StoryProgressData((int)act, (int)chapter, (int)scene);
            var currentStory = GameDataController.Instance.Data.GetStoryProgress();

            double returnValue = 0;

            if (currentStory < storySceneConfigCompare)
                returnValue = - 1;
            else if (currentStory == storySceneConfigCompare)
                returnValue = 0;
            else
                returnValue = 1;

            Debug.Log("CompareStoryProgressByUniqueId(" + uniqueId + " return " + returnValue);
            return returnValue;
        }

        // Update is called once per frame
        //void Update()
        //{

        //}

        //public void Init()
        //{

        //}

        //public UIPanel FindPanel(string name)
        //{
        //    //UIPanel = 
        //    UIPanel[] foundpanels = transform.parent.GetComponentsInChildren<UIPanel>();
        //    return foundpanels.SingleOrDefault(i => i.name == name);
        //}

        //public void SetGameManager(GameManager gameManager)
        //{
        //    _gameManager = gameManager;
        //}

        public void HideAllPanels()
        {
            if (panels != null)
            {
                foreach (var panel in panels)
                {
                    ShowPanel(panel.Value, false);
                }
            }
        }

        public void ShowPanel(string panel, bool state)
        {
            ShowPanel(panels[panel], state);
        }

        public UIPanel GetPanel(string panel)
        {
            if (panels == null)
                SetupPanels();
            return panels[panel];
        }

        private void SetupPanels()
        {
            panels = new Dictionary<string, UIPanel>();

            // Create dictionary for faster lookup
            foreach (PanelInfo panel in PanelList)
            {
                panels.Add(panel.Name, panel.UIPanel);
            }
        }

        public void ShowPanel(UIPanel panel, bool state)
        {
            if (panel != null)
                NGUITools.SetActive(panel.gameObject, state);

        }

        public void MessageBoxOkClicked()
        {
            //UILabel label;
            //label.type
            //if (_eventDelegate != null)
            //    _eventDelegate();
            //if (_isTypewriter)
            if (ConversationTypewriter.isActive)
            {
                ConversationTypewriter.Finish();
                //_isTypewriter = false; // Typewriting effect has finished
            }
            else
            {
                if (NguiDialogUI != null)
                    NguiDialogUI.OnContinue(); // Let the dialogue system know the Continue button was clicked.
            }
        }

        public void ShowMessageBox(string text)
        {
            //Enum.GameState oldState = GameController._instance.GetState();
            // Default action is to simply close the message box after OK is clicked
            ShowMessageBox(text, () =>
                {
                    ShowPanel("MessageBox", false);
                    // By default, set the state back to what the old state was prior to the dialog box
                    //GameController._instance.GetFSM().RevertToPreviousState();
                });
        }

        public void ShowMessageBox(string text, Action eventDelegate)
        {
            //GameController._instance.GetFSM().ChangeState(Dialog.Instance);
            MessageBoxTextBox.text = text;
            //ButtonManager.DialogOkDelegate += eventDelegate;
            //_eventDelegate = eventDelegate;
            //HideAllPanels();
            ShowPanel("MessageBox", true);
        }

        public void DisplayModal(string text)
        {
            SetModalDialogCancelButton(false);
            SetModalText(text);
            MessageDispatcher2.Instance.DispatchMsg("BeginModal", 0f, this.UniqueId, 
                "Game Controller", null);
        }

        public void SetModalText(string text)
        {
            ModalText.text = text;
        }

        public void SetModalDialogCancelButton(bool hasCancelButton)
        {
            NGUITools.SetActive(ModalCancelButton.gameObject,
                hasCancelButton);
        }

        public IButtonManager GetButtonManager()
        {
            return ButtonManager;
        }

        public void ClearPersistenceGrid()
        {
            PersistenceGrid.ClearGrid();
        }

        //public void ClearInputCommandGrid()
        //{
        //    InputGrid.ClearGrid();
        //}

        public void SelectContinueButtion()
        {
            ContinueButton.SetState(UIButtonColor.State.Pressed, true);
        }

        public void ClearTogglesForPanel(string panel)
        {
            var worldMap = GetPanel(panel);
            var toggles = worldMap.GetComponentsInChildren<UIToggle>();
            foreach (var toggle in toggles)
            {
                toggle.value = false;
            }
        }

        //private bool _newGameSlot = false;
        //[SerializeField]
        //private string _mainLabelText = null;
        //[SerializeField]
        //private RQ.Entity.StatesV2.SaveLoad _savePanelState;

        public void SetupPersistenceGrid(string mainLabelText, SaveOrLoad savePanelState,
            bool newGameSlot)
        {
            PersistenceMainLabelText = mainLabelText;
            PersistenceGrid.ClearGrid();
            PersistenceGrid.AddNewSaveSlot = newGameSlot;
            PersistenceGrid.SavePanelState = savePanelState;
            PersistenceGrid.PopulateGrid();
        }

        //public void SetupInputCommandGrid()
        //{
        //    //InputGrid.ClearGrid();
        //    //InputGrid.PopulateGrid();
        //}

        public void SetupModal(bool hasCancelButton)
        {
            NGUITools.SetActive(ModalCancelButton.gameObject,
                hasCancelButton);
            //UICamera.currentScheme = UICamera.ControlScheme.Controller;
            UICamera.hoveredObject = ModalOkButton.gameObject;
        }

        public void HoverContinueButton()
        {
            var continueButton = ContinueButton;
            //UICamera.currentScheme = UICamera.ControlScheme.Controller;
            UICamera.hoveredObject = null;
            UICamera.hoveredObject = continueButton.gameObject;
        }
    }
}