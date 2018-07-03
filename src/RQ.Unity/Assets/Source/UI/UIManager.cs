using PixelCrushers.DialogueSystem;
using PixelCrushers.DialogueSystem.NGUI;
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
        public DisplayDebugInfo DisplayDebugInfo;
        public FPSCounter FPSCounter;
        public SpriteBaseComponent PortraitEntity;
        public UILabel HPLabel;
        public UIProgressBar HPBar;
        public UILabel MPLabel;
        public UILabel LevelLabel;
        public UIProgressBar MPBar;
        public UILabel GoldLabel;
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
        public Transform HudRoot;
        public GameObject FloatingLabel;
        public List<HUDText> hudTextList;
        public UIPanel MinimapPanel;

        [SerializeField]
        public PersistenceGrid PersistenceGrid;
        [SerializeField]
        public InventoryGrid InventoryGrid;

        public UITexture HUDSkill;

        //[SerializeField]
        //private ItemSlot _itemSlot;
        [SerializeField]
        private UILabel _persistenceMainLabel = null;
        [SerializeField]
        private RQ.UI.ConversationLink _conversationLink = null;

        //public USSequencer StartSequenceOnConversationEnd { get; set; }
        public USSequencer CurrentSequence { get; set; }

        public SaveSlotData ClickedSaveSlotData { get; set; }
        /// <summary>
        /// Click event listener.
        /// </summary>

        //[Obsolete]
        //public List<EventDelegate> onClickSave = new List<EventDelegate>();
        //[Obsolete]
        //public List<EventDelegate> onClickNewSave = new List<EventDelegate>();
        /// <summary>
        /// Click event listener.
        /// </summary>
        //[Obsolete]
        //public List<EventDelegate> onClickLoad = new List<EventDelegate>();
        //public event void DialogOkClikedEvent;
        //public event Action DialogOkDelegate;
        //public event Action DialogCancelDelegate;

        public RQ.UI.ButtonManager ButtonManager;
        [SerializeField]
        //private StateMachine _stateMachine = null;


        Dictionary<RQ.Enum.Panels, UIPanel> panels;
        [SerializeField]
        private AudioClip _levelUp;
        //private GameManager _gameManager;
        //private GameController _gameController
        //private Action _eventDelegate;
        //private bool _isTypewriter;
        public TypewriterEffect ConversationTypewriter;
        private RQ.CameraClass _camera;
        public UILabel CreationLabel;
        public UILabel JusticeLabel;
        public UILabel TemperanceLabel;
        public UILabel SpiritLabel;

        public ItemConfig CreationOrb;
        public ItemConfig JusticeOrb;
        public ItemConfig TemperanceOrb;
        public ItemConfig SpiritOrb;

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

            hudTextList = new List<HUDText>();
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

        public override void Start()
        {
            base.Start();
            if (!Application.isPlaying)
                return;
            if (UICamera != null)
                InputManager.Instance.SetCamera(UICamera.GetComponent<RQ.Common.UI.IUICamera>());
        }

        public override void Update()
        {
            base.Update();
            if (DisplayDebugInfo != null && FPSCounter != null)
            {
                DisplayDebugInfo.AverageFPS = FPSCounter.AverageFPS;
            }
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
            MessageDispatcher2.Instance.StartListening("SetCamera", this.UniqueId, (data) => 
            {
                _camera = data.ExtraInfo as RQ.CameraClass;
                if (_camera == null)
                    return;
                var uiFollowTargets = HudRoot.GetComponentsInChildren<UIFollowTarget>(true);
                foreach (var uiFollowTarget in uiFollowTargets)
                {
                    uiFollowTarget.gameCamera = _camera.GetComponent<Camera>();
                }
            });
            MessageDispatcher2.Instance.StartListening("AddHudText", this.UniqueId, (data) =>
            {
                var hudTextSetupData = (HudTextSetupData)data.ExtraInfo;
                GameObject child = NGUITools.AddChild(HudRoot.gameObject, hudTextSetupData.HudText);
                var hudText = child.GetComponentInChildren<HUDText>();
                hudTextList.Add(hudText);
                var uiFollowTarget = child.AddComponent<UIFollowTarget>();
                uiFollowTarget.target = hudTextSetupData.Target;
                if (_camera == null)
                    _camera = LocateCamera();
                //uiFollowTarget.gameCamera = GameController.Instance.GetCamera().GetComponent<Camera>();
                //if (_camera != null)
                    uiFollowTarget.gameCamera = _camera.GetComponent<Camera>();
                MessageDispatcher2.Instance.DispatchMsg("SetHudText", 0f, this.UniqueId, 
                    data.SenderId, hudText);
            });
            MessageDispatcher2.Instance.StartListening("RemoveHudText", this.UniqueId, (data) =>
            {
                var hudText = (HUDText)data.ExtraInfo;
                hudTextList.Remove(hudText);
                //var hud = HudRoot.GetComponentsInChildren<HUDText>().FirstOrDefault(i => i == hudText);

                if (hudText != null)
                    NGUITools.Destroy(hudText.gameObject);
                    //GameObject.Destroy(hud.gameObject);
            });
            MessageDispatcher2.Instance.StartListening("DisplayModal", this.UniqueId, (data) =>
                {
                    DisplayModal((string)data.ExtraInfo);
                });
            MessageDispatcher2.Instance.StartListening("LevelUp", this.UniqueId, (data) =>
                {
                    if (PortraitEntity == null)
                        return;
                    MessageDispatcher2.Instance.DispatchMsg("LevelUp", 0f, this.UniqueId, PortraitEntity.UniqueId, null);
                    var entityStats = data.ExtraInfo as EntityStatsData;
                    SetHealth(entityStats.CurrentHP, entityStats.MaxHP);
                    SetMP(entityStats.CurrentSP, entityStats.MaxSP);
                    SetLevel(entityStats.Level);
                    MessageDispatcher2.Instance.DispatchMsg("PlayOneShot", 0f, this.UniqueId, "Game Controller", _levelUp);
                });
            MessageDispatcher2.Instance.StartListening("UpdateHud", this.UniqueId, (data) =>
            {
                if (PortraitEntity == null)
                    return;
                var entityStats = data.ExtraInfo as EntityStatsData;
                SetHealth(entityStats.CurrentHP, entityStats.MaxHP);
                SetMP(entityStats.CurrentSP, entityStats.MaxSP);
                SetLevel(entityStats.Level);
                SetOrbs(entityStats);
                SetHUDSkillColor();
                //MessageDispatcher2.Instance.DispatchMsg("LevelUp", 0f, this.UniqueId, PortraitEntity.UniqueId, null);
            });
            MessageDispatcher2.Instance.StartListening("UpdateStatsInHud", this.UniqueId, (data) =>
            {
                if (PortraitEntity == null)
                    return;
                var entityStats = data.ExtraInfo as EntityStatsData;
                if (entityStats == null)
                {
                    var mainCharacter = EntityContainer._instance.GetMainCharacter();
                    if (mainCharacter != null)
                        entityStats = mainCharacter.Components.GetComponent<EntityStatsComponent>().GetEntityStats();
                }
                if (entityStats == null)
                    return;
                SetHealth(entityStats.CurrentHP, entityStats.MaxHP);
                SetMP(entityStats.CurrentSP, entityStats.MaxSP);
                SetLevel(entityStats.Level);
                SetOrbs(entityStats);
                SetHUDSkillColor();
                //MessageDispatcher2.Instance.DispatchMsg("LevelUp", 0f, this.UniqueId, PortraitEntity.UniqueId, null);
            });
            MessageDispatcher2.Instance.StartListening("SetGold", this.UniqueId, (data) =>
            {
                var gold = (int)data.ExtraInfo;
                GoldLabel.text = gold.ToString();
                //MessageDispatcher2.Instance.DispatchMsg("LevelUp", 0f, this.UniqueId, PortraitEntity.UniqueId, null);
            });
            MessageDispatcher2.Instance.StartListening("SetHUDSkill", this.UniqueId, (data) =>
            {
                //var gold = (int)data.ExtraInfo;
                var itemUniqueId = (string)data.ExtraInfo;
                var itemConfig = ConfigsContainer.Instance.GetConfig<ItemConfig>(itemUniqueId);
                HUDSkill.mainTexture = itemConfig.GridTexture;
                SetHUDSkillColor();
                //MessageDispatcher2.Instance.DispatchMsg("LevelUp", 0f, this.UniqueId, PortraitEntity.UniqueId, null);
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

        public void EnablePanelButtons(IEnumerable<RQ.Enum.Panels> panels, bool state)
        {
            var buttons = GetAllPanelButtons(panels);
            foreach (var button in buttons)
            {
                if (button == null)
                    continue;
                button.isEnabled = state;
            }
        }

        protected IEnumerable<UIButton> GetAllPanelButtons(IEnumerable<RQ.Enum.Panels> panels)
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

        protected UIButton[] GetPanelButtons(RQ.Enum.Panels panel)
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
            MessageDispatcher2.Instance.StopListening("SetCamera", this.UniqueId, -1);
            MessageDispatcher2.Instance.StopListening("AddHudText", this.UniqueId, -1);
            MessageDispatcher2.Instance.StopListening("RemoveHudText", this.UniqueId, -1);
            MessageDispatcher2.Instance.StopListening("DisplayModal", this.UniqueId, -1);
            MessageDispatcher2.Instance.StopListening("LevelUp", this.UniqueId, -1);
            MessageDispatcher2.Instance.StopListening("UpdateHud", this.UniqueId, -1);
            MessageDispatcher2.Instance.StopListening("UpdateStatsInHud", this.UniqueId, -1);
            MessageDispatcher2.Instance.StopListening("SetGold", this.UniqueId, -1);
            MessageDispatcher2.Instance.StopListening("SetHUDSkill", this.UniqueId, -1);
            MessageDispatcher2.Instance.StopListening("ShowMinimap", this.UniqueId, -1);
            //MessageDispatcher2.Instance.StopListening("EnablePanelButtons", this.UniqueId);

            //MessageDispatcher2.Instance.StopListening("SetMinimapCamera", this.UniqueId);
        }

        private void SetHUDSkillColor()
        {
            Color color;
            if (Utility.IsSelectedSkillAffordable())
                color = new Color(1, 1, 1, 1);
            else
                color = new Color(.8f, 0, 0, .9f);
            HUDSkill.color = color;
        }

        private RQ.CameraClass LocateCamera()
        {
            return GameObject.FindObjectOfType<RQ.CameraClass>();
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
            return GameDataController.Instance.Data.Variables[variableName].Value;
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
            GameDataController.Instance.Data.Variables[variableName].Value = value;
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
                foreach (KeyValuePair<RQ.Enum.Panels, UIPanel> panel in panels)
                {
                    ShowPanel(panel.Value, false);
                }
            }
        }

        public void ShowPanel(RQ.Enum.Panels panel, bool state)
        {
            ShowPanel(panels[panel], state);
        }

        public UIPanel GetPanel(RQ.Enum.Panels panel)
        {
            if (panels == null)
                SetupPanels();
            return panels[panel];
        }

        private void SetupPanels()
        {
            panels = new Dictionary<RQ.Enum.Panels, UIPanel>();

            // Create dictionary for faster lookup
            foreach (PanelInfo panel in PanelList)
            {
                panels.Add(panel.Panel, panel.UIPanel);
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
                    ShowPanel(RQ.Enum.Panels.MessageBox, false);
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
            ShowPanel(RQ.Enum.Panels.MessageBox, true);
        }

        public override bool HandleMessage(Telegram msg)
        {
            if (base.HandleMessage(msg))
                return true;

            switch(msg.Msg)
            {
                case RQ.Enums.Telegrams.SetCurrentHealth:
                    var entityStatsData = msg.ExtraInfo as EntityStatsData;
                    //if (health < 0)
                    //    health = 0;
                    SetHealth(entityStatsData.CurrentHP, entityStatsData.MaxHP);
                    SetMP(entityStatsData.CurrentSP, entityStatsData.MaxSP);
                    //LifeBar.transform.localScale = new Vector3(health, 1, 1);
                    break;
                //case Enums.Telegrams.SetStartSequenceOnConversationEnd:
                //    StartSequenceOnConversationEnd = msg.ExtraInfo as USSequencer;
                //    break;
            }
            return false;
        }

        public void SetHealth(float current, float max)
        {
            HPBar.value = current / max;
            HPLabel.text = current.ToString();
        }

        public void SetMP(float current, float max)
        {
            MPBar.value = current / max;
            MPLabel.text = current.ToString();
        }

        public void SetLevel(int level)
        {
            LevelLabel.text = level.ToString();
        }

        public void SetOrbs(EntityStatsData entityStats)
        {
            SetOrb(CreationLabel, CreationOrb);
            SetOrb(JusticeLabel, JusticeOrb);
            SetOrb(SpiritLabel, SpiritOrb);
            SetOrb(TemperanceLabel, TemperanceOrb);

            //InventoryController.Instance.GetItem();

            //CreationLabel.text = entityStats.CreationOrbCount.ToString();
            //JusticeLabel.text = entityStats.JusticeOrbCount.ToString();
            //SpiritLabel.text = entityStats.SpiritOrbCount.ToString();
            //TemperanceLabel.text = entityStats.TemperanceOrbCount.ToString();
        }

        public void SetOrb(UILabel nguiLabel, ItemConfig orbItem)
        {
            if (orbItem == null)
                return;
            var itemInInventory = GameDataController.Instance.Data.Inventory.GetItem(orbItem.UniqueId);
            int count = 0;
            if (itemInInventory != null)
                count = itemInInventory.Quantity;
                //return;
            nguiLabel.text = count.ToString();
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

        public RQ.UI.ButtonManager GetButtonManager()
        {
            return ButtonManager;
        }

        public void ClearPersistenceGrid()
        {
            PersistenceGrid.ClearGrid();
        }

        public void SelectContinueButtion()
        {
            ContinueButton.SetState(UIButtonColor.State.Pressed, true);
        }

        public void ClearTogglesForPanel(RQ.Enum.Panels panel)
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

        public void SetupPersistenceGrid(string mainLabelText, RQ.Entity.StatesV2.SaveLoad savePanelState,
            bool newGameSlot)
        {
            PersistenceMainLabelText = mainLabelText;
            PersistenceGrid.ClearGrid();
            PersistenceGrid.AddNewSaveSlot = newGameSlot;
            PersistenceGrid.SavePanelState = savePanelState;
            PersistenceGrid.PopulateGrid();
        }

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
            UICamera.currentScheme = UICamera.ControlScheme.Controller;
            UICamera.hoveredObject = continueButton.gameObject;
        }

        public void AddFollowingLabel(string text, Transform target)
        {
            var floatingLabel = NGUITools.AddChild(HudRoot.gameObject, FloatingLabel);
            //FloatingLabel
            var label = floatingLabel.GetComponent<UILabel>();

            label.text = text;
            var uiFollowTarget = floatingLabel.AddComponent<UIFollowTarget>();
            uiFollowTarget.gameCamera = GameController.Instance.GetCamera().GetCamera();
            uiFollowTarget.target = target;
        }
    }
}