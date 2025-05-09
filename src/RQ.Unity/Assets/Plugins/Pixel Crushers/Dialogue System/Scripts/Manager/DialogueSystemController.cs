 // Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PixelCrushers.DialogueSystem
{

    public delegate bool GetInputButtonDownDelegate(string buttonName);

    public delegate void TransformDelegate(Transform t);

    /// <summary>
    /// This component ties together the elements of the Dialogue System: dialogue database, 
    /// dialogue UI, sequencer, and conversation controller. You will typically add this to a
    /// GameObject named DialogueManager in your scene. For simplified script access, you can
    /// use the DialogueManager static class.
    /// </summary>
    [AddComponentMenu("")] // Use wrapper.
    public class DialogueSystemController : MonoBehaviour
    {

        /// <summary>
        /// The initial dialogue database.
        /// </summary>
        [Tooltip("This dialogue database is loaded automatically. Use an Extra Databases component to load additional databases.")]
        public DialogueDatabase initialDatabase = null;

        /// <summary>
        /// The display settings to use for the dialogue UI and sequencer.
        /// </summary>
        public DisplaySettings displaySettings = new DisplaySettings();

        /// <summary>
        /// Settings to apply to the PersistentDataManager.
        /// </summary>
        public PersistentDataSettings persistentDataSettings = new PersistentDataSettings();

        /// <summary>
        /// Set <c>true</c> to allow more than one conversation to play simultaneously.
        /// </summary>
        [Tooltip("Allow more than one conversation to play simultaneously.")]
        public bool allowSimultaneousConversations = false;

        /// <summary>
        /// Set <c>true</c> to include sim status for each dialogue entry.
        /// </summary>
        [Tooltip("Tick if your conversations reference Dialog[x].SimStatus.")]
        public bool includeSimStatus = false;

        /// <summary>
        /// If <c>true</c>, preloads the master database and dialogue UI. Otherwise they're lazy-
        /// loaded only before the first time they're needed.
        /// </summary>
        [Tooltip("Preload the dialogue database and dialogue UI at Start. Otherwise they're loaded at first use.")]
        public bool preloadResources = false;

        [Tooltip("Use a copy of the dialogue database at runtime instead of the asset file directly. This allows you to change the database without affecting the asset.")]
        public bool instantiateDatabase = false;

        /// <summary>
        /// If <c>true</c>, Unity will not destroy the game object when loading a new level.
        /// </summary>
        [Tooltip("Retain this GameObject when changing levels. Note: If InputDeviceManager's Singleton checkbox is ticked or GameObject has SaveSystem, GameObject will still be marked Don't Destroy On Load.")]
        public bool dontDestroyOnLoad = true;

        /// <summary>
        /// If <c>true</c>, new DialogueSystemController objects will destroy themselves if one
        /// already exists in the scene. Otherwise, if you reload a level and dontDestroyOnLoad
        /// is true, you'll end up with a second object.
        /// </summary>
        [Tooltip("Ensure only one Dialogue Manager in the scene.")]
        public bool allowOnlyOneInstance = true;

        /// <summary>
        /// Time mode to use for conversations.
        /// - Realtime: Independent of Time.timeScale.
        /// - Gameplay: Observe Time.timeScale.
        /// - Custom: You must manually set DialogueTime.time.
        /// </summary>
        [Tooltip("Time mode to use for conversations.\nRealtime: Independent of Time.timeScale.\nGameplay: Observe Time.timeScale.\nCustom: You must manually set DialogueTime.time.")]
        public DialogueTime.TimeMode dialogueTimeMode = DialogueTime.TimeMode.Realtime;

        /// <summary>
        /// The debug level. Information at this level or higher is logged to the console. This can
        /// be helpful when tracing through conversations.
        /// </summary>
        [Tooltip("Set to higher levels for troubleshooting.")]
        public DialogueDebug.DebugLevel debugLevel = DialogueDebug.DebugLevel.Warning;

        /// <summary>
        /// Raised when the Dialogue System receives an UpdateTracker message
        /// to update the quest tracker HUD and quest log window.
        /// </summary>
        public event System.Action receivedUpdateTracker = delegate { };

        /// <summary>
        /// Raised when a conversation starts. Parameter is primary actor.
        /// </summary>
        public event TransformDelegate conversationStarted = delegate { };

        /// <summary>
        /// Raised when a conversation ends. Parameter is primary actor.
        /// </summary>
        public event TransformDelegate conversationEnded = delegate { };

        /// <summary>
        /// Raised when the Dialogue System has completely initialized, including
        /// loadin the initial dialogue database and registering Lua functions.
        /// </summary>
        public event System.Action initializationComplete = delegate { };

        /// <summary>
        /// True when this Dialogue System Controller is fully initialized.
        /// </summary>
        public bool isInitialized { get { return m_isInitialized; } }
        private bool m_isInitialized = false;

        private const string DefaultDialogueUIResourceName = "Default Dialogue UI";

        private DatabaseManager m_databaseManager = null;
        private IDialogueUI m_currentDialogueUI = null;
        [HideInInspector] // Prevents accidental serialization if inspector is in Debug mode.
        private IDialogueUI m_originalDialogueUI = null;
        [HideInInspector] // Prevents accidental serialization if inspector is in Debug mode.
        private DisplaySettings m_originalDisplaySettings = null;
        private bool m_isOverrideUIPrefab = false;
        private ConversationController m_conversationController = null;
        private IsDialogueEntryValidDelegate m_isDialogueEntryValid = null;
        private GetInputButtonDownDelegate m_savedGetInputButtonDownDelegate = null;
        private LuaWatchers m_luaWatchers = new LuaWatchers();
        private AssetBundleManager m_assetBundleManager = new AssetBundleManager();
        private bool m_started = false;
        private DialogueDebug.DebugLevel m_lastDebugLevelSet = DialogueDebug.DebugLevel.None;
        private List<ActiveConversationRecord> m_activeConversations = new List<ActiveConversationRecord>();
        private UILocalizationManager m_uiLocalizationManager = null;
        private bool m_calledRandomizeNextEntry = false;

        public static bool applicationIsQuitting = false;
        public static string lastInitialDatabaseName = null;

        /// <summary>
        /// Gets the dialogue database manager.
        /// </summary>
        /// <value>
        /// The database manager.
        /// </value>
        public DatabaseManager databaseManager { get { return m_databaseManager; } }

        /// <summary>
        /// Gets the master dialogue database, which contains the initial database and any
        /// additional databases that you have added.
        /// </summary>
        /// <value>
        /// The master database.
        /// </value>
        public DialogueDatabase masterDatabase { get { return m_databaseManager.masterDatabase; } }

        /// <summary>
        /// Gets or sets the dialogue UI, which is an implementation of IDialogueUI. 
        /// </summary>
        /// <value>
        /// The dialogue UI.
        /// </value>
        public IDialogueUI dialogueUI
        {
            get { return GetDialogueUI(); }
            set { SetDialogueUI(value); }
        }

        /// <summary>
        /// The IsDialogueEntryValid delegate (if one is assigned). This is an optional delegate that you
        /// can add to check if a dialogue entry is valid before allowing a conversation to use it.
        /// </summary>
        public IsDialogueEntryValidDelegate isDialogueEntryValid
        {
            get
            {
                return m_isDialogueEntryValid;
            }
            set
            {
                m_isDialogueEntryValid = value;
                if (m_conversationController != null) m_conversationController.isDialogueEntryValid = value;
            }
        }

        /// <summary>
        /// The GetInputButtonDown delegate. Overrides calls to the standard Unity 
        /// Input.GetButtonDown function.
        /// </summary>
        public GetInputButtonDownDelegate getInputButtonDown { get; set; }

        /// <summary>
        /// Indicates whether a conversation is currently active.
        /// </summary>
        /// <value>
        /// <c>true</c> if the conversation is active; otherwise, <c>false</c>.
        /// </value>
        public bool isConversationActive { get { return (m_conversationController != null) && m_conversationController.isActive; } }

        /// <summary>
        /// Gets the current actor of the last conversation started if a conversation is active.
        /// </summary>
        /// <value>The current actor.</value>
        public Transform currentActor { get; private set; }

        /// <summary>
        /// Gets the current conversant of the last conversation started if a conversation is active.
        /// </summary>
        /// <value>The current conversant.</value>
        public Transform currentConversant { get; private set; }

        /// <summary>
        /// Gets or sets the current conversation state of the last conversation that had a line.
        /// This is set by ConversationController as the conversation moves from state to state.
        /// </summary>
        /// <value>The current conversation state.</value>
        public ConversationState currentConversationState { get; set; }

        /// <summary>
        /// Gets the title of the last conversation started. If a conversation is active,
        /// this is the title of the active conversation.
        /// </summary>
        /// <value>The title of the last conversation started.</value>
        public string lastConversationStarted { get; private set; }

        /// <summary>
        /// Gets the ID of the last conversation started.
        /// </summary>
        public int lastConversationID { get; private set; }

        public ConversationController conversationController { get { return m_conversationController; } }

        public ConversationModel conversationModel { get { return (m_conversationController != null) ? m_conversationController.conversationModel : null; } }

        public ConversationView conversationView { get { return (m_conversationController != null) ? m_conversationController.conversationView : null; } }

        public List<ActiveConversationRecord> activeConversations { get { return m_activeConversations; } }

        /// @cond FOR_V1_COMPATIBILITY
        public DatabaseManager DatabaseManager { get { return databaseManager; } }
        public DialogueDatabase MasterDatabase { get { return masterDatabase; } }
        public IDialogueUI DialogueUI { get { return dialogueUI; } set { dialogueUI = value; } }
        public IsDialogueEntryValidDelegate IsDialogueEntryValid { get { return isDialogueEntryValid; } set { isDialogueEntryValid = value; } }
        public GetInputButtonDownDelegate GetInputButtonDown { get { return getInputButtonDown; } set { getInputButtonDown = value; } }
        public bool IsConversationActive { get { return isConversationActive; } }
        public Transform CurrentActor { get { return currentActor; } private set { currentActor = value; } }
        public Transform CurrentConversant { get { return currentConversant; } set { currentConversant = value; } }
        public ConversationState CurrentConversationState { get { return currentConversationState; } set { currentConversationState = value; } }
        public string LastConversationStarted { get { return lastConversationStarted; } set { lastConversationStarted = value; } }
        public int LastConversationID { get { return lastConversationID; } set { lastConversationID = value; } }
        public ConversationController ConversationController { get { return conversationController; } }
        public ConversationModel ConversationModel { get { return conversationModel; } }
        public ConversationView ConversationView { get { return conversationView; } }
        public List<ActiveConversationRecord> ActiveConversations { get { return activeConversations; } }
        /// @endcond

        /// <summary>
        /// Indicates whether to allow the Lua environment to pass exceptions up to the
        /// caller. The default is <c>false</c>, which allows Lua to catch exceptions
        /// and just log an error to the console.
        /// </summary>
        /// <value><c>true</c> to allow Lua exceptions; otherwise, <c>false</c>.</value>
        public bool allowLuaExceptions { get; set; }

        /// <summary>
        /// If `true`, warns if a conversation starts with the actor and conversant
        /// pointing to the same transform. Default is `false`.
        /// </summary>
        /// <value><c>true</c> to warn when actor and conversant are the same; otherwise, <c>false</c>.</value>
        public bool warnIfActorAndConversantSame { get; set; }

        public void OnDestroy()
        {
            if (dontDestroyOnLoad && allowOnlyOneInstance) applicationIsQuitting = true;
        }

        /// <summary>
        /// Initializes the component by setting up the dialogue database and preparing the 
        /// dialogue UI. If the assigned UI is a prefab, an instance is added. If no UI is 
        /// assigned, the default UI is loaded.
        /// </summary>
        public void Awake()
        {
            if (allowOnlyOneInstance && (FindObjectsOfType<DialogueSystemController>().Length > 1))
            {
                // Scene already has an instance, so destroy this one:
                Destroy(gameObject);
            }
            else
            {
                // Otherwise initialize this one:
                getInputButtonDown = StandardGetInputButtonDown;
                if (instantiateDatabase && initialDatabase != null)
                {
                    var databaseInstance = Instantiate(initialDatabase) as DialogueDatabase;
                    databaseInstance.name = initialDatabase.name;
                    initialDatabase = databaseInstance;
                }
                bool visuallyMarkOldResponses = ((displaySettings != null) && (displaySettings.inputSettings != null) && (displaySettings.inputSettings.emTagForOldResponses != EmTag.None));
                DialogueLua.includeSimStatus = includeSimStatus || visuallyMarkOldResponses;
                PersistentDataManager.includeSimStatus = DialogueLua.includeSimStatus;
                PersistentDataManager.includeActorData = persistentDataSettings.includeActorData;
                PersistentDataManager.includeAllItemData = persistentDataSettings.includeAllItemData;
                PersistentDataManager.includeLocationData = persistentDataSettings.includeLocationData;
                PersistentDataManager.includeRelationshipAndStatusData = persistentDataSettings.includeStatusAndRelationshipData;
                PersistentDataManager.includeAllConversationFields = persistentDataSettings.includeAllConversationFields;
                PersistentDataManager.initializeNewVariables = persistentDataSettings.initializeNewVariables;
                PersistentDataManager.saveConversationSimStatusWithField = persistentDataSettings.saveConversationSimStatusWithField;
                PersistentDataManager.saveDialogueEntrySimStatusWithField = persistentDataSettings.saveDialogueEntrySimStatusWithField;
                PersistentDataManager.recordPersistentDataOn = persistentDataSettings.recordPersistentDataOn;
                PersistentDataManager.asyncGameObjectBatchSize = persistentDataSettings.asyncGameObjectBatchSize;
                PersistentDataManager.asyncDialogueEntryBatchSize = persistentDataSettings.asyncDialogueEntryBatchSize;
                if (dontDestroyOnLoad)
                {
                    if (this.transform.parent != null) this.transform.SetParent(null, false);
                    DontDestroyOnLoad(this.gameObject);
                }
                else
                {
                    var saveSystem = GetComponent<SaveSystem>();
                    var inputDeviceManager = GetComponent<InputDeviceManager>();
                    if (saveSystem != null)
                    {
                        if (DialogueDebug.logWarnings) Debug.LogWarning("Dialogue System: The Dialogue Manager's Don't Destroy On Load checkbox is UNticked, but the GameObject has a Save System which will mark it Don't Destroy On Load anyway. You may want to tick Don't Destroy On Load or move the Save System to another GameObject.", this);
                    }
                    else if (inputDeviceManager != null && inputDeviceManager.singleton)
                    {
                        if (DialogueDebug.logWarnings) Debug.LogWarning("Dialogue System: The Dialogue Manager's Don't Destroy On Load checkbox is UNticked, but the GameObject has an Input Device Manager whose Singleton checkbox is ticked, which will mark it Don't Destroy On Load anyway. You may want to tick Don't Destroy On Load or untick the Input Device Manager's Singleton checkbox.", this);
                    }
                }
                allowLuaExceptions = false;
                warnIfActorAndConversantSame = false;
                DialogueTime.mode = dialogueTimeMode;
                DialogueDebug.level = debugLevel;
                m_lastDebugLevelSet = debugLevel;
                lastConversationStarted = string.Empty;
                lastConversationID = -1;
                currentActor = null;
                currentConversant = null;
                currentConversationState = null;
                InitializeDatabase();
                InitializeDisplaySettings();
                InitializeLocalization();
            }
        }

        /// <summary>
        /// Standard Unity Input method to check if a button is down.
        /// </summary>
        /// <param name="buttonName"></param>
        /// <returns></returns>
        public bool StandardGetInputButtonDown(string buttonName)
        {
            return InputDeviceManager.IsButtonDown(buttonName); // Was: Input.GetButtonDown(buttonName);
        }

        private bool DisabledGetInputButtonDown(string buttonName)
        {
            return false;
        }

        private bool m_disableInput = false;

        /// <summary>
        /// Returns true if Dialogue System input is disabled.
        /// </summary>
        /// <returns></returns>
        public bool IsDialogueSystemInputDisabled()
        {
            return m_disableInput; //---Was delegate comparison, which generates GC: getInputButtonDown == DisabledGetInputButtonDown;
        }

        /// <summary>
        /// Enables or disables Dialogue System input.
        /// </summary>
        /// <param name="value">True to enable, false to disable.</param>
        public void SetDialogueSystemInput(bool value)
        {
            if (value == true)
            {
                if (IsDialogueSystemInputDisabled())
                {
                    getInputButtonDown = m_savedGetInputButtonDownDelegate ?? StandardGetInputButtonDown;
                    m_disableInput = false;
                }
            }
            else
            {
                if (!IsDialogueSystemInputDisabled())
                {
                    m_savedGetInputButtonDownDelegate = getInputButtonDown;
                    getInputButtonDown = DisabledGetInputButtonDown;
                    m_disableInput = true;
                }
            }
        }

        /// <summary>
        /// Start by enforcing only one instance is specified. Then start monitoring alerts.
        /// </summary>
        public void Start()
        {
            StartCoroutine(MonitorAlerts());
            m_started = true;
            if (preloadResources) PreloadResources();
            QuestLog.RegisterQuestLogFunctions();
            RegisterLuaFunctions();
            m_isInitialized = true;
            initializationComplete();
        }

        private void InitializeDisplaySettings()
        {
            if (displaySettings == null)
            {
                displaySettings = new DisplaySettings();
                displaySettings.cameraSettings = new DisplaySettings.CameraSettings();
                displaySettings.inputSettings = new DisplaySettings.InputSettings();
                displaySettings.inputSettings.cancel = new InputTrigger(KeyCode.Escape);
                displaySettings.inputSettings.qteButtons = new string[2] { "Fire1", "Fire2" };
                displaySettings.subtitleSettings = new DisplaySettings.SubtitleSettings();
                displaySettings.localizationSettings = new DisplaySettings.LocalizationSettings();
            }
        }

        private void InitializeLocalization()
        {
            if (displaySettings.localizationSettings.useSystemLanguage)
            {
                displaySettings.localizationSettings.language = Localization.GetLanguage(Application.systemLanguage);
            }
            var m_uiLocalizationManager = GetComponent<UILocalizationManager>();
            var needsLocalizationManager = !string.IsNullOrEmpty(displaySettings.localizationSettings.language) || displaySettings.localizationSettings.textTable != null;
            if (needsLocalizationManager && m_uiLocalizationManager == null)
            {
                m_uiLocalizationManager = gameObject.AddComponent<UILocalizationManager>(); ;
            }
            if (m_uiLocalizationManager != null && m_uiLocalizationManager.textTable == null)
            {
                m_uiLocalizationManager.textTable = displaySettings.localizationSettings.textTable;
            }
            if (m_uiLocalizationManager != null && !string.IsNullOrEmpty(m_uiLocalizationManager.currentLanguage))
            {
                SetLanguage(m_uiLocalizationManager.currentLanguage);
            }
            else
            {
                SetLanguage(displaySettings.localizationSettings.language);
            }
        }

        /// <summary>
        /// Sets the language to use for localized text.
        /// </summary>
        /// <param name='language'>
        /// Language to use. Specify <c>null</c> or an emtpy string to use the default language.
        /// </param>
        public void SetLanguage(string language)
        {
            displaySettings.localizationSettings.language = language;
            Localization.language = language;
            if (m_uiLocalizationManager != null) m_uiLocalizationManager.currentLanguage = language;
        }

        private void CheckDebugLevel()
        {
            if (debugLevel != m_lastDebugLevelSet)
            {
                DialogueDebug.level = debugLevel;
                m_lastDebugLevelSet = debugLevel;
            }
        }

        private void InitializeDatabase()
        {
            m_databaseManager = new DatabaseManager(initialDatabase);
            if (initialDatabase != null && initialDatabase.name == lastInitialDatabaseName)
            {
                m_databaseManager.Add(initialDatabase);
            }
            else
            {
                m_databaseManager.Reset(DatabaseResetOptions.KeepAllLoaded);
            }
            if (initialDatabase != null) lastInitialDatabaseName = initialDatabase.name;
            if (DialogueDebug.logWarnings && (initialDatabase == null)) Debug.LogWarning(string.Format("{0}: No dialogue database is assigned.", new System.Object[] { DialogueDebug.Prefix }));
        }

        /// <summary>
        /// Adds a dialogue database to memory. To save memory or reduce load time, you may want to 
        /// break up your dialogue data into multiple smaller databases. You can add or remove 
        /// these databases as needed.
        /// </summary>
        /// <param name='database'>
        /// The database to add.
        /// </param>
        public void AddDatabase(DialogueDatabase database)
        {
            if (m_databaseManager != null) m_databaseManager.Add(database);
        }

        /// <summary>
        /// Removes a dialogue database from memory. To save memory or reduce load time, you may 
        /// want to break up your dialogue data into multiple smaller databases. You can add or 
        /// remove these databases as needed.
        /// </summary>
        /// <param name='database'>
        /// The database to remove.
        /// </param>
        public void RemoveDatabase(DialogueDatabase database)
        {
            if (m_databaseManager != null) m_databaseManager.Remove(database);
        }

        /// <summary>
        /// Resets the database to a default state.
        /// </summary>
        /// <param name='databaseResetOptions'>
        /// Accepts the following values:
        /// - RevertToDefault: Restores the default database, removing any other databases that 
        /// were added after startup.
        /// - KeepAllLoaded: Keeps all loaded databases in memory, but reverts them to their 
        /// starting values.
        /// </param>
        public void ResetDatabase(DatabaseResetOptions databaseResetOptions)
        {
            if (m_databaseManager != null) m_databaseManager.Reset(databaseResetOptions);
        }

        /// <summary>
        /// Preloads the master database. The Dialogue System delays loading of the dialogue database 
        /// until the data is needed. This avoids potentially long delays during Start(). If you want
        /// to load the database manually (for example to run Lua commands on its contents), call
        /// this method.
        /// </summary>
        public void PreloadMasterDatabase()
        {
            DialogueDatabase db = masterDatabase;
            if (DialogueDebug.logInfo) Debug.Log(string.Format("{0}: Loaded master database '{1}'", new System.Object[] { DialogueDebug.Prefix, db.name }));
        }

        /// <summary>
        /// Preloads the dialogue UI. The Dialogue System delays loading of the dialogue UI until the
        /// first time it's needed for a conversation or alert. Since dialogue UIs often contain
        /// textures and other art assets, loading can cause a slight pause. You may want to preload
        /// the UI at a time of your design by using this method.
        /// </summary>
        public void PreloadDialogueUI()
        {
            IDialogueUI ui = dialogueUI;
            if ((ui == null) && DialogueDebug.logWarnings) Debug.LogWarning(string.Format("{0}: Unable to load the dialogue UI.", DialogueDebug.Prefix));
        }

        /// <summary>
        /// Checks whether a conversation has any valid entries linked from the start entry, since it's possible that
        /// the conditions of all entries could be false.
        /// </summary>
        /// <returns>
        /// <c>true</c>, if the conversation has a valid entry, <c>false</c> otherwise.
        /// </returns>
        /// <param name="title">
        /// The title of the conversation to look up in the master database.
        /// </param>
        /// <param name='actor'>
        /// The transform of the actor (primary participant). The sequencer uses this to direct 
        /// camera angles and perform other actions. In PC-NPC conversations, the actor is usually
        /// the PC.
        /// </param>
        /// <param name='conversant'>
        /// The transform of the conversant (the other participant). The sequencer uses this to 
        /// direct camera angles and perform other actions. In PC-NPC conversations, the conversant
        /// is usually the NPC.
        /// </param>
        public bool ConversationHasValidEntry(string title, Transform actor, Transform conversant)
        {
            if (string.IsNullOrEmpty(title)) return false;
            var model = new ConversationModel(m_databaseManager.masterDatabase, title, actor, conversant, allowLuaExceptions, isDialogueEntryValid);
            return model.hasValidEntry;
        }

        /// <summary>
        /// Checks whether a conversation has any valid entries linked from the start entry, since it's possible that
        /// the conditions of all entries could be false.
        /// </summary>
        /// <returns>
        /// <c>true</c>, if the conversation has a valid entry, <c>false</c> otherwise.
        /// </returns>
        /// <param name="title">
        /// The title of the conversation to look up in the master database.
        /// </param>
        /// <param name='actor'>
        /// The transform of the actor (primary participant). The sequencer uses this to direct 
        /// camera angles and perform other actions. In PC-NPC conversations, the actor is usually
        /// the PC.
        /// </param>
        public bool ConversationHasValidEntry(string title, Transform actor)
        {
            return ConversationHasValidEntry(title, actor, null);
        }

        /// <summary>
        /// Checks whether a conversation has any valid entries linked from the start entry, since it's possible that
        /// the conditions of all entries could be false.
        /// </summary>
        /// <returns>
        /// <c>true</c>, if the conversation has a valid entry, <c>false</c> otherwise.
        /// </returns>
        /// <param name="title">
        /// The title of the conversation to look up in the master database.
        /// </param>
        public bool ConversationHasValidEntry(string title)
        {
            return ConversationHasValidEntry(title, null, null);
        }

        /// <summary>
        /// Preloads the resources used by the Dialogue System to avoid delays caused by lazy loading.
        /// </summary>
        public void PreloadResources()
        {
            PreloadMasterDatabase();
            PreloadDialogueUI();
        }

        /// <summary>
        /// Starts a conversation, which also broadcasts an OnConversationStart message to the 
        /// actor and conversant. Your scripts can listen for OnConversationStart to do anything
        /// necessary at the beginning of a conversation, such as pausing other gameplay or 
        /// temporarily disabling player control. See the Feature Demo scene, which uses the
        /// SetEnabledOnDialogueEvent component to disable player control during conversations.
        /// </summary>
        /// <param name='title'>
        /// The title of the conversation to look up in the master database.
        /// </param>
        /// <param name='actor'>
        /// The transform of the actor (primary participant). The sequencer uses this to direct 
        /// camera angles and perform other actions. In PC-NPC conversations, the actor is usually
        /// the PC.
        /// </param>
        /// <param name='conversant'>
        /// The transform of the conversant (the other participant). The sequencer uses this to 
        /// direct camera angles and perform other actions. In PC-NPC conversations, the conversant
        /// is usually the NPC.
        /// </param>
        /// <param name='initialDialogueEntryID'> 
        /// The initial dialogue entry ID, or -1 to start from the beginning.
        /// </param>
        /// <example>
        /// Example:
        /// <code>StartConversation("Shopkeeper Conversation", player, shopkeeper, 8);</code>
        /// </example>
        public void StartConversation(string title, Transform actor, Transform conversant, int initialDialogueEntryID)
        {
            if (isConversationActive && !allowSimultaneousConversations)
            {
                if (DialogueDebug.logWarnings) Debug.LogWarning(string.Format("{0}: Another conversation is already active. Not starting '{1}'.", new System.Object[] { DialogueDebug.Prefix, title }));
            }
            else if (dialogueUI == null)
            {
                if (DialogueDebug.logErrors) Debug.LogError(string.Format("{0}: No Dialogue UI is assigned. Can't start conversation '{1}'.", new System.Object[] { DialogueDebug.Prefix, title }));
            }
            else
            {
                if (actor == null) actor = FindActorTransformFromConversation(title, "Actor");
                if (conversant == null) conversant = FindActorTransformFromConversation(title, "Conversant");
                if (DialogueDebug.logInfo) Debug.Log(string.Format("{0}: Starting conversation '{1}', actor={2}, conversant={3}.", new System.Object[] { DialogueDebug.Prefix, title, actor, conversant }), actor);
                if (warnIfActorAndConversantSame && (actor != null) && (actor == conversant))
                {
                    if (DialogueDebug.logWarnings) Debug.LogWarning(string.Format("{0}: Actor and conversant are the same GameObject.", new System.Object[] { DialogueDebug.Prefix }));
                }
                CheckDebugLevel();
                currentActor = actor;
                currentConversant = conversant;
                lastConversationStarted = title;

                SetConversationUI(actor, conversant);

                m_calledRandomizeNextEntry = false;
                var model = new ConversationModel(m_databaseManager.masterDatabase, title, actor, conversant, allowLuaExceptions, isDialogueEntryValid, initialDialogueEntryID);
                var needToSetRandomizeNextEntryAgain = m_calledRandomizeNextEntry; // Special case when START node leads to group node with RandomizeNextEntry().
                m_calledRandomizeNextEntry = false;
                if (!model.hasValidEntry) return;
                if (model.firstState != null && model.firstState.subtitle != null && model.firstState.subtitle.dialogueEntry != null) lastConversationID = model.firstState.subtitle.dialogueEntry.conversationID;
                var view = this.gameObject.AddComponent<ConversationView>();
                view.Initialize(dialogueUI, GetNewSequencer(), displaySettings, OnDialogueEntrySpoken);
                view.SetPCPortrait(model.GetPCTexture(), model.GetPCName());
                m_conversationController = new ConversationController(model, view, displaySettings.inputSettings.alwaysForceResponseMenu, OnEndConversation);
                if (needToSetRandomizeNextEntryAgain) RandomizeNextEntry();
                var target = (actor != null) ? actor : this.transform;
                if (actor != this.transform) gameObject.BroadcastMessage(DialogueSystemMessages.OnConversationStart, target, SendMessageOptions.DontRequireReceiver);

                // Add an active conversation record to the list:
                var record = new ActiveConversationRecord();
                record.actor = actor;
                record.conversant = conversant;
                record.conversationController = m_conversationController;
                record.originalDialogueUI = m_originalDialogueUI;
                record.originalDisplaySettings = m_originalDisplaySettings;
                record.isOverrideUIPrefab = m_isOverrideUIPrefab;
                m_activeConversations.Add(record);
            }
        }

        /// <summary>
        /// Starts a conversation, which also broadcasts an OnConversationStart message to the 
        /// actor and conversant. Your scripts can listen for OnConversationStart to do anything
        /// necessary at the beginning of a conversation, such as pausing other gameplay or 
        /// temporarily disabling player control. See the Feature Demo scene, which uses the
        /// SetEnabledOnDialogueEvent component to disable player control during conversations.
        /// </summary>
        /// <param name='title'>
        /// The title of the conversation to look up in the master database.
        /// </param>
        /// <param name='actor'>
        /// The transform of the actor (primary participant). The sequencer uses this to direct 
        /// camera angles and perform other actions. In PC-NPC conversations, the actor is usually
        /// the PC.
        /// </param>
        /// <param name='conversant'>
        /// The transform of the conversant (the other participant). The sequencer uses this to 
        /// direct camera angles and perform other actions. In PC-NPC conversations, the conversant
        /// is usually the NPC.
        /// </param>
        /// <example>
        /// Example:
        /// <code>StartConversation("Shopkeeper Conversation", player, shopkeeper);</code>
        /// </example>
        public void StartConversation(string title, Transform actor, Transform conversant)
        {
            StartConversation(title, actor, conversant, -1);
        }

        /// <summary>
        /// Starts a conversation, which also broadcasts an OnConversationStart message to the 
        /// actor. Your scripts can listen for OnConversationStart to do anything
        /// necessary at the beginning of a conversation, such as pausing other gameplay or 
        /// temporarily disabling player control. See the Feature Demo scene, which uses the
        /// SetEnabledOnDialogueEvent component to disable player control during conversations.
        /// </summary>
        /// <param name='title'>
        /// The title of the conversation to look up in the master database.
        /// </param>
        /// <param name='actor'>
        /// The transform of the actor (primary participant). The sequencer uses this to direct 
        /// camera angles and perform other actions. In PC-NPC conversations, the actor is usually
        /// the PC.
        /// </param>
        public void StartConversation(string title, Transform actor)
        {
            StartConversation(title, actor, null, -1);
        }

        /// <summary>
        /// Starts the conversation with no transforms specified for the actor or conversant.
        /// </summary>
        /// <param name='title'>
        /// The title of the conversation to look up in the master database.
        /// </param>
        public void StartConversation(string title)
        {
            StartConversation(title, null, null, -1);
        }

        /// <summary>
        /// Stops the current conversation immediately, and sends an OnConversationEnd message to 
        /// the actor and conversant. Your scripts can listen for OnConversationEnd to do anything
        /// necessary at the end of a conversation, such as resuming other gameplay or re-enabling
        /// player control.
        /// </summary>
        public void StopConversation()
        {
            if (m_conversationController != null)
            {
                m_conversationController.Close();
                m_conversationController = null;
            }
        }

        /// <summary>
        /// Updates the responses for the current state of the current conversation.
        /// If the response menu entries' conditions have changed while the response menu is
        /// being shown, you can call this method to update the response menu.
        /// </summary>
        public void UpdateResponses()
        {
            if (m_conversationController != null)
            {
                m_conversationController.UpdateResponses();
            }
        }

        /// <summary>
        /// Given a conversation and an actor ID field name (Actor or Conversant), return a 
        /// corresponding transform in the scene, or null if none found.
        /// </summary>
        /// <param name="conversationTitle">Conversation title.</param>
        /// <param name="actorField">Actor or Conversant.</param>
        /// <returns>Transform in the scene, or null if no appropriate one found.</returns>
        public Transform FindActorTransformFromConversation(string conversationTitle, string actorField)
        {
            var conversation = masterDatabase.GetConversation(conversationTitle);
            if (conversation == null) return null;
            var actor = masterDatabase.GetActor(conversation.LookupInt(actorField));
            if (actor == null) return null;
            var actorGO = SequencerTools.FindSpecifier(actor.Name, true);
            return (actorGO != null) ? actorGO.transform : null;
        }

        /// <summary>
        /// Sets an actor's portrait. If can be:
        /// - 'default' or <c>null</c> to use the primary portrait defined in the database,
        /// - 'pic=#' to use an alternate portrait defined in the database (numbered from 2), or
        /// - the name of a texture in a Resources folder.
        /// </summary>
        /// <param name="actorName">Actor name.</param>
        /// <param name="portraitName">Portrait name.</param>
        public void SetPortrait(string actorName, string portraitName)
        {
            Actor actor = masterDatabase.GetActor(actorName);
            if (actor == null)
            {
                if (DialogueDebug.logWarnings) Debug.LogWarning(string.Format("{0}: SetPortrait({1}, {2}): actor '{1}' not found.", new System.Object[] { DialogueDebug.Prefix, actorName, portraitName }));
                return;
            }
            Texture2D texture = null;
            if (string.IsNullOrEmpty(portraitName) || string.Equals(portraitName, "default"))
            {
                DialogueLua.SetActorField(actorName, DialogueSystemFields.CurrentPortrait, string.Empty);
                texture = actor.portrait;
            }
            else
            {
                DialogueLua.SetActorField(actorName, DialogueSystemFields.CurrentPortrait, portraitName);
                if (portraitName.StartsWith("pic="))
                {
                    texture = actor.GetPortraitTexture(Tools.StringToInt(portraitName.Substring("pic=".Length)));
                }
                else
                {
                    texture = DialogueManager.LoadAsset(portraitName) as Texture2D;
                }
            }
            if (DialogueDebug.logWarnings && (texture == null)) Debug.LogWarning(string.Format("{0}: SetPortrait({1}, {2}): portrait texture not found.", new System.Object[] { DialogueDebug.Prefix, actorName, portraitName }));
            SetActorPortraitTexture(actorName, texture);
        }

        /// <summary>
        /// This method is used internally by SetPortrait() to set the portrait texture
        /// for an actor.
        /// </summary>
        /// <param name="actorName">Actor name.</param>
        /// <param name="portraitTexture">Portrait texture.</param>
        public void SetActorPortraitTexture(string actorName, Texture2D portraitTexture)
        {
            if (isConversationActive && (m_conversationController != null))
            {
                m_conversationController.SetActorPortraitTexture(actorName, portraitTexture);
            }
        }

        /// <summary>
        /// Looks for any dialogue UI or display settings overrides on the conversant (preferred)
        /// or the actor (otherwise).
        /// </summary>
        /// <param name='actor'>Actor.</param>
        /// <param name='conversant'>Conversant.</param>
        private void SetConversationUI(Transform actor, Transform conversant)
        {
            var overrideUI = FindHighestPriorityOverrideUI(actor, conversant);
            if (overrideUI != null)
            {
                ApplyOverrideUI(overrideUI);
            }
            ValidateCurrentDialogueUI();
        }

        private OverrideUIBase FindHighestPriorityOverrideUI(Transform actor, Transform conversant)
        {
            var actorOverrideUI = FindOverrideUI(actor);
            var conversantOverrideUI = FindOverrideUI(conversant);
            if (actorOverrideUI != null)
            {
                if (conversantOverrideUI != null)
                {
                    return (actorOverrideUI.priority > conversantOverrideUI.priority) ? actorOverrideUI : conversantOverrideUI;
                }
                else
                {
                    return actorOverrideUI;
                }
            }
            else
            {
                return conversantOverrideUI;
            }
        }

        private OverrideUIBase FindOverrideUI(Transform character)
        {
            if (character == null) return null;
            var overrideUI = character.GetComponentInChildren<OverrideUIBase>();
            var overrideDialogueUI = overrideUI as OverrideDialogueUI;
            var overrideDisplaySettings = overrideUI as OverrideDisplaySettings;
            return (overrideUI != null) && overrideUI.enabled &&
                ((overrideDialogueUI != null && overrideDialogueUI.ui != null) || (overrideDisplaySettings != null))
                    ? overrideUI : null;
        }

        private void ApplyOverrideUI(OverrideUIBase overrideUI)
        {
            var overrideDialogueUI = overrideUI as OverrideDialogueUI;
            var overrideDisplaySettings = overrideUI as OverrideDisplaySettings;
            if (overrideDialogueUI != null)
            {
                m_isOverrideUIPrefab = Tools.IsPrefab(overrideDialogueUI.ui);
                m_originalDialogueUI = dialogueUI;
                displaySettings.dialogueUI = overrideDialogueUI.ui;
                m_currentDialogueUI = null;
            }
            else if (overrideDisplaySettings)
            {
                if (overrideDisplaySettings.displaySettings.dialogueUI != null)
                {
                    m_isOverrideUIPrefab = Tools.IsPrefab(overrideDisplaySettings.displaySettings.dialogueUI);
                    m_originalDialogueUI = dialogueUI;
                    m_currentDialogueUI = null;
                }
                m_originalDisplaySettings = displaySettings;
                displaySettings = overrideDisplaySettings.displaySettings;
                if (overrideDisplaySettings.displaySettings.dialogueUI == null) overrideDisplaySettings.displaySettings.dialogueUI = m_originalDisplaySettings.dialogueUI;
            }
        }

        private void RestoreOriginalUI()
        {
            if (m_originalDisplaySettings != null) displaySettings = m_originalDisplaySettings;
            if (m_originalDialogueUI != null)
            {
                if (m_isOverrideUIPrefab)
                {
                    MonoBehaviour uiBehaviour = m_currentDialogueUI as MonoBehaviour;
                    if (uiBehaviour != null) Destroy(uiBehaviour.gameObject);
                }
                m_currentDialogueUI = m_originalDialogueUI;
                displaySettings.dialogueUI = (m_originalDialogueUI as MonoBehaviour).gameObject;
            }
            m_isOverrideUIPrefab = false;
            m_originalDialogueUI = null;
            m_originalDisplaySettings = null;
        }

        private void OnDialogueEntrySpoken(Subtitle subtitle)
        {
            m_luaWatchers.NotifyObservers(LuaWatchFrequency.EveryDialogueEntry);
        }

        /// <summary>
        /// Handles the end conversation event.
        /// </summary>
        public void OnEndConversation(ConversationController endingConversationController)
        {
            // Find and remove the record associated with the ConversationController:
            var record = m_activeConversations.Find(r => r.conversationController == endingConversationController);

            m_activeConversations.Remove(record);

            // Promote a remaining active conversation, or clear out the active conversation properties:
            if (m_activeConversations.Count > 0)
            {
                var nextRecord = m_activeConversations[0];
                m_conversationController = nextRecord.conversationController;
                currentActor = nextRecord.actor;
                currentConversant = nextRecord.conversant;
            }
            else
            {
                m_conversationController = null;
                currentActor = null;
                currentConversant = null;
            }

            // Restore UI:
            m_originalDialogueUI = record.originalDialogueUI;
            m_originalDisplaySettings = record.originalDisplaySettings;
            m_isOverrideUIPrefab = record.isOverrideUIPrefab;
            RestoreOriginalUI();

            // End of conversation checks:
            m_luaWatchers.NotifyObservers(LuaWatchFrequency.EndOfConversation);
            CheckAlerts();
        }

        /// <summary>
        /// Handles the conversation response menu timeout event.
        /// </summary>
        public void OnConversationTimeout()
        {
            if (isConversationActive)
            {
                if (displaySettings.inputSettings.responseTimeoutAction == ResponseTimeoutAction.EndConversation)
                {
                    StopConversation();
                }
                else
                {
                    StartCoroutine(ChooseResponseAfterOneFrame(displaySettings.inputSettings.responseTimeoutAction));
                }
            }
        }

        private void OnConversationStart(Transform actor)
        {
            conversationStarted(actor);
        }

        private void OnConversationEnd(Transform actor)
        {
            conversationEnded(actor);
        }

        /// <summary>
        /// Chooses the first response after one frame. We wait one frame in case a customer-defined
        /// OnConversationTimeout handler decides to stop the conversation first.
        /// </summary>
        private IEnumerator ChooseResponseAfterOneFrame(ResponseTimeoutAction responseTimeoutAction)
        {
            yield return null;
            if (isConversationActive)
            {
                switch (responseTimeoutAction)
                {
                    default:
                    case ResponseTimeoutAction.ChooseFirstResponse:
                        m_conversationController.GotoFirstResponse();
                        break;
                    case ResponseTimeoutAction.ChooseRandomResponse:
                        m_conversationController.GotoRandomResponse();
                        break;
                    case ResponseTimeoutAction.ChooseCurrentResponse:
                        m_conversationController.GotoCurrentResponse();
                        break;
                }
            }
        }

        /// <summary>
        /// Causes a character to bark a line at another character. A bark is a line spoken outside
        /// of a full conversation. It uses a simple gameplay bark UI instead of the dialogue UI.
        /// </summary>
        /// <param name='conversationTitle'>
        /// Title of the conversation that contains the bark lines. In this conversation, all 
        /// dialogue entries linked from the first entry are considered bark lines.
        /// </param>
        /// <param name='speaker'>
        /// The character barking the line.
        /// </param>
        /// <param name='listener'>
        /// The character being barked at.
        /// </param>
        /// <param name='barkHistory'>
        /// Bark history used to track the most recent bark, so the bark controller can go through
        /// the bark lines in a specified order.
        /// </param>
        public void Bark(string conversationTitle, Transform speaker, Transform listener, BarkHistory barkHistory)
        {
            CheckDebugLevel();
            StartCoroutine(BarkController.Bark(conversationTitle, speaker, listener, barkHistory));
        }

        /// <summary>
        /// Causes a character to bark a line at another character. A bark is a line spoken outside
        /// of a full conversation. It uses a simple gameplay bark UI instead of the dialogue UI.
        /// Since this form of the Bark() method does not include a BarkHistory, a random bark is
        /// selected from the bark lines.
        /// </summary>
        /// <param name='conversationTitle'>
        /// Title of the conversation that contains the bark lines. In this conversation, all 
        /// dialogue entries linked from the first entry are considered bark lines.
        /// </param>
        /// <param name='speaker'>
        /// The character barking the line.
        /// </param>
        /// <param name='listener'>
        /// The character being barked at.
        /// </param>
        public void Bark(string conversationTitle, Transform speaker, Transform listener)
        {
            Bark(conversationTitle, speaker, listener, new BarkHistory(BarkOrder.Random));
        }

        /// <summary>
        /// Causes a character to bark a line. A bark is a line spoken outside
        /// of a full conversation. It uses a simple gameplay bark UI instead of the dialogue UI.
        /// Since this form of the Bark() method does not include a BarkHistory, a random bark is
        /// selected from the bark lines.
        /// </summary>
        /// <param name='conversationTitle'>
        /// Title of the conversation that contains the bark lines. In this conversation, all 
        /// dialogue entries linked from the first entry are considered bark lines.
        /// </param>
        /// <param name='speaker'>
        /// The character barking the line.
        /// </param>
        public void Bark(string conversationTitle, Transform speaker)
        {
            Bark(conversationTitle, speaker, null, new BarkHistory(BarkOrder.Random));
        }

        /// <summary>
        /// Causes a character to bark a line. A bark is a line spoken outside
        /// of a full conversation. It uses a simple gameplay bark UI instead of the dialogue UI.
        /// </summary>
        /// <param name='conversationTitle'>
        /// Title of the conversation that contains the bark lines. In this conversation, all 
        /// dialogue entries linked from the first entry are considered bark lines.
        /// </param>
        /// <param name='speaker'>
        /// The character barking the line.
        /// </param>
        /// <param name='barkHistory'>
        /// Bark history used to track the most recent bark, so the bark controller can go through
        /// the bark lines in a specified order.
        /// </param>
        public void Bark(string conversationTitle, Transform speaker, BarkHistory barkHistory)
        {
            Bark(conversationTitle, speaker, null, barkHistory);
        }

        /// <summary>
        /// Causes a character to bark a literal string instead of looking up its line from
        /// a conversation.
        /// </summary>
        /// <param name="barkText">The string to bark.</param>
        /// <param name="speaker">The barker.</param>
        /// <param name="listener">The target of the bark. (May be null)</param>
        /// <param name="sequence">The optional sequence to play. (May be null or empty)</param>
        public void BarkString(string barkText, Transform speaker, Transform listener = null, string sequence = null)
        {
            if (speaker == null)
            {
                if (DialogueDebug.logWarnings) Debug.LogWarning("Dialogue System: Can't bark '" + barkText + "'. No speaker specified.");
                return;
            }
            var speakerInfo = GetCharacterInfoFromTransform(speaker);
            var listenerInfo = GetCharacterInfoFromTransform(listener);
            var formattedText = FormattedText.Parse(barkText, masterDatabase.emphasisSettings);
            if (sequence == null) sequence = string.Empty;
            var responseMenuSequence = string.Empty; // Not used in barks.
            DialogueEntry dialogueEntry = null;
            var subtitle = new Subtitle(speakerInfo, listenerInfo, formattedText, sequence, responseMenuSequence, dialogueEntry);
            if (DialogueDebug.logInfo) Debug.Log("Dialogue System: " + speaker + " barking string '" + barkText + "'.");
            StartCoroutine(BarkController.Bark(subtitle));
        }

        /// <summary>
        /// Returns the default duration that a string of bark text will be shown.
        /// </summary>
        /// <param name="barkText">The text that will be barked (to determine the duration).</param>
        /// <returns>The default duration in seconds for the specified bark text.</returns>
        public float GetBarkDuration(string barkText)
        {
            var minSecs = displaySettings.barkSettings.minBarkSeconds;
            if (Mathf.Approximately(0, minSecs)) minSecs = displaySettings.subtitleSettings.minSubtitleSeconds;
            var charsPerSec = displaySettings.barkSettings.barkCharsPerSecond;
            if (Mathf.Approximately(0, charsPerSec)) charsPerSec = displaySettings.subtitleSettings.subtitleCharsPerSecond;
            float duration = string.IsNullOrEmpty(barkText) ? 0 : Mathf.Max(minSecs, barkText.Length / charsPerSec);
            return duration;
        }

        private CharacterInfo GetCharacterInfoFromTransform(Transform actorTransform)
        {
            var actorName = DialogueActor.GetActorName(actorTransform);
            var actor = masterDatabase.GetActor(actorName);
            var actorID = (actor != null) ? actor.id : 1;
            var portrait = (actor != null) ? actor.portrait : null;
            return new CharacterInfo(actorID, actorName, actorTransform, CharacterType.NPC, portrait);
        }

        /// <summary>
        /// Shows an alert message using the dialogue UI.
        /// </summary>
        /// <param name='message'>
        /// The message to show.
        /// </param>
        /// <param name='duration'>
        /// The duration in seconds to show the message.
        /// </param>
        public void ShowAlert(string message, float duration)
        {
            if (dialogueUI != null && (displaySettings.alertSettings.allowAlertsDuringConversations || !isConversationActive))
            {
                if (message.Contains("\\n")) message = message.Replace("\\n", "\n");
                dialogueUI.ShowAlert(GetLocalizedText(message), duration);
            }
        }

        /// <summary>
        /// Shows an alert message using the dialogue UI for the UI's default duration.
        /// </summary>
        /// <param name='message'>
        /// The message to show.
        /// </param>
        public void ShowAlert(string message)
        {
            var minSecs = displaySettings.alertSettings.minAlertSeconds;
            if (Mathf.Approximately(0, minSecs)) minSecs = displaySettings.subtitleSettings.minSubtitleSeconds;
            var charsPerSec = displaySettings.alertSettings.alertCharsPerSecond;
            if (Mathf.Approximately(0, charsPerSec)) charsPerSec = displaySettings.subtitleSettings.subtitleCharsPerSecond;
            float duration = string.IsNullOrEmpty(message) ? 0 : Mathf.Max(minSecs, message.Length / charsPerSec);
            ShowAlert(message, duration);
        }

        /// <summary>
        /// Checks the value of Lua Variable['Alert']. If set, it shows the alert and clears the
        /// variable.
        /// </summary>
        public void CheckAlerts()
        {
            if (displaySettings.alertSettings.allowAlertsDuringConversations || !isConversationActive)
            {
                string message = DialogueLua.GetVariable("Alert").asString;
                if (!string.IsNullOrEmpty(message) && !string.Equals(message, "nil"))
                {
                    Lua.Run("Variable['Alert'] = ''");
                    ShowAlert(message);
                }
            }
        }

        /// <summary>
        /// Monitors the value of Lua Variable['Alert'] on a regular frequency specified in
        /// displaySettings. If the frequency is zero, it doesn't monitor.
        /// </summary>
        /// <returns>
        /// The alerts.
        /// </returns>
        private IEnumerator MonitorAlerts()
        {
            if ((displaySettings == null) || (displaySettings.alertSettings == null) || (Tools.ApproximatelyZero(displaySettings.alertSettings.alertCheckFrequency))) yield break;
            var currentFrequency = displaySettings.alertSettings.alertCheckFrequency;
            var waitForSeconds = new WaitForSeconds(displaySettings.alertSettings.alertCheckFrequency);
            while (true)
            {
                if (currentFrequency != displaySettings.alertSettings.alertCheckFrequency)
                {
                    waitForSeconds = new WaitForSeconds(displaySettings.alertSettings.alertCheckFrequency);
                }
                yield return waitForSeconds;
                CheckAlerts();
            }
        }

        public void HideAlert()
        {
            if (dialogueUI != null)
            {
                dialogueUI.HideAlert();
            }
        }

        /// <summary>
        /// Gets localized text.
        /// </summary>
        /// <returns>If the specified field exists in the table, returns the field's 
        /// localized text for the current language. Otherwise returns the field itself.</returns>
        /// <param name="s">The field to look up.</param>
        public string GetLocalizedText(string s)
        {
            if (displaySettings.localizationSettings.textTable == null)
            {
                return s;
            }
            else
            {
                var textTable = displaySettings.localizationSettings.textTable;
                string localizedText = textTable.GetFieldTextForLanguage(s, Localization.GetCurrentLanguageID(textTable));
                return string.IsNullOrEmpty(localizedText) ? s : localizedText;
            }
        }

        /// <summary>
        /// Starts a sequence. See @ref sequencer.
        /// </summary>
        /// <param name="sequence">The sequence to play.</param>
        /// <param name="speaker">The speaker, for sequence commands that reference the speaker.</param>
        /// <param name="listener">The listener, for sequence commands that reference the listener.</param>
        /// <param name="informParticipants">Specifies whether to send OnSequenceStart and OnSequenceEnd messages to the speaker and listener. Default is <c>true</c>.</param>
        /// <param name="destroyWhenDone">Specifies whether destroy the sequencer when done playing the sequence. Default is </param>
        /// <param name="entrytag">The entrytag to associate with the sequence.</param>
        /// <returns>The sequencer that is playing the sequence.</returns>
        public Sequencer PlaySequence(string sequence, Transform speaker, Transform listener, bool informParticipants, bool destroyWhenDone, string entrytag)
        {
            CheckDebugLevel();
            Sequencer sequencer = GetNewSequencer();
            sequencer.Open();
            sequencer.entrytag = entrytag;
            sequencer.PlaySequence(sequence, speaker, listener, informParticipants, destroyWhenDone);
            return sequencer;
        }

        /// <summary>
        /// Starts a sequence. See @ref sequencer.
        /// </summary>
        /// <returns>
        /// The sequencer that is playing the sequence.
        /// </returns>
        /// <param name='sequence'>
        /// The sequence to play.
        /// </param>
        /// <param name='speaker'>
        /// The speaker, for sequence commands that reference the speaker.
        /// </param>
        /// <param name='listener'>
        /// The listener, for sequence commands that reference the listener.
        /// </param>
        /// <param name='informParticipants'>
        /// Specifies whether to send OnSequenceStart and OnSequenceEnd messages to the speaker and
        /// listener. Default is <c>true</c>.
        /// </param>
        /// <param name='destroyWhenDone'>
        /// Specifies whether destroy the sequencer when done playing the sequence. Default is 
        /// <c>true</c>.
        /// </param>
        public Sequencer PlaySequence(string sequence, Transform speaker, Transform listener, bool informParticipants, bool destroyWhenDone)
        {
            return PlaySequence(sequence, speaker, listener, informParticipants, destroyWhenDone, string.Empty);
        }

        /// <summary>
        /// Starts a sequence. See @ref sequencer.
        /// </summary>
        /// <returns>
        /// The sequencer that is playing the sequence.
        /// </returns>
        /// <param name='sequence'>
        /// The sequence to play.
        /// </param>
        /// <param name='speaker'>
        /// The speaker, for sequence commands that reference the speaker.
        /// </param>
        /// <param name='listener'>
        /// The listener, for sequence commands that reference the listener.
        /// </param>
        /// <param name='informParticipants'>
        /// Specifies whether to send OnSequenceStart and OnSequenceEnd messages to the speaker and
        /// listener. Default is <c>true</c>.
        /// </param>
        public Sequencer PlaySequence(string sequence, Transform speaker, Transform listener, bool informParticipants)
        {
            return PlaySequence(sequence, speaker, listener, informParticipants, true, string.Empty);
        }

        /// <summary>
        /// Starts a sequence, and sends OnSequenceStart/OnSequenceEnd messages to the 
        /// participants. See @ref sequencer.
        /// </summary>
        /// <returns>
        /// The sequencer that is playing the sequence.
        /// </returns>
        /// <param name='sequence'>
        /// The sequence to play.
        /// </param>
        /// <param name='speaker'>
        /// The speaker, for sequence commands that reference the speaker.
        /// </param>
        /// <param name='listener'>
        /// The listener, for sequence commands that reference the listener.
        /// </param>
        public Sequencer PlaySequence(string sequence, Transform speaker, Transform listener)
        {
            return PlaySequence(sequence, speaker, listener, true);
        }

        /// <summary>
        /// Starts a sequence. See @ref sequencer.
        /// </summary>
        /// <returns>
        /// The sequencer that is playing the sequence.
        /// </returns>
        /// <param name='sequence'>
        /// The sequence to play.
        /// </param>
        public Sequencer PlaySequence(string sequence)
        {
            return PlaySequence(sequence, null, null, false);
        }

        /// <summary>
        /// Stops a sequence.
        /// </summary>
        /// <param name='sequencer'>
        /// The sequencer playing the sequence.
        /// </param>
        public void StopSequence(Sequencer sequencer)
        {
            if (sequencer != null) sequencer.Close();
        }

        /// <summary>
        /// Pauses the Dialogue System. Also broadcasts OnDialogueSystemPause to 
        /// the Dialogue Manager and conversation participants. Conversations,
        /// timers, typewriter and fade effects, and the AudioWait() and Voice()
        /// sequencer commands will be paused. Other than this, AudioSource,
        /// Animation, and Animator components will not be paused; it's up to
        /// you to handle them as appropriate for your project.
        /// </summary>
        public void Pause()
        {
            DialogueTime.isPaused = true;
            BroadcastDialogueSystemMessage(DialogueSystemMessages.OnDialogueSystemPause);
        }

        /// <summary>
        /// Unpauses the Dialogue System. Also broadcasts OnDialogueSystemUnpause to 
        /// the Dialogue Manager and conversation participants.
        /// </summary>
        public void Unpause()
        {
            DialogueTime.isPaused = false;
            BroadcastDialogueSystemMessage(DialogueSystemMessages.OnDialogueSystemUnpause);
        }

        private void BroadcastDialogueSystemMessage(string message)
        {
            BroadcastMessage(message, SendMessageOptions.DontRequireReceiver);
            if (isConversationActive)
            {
                if (currentActor != null) currentActor.BroadcastMessage(message, SendMessageOptions.DontRequireReceiver);
                if (currentConversant != null) currentConversant.BroadcastMessage(message, SendMessageOptions.DontRequireReceiver);
            }
        }

        /// <summary>
        /// Sets the dialogue UI. (Deprecated; just set DialogueUI now.)
        /// </summary>
        /// <param name='gameObject'>
        /// Game object containing an implementation of IDialogueUI.
        /// </param>
        public void UseDialogueUI(GameObject gameObject)
        {
            m_currentDialogueUI = null;
            displaySettings.dialogueUI = gameObject;
            ValidateCurrentDialogueUI();
        }

        private IDialogueUI GetDialogueUI()
        {
            ValidateCurrentDialogueUI();
            return m_currentDialogueUI;
        }

        private void SetDialogueUI(IDialogueUI ui)
        {
            var uiBehaviour = ui as MonoBehaviour;
            if (uiBehaviour != null)
            {
                if (Tools.IsPrefab(uiBehaviour.gameObject))
                {
                    // If prefab, instantiate it:
                    var instanceGO = Instantiate(uiBehaviour.gameObject);
                    instanceGO.name = uiBehaviour.gameObject.name;
                    uiBehaviour = instanceGO.GetComponent(typeof(IDialogueUI)) as MonoBehaviour;
                    if (uiBehaviour is CanvasDialogueUI && uiBehaviour.GetComponentInChildren<Canvas>() == null)
                    {
                        // If a canvas-based UI that doesn't come with its own canvas, add a canvas:
                        var canvas = GetOrCreateDefaultCanvas();
                        instanceGO.transform.SetParent(canvas.transform, false);
                    }
                    else
                    {
                        // Otherwise make it a direct child of the Dialogue Manager:
                        instanceGO.transform.SetParent(transform);
                    }
                }

                displaySettings.dialogueUI = uiBehaviour.gameObject;
                m_currentDialogueUI = null;
                ValidateCurrentDialogueUI();
            }
        }

        /// <summary>
        /// If currentDialogueUI is <c>null</c>, gets it from displaySettings.dialogueUI. If the
        /// value in displaySettings.dialogueUI is a prefab, loads the prefab. Then updates
        /// displaySettings.dialogueUI.
        /// </summary>
        private void ValidateCurrentDialogueUI()
        {
            if (m_currentDialogueUI == null)
            {
                GetDialogueUIFromDisplaySettings();
                if (m_currentDialogueUI == null)
                {
                    m_currentDialogueUI = LoadDefaultDialogueUI();
                }
                MonoBehaviour uiBehaviour = m_currentDialogueUI as MonoBehaviour;
                if (uiBehaviour != null) displaySettings.dialogueUI = uiBehaviour.gameObject;
            }
        }

        private void GetDialogueUIFromDisplaySettings()
        {
            if (displaySettings.dialogueUI != null)
            {
                // Dialogue UI field is assigned, so use it:
                m_currentDialogueUI = Tools.IsPrefab(displaySettings.dialogueUI)
                    ? LoadDialogueUIPrefab(displaySettings.dialogueUI)
                    : displaySettings.dialogueUI.GetComponentInChildren(typeof(IDialogueUI)) as IDialogueUI;
                if ((m_currentDialogueUI == null) && DialogueDebug.logWarnings) Debug.LogWarning(string.Format("{0}: No Dialogue UI found on '{1}'. Is the GameObject active and the dialogue UI script enabled? (Will load default UI for now.)", new System.Object[] { DialogueDebug.Prefix, displaySettings.dialogueUI }), displaySettings.dialogueUI);
            }
            else
            {
                // Otherwise search for IDialogueUI on children:
                m_currentDialogueUI = GetComponentInChildren(typeof(IDialogueUI)) as IDialogueUI;
            }
        }

        private IDialogueUI LoadDefaultDialogueUI()
        {
            if (DialogueDebug.logInfo) Debug.Log(string.Format("{0}: Loading default Dialogue UI '{1}'.", new System.Object[] { DialogueDebug.Prefix, DefaultDialogueUIResourceName }));
            var defaultUIGameObject = DialogueManager.LoadAsset(DefaultDialogueUIResourceName) as GameObject;
            if (defaultUIGameObject == null)
            {
                Debug.LogError(DialogueDebug.Prefix + ": Can't load " + DefaultDialogueUIResourceName + "! Did you delete this file from Dialogue System/Prefabs/Legacy Unity GUI Prefabs/Default/Resources? If so, add it back, or assign a dialogue UI to the Dialogue Manager so it doesn't have to try to load the default UI.");
                return null;
            }
            else
            {
                return LoadDialogueUIPrefab(defaultUIGameObject);
            }
        }

        private IDialogueUI LoadDialogueUIPrefab(GameObject prefab)
        {
            GameObject go = Instantiate(prefab) as GameObject;
            go.name = prefab.name;
            IDialogueUI ui = null;
            if (go != null)
            {
                ui = go.GetComponentInChildren(typeof(IDialogueUI)) as IDialogueUI;
                if (ui == null)
                {
                    if (DialogueDebug.logErrors) Debug.LogError(string.Format("{0}: No Dialogue UI component found on {1}.", new System.Object[] { DialogueDebug.Prefix, prefab }));
                    Destroy(go);
                }
                else if (ui is CanvasDialogueUI)
                {
                    // If a canvas-based UI, add to canvas:
                    var canvas = GetOrCreateDefaultCanvas();
                    go.transform.SetParent(canvas.transform, false);
                }
                else
                {
                    // Otherwise make it a direct child of the Dialogue Manager:
                    go.transform.SetParent(transform, false);
                }
            }
            return ui;
        }

        private Canvas GetOrCreateDefaultCanvas()
        {
            var canvas = GetComponentInChildren<Canvas>();
            if (canvas == null)
            {
                var canvasObject = new GameObject("Canvas");
                canvasObject.layer = 5; // Built-in UI layer.
                canvasObject.AddComponent<Canvas>();
                canvasObject.AddComponent<UnityEngine.UI.CanvasScaler>();
                canvasObject.AddComponent<UnityEngine.UI.GraphicRaycaster>();
                canvas = canvasObject.GetComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                canvasObject.transform.SetParent(transform);
            }
            return canvas;
        }

        private Sequencer GetNewSequencer()
        {
            Sequencer sequencer = this.gameObject.AddComponent<Sequencer>();
            if (sequencer != null)
            {
                sequencer.UseCamera(displaySettings.cameraSettings.sequencerCamera, displaySettings.cameraSettings.alternateCameraObject, displaySettings.cameraSettings.cameraAngles);
                sequencer.disableInternalSequencerCommands = displaySettings.cameraSettings.disableInternalSequencerCommands;
            }
            return sequencer;
        }

        private void RegisterLuaFunctions()
        {
            // Unity 2017.3 bug IL2CPP can't do lambdas:
            //Lua.RegisterFunction("ShowAlert", null, SymbolExtensions.GetMethodInfo(() => LuaShowAlert(string.Empty)));

            Lua.RegisterFunction("ShowAlert", null, typeof(DialogueSystemController).GetMethod("LuaShowAlert"));
            Lua.RegisterFunction("HideAlert", null, typeof(DialogueSystemController).GetMethod("LuaHideAlert"));
            Lua.RegisterFunction("RandomizeNextEntry", this, typeof(DialogueSystemController).GetMethod("RandomizeNextEntry"));
        }

        public static void LuaShowAlert(string message)
        {
            DialogueManager.ShowAlert(message);
        }

        public static void LuaHideAlert()
        {
            DialogueManager.HideAlert();
        }

        public void RandomizeNextEntry()
        {
            m_calledRandomizeNextEntry = true;
            if (conversationController != null) conversationController.randomizeNextEntry = true;
        }

        /// <summary>
        /// Adds a Lua expression observer.
        /// </summary>
        /// <param name='luaExpression'>
        /// Lua expression to watch.
        /// </param>
        /// <param name='frequency'>
        /// Frequency to check the expression.
        /// </param>
        /// <param name='luaChangedHandler'>
        /// Delegate to call when the expression changes. This should be in the form:
        /// <code>void MyDelegate(LuaWatchItem luaWatchItem, Lua.Result newValue) {...}</code>
        /// </param>
        public void AddLuaObserver(string luaExpression, LuaWatchFrequency frequency, LuaChangedDelegate luaChangedHandler)
        {
            StartCoroutine(AddLuaObserverAfterStart(luaExpression, frequency, luaChangedHandler));
        }

        private IEnumerator AddLuaObserverAfterStart(string luaExpression, LuaWatchFrequency frequency, LuaChangedDelegate luaChangedHandler)
        {
            int MaxFramesToWait = 10;
            int framesWaited = 0;
            while (!m_started && (framesWaited < MaxFramesToWait))
            {
                framesWaited++;
                yield return null;
            }
            yield return null;
            m_luaWatchers.AddObserver(luaExpression, frequency, luaChangedHandler);
        }

        /// <summary>
        /// Removes a Lua expression observer. To be removed, the expression, frequency, and
        /// notification delegate must all match.
        /// </summary>
        /// <param name='luaExpression'>
        /// Lua expression being watched.
        /// </param>
        /// <param name='frequency'>
        /// Frequency that the expression is being watched.
        /// </param>
        /// <param name='luaChangedHandler'>
        /// Delegate that's called when the expression changes.
        /// </param>
        public void RemoveLuaObserver(string luaExpression, LuaWatchFrequency frequency, LuaChangedDelegate luaChangedHandler)
        {
            m_luaWatchers.RemoveObserver(luaExpression, frequency, luaChangedHandler);
        }

        /// <summary>
        /// Removes all Lua expression observers for a specified frequency.
        /// </summary>
        /// <param name='frequency'>
        /// Frequency.
        /// </param>
        public void RemoveAllObservers(LuaWatchFrequency frequency)
        {
            m_luaWatchers.RemoveAllObservers(frequency);
        }

        /// <summary>
        /// Removes all Lua expression observers.
        /// </summary>
        public void RemoveAllObservers()
        {
            m_luaWatchers.RemoveAllObservers();
        }

        void Update()
        {
            if (Lua.wasInvoked)
            {
                m_luaWatchers.NotifyObservers(LuaWatchFrequency.EveryUpdate);
                Lua.wasInvoked = false;
            }
        }

        private void UpdateTracker()
        {
            receivedUpdateTracker();
        }

        /// <summary>
        /// Registers an asset bundle with the Dialogue System. This allows sequencer
        /// commands to load assets inside it.
        /// </summary>
        /// <param name="bundle">Asset bundle.</param>
        public void RegisterAssetBundle(AssetBundle bundle)
        {
            m_assetBundleManager.RegisterAssetBundle(bundle);
        }

        /// <summary>
        /// Unregisters an asset bundle from the Dialogue System. Always unregister
        /// asset bundles before freeing them.
        /// </summary>
        /// <param name="bundle">Asset bundle.</param>
        public void UnregisterAssetBundle(AssetBundle bundle)
        {
            m_assetBundleManager.UnregisterAssetBundle(bundle);
        }

        /// <summary>
        /// Loads a named asset from the registered asset bundles or from Resources.
        /// </summary>
        /// <returns>The asset, or <c>null</c> if not found.</returns>
        /// <param name="name">Name of the asset.</param>
        public UnityEngine.Object LoadAsset(string name)
        {
            return m_assetBundleManager.Load(name);
        }

        /// <summary>
        /// Loads a named asset from the registered asset bundles or from Resources.
        /// </summary>
        /// <returns>The asset, or <c>null</c> if not found.</returns>
        /// <param name="name">Name of the asset.</param>
        /// <param name="type">Type of the asset.</param>
        public UnityEngine.Object LoadAsset(string name, System.Type type)
        {
            return m_assetBundleManager.Load(name, type);
        }

#if EVALUATION_VERSION
		private GUIStyle evaluationWatermarkStyle = null;
		private Rect watermarkRect1;
		private Rect watermarkRect2;
		
		public void OnGUI() {
			if (Camera.main == null) return;
			if (Camera.main.GetComponent<GUILayer>() == null) Camera.main.gameObject.AddComponent<GUILayer>();
			if (evaluationWatermarkStyle == null) {
				evaluationWatermarkStyle = new GUIStyle(GUI.skin.label);
				evaluationWatermarkStyle.fontSize = 20;
				evaluationWatermarkStyle.fontStyle = FontStyle.Bold;
				evaluationWatermarkStyle.alignment = TextAnchor.MiddleCenter;
				evaluationWatermarkStyle.normal.textColor = new Color(1, 1, 1, 0.5f);
				Vector2 size = evaluationWatermarkStyle.CalcSize(new GUIContent("Evaluation Version"));
                if (Random.value < 0.5f) {
                    watermarkRect1 = new Rect(Screen.width - size.x - 20, 0, size.x + 20, size.y);
                    watermarkRect2 = new Rect(Screen.width - size.x - 20, size.y - 8, size.x + 20, size.y);
                } else {
                    watermarkRect1 = new Rect(20, Screen.height - 2 * size.y - 8 , size.x + 20, size.y);
                    watermarkRect2 = new Rect(20, Screen.height - size.y - 8, size.x + 20, size.y);
                }
            }
			GUI.Label(watermarkRect1, "Dialogue System", evaluationWatermarkStyle);
			GUI.Label(watermarkRect2, "Evaluation Version", evaluationWatermarkStyle);
		}
#endif

    }

}
