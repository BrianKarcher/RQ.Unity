//using Assets.Source.UtilityManager;
using RQ.Editor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;

namespace UtilityManager.Editor
{
    [CustomEditor(typeof(DSEAsset), true)]
    public class DSEConfigEditor : EditorBase
    {

        [MenuItem("Assets/Create/RQ/DSE Config")]
        public static void CreateNewAsset()
        {
            CreateAsset<DSEAsset>("NewDSE.asset");
            //SceneConfig sceneData = ScriptableObject.CreateInstance<SceneConfig>();
            //AssetDatabase.CreateAsset(sceneData, "Assets/Areas/NewScene.asset");
            //EditorUtility.FocusProjectWindow();
            //Selection.activeObject = sceneData;
        }
    }
}

//namespace RQ.Editor
//{
    
//    public class SceneConfigEditor : EditorBase
//    {
//        SceneConfig agent;
//        //protected tk2dTileMap tileMap;
//        //protected PointGrabber pointGrabber = null;

//        private VariablesEditor _variablesEditor;
//        //private PointsEditor<SpawnPoint> _pointsEditor;

//        private bool _showSpawnPoints = false;


//        //[MenuItem("Assets/Create/RQ/Timer Condition Config")]
//        //public static void CreateNewAssetTimer()
//        //{
//        //    TimerConditionConfig timerData = ScriptableObject.CreateInstance<TimerConditionConfig>();
//        //    AssetDatabase.CreateAsset(timerData, "Assets/Areas/NewTimer.asset");
//        //    EditorUtility.FocusProjectWindow();
//        //    Selection.activeObject = timerData;
//        //}

//        public void OnEnable()
//        {
//            agent = target as SceneConfig;
//            //if (agent.SpawnPoints == null)
//            //{
//            //    agent.SpawnPoints = new List<SpawnPointInAsset>();
//            //}
//            //SceneView.onSceneGUIDelegate += EventUpdate;
//            SceneView.onSceneGUIDelegate += OnScene;
//            //tileMap = GameObject.FindObjectOfType<tk2dTileMap>();
//            _variablesEditor = new VariablesEditor(this);
//            if (agent.Variables == null)
//                agent.Variables = new List<Model.Variable>();
//            //_pointsEditor = new PointsEditor<SpawnPoint>();
//            //_pointsEditor.Dirty += () => base.Dirty(false);
//            //_pointsEditor.Repaint += () => Repaint();
//            //_pointsEditor.OnEnable(new SpawnPointEditor(), agent.SpawnPoints);
//        }

//        public void OnDisable()
//        {
//            //SceneView.onSceneGUIDelegate -= OnScene;
//            //if (pointGrabber != null)
//            //{
//            //    pointGrabber.Close();
//            //}
//            //_pointsEditor.OnDisable();
//            //_pointsEditor = null;
//        }

//        public void OnScene(SceneView sceneView)
//        {
//            OnSceneGUI();
//        }

//        public void OnSceneGUI()
//        {
//            //if (_pointsEditor == null)
//            //    return;

//            var ev = Event.current;

//            //if (pointGrabber != null)
//            //{
//            //    pointGrabber.CheckKeypresses(ev);
//            //}

//            //_pointsEditor.OnSceneGUI();
//        }

//        public override void OnInspectorGUI()
//        {
//            GUI.changed = false;
//            base.OnInspectorGUI();

//            agent.SceneName = EditorGUILayout.TextField("Scene Name", agent.SceneName ?? string.Empty);

//            _variablesEditor.OnInspectorGUI(agent.Variables);

//            _showSpawnPoints = GUILayout.Toggle(_showSpawnPoints, "Spawn Points", "Button");

//            var spawnPoints = agent.SpawnPoints;

//            if (_showSpawnPoints)
//            {
//                if (GUILayout.Button("Add Spawn Point", GUILayout.Width(130)))
//                {
//                    //spriteAnimations.Add(new Animation.Common.SpriteAnimation());
//                    var newSpawnPoint = new SpawnPointInAsset(); //KeyValuePair<string, List<SpriteAnimation>>(string.Empty, new List<SpriteAnimation>());
//                    newSpawnPoint.UniqueId = Guid.NewGuid().ToString();
//                    //newType.SpriteAnimations = new List<SpriteAnimation>();
//                    //spriteAnimationTypes.Add(newType);
//                    spawnPoints.Add(newSpawnPoint);
//                }

//                int removeSpawnpointIndex = -1;

//                for (int i = 0; i < spawnPoints.Count(); i++)
//                {
//                    var spawnPoint = spawnPoints[i];
//                    GUILayout.Space(4.0f);
//                    GUILayout.BeginVertical(Styles.CreateInspectorStyle());
//                    EditorGUILayout.BeginHorizontal();
//                    spawnPoint.Name = EditorGUILayout.TextField("Name", spawnPoint.Name);
//                    if (GUILayout.Button("", Styles.CreateTilemapDeleteItemStyle()))
//                    {
//                        if (EditorUtility.DisplayDialog("Remove spawn point?", "Remove spawn point " + spawnPoint.Name + "?", "Yes", "No"))
//                        {
//                            removeSpawnpointIndex = i;
//                        }
//                    }
//                    EditorGUILayout.EndHorizontal();
//                    EditorGUILayout.LabelField("Unique Id", spawnPoint.UniqueId);
//                    spawnPoint.SceneCameFrom = EditorGUILayout.ObjectField("Scene came from", spawnPoint.SceneCameFrom, typeof(SceneConfig)) as SceneConfig;
//                    spawnPoint.IsInitialSpawnPoint = EditorGUILayout.Toggle("Initial Spawn Point", spawnPoint.IsInitialSpawnPoint);
//                    spawnPoint.ExtraInfo = EditorGUILayout.TextField("Extra Info", spawnPoint.ExtraInfo);

//                    GUILayout.EndVertical();
//                }

//                if (removeSpawnpointIndex != -1)
//                    spawnPoints.RemoveAt(removeSpawnpointIndex);
//            }

//            if (GUI.changed)
//            {
//                Dirty(false);
//            }
//        }




//    }
//}
