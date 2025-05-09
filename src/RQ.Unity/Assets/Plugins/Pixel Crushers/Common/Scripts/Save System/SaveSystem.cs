﻿// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PixelCrushers
{

    /// <summary>
    /// This is the main Save System class. It runs as a singleton MonoBehaviour
    /// and provides static methods to save and load games.
    /// </summary>
    [AddComponentMenu("")] // Use wrapper instead.
    public class SaveSystem : MonoBehaviour
    {

        public const int NoSceneIndex = -1;

        /// <summary>
        /// Stores an int indicating the slot number of the most recently saved game.
        /// </summary>
        public const string LastSavedGameSlotPlayerPrefsKey = "savedgame_lastSlotNum";

        [Tooltip("When loading a game, load the scene that the game was saved in.")]
        [SerializeField]
        private bool m_saveCurrentScene = true;

        [Tooltip("When loading a game/scene, wait this many frames before applying saved data to allow other scripts to initialize first.")]
        [SerializeField]
        private int m_framesToWaitBeforeApplyData = 1;

        [Tooltip("Log debug info.")]
        [SerializeField]
        private bool m_debug = false;

        private static SaveSystem m_instance = null;

        private static List<Saver> m_savers = new List<Saver>();

        private static SavedGameData m_savedGameData = new SavedGameData();

        private static DataSerializer m_serializer = null;

        private static SavedGameDataStorer m_storer = null;

        private static SceneTransitionManager m_sceneTransitionManager = null;

        private static GameObject m_playerSpawnpoint = null;

        private static int m_currentSceneIndex = NoSceneIndex;

        /// <summary>
        /// When loading a game, load the scene that the game was saved in.
        /// </summary>
        public static bool saveCurrentScene
        {
            get
            {
                return (m_instance != null) ? m_instance.m_saveCurrentScene : true;
            }
            set
            {
                if (m_instance != null) m_instance.m_saveCurrentScene = value;
            }
        }

        /// <summary>
        /// When loading a game/scene, wait this many frames before applying saved data to allow other scripts to initialize first.
        /// </summary>
        public static int framesToWaitBeforeApplyData
        {
            get
            {
                return (m_instance != null) ? m_instance.m_framesToWaitBeforeApplyData : 1;
            }
            set
            {
                if (m_instance != null) m_instance.m_framesToWaitBeforeApplyData = value;
            }
        }

        public static bool debug
        {
            get
            {
                return (m_instance != null) ? m_instance.m_debug && Debug.isDebugBuild : false;
            }
            set
            {
                if (m_instance != null) m_instance.m_debug = value;
            }
        }

        public static SaveSystem instance
        {
            get
            {
                if (m_instance == null)
                {
                    m_instance = new GameObject("Save System", typeof(SaveSystem)).GetComponent<SaveSystem>();
                }
                return m_instance;
            }
        }

        private static DataSerializer serializer
        {
            get
            {
                if (m_serializer == null)
                {
                    m_serializer = instance.GetComponent<DataSerializer>();
                    if (m_serializer == null)
                    {
                        Debug.Log("Save System: No DataSerializer found on " + instance.name + ". Adding JsonDataSerializer.", instance);
                        m_serializer = instance.gameObject.AddComponent<JsonDataSerializer>();
                    }
                }
                return m_serializer;
            }
        }

        private static SavedGameDataStorer storer
        {
            get
            {
                if (m_storer == null)
                {
                    m_storer = instance.GetComponent<SavedGameDataStorer>();
                    if (m_storer == null)
                    {
                        Debug.Log("Save System: No SavedGameDataStorer found on " + instance.name + ". Adding PlayerPrefsSavedGameDataStorer.", instance);
                        m_storer = instance.gameObject.AddComponent<PlayerPrefsSavedGameDataStorer>();
                    }
                }
                return m_storer;
            }
        }

        public static SceneTransitionManager sceneTransitionManager
        {
            get
            {
                if (m_sceneTransitionManager == null)
                {
                    m_sceneTransitionManager = instance.GetComponentInChildren<SceneTransitionManager>();
                }
                return m_sceneTransitionManager;
            }
        }

        /// <summary>
        /// The saved game data recorded by the last call to SaveToSlot,
        /// LoadScene, or RecordSavedGameData.
        /// </summary>
        public static SavedGameData currentSavedGameData
        {
            get { return m_savedGameData; }
        }

        /// <summary>
        /// Where the player should spawn in the current scene.
        /// </summary>
        public static GameObject playerSpawnpoint
        {
            get { return m_playerSpawnpoint; }
            set { m_playerSpawnpoint = value; }
        }

        /// <summary>
        /// Build index of the current scene.
        /// </summary>
        public static int currentSceneIndex
        {
            get
            {
                if (m_currentSceneIndex == NoSceneIndex) m_currentSceneIndex = GetCurrentSceneIndex();
                return m_currentSceneIndex;
            }
        }

        public delegate void SceneLoadedDelegate(string sceneName, int sceneIndex);

        /// <summary>
        /// Invoked after a scene has been loaded.
        /// </summary>
        public static event SceneLoadedDelegate sceneLoaded = delegate { };

        /// <summary>
        /// Invoked when starting to save a game. If assigned, waits one frame before
        /// starting the save to allow UIs to update.
        /// </summary>
        public static event System.Action saveStarted = delegate { };

        /// <summary>
        /// Invoked when finished saving a game.
        /// </summary>
        public static event System.Action saveEnded = delegate { };

        /// <summary>
        /// Invoked when starting to load a game. If assigned, waits one frame before
        /// starting the load to allow UIs to update.
        /// </summary>
        public static event System.Action loadStarted = delegate { };

        /// <summary>
        /// Invoked when finished loading a game.
        /// </summary>
        public static event System.Action loadEnded = delegate { };

        private void Awake()
        {
            if (m_instance == null)
            {
                m_instance = this;
                if (transform.parent != null) transform.SetParent(null);
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void OnApplicationQuit()
        {
            BeforeSceneChange();
        }

#if UNITY_5_4_OR_NEWER
        private void OnEnable()
        {
            UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
            UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDisable()
        {
            UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        public void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode mode)
        {
            FinishedLoadingScene(scene.name, scene.buildIndex);
        }

#else
        public void OnLevelWasLoaded(int level)
        {
            FinishedLoadingScene(GetCurrentSceneName(), level);
        }
#endif

#if UNITY_5_3 || UNITY_5_3_OR_NEWER
        public static string GetCurrentSceneName()
        {
            return UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        }

        public static int GetCurrentSceneIndex()
        {
            return UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex;
        }

        private static IEnumerator LoadSceneInternal(string sceneName)
        {
            if (sceneTransitionManager == null)
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
                yield break;
            }
            else
            {
                yield return instance.StartCoroutine(LoadSceneInternalTransitionCoroutine(sceneName));
            }
        }

        private static IEnumerator LoadSceneInternalTransitionCoroutine(string sceneName)
        {
            yield return instance.StartCoroutine(sceneTransitionManager.LeaveScene());
            yield return UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName);
            instance.StartCoroutine(sceneTransitionManager.EnterScene());
        }
#else
        public static string GetCurrentSceneName()
        {
            return Application.loadedLevelName;
        }

        public static int GetCurrentSceneIndex()
        {
            return Application.loadedLevel;
        }

        private static IEnumerator LoadSceneInternal(string sceneName)
        {
            Application.LoadLevel(sceneName);
            yield break;
        }
#endif

        /// <summary>
        /// Saves a game into a slot using the storage provider on the 
        /// Save System GameObject.
        /// </summary>
        /// <param name="slotNumber">Slot in which to store saved game data.</param>
        public void SaveGameToSlot(int slotNumber)
        {
            SaveToSlot(slotNumber);
        }

        /// <summary>
        /// Loads a game from a slot using the storage provider on the
        /// Save System GameObject.
        /// </summary>
        /// <param name="slotNumber"></param>
        public void LoadGameFromSlot(int slotNumber)
        {
            LoadFromSlot(slotNumber);
        }

        /// <summary>
        /// Loads a scene, optionally positioning the player at a
        /// specified spawnpoint.
        /// </summary>
        /// <param name="sceneNameAndSpawnpoint">
        /// A string containing the name of the scene to load, optionally
        /// followed by "@spawnpoint" where "spawnpoint" is the name of
        /// a GameObject in that scene. The player will be spawned at that
        /// GameObject's position.
        /// </param>
        public void LoadSceneAtSpawnpoint(string sceneNameAndSpawnpoint)
        {
            LoadScene(sceneNameAndSpawnpoint);
        }

        /// <summary>
        /// Returns true if there is a saved game in the specified slot.
        /// </summary>
        public static bool HasSavedGameInSlot(int slotNumber)
        {
            return storer.HasDataInSlot(slotNumber);
        }

        /// <summary>
        /// Deletes the saved game in the specified slot.
        /// </summary>
        public static void DeleteSavedGameInSlot(int slotNumber)
        {
            storer.DeleteSavedGameData(slotNumber);
        }

        /// <summary>
        /// Saves the current game to a slot.
        /// </summary>
        public static void SaveToSlot(int slotNumber)
        {
            if (saveStarted.GetInvocationList().Length > 1)
            {
                instance.StartCoroutine(SaveToSlotCoroutine(slotNumber));
            }
            else
            {
                SaveToSlotNow(slotNumber);
            }
        }

        private static IEnumerator SaveToSlotCoroutine(int slotNumber)
        {
            saveStarted();
            yield return null;
            SaveToSlotNow(slotNumber);
            saveEnded();
        }

        private static void SaveToSlotNow(int slotNumber)
        {
            storer.StoreSavedGameData(slotNumber, RecordSavedGameData());
            PlayerPrefs.SetInt(LastSavedGameSlotPlayerPrefsKey, slotNumber);
        }

        /// <summary>
        /// Loads a game from a slot.
        /// </summary>
        public static void LoadFromSlot(int slotNumber)
        {
            if (!HasSavedGameInSlot(slotNumber))
            {
                if (Debug.isDebugBuild) Debug.LogWarning("Save System: LoadFromSlot(" + slotNumber + ") but there is no saved game in this slot.");
                return;
            }
            if (loadStarted.GetInvocationList().Length > 1)
            {
                instance.StartCoroutine(LoadFromSlotCoroutine(slotNumber));
            }
            else
            {
                LoadFromSlotNow(slotNumber);
            }
        }

        private static IEnumerator LoadFromSlotCoroutine(int slotNumber)
        {
            loadStarted();
            yield return null;
            LoadFromSlotNow(slotNumber);
            if (loadEnded.GetInvocationList().Length > 1)
            {
                sceneLoaded += NotifyLoadEndedWhenSceneLoaded;
            }
        }

        private static void NotifyLoadEndedWhenSceneLoaded(string sceneName, int sceneIndex)
        {
            sceneLoaded -= NotifyLoadEndedWhenSceneLoaded;
            loadEnded();
        }

        private static void LoadFromSlotNow(int slotNumber)
        { 
            LoadGame(storer.RetrieveSavedGameData(slotNumber));
        }

        public static void RegisterSaver(Saver saver)
        {
            if (saver == null || m_savers.Contains(saver)) return;
            m_savers.Add(saver);
        }

        public static void UnregisterSaver(Saver saver)
        {
            m_savers.Remove(saver);
        }

        /// <summary>
        /// Clears the SaveSystem's internal saved game data cache.
        /// </summary>
        public static void ClearSavedGameData()
        {
            m_savedGameData = new SavedGameData();
        }

        /// <summary>
        /// Records the current scene's savers' data into the SaveSystem's
        /// internal saved game data cache.
        /// </summary>
        /// <returns></returns>
        public static SavedGameData RecordSavedGameData()
        {
            m_savedGameData.sceneName = GetCurrentSceneName();
            for (int i = 0; i < m_savers.Count; i++)
            {
                try
                {
                    var saver = m_savers[i];
                    m_savedGameData.SetData(saver.key, GetSaverSceneIndex(saver), saver.RecordData());
                }
                catch (System.Exception e)
                {
                    Debug.LogException(e);
                }
            }
            return m_savedGameData;
        }

        private static int GetSaverSceneIndex(Saver saver)
        {
            return (saver == null || !saver.saveAcrossSceneChanges) ? currentSceneIndex : NoSceneIndex;
        }

        /// <summary>
        /// Updates the SaveSystem's internal saved game data cache with data for a 
        /// specific saver.
        /// </summary>
        /// <param name="saver"></param>
        /// <param name="data"></param>
        public static void UpdateSaveData(Saver saver, string data)
        {
            m_savedGameData.SetData(saver.key, GetSaverSceneIndex(saver), data);
        }

        /// <summary>
        /// Applies the saved game data to the savers in the current scene.
        /// </summary>
        /// <param name="savedGameData">Saved game data.</param>
        public static void ApplySavedGameData(SavedGameData savedGameData)
        {
            if (m_savers.Count <= 0) return;
            for (int i = m_savers.Count - 1; i >= 0; i--) // A saver may remove itself from list during apply.
            {
                try
                {
                    if (0 <= i && i < m_savers.Count)
                    {
                        var saver = m_savers[i];
                        if (saver != null) saver.ApplyData(savedGameData.GetData(saver.key));
                    }
                }
                catch (System.Exception e)
                {
                    Debug.LogException(e);
                }
            }
        }

        /// <summary>
        /// Applies the most recently recorded saved game data.
        /// </summary>
        public static void ApplySavedGameData()
        {
            ApplySavedGameData(m_savedGameData);
        }

        /// <summary>
        /// If changing scenes manually, calls before changing scenes to inform components
        /// that listen for OnDestroy messages that they're being destroyed because of the
        /// scene change.
        /// </summary>
        public static void BeforeSceneChange()
        {
            // Notify savers:
            for (int i = 0; i < m_savers.Count; i++)
            {
                try
                {
                    var saver = m_savers[i];
                    saver.OnBeforeSceneChange();
                }
                catch (System.Exception e)
                {
                    Debug.LogException(e);
                }
            }
            // Notify SceneNotifier:
            try
            {
                SceneNotifier.NotifyWillUnloadScene(m_currentSceneIndex);
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
            }
        }

        /// <summary>
        /// Loads the scene recorded in the saved game data (if saveCurrentScene is true) and 
        /// applies the saved game data to it.
        /// </summary>
        /// <param name="savedGameData"></param>
        public static void LoadGame(SavedGameData savedGameData)
        {
            if (saveCurrentScene)
            {
                instance.StartCoroutine(LoadSceneCoroutine(savedGameData, null));
            }
            else
            {
                ApplySavedGameData(savedGameData);
            }
        }

        /// <summary>
        /// Loads a scene, optionally moving the player to a specified spawnpoint.
        /// </summary>
        /// <param name="sceneNameAndSpawnpoint">Scene name, followed by an optional spawnpoint separated by '@'.</param>
        public static void LoadScene(string sceneNameAndSpawnpoint)
        {
            if (string.IsNullOrEmpty(sceneNameAndSpawnpoint)) return;
            string sceneName = sceneNameAndSpawnpoint;
            string spawnpointName = string.Empty;
            if (sceneNameAndSpawnpoint.Contains("@"))
            {
                var strings = sceneNameAndSpawnpoint.Split('@');
                sceneName = strings[0];
                spawnpointName = (strings.Length > 1) ? strings[1] : null;
            }
            var savedGameData = RecordSavedGameData();
            savedGameData.sceneName = sceneName;
            instance.StartCoroutine(LoadSceneCoroutine(savedGameData, spawnpointName));
        }

        private static IEnumerator LoadSceneCoroutine(SavedGameData savedGameData, string spawnpointName)
        {
            if (savedGameData == null) yield break;
            if (debug) Debug.Log("Save System: Loading scene " + savedGameData.sceneName +
                (string.IsNullOrEmpty(spawnpointName) ? string.Empty : " [spawn at " + spawnpointName + "]"));
            m_savedGameData = savedGameData;
            BeforeSceneChange();
            yield return LoadSceneInternal(savedGameData.sceneName);
            // Allow other scripts to spin up scene first:
            for (int i = 0; i < framesToWaitBeforeApplyData; i++)
            {
                yield return null;
            }
            yield return new WaitForEndOfFrame();
            m_playerSpawnpoint = !string.IsNullOrEmpty(spawnpointName) ? GameObject.Find(spawnpointName) : null;
            if (!string.IsNullOrEmpty(spawnpointName) && m_playerSpawnpoint == null) Debug.LogWarning("Save System: Can't find spawnpoint '" + spawnpointName + "'. Is spelling and capitalization correct?");
            ApplySavedGameData(savedGameData);
        }

        private void FinishedLoadingScene(string sceneName, int sceneIndex)
        {
            m_currentSceneIndex = sceneIndex;
            m_savedGameData.DeleteObsoleteSaveData(sceneIndex);
            sceneLoaded(sceneName, sceneIndex);
        }

        /// <summary>
        /// Clears the SaveSystem's saved game data cache and loads a
        /// starting scene.
        /// </summary>
        /// <param name="startingSceneName"></param>
        public static void RestartGame(string startingSceneName)
        {
            ClearSavedGameData();
            BeforeSceneChange();
            instance.StartCoroutine(LoadSceneInternal(startingSceneName));
        }

        /// <summary>
        /// Returns a serialized version of an object using whatever serializer is
        /// assigned to the SaveSystem (JSON by default).
        /// </summary>
        public static string Serialize(object data)
        {
            return serializer.Serialize(data);
        }

        /// <summary>
        /// Deserializes a previously-serialized string representation of an object
        /// back into an object. Uses whatever serializer is assigned to the 
        /// SaveSystem (JSON by default).
        /// </summary>
        /// <typeparam name="T">The type of the object.</typeparam>
        /// <param name="s">The object's serialized data.</param>
        /// <param name="data">Optional preallocated object to serialize data into.</param>
        /// <returns>The deserialized object, or null if it couldn't be deserialized.</returns>
        public static T Deserialize<T>(string s, T data = default(T))
        {
            return serializer.Deserialize<T>(s, data);
        }

    }
}